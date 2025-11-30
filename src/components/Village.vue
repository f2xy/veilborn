<template>
  <div class="village">
    <div class="stars"></div>

    <div class="village-container">
      <!-- Village Header -->
      <div class="village-header">
        <div class="village-info">
          <h1 class="village-name">{{ villageName }}</h1>
          <div class="village-tier">{{ villageTierText }}</div>
          <div class="progress-bar">
            <div class="progress-fill" :style="{ width: villageState.villageProgress.value + '%' }"></div>
            <span class="progress-text">{{ villageState.villageProgress.value }}% Restored</span>
          </div>
        </div>

        <div class="village-resources">
          <div class="resource" v-for="(amount, type) in villageState.resources.value" :key="type">
            <span class="resource-icon">{{ getResourceIcon(type) }}</span>
            <span class="resource-amount">{{ Math.floor(amount) }}</span>
          </div>
        </div>
      </div>

      <!-- Population Panel -->
      <div class="population-panel">
        <div class="population-info">
          <div class="population-stat">
            <span class="stat-icon">👥</span>
            <span class="stat-label">Total Population:</span>
            <span class="stat-value">{{ villageState.population.value }}</span>
          </div>
          <div class="population-stat">
            <span class="stat-icon">💼</span>
            <span class="stat-label">Assigned:</span>
            <span class="stat-value">{{ villageState.getAssignedPopulation.value }}</span>
          </div>
          <div class="population-stat">
            <span class="stat-icon">🆓</span>
            <span class="stat-label">Unassigned:</span>
            <span class="stat-value">{{ villageState.getUnassignedPopulation.value }}</span>
          </div>
          <button class="add-population-btn" @click="handleAddPopulation">+ Recruit (Cost: 50 essence)</button>
        </div>
      </div>

      <!-- Production Panel -->
      <div class="production-panel">
        <h3>📊 Resource Production</h3>
        <div class="production-rates">
          <div
            v-for="(rate, resource) in villageState.getProductionRate.value"
            :key="resource"
            class="production-item"
            v-if="rate > 0"
          >
            <span class="production-icon">{{ getResourceIcon(resource) }}</span>
            <span class="production-rate">+{{ rate.toFixed(1) }}/s</span>
          </div>
          <div v-if="Object.values(villageState.getProductionRate.value).every(r => r === 0)" class="no-production">
            No active production - assign population to buildings
          </div>
        </div>
      </div>

      <!-- Quest Panel -->
      <div class="quest-panel" v-if="villageState.questState.currentQuest.value">
        <h3>🎯 Aktif Görev</h3>
        <div class="quest-card">
          <div class="quest-header">
            <div class="quest-title">{{ villageState.questState.currentQuest.value.title }}</div>
            <div class="quest-chapter">Bölüm {{ villageState.questState.currentQuest.value.chapter }}</div>
          </div>
          <div class="quest-description">{{ villageState.questState.currentQuest.value.description }}</div>

          <div class="quest-objectives">
            <div class="objectives-title">Hedefler:</div>
            <div
              v-for="objective in villageState.questState.currentQuest.value.objectives"
              :key="objective.id"
              class="objective-item"
              :class="{ 'completed': objective.completed }"
            >
              <span class="objective-status">{{ objective.completed ? '✅' : '⬜' }}</span>
              <span class="objective-text">{{ objective.description }}</span>
            </div>
          </div>

          <!-- Quest Progress -->
          <div class="quest-progress">
            <div class="quest-progress-bar">
              <div
                class="quest-progress-fill"
                :style="{ width: getQuestCompletionPercentage() + '%' }"
              ></div>
            </div>
            <div class="quest-progress-text">{{ getQuestCompletionPercentage() }}% Tamamlandı</div>
          </div>
        </div>

        <!-- Available Quests -->
        <div class="available-quests" v-if="villageState.questState.availableQuests.value.length > 1">
          <h4>📜 Diğer Görevler</h4>
          <div class="quest-list">
            <div
              v-for="quest in getOtherAvailableQuests()"
              :key="quest.id"
              class="quest-list-item"
              @click="() => setActiveQuest(quest.id)"
            >
              <span class="quest-list-title">{{ quest.title }}</span>
              <span class="quest-list-chapter">Bölüm {{ quest.chapter }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Village Actions -->
      <div class="village-actions">
        <button
          class="action-btn journey-btn"
          :class="{ 'has-new-story': hasNewStoryScenes }"
          @click="$emit('navigate', 'gameplay')"
          title="Continue Journey"
        >
          <span>⚔️ Continue Journey</span>
          <span v-if="hasNewStoryScenes" class="story-badge">{{ newStoryCount }}</span>
        </button>
        <button class="action-btn" @click="$emit('navigate', 'menu')" title="Menu">
          <span>☰ Menu</span>
        </button>
      </div>

      <!-- Buildings Grid -->
      <div class="buildings-grid">
        <div
          v-for="building in buildings"
          :key="building.id"
          class="building-card"
          :class="[getBuildingClass(building), { 'locked': !isBuildingUnlocked(building.id) }]"
        >
          <!-- Locked Overlay -->
          <div class="locked-overlay" v-if="!isBuildingUnlocked(building.id)">
            <div class="locked-icon">🔒</div>
            <div class="locked-text">Kilitli</div>
            <div class="unlock-hint">{{ getUnlockHint(building.id) }}</div>
          </div>

          <div class="building-visual">
            <div class="building-icon">{{ getBuildingIcon(building.id) }}</div>
            <div class="building-level-badge">Lv {{ currentLevel(building.id) }}</div>
          </div>

          <div class="building-info">
            <h3 class="building-name">{{ building.name }}</h3>
            <p class="building-status">{{ getBuildingStatus(building) }}</p>
            <p class="building-description">{{ building.description }}</p>

            <!-- Current Level Info -->
            <div class="level-info" v-if="currentLevelData(building)">
              <div class="level-name">{{ currentLevelData(building).name }}</div>
              <div class="level-desc">{{ currentLevelData(building).description }}</div>

              <!-- Unlocks -->
              <div class="unlocks" v-if="currentLevelData(building).unlocks && currentLevelData(building).unlocks.length > 0">
                <div class="unlocks-title">Unlocked:</div>
                <ul class="unlocks-list">
                  <li v-for="unlock in currentLevelData(building).unlocks" :key="unlock">
                    ✓ {{ unlock }}
                  </li>
                </ul>
              </div>
            </div>

            <!-- Workers Section -->
            <div class="workers-section" v-if="currentLevel(building.id) > 0">
              <div class="workers-header">
                <span class="workers-title">👷 Workers:</span>
                <span class="workers-count">
                  {{ villageState.buildingAssignments.value[building.id] }}/{{ villageState.getBuildingCapacity(building.id) }}
                </span>
              </div>
              <div class="workers-controls">
                <button
                  class="worker-btn decrease"
                  @click="unassignPopulation(building.id)"
                  :disabled="!canUnassign(building.id)"
                  title="Remove worker"
                >
                  -
                </button>
                <div class="worker-count-display">
                  {{ villageState.buildingAssignments.value[building.id] }}
                </div>
                <button
                  class="worker-btn increase"
                  @click="assignPopulation(building.id)"
                  :disabled="!canAssign(building.id)"
                  title="Add worker"
                >
                  +
                </button>
              </div>
              <div class="production-info" v-if="villageState.buildingAssignments.value[building.id] > 0">
                <div class="building-production-title">Production:</div>
                <div class="building-production-values">
                  <span v-for="(rate, resource) in getBuildingProduction(building.id)" :key="resource" class="prod-value">
                    {{ getResourceIcon(resource) }} +{{ rate.toFixed(1) }}/s
                  </span>
                </div>
              </div>
            </div>

            <!-- Upgrade Button -->
            <div class="upgrade-section" v-if="canUpgradeBuilding(building)">
              <div class="upgrade-cost" v-if="nextLevelData(building)?.cost">
                <div class="cost-title">Upgrade Cost:</div>
                <div class="cost-items">
                  <span
                    v-for="(amount, resource) in nextLevelData(building).cost"
                    :key="resource"
                    class="cost-item"
                    :class="{ 'insufficient': !hasEnoughResource(resource, amount) }"
                  >
                    {{ getResourceIcon(resource) }} {{ amount }}
                  </span>
                </div>
              </div>

              <button
                class="upgrade-btn"
                @click="handleUpgrade(building)"
                :disabled="!canAffordUpgrade(building)"
              >
                <span v-if="nextLevelData(building)">
                  Upgrade to {{ nextLevelData(building).name }}
                </span>
                <span v-else>Max Level</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- First Time Welcome Message -->
      <div class="welcome-overlay" v-if="showWelcome">
        <div class="welcome-content">
          <h2>{{ villageName }}</h2>
          <p>Bir zamanlar büyük bir köyün kalıntıları önünde duruyorsun. Binalar çökmüş, sokaklar yabani otlarla kaplanmış ve havada ürkütücü bir sessizlik asılı.</p>
          <p>Ama burada potansiyel hissediyorsun - zaman ve çabayla bu yer yeniden yükselebilir. Yolculuğun seni buraya bir sebepten getirdi.</p>
          <p>Keşfederken, harabelerin arasında dağılmış bazı temel kaynaklar buluyorsun. Burası senin yeni evin, karanlıktaki sığınağın olacak.</p>
          <button class="welcome-btn" @click="closeWelcome">Restorasyona Başla</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useVillageState } from '../composables/useVillageState.js'
