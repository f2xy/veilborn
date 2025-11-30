import { ref, computed, watch } from 'vue';
import { quests, questChapters, buildingUnlockMap } from '../data/quests.js';

// Global state
const questStates = ref({});
const currentChapter = ref(1);
const completedQuests = ref([]);
const activeQuest = ref(null);

// Quest state'ini başlat
const initializeQuestStates = () => {
  const states = {};

  Object.keys(quests).forEach(questId => {
    const quest = quests[questId];

    // İlk görevi unlocked yap, diğerlerini locked
    const status = questId === 'discovery_begins' ? 'unlocked' : 'locked';

    states[questId] = {
      ...quest,
      status,
      objectives: quest.objectives.map(obj => ({ ...obj, completed: false }))
    };
  });

  questStates.value = states;

  // İlk görevi aktif yap
  if (states.discovery_begins) {
    activeQuest.value = 'discovery_begins';
    states.discovery_begins.status = 'in_progress';
  }
};

// Initialize on first import
if (Object.keys(questStates.value).length === 0) {
  initializeQuestStates();
}

export function useQuestState() {
  // Computed: Mevcut görevler (unlocked veya in_progress)
  const availableQuests = computed(() => {
    return Object.values(questStates.value).filter(
      quest => quest.status === 'unlocked' || quest.status === 'in_progress'
    );
  });

  // Computed: Tamamlanan görevler
  const completedQuestsList = computed(() => {
    return Object.values(questStates.value).filter(
      quest => quest.status === 'completed'
    );
  });

  // Computed: Aktif görev
  const currentQuest = computed(() => {
    if (!activeQuest.value) return null;
    return questStates.value[activeQuest.value];
  });

  // Computed: Mevcut bölüm bilgisi
  const currentChapterInfo = computed(() => {
    return questChapters[currentChapter.value] || null;
  });

  // Computed: Quest ilerleme yüzdesi (tüm görevler için)
  const overallProgress = computed(() => {
    const total = Object.keys(questStates.value).length;
    const completed = completedQuestsList.value.length;
    return Math.round((completed / total) * 100);
  });

  /**
   * Bir görevin gereksinimlerini kontrol et
   */
  const checkQuestRequirements = (questId, buildingLevels, population) => {
    const quest = questStates.value[questId];
    if (!quest) return false;

    // Önceki görevler tamamlanmış mı?
    const previousQuestsCompleted = quest.requirements.previousQuests.every(
      prevQuestId => completedQuests.value.includes(prevQuestId)
    );

    if (!previousQuestsCompleted) return false;

    // Bina seviye gereksinimleri karşılanıyor mu?
    const buildingRequirements = quest.requirements.buildings || {};
    const buildingRequirementsMet = Object.entries(buildingRequirements).every(
      ([buildingId, requiredLevel]) => {
        return (buildingLevels[buildingId] || 0) >= requiredLevel;
      }
    );

    return buildingRequirementsMet;
  };

  /**
   * Bir görevin objective'lerini kontrol et ve güncelle
   */
  const updateQuestObjectives = (questId, buildingLevels, population) => {
    const quest = questStates.value[questId];
    if (!quest || quest.status !== 'in_progress') return;

    let allObjectivesCompleted = true;

    quest.objectives.forEach(objective => {
      if (objective.completed) return;

      let isCompleted = false;

      switch (objective.type) {
        case 'building_level':
          // Belirli bir binanın belirli seviyeye ulaşması
          isCompleted = (buildingLevels[objective.building] || 0) >= objective.level;
          break;

        case 'population':
          // Belirli nüfusa ulaşma
          isCompleted = population >= objective.target;
          break;

        case 'all_buildings_min_level':
          // Tüm binaların minimum seviyeye ulaşması
          isCompleted = Object.values(buildingLevels).every(
            level => level >= objective.level
          );
          break;

        case 'all_buildings_max_level':
          // Tüm binaların maksimum seviyeye ulaşması
          // Bu bilgiyi village.js'den almalıyız, şimdilik basitleştirelim
          const minMaxLevel = 4; // Çoğu bina 4-5 seviye
          isCompleted = Object.values(buildingLevels).every(
            level => level >= minMaxLevel
          );
          break;

        default:
          break;
      }

      objective.completed = isCompleted;
      if (!isCompleted) {
        allObjectivesCompleted = false;
      }
    });

    // Tüm objective'ler tamamlandıysa görevi tamamla
    if (allObjectivesCompleted && quest.status === 'in_progress') {
      completeQuest(questId);
    }
  };

  /**
   * Bir görevi tamamla
   */
  const completeQuest = (questId) => {
    const quest = questStates.value[questId];
    if (!quest || quest.status === 'completed') return null;

    // Görevi tamamlandı olarak işaretle
    quest.status = 'completed';
    completedQuests.value.push(questId);

    // Aktif görev buysa, bir sonrakini aktif yap
    if (activeQuest.value === questId) {
      activeQuest.value = null;
    }

    // Ödülleri döndür (Village state'e verilecek)
    const rewards = { ...quest.rewards };

    // Sonraki görevleri unlock et
    unlockNextQuests(buildingLevels, population);

    console.log(`✅ Görev tamamlandı: ${quest.title}`);

    return rewards;
  };

  /**
   * Gereksinimler karşılanan görevlerin kilidini aç
   */
  const unlockNextQuests = (buildingLevels = {}, population = 0) => {
    Object.keys(questStates.value).forEach(questId => {
      const quest = questStates.value[questId];

      if (quest.status === 'locked') {
        const canUnlock = checkQuestRequirements(questId, buildingLevels, population);

        if (canUnlock) {
          quest.status = 'unlocked';
          console.log(`🔓 Yeni görev açıldı: ${quest.title}`);

          // İlk unlocked görevi aktif yap (eğer aktif görev yoksa)
          if (!activeQuest.value) {
            activeQuest.value = questId;
            quest.status = 'in_progress';
          }
        }
      }
    });
  };

  /**
   * Bir görevi aktif görevi yap
   */
  const setActiveQuest = (questId) => {
    const quest = questStates.value[questId];
    if (!quest || quest.status === 'locked' || quest.status === 'completed') {
      return false;
    }

    // Önceki aktif görevi unlocked'a çevir
    if (activeQuest.value && activeQuest.value !== questId) {
      const prevQuest = questStates.value[activeQuest.value];
      if (prevQuest && prevQuest.status === 'in_progress') {
        prevQuest.status = 'unlocked';
      }
    }

    activeQuest.value = questId;
    quest.status = 'in_progress';
    return true;
  };

  /**
   * Bina kilidi açık mı kontrol et
   */
  const isBuildingUnlocked = (buildingId) => {
    const buildingInfo = buildingUnlockMap[buildingId];
    if (!buildingInfo) return true; // Tanımsız binalar açık sayılır

    if (buildingInfo.unlocked) return true;

    // Bu binayı unlock eden görev tamamlanmış mı?
    const unlockQuest = buildingInfo.unlockedBy;
    if (!unlockQuest) return buildingInfo.unlocked;

    return completedQuests.value.includes(unlockQuest);
  };

  /**
   * Tüm unlocked binaların listesi
   */
  const unlockedBuildings = computed(() => {
    return Object.keys(buildingUnlockMap).filter(buildingId =>
      isBuildingUnlocked(buildingId)
    );
  });

  /**
   * Save quest state to localStorage
   */
  const saveQuestState = () => {
    const saveData = {
      questStates: questStates.value,
      currentChapter: currentChapter.value,
      completedQuests: completedQuests.value,
      activeQuest: activeQuest.value
    };

    localStorage.setItem('veilborn_quests', JSON.stringify(saveData));
  };

  /**
   * Load quest state from localStorage
   */
  const loadQuestState = () => {
    const saved = localStorage.getItem('veilborn_quests');

    if (saved) {
      try {
        const data = JSON.parse(saved);
        questStates.value = data.questStates || questStates.value;
        currentChapter.value = data.currentChapter || 1;
        completedQuests.value = data.completedQuests || [];
        activeQuest.value = data.activeQuest || null;
        return true;
      } catch (e) {
        console.error('Quest state yüklenemedi:', e);
        return false;
      }
    }

    return false;
  };

  /**
   * Reset quest state (yeni oyun için)
   */
  const resetQuestState = () => {
    initializeQuestStates();
    currentChapter.value = 1;
    completedQuests.value = [];
    activeQuest.value = 'discovery_begins';
    if (questStates.value.discovery_begins) {
      questStates.value.discovery_begins.status = 'in_progress';
    }
    saveQuestState();
  };

  /**
   * Belirli bir görevin detaylarını al
   */
  const getQuestById = (questId) => {
    return questStates.value[questId] || null;
  };

  /**
   * Bir bölümdeki tüm görevleri al
   */
  const getQuestsByChapter = (chapterId) => {
    const chapter = questChapters[chapterId];
    if (!chapter) return [];

    return chapter.quests.map(questId => questStates.value[questId]).filter(Boolean);
  };

  // Auto-save when quest state changes
  watch([questStates, completedQuests, activeQuest], () => {
    saveQuestState();
  }, { deep: true });

  return {
    // State
    questStates,
    currentChapter,
    completedQuests,
    activeQuest,

    // Computed
    availableQuests,
    completedQuestsList,
    currentQuest,
    currentChapterInfo,
    overallProgress,
    unlockedBuildings,

    // Methods
    checkQuestRequirements,
    updateQuestObjectives,
    completeQuest,
    unlockNextQuests,
    setActiveQuest,
    isBuildingUnlocked,
    getQuestById,
    getQuestsByChapter,
    saveQuestState,
    loadQuestState,
    resetQuestState
  };
}