import { useGameState } from '../composables/useGameState.js'
import { getAllBuildings, getBuildingLevel } from '../data/village.js'

export default {
  name: 'Village',
  emits: ['navigate'],
  setup() {
    const villageState = useVillageState()
    const gameState = useGameState()
    const buildings = ref(getAllBuildings())
    const showWelcome = ref(false)
    const villageName = ref("Unutulmuş Sığınak")

    // Check for new story scenes
    const hasNewStoryScenes = computed(() => {
      return gameState.availableStoryScenes.value.length > 0
    })

    const newStoryCount = computed(() => {
      return gameState.availableStoryScenes.value.length
    })

    const villageTierText = computed(() => {
      const tier = villageState.villageTier.value
      const tierNames = {
        ruins: 'Harabeler',
        settlement: 'Küçük Yerleşim',
        village: 'Gelişen Köy',
        town: 'Müreffeh Kasaba',
        stronghold: 'Güçlendirilmiş Kale',
        citadel: 'Görkemli Kale'
      }
      return tierNames[tier] || 'Bilinmeyen'
    })

    const currentLevel = (buildingId) => {
      return villageState.getBuildingLevel(buildingId)
    }

    const currentLevelData = (building) => {
      const level = currentLevel(building.id)
      return getBuildingLevel(building.id, level)
    }

    const nextLevelData = (building) => {
      const nextLevel = currentLevel(building.id) + 1
      if (nextLevel > building.maxLevel) return null
      return getBuildingLevel(building.id, nextLevel)
    }

    const canUpgradeBuilding = (building) => {
      return currentLevel(building.id) < building.maxLevel
    }

    const canAffordUpgrade = (building) => {
      const nextLevel = nextLevelData(building)
      if (!nextLevel) return false
      return villageState.canUpgrade(building.id, nextLevel.cost)
    }

    const hasEnoughResource = (resource, amount) => {
      return villageState.resources.value[resource] >= amount
    }

    const getBuildingClass = (building) => {
      const level = currentLevel(building.id)
      const levelData = currentLevelData(building)
      return `visual-${levelData?.visual || 'ruins'}`
    }

    const getBuildingStatus = (building) => {
      const level = currentLevel(building.id)
      if (level === 0) return 'Terk Edilmiş Harabeler'
      if (level === building.maxLevel) return 'Tamamen Restore Edilmiş'
      return `Seviye ${level} / ${building.maxLevel}`
    }

    const getBuildingIcon = (buildingId) => {
      const icons = {
        town_hall: '🏛️',
        temple: '⛩️',
        barracks: '🗡️',
        workshop: '🔨',
        market: '🏪',
        library: '📚'
      }
      return icons[buildingId] || '🏗️'
    }

    const getResourceIcon = (resource) => {
      const icons = {
        essence: '✨',
        materials: '🪨',
        rare_materials: '💎',
        legendary_materials: '🌟'
      }
      return icons[resource] || '❓'
    }

    // Population assignment functions
    const canAssign = (buildingId) => {
      if (currentLevel(buildingId) === 0) return false
      const capacity = villageState.getBuildingCapacity(buildingId)
      const current = villageState.buildingAssignments.value[buildingId] || 0
      const unassigned = villageState.getUnassignedPopulation.value
      return current < capacity && unassigned > 0
    }

    const canUnassign = (buildingId) => {
      const current = villageState.buildingAssignments.value[buildingId] || 0
      return current > 0
    }

    const assignPopulation = (buildingId) => {
      if (villageState.assignPopulation(buildingId, 1)) {
        console.log(`Assigned 1 population to ${buildingId}`)
      }
    }

    const unassignPopulation = (buildingId) => {
      if (villageState.unassignPopulation(buildingId, 1)) {
        console.log(`Unassigned 1 population from ${buildingId}`)
      }
    }

    const handleAddPopulation = () => {
      if (villageState.resources.value.essence >= 50) {
        villageState.resources.value.essence -= 50
        villageState.addPopulation(1)
        villageState.saveVillageState()
      } else {
        alert('Yeterli essence yok! (50 gerekli)')
      }
    }

    const getBuildingProduction = (buildingId) => {
      const assigned = villageState.buildingAssignments.value[buildingId] || 0
      if (assigned === 0) return {}

      const buildingConfig = {
        town_hall: { essence: 0.5, materials: 1 },
        temple: { essence: 2, rare_materials: 0.1, legendary_materials: 0.05 },
        barracks: { materials: 0.5 },
        workshop: { materials: 2, rare_materials: 0.3, legendary_materials: 0.02 },
        market: { essence: 1, materials: 1, rare_materials: 0.2 },
        library: { essence: 1.5, rare_materials: 0.2, legendary_materials: 0.08 }
      }

      const levelMultipliers = [1, 1.5, 2, 2.5, 3, 4]
      const level = currentLevel(buildingId)
      const multiplier = levelMultipliers[level] || 1

      const production = {}
      const config = buildingConfig[buildingId]
      if (config) {
        for (const [resource, baseAmount] of Object.entries(config)) {
          production[resource] = baseAmount * multiplier * assigned
        }
      }

      return production
    }

    const handleUpgrade = (building) => {
      const nextLevel = nextLevelData(building)
      if (nextLevel && villageState.upgradeBuilding(building.id, nextLevel.cost)) {
        console.log(`Upgraded ${building.name} to level ${currentLevel(building.id)}`)
      }
    }

    const closeWelcome = () => {
      showWelcome.value = false
    }

    // Quest related functions
    const isBuildingUnlocked = (buildingId) => {
      return villageState.isBuildingUnlocked(buildingId)
    }

    const getUnlockHint = (buildingId) => {
      const buildingUnlockMap = {
        barracks: 'discovery_begins',
        workshop: 'military_foundation',
        market: 'military_foundation',
        library: 'craft_and_trade'
      }

      const unlockQuestId = buildingUnlockMap[buildingId]
      if (!unlockQuestId) return 'Görev tamamla'

      const quest = villageState.questState.getQuestById(unlockQuestId)
      return quest ? `"${quest.title}" görevini tamamla` : 'Görev tamamla'
    }

    const getQuestCompletionPercentage = () => {
      const quest = villageState.questState.currentQuest.value
      if (!quest) return 0

      const total = quest.objectives.length
      const completed = quest.objectives.filter(obj => obj.completed).length

      return Math.round((completed / total) * 100)
    }

    const getOtherAvailableQuests = () => {
      return villageState.questState.availableQuests.value.filter(
        q => q.id !== villageState.questState.activeQuest.value
      )
    }

    const setActiveQuest = (questId) => {
      villageState.questState.setActiveQuest(questId)
    }

    onMounted(() => {
      // Check if village was just discovered
      if (!villageState.villageDiscovered.value) {
        villageState.discoverVillage()
        showWelcome.value = true
      } else {
        villageState.loadVillageState()
      }
    })

    onUnmounted(() => {
      villageState.stopProduction()
    })

    return {
      villageState,
      buildings,
      villageName,
      villageTierText,
      showWelcome,
      currentLevel,
      currentLevelData,
      nextLevelData,
      canUpgradeBuilding,
      canAffordUpgrade,
      hasEnoughResource,
      getBuildingClass,
      getBuildingStatus,
      getBuildingIcon,
      getResourceIcon,
      handleUpgrade,
      closeWelcome,
      canAssign,
      canUnassign,
      assignPopulation,
      unassignPopulation,
      handleAddPopulation,
      getBuildingProduction,
      isBuildingUnlocked,
      getUnlockHint,
      getQuestCompletionPercentage,
      getOtherAvailableQuests,
      setActiveQuest,
      hasNewStoryScenes,
      newStoryCount
    }
  }
}
</script>

<style scoped>
.village {
  width: 100%;
  min-height: 100vh;
  position: relative;
  background: linear-gradient(135deg, #1a1410 0%, #2a1a1a 25%, #1a2a20 50%, #1a1a2a 75%, #1a1410 100%);
  overflow-y: auto;
  overflow-x: hidden;
  padding: 2rem;
}

.stars {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image:
    radial-gradient(2px 2px at 20px 30px, #eee, rgba(0,0,0,0)),
    radial-gradient(2px 2px at 60px 70px, #fff, rgba(0,0,0,0)),
    radial-gradient(1px 1px at 50px 50px, #fff, rgba(0,0,0,0)),
    radial-gradient(1px 1px at 130px 80px, #fff, rgba(0,0,0,0)),
    radial-gradient(2px 2px at 90px 10px, #fff, rgba(0,0,0,0));
  background-repeat: repeat;
  background-size: 200px 200px;
  opacity: 0.3;
  animation: twinkle 200s linear infinite;
  pointer-events: none;
  z-index: 0;
}

@keyframes twinkle {
  from { transform: translateY(0); }
  to { transform: translateY(-1000px); }
}

.village-container {
  position: relative;
  z-index: 1;
  max-width: 1400px;
  margin: 0 auto;
}

/* Header */
.village-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: rgba(0, 0, 0, 0.4);
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 12px;
  backdrop-filter: blur(10px);
}

.village-name {
  font-family: 'Cinzel', serif;
  font-size: 2.5rem;
  color: #e8d5f2;
  margin: 0 0 0.5rem 0;
  text-shadow: 0 0 20px rgba(168, 85, 247, 0.6);
}

.village-tier {
  font-size: 1.2rem;
  color: rgba(168, 85, 247, 0.9);
  margin-bottom: 1rem;
}

.progress-bar {
  position: relative;
  width: 300px;
  height: 30px;
  background: rgba(0, 0, 0, 0.5);
  border: 2px solid rgba(168, 85, 247, 0.4);
  border-radius: 15px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, rgba(109, 40, 217, 0.8), rgba(168, 85, 247, 0.8));
  transition: width 0.5s ease;
}

.progress-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-weight: bold;
  font-size: 0.9rem;
}

.village-resources {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.resource {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.8rem 1.2rem;
  background: rgba(0, 0, 0, 0.6);
  border: 1px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
}

.resource-icon {
  font-size: 1.5rem;
}

.resource-amount {
  font-size: 1.2rem;
  font-weight: bold;
  color: #fff;
}

/* Population Panel */
.population-panel {
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: rgba(0, 0, 0, 0.4);
  border: 2px solid rgba(76, 175, 80, 0.3);
  border-radius: 12px;
  backdrop-filter: blur(10px);
}

.population-info {
  display: flex;
  gap: 2rem;
  align-items: center;
  flex-wrap: wrap;
}

.population-stat {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.8rem 1.2rem;
  background: rgba(0, 0, 0, 0.6);
  border: 1px solid rgba(76, 175, 80, 0.3);
  border-radius: 8px;
}

.stat-icon {
  font-size: 1.5rem;
}

.stat-label {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.9rem;
}

.stat-value {
  font-size: 1.3rem;
  font-weight: bold;
  color: #4CAF50;
}

.add-population-btn {
  padding: 0.8rem 1.5rem;
  background: linear-gradient(135deg, rgba(76, 175, 80, 0.6), rgba(56, 142, 60, 0.6));
  color: white;
  border: 2px solid rgba(76, 175, 80, 0.4);
  border-radius: 8px;
  cursor: pointer;
  font-size: 0.95rem;
  font-weight: bold;
  transition: all 0.3s ease;
}

.add-population-btn:hover {
  background: linear-gradient(135deg, rgba(76, 175, 80, 0.8), rgba(56, 142, 60, 0.8));
  border-color: rgba(76, 175, 80, 0.6);
  transform: translateY(-2px);
}

/* Production Panel */
.production-panel {
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: rgba(0, 0, 0, 0.4);
  border: 2px solid rgba(255, 193, 7, 0.3);
  border-radius: 12px;
  backdrop-filter: blur(10px);
}

.production-panel h3 {
  color: #FFC107;
  margin: 0 0 1rem 0;
  font-size: 1.3rem;
}

/* Quest Panel */
.quest-panel {
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: rgba(0, 0, 0, 0.4);
  border: 2px solid rgba(33, 150, 243, 0.4);
  border-radius: 12px;
  backdrop-filter: blur(10px);
}

.quest-panel h3 {
  color: #2196F3;
  margin: 0 0 1rem 0;
  font-size: 1.4rem;
  font-weight: bold;
}

.quest-panel h4 {
  color: rgba(33, 150, 243, 0.8);
  margin: 1.5rem 0 0.8rem 0;
  font-size: 1.1rem;
}

.quest-card {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(33, 150, 243, 0.3);
  border-radius: 8px;
  padding: 1.2rem;
}

.quest-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.8rem;
}

.quest-title {
  font-size: 1.3rem;
  font-weight: bold;
  color: #e8d5f2;
}

.quest-chapter {
  font-size: 0.9rem;
  color: rgba(33, 150, 243, 0.8);
  padding: 0.3rem 0.8rem;
  background: rgba(33, 150, 243, 0.2);
  border-radius: 12px;
}

.quest-description {
  color: rgba(255, 255, 255, 0.8);
  font-size: 1rem;
  line-height: 1.5;
  margin-bottom: 1.2rem;
  font-style: italic;
}

.quest-objectives {
  margin-bottom: 1rem;
}

.objectives-title {
  color: #2196F3;
  font-weight: bold;
  margin-bottom: 0.6rem;
  font-size: 1rem;
}

.objective-item {
  display: flex;
  align-items: flex-start;
  gap: 0.8rem;
  padding: 0.6rem;
  margin-bottom: 0.4rem;
  background: rgba(0, 0, 0, 0.3);
  border-left: 3px solid rgba(33, 150, 243, 0.4);
  border-radius: 4px;
  transition: all 0.3s ease;
}

.objective-item.completed {
  border-left-color: rgba(76, 175, 80, 0.6);
  background: rgba(76, 175, 80, 0.1);
}

.objective-status {
  font-size: 1.2rem;
  flex-shrink: 0;
}

.objective-text {
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.95rem;
  line-height: 1.4;
}

.objective-item.completed .objective-text {
  color: rgba(255, 255, 255, 0.7);
  text-decoration: line-through;
}

.quest-progress {
  margin-top: 1rem;
}

.quest-progress-bar {
  position: relative;
  width: 100%;
  height: 24px;
  background: rgba(0, 0, 0, 0.5);
  border: 2px solid rgba(33, 150, 243, 0.4);
  border-radius: 12px;
  overflow: hidden;
  margin-bottom: 0.5rem;
}

.quest-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, rgba(33, 150, 243, 0.8), rgba(66, 165, 245, 0.8));
  transition: width 0.5s ease;
}

.quest-progress-text {
  text-align: center;
  color: rgba(33, 150, 243, 0.9);
  font-size: 0.9rem;
  font-weight: bold;
}

.available-quests {
  margin-top: 1.2rem;
}

.quest-list {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.quest-list-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.8rem 1rem;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(33, 150, 243, 0.3);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.quest-list-item:hover {
  background: rgba(33, 150, 243, 0.2);
  border-color: rgba(33, 150, 243, 0.6);
  transform: translateX(5px);
}

.quest-list-title {
  color: #e8d5f2;
  font-size: 0.95rem;
  font-weight: 500;
}

.quest-list-chapter {
  color: rgba(33, 150, 243, 0.7);
  font-size: 0.85rem;
}

.production-rates {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.production-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.8rem 1.2rem;
  background: rgba(0, 0, 0, 0.6);
  border: 1px solid rgba(255, 193, 7, 0.3);
  border-radius: 8px;
}

.production-icon {
  font-size: 1.3rem;
}

.production-rate {
  font-size: 1.1rem;
  font-weight: bold;
  color: #FFC107;
}

.no-production {
  color: rgba(255, 255, 255, 0.5);
  font-style: italic;
}

/* Village Actions */
.village-actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.action-btn {
  padding: 0.9rem 2rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.6), rgba(126, 58, 242, 0.6));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.action-btn:hover {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  border-color: rgba(168, 85, 247, 0.6);
  transform: translateY(-2px);
}

.journey-btn {
  position: relative;
}

.journey-btn.has-new-story {
  background: linear-gradient(135deg, rgba(255, 152, 0, 0.6), rgba(255, 193, 7, 0.6));
  border-color: rgba(255, 193, 7, 0.5);
  animation: pulse 2s ease-in-out infinite;
}

.journey-btn.has-new-story:hover {
  background: linear-gradient(135deg, rgba(255, 152, 0, 0.8), rgba(255, 193, 7, 0.8));
  border-color: rgba(255, 193, 7, 0.7);
}

@keyframes pulse {
  0%, 100% {
    box-shadow: 0 0 0 0 rgba(255, 193, 7, 0.7);
  }
  50% {
    box-shadow: 0 0 20px 10px rgba(255, 193, 7, 0.3);
  }
}

.story-badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background: #f44336;
  color: white;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: bold;
  border: 2px solid white;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

/* Buildings Grid */
.buildings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.building-card {
  background: rgba(0, 0, 0, 0.5);
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 12px;
  padding: 1.5rem;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.building-card:hover {
  border-color: rgba(168, 85, 247, 0.6);
  box-shadow: 0 0 30px rgba(168, 85, 247, 0.3);
  transform: translateY(-5px);
}

.building-card.locked {
  opacity: 0.7;
  position: relative;
  pointer-events: none;
}

.locked-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.8);
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  z-index: 10;
  backdrop-filter: blur(5px);
}

.locked-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  animation: lockPulse 2s ease-in-out infinite;
}

@keyframes lockPulse {
  0%, 100% { transform: scale(1); opacity: 0.8; }
  50% { transform: scale(1.1); opacity: 1; }
}

.locked-text {
  font-size: 1.5rem;
  font-weight: bold;
  color: #f44336;
  margin-bottom: 0.5rem;
  text-shadow: 0 0 10px rgba(244, 67, 54, 0.6);
}

.unlock-hint {
  font-size: 0.95rem;
  color: rgba(255, 255, 255, 0.8);
  text-align: center;
  padding: 0 1rem;
  line-height: 1.4;
}

.visual-ruins {
  border-color: rgba(139, 69, 19, 0.4);
}

.visual-basic {
  border-color: rgba(76, 175, 80, 0.4);
}

.visual-improved {
  border-color: rgba(33, 150, 243, 0.4);
}

.visual-advanced {
  border-color: rgba(156, 39, 176, 0.4);
}

.visual-masterwork {
  border-color: rgba(255, 215, 0, 0.5);
}

.building-visual {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.building-icon {
  font-size: 3rem;
}

.building-level-badge {
  background: rgba(168, 85, 247, 0.6);
  color: white;
  padding: 0.4rem 0.8rem;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: bold;
}

.building-name {
  font-family: 'Cinzel', serif;
  font-size: 1.5rem;
  color: #e8d5f2;
  margin: 0 0 0.5rem 0;
}

.building-status {
  color: rgba(168, 85, 247, 0.8);
  font-size: 0.9rem;
  margin: 0 0 0.5rem 0;
}

.building-description {
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.9rem;
  line-height: 1.5;
  margin-bottom: 1rem;
}

.level-info {
  margin: 1rem 0;
  padding: 1rem;
  background: rgba(0, 0, 0, 0.3);
  border-left: 3px solid rgba(168, 85, 247, 0.5);
  border-radius: 4px;
}

.level-name {
  font-weight: bold;
  color: #e8d5f2;
  margin-bottom: 0.5rem;
}

.level-desc {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
}

.unlocks-title {
  color: rgba(76, 175, 80, 0.9);
  font-size: 0.9rem;
  margin: 0.5rem 0 0.3rem 0;
  font-weight: bold;
}

.unlocks-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.unlocks-list li {
  color: rgba(76, 175, 80, 0.8);
  font-size: 0.85rem;
  padding: 0.2rem 0;
}

/* Workers Section */
.workers-section {
  margin: 1rem 0;
  padding: 1rem;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(76, 175, 80, 0.3);
  border-radius: 8px;
}

.workers-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.8rem;
}

.workers-title {
  color: #4CAF50;
  font-weight: bold;
}

.workers-count {
  color: #4CAF50;
  font-size: 1.1rem;
  font-weight: bold;
}

.workers-controls {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 0.8rem;
}

.worker-btn {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: 2px solid rgba(76, 175, 80, 0.5);
  background: rgba(0, 0, 0, 0.6);
  color: #4CAF50;
  font-size: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
}

.worker-btn:hover:not(:disabled) {
  background: rgba(76, 175, 80, 0.3);
  border-color: #4CAF50;
  transform: scale(1.1);
}

.worker-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.worker-count-display {
  font-size: 2rem;
  font-weight: bold;
  color: #4CAF50;
  min-width: 60px;
  text-align: center;
}

.production-info {
  margin-top: 0.8rem;
  padding-top: 0.8rem;
  border-top: 1px solid rgba(76, 175, 80, 0.2);
}

.building-production-title {
  color: rgba(255, 193, 7, 0.9);
  font-size: 0.85rem;
  margin-bottom: 0.4rem;
  font-weight: bold;
}

.building-production-values {
  display: flex;
  gap: 0.8rem;
  flex-wrap: wrap;
}

.prod-value {
  color: #FFC107;
  font-size: 0.9rem;
  padding: 0.3rem 0.6rem;
  background: rgba(255, 193, 7, 0.1);
  border-radius: 4px;
}

/* Upgrade Section */
.upgrade-section {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 2px solid rgba(168, 85, 247, 0.2);
}

.upgrade-cost {
  margin-bottom: 0.8rem;
}

.cost-title {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
}

.cost-items {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.cost-item {
  padding: 0.4rem 0.8rem;
  background: rgba(0, 0, 0, 0.4);
  border: 1px solid rgba(168, 85, 247, 0.3);
  border-radius: 4px;
  color: #fff;
  font-size: 0.9rem;
}

.cost-item.insufficient {
  color: #f44336;
  border-color: rgba(244, 67, 54, 0.5);
}

.upgrade-btn {
  width: 100%;
  padding: 0.9rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.6), rgba(126, 58, 242, 0.6));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: bold;
  transition: all 0.3s ease;
}

.upgrade-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  border-color: rgba(168, 85, 247, 0.6);
  transform: translateY(-2px);
}

.upgrade-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Welcome Overlay */
.welcome-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  animation: fadeIn 1s ease;
}

.welcome-content {
  max-width: 600px;
  padding: 3rem;
  background: rgba(26, 20, 16, 0.95);
  border: 3px solid rgba(168, 85, 247, 0.5);
  border-radius: 12px;
  text-align: center;
  animation: slideUp 1s ease;
}

.welcome-content h2 {
  font-family: 'Cinzel', serif;
  font-size: 2.5rem;
  color: #e8d5f2;
  margin: 0 0 1.5rem 0;
  text-shadow: 0 0 20px rgba(168, 85, 247, 0.6);
}

.welcome-content p {
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.8);
  line-height: 1.8;
  margin-bottom: 1.5rem;
}

.welcome-btn {
  padding: 1rem 3rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.6), rgba(126, 58, 242, 0.6));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  font-size: 1.2rem;
  font-weight: bold;
  transition: all 0.3s ease;
}

.welcome-btn:hover {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  border-color: rgba(168, 85, 247, 0.6);
  transform: translateY(-2px);
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(50px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .village {
    padding: 1rem;
  }

  .buildings-grid {
    grid-template-columns: 1fr;
  }

  .village-header {
    flex-direction: column;
    gap: 1rem;
  }

  .population-info {
    flex-direction: column;
    align-items: stretch;
  }

  .village-name {
    font-size: 2rem;
  }
}
</style>
