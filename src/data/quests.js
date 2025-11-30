/**
 * Görev Sistemi - Quest System
 *
 * Her görev hikayede ilerlemek için tamamlanmalıdır.
 * Görevler binalar ve hikaye arasında köprü görevi görür.
 */

export const quests = {
  // Bölüm 1: Köyün Keşfi
  discovery_begins: {
    id: 'discovery_begins',
    chapter: 1,
    title: 'Yeni Bir Başlangıç',
    description: 'Köyü keşfet ve ilk binalarını inşa etmeye başla.',

    // Bu görevi açmak için gereksinimler
    requirements: {
      buildings: {
        // Hiçbir gereksinim yok - başlangıç görevi
      },
      previousQuests: [] // Önceki görev gerekmez
    },

    // Görevi tamamlama kriterleri
    objectives: [
      {
        id: 'build_town_hall',
        description: 'Belediye Binasını 1. seviyeye yükselt',
        type: 'building_level',
        building: 'town_hall',
        level: 1,
        completed: false
      },
      {
        id: 'build_temple',
        description: 'Tapınağı 1. seviyeye yükselt',
        type: 'building_level',
        building: 'temple',
        level: 1,
        completed: false
      }
    ],

    // Görev tamamlandığında ödüller
    rewards: {
      unlockBuildings: ['barracks'], // Kışla binasının kilidi açılır
      unlockStoryScenes: ['quest_1_complete'], // Yeni hikaye sahnesi
      resources: {
        essence: 100,
        materials: 50
      }
    },

    status: 'locked' // locked, unlocked, in_progress, completed
  },

  military_foundation: {
    id: 'military_foundation',
    chapter: 1,
    title: 'Savunmanın Temelleri',
    description: 'Köyü korumak için askeri yapılar inşa et.',

    requirements: {
      buildings: {
        town_hall: 1 // Belediye binası en az 1. seviye olmalı
      },
      previousQuests: ['discovery_begins']
    },

    objectives: [
      {
        id: 'build_barracks',
        description: 'Kışlayı 1. seviyeye yükselt',
        type: 'building_level',
        building: 'barracks',
        level: 1,
        completed: false
      },
      {
        id: 'recruit_population',
        description: '10 nüfusa ulaş',
        type: 'population',
        target: 10,
        completed: false
      }
    ],

    rewards: {
      unlockBuildings: ['workshop'],
      unlockStoryScenes: ['quest_2_complete'],
      resources: {
        essence: 150,
        materials: 75,
        rare_materials: 10
      }
    },

    status: 'locked'
  },

  craft_and_trade: {
    id: 'craft_and_trade',
    chapter: 2,
    title: 'Zanaat ve Ticaret',
    description: 'Ekonomiyi geliştirmek için atölye ve pazar kur.',

    requirements: {
      buildings: {
        town_hall: 2,
        barracks: 1
      },
      previousQuests: ['military_foundation']
    },

    objectives: [
      {
        id: 'build_workshop',
        description: 'Atölyeyi 2. seviyeye yükselt',
        type: 'building_level',
        building: 'workshop',
        level: 2,
        completed: false
      },
      {
        id: 'build_market',
        description: 'Pazarı 1. seviyeye yükselt',
        type: 'building_level',
        building: 'market',
        level: 1,
        completed: false
      }
    ],

    rewards: {
      unlockBuildings: ['library'],
      unlockStoryScenes: ['quest_3_complete'],
      resources: {
        essence: 200,
        materials: 100,
        rare_materials: 25
      }
    },

    status: 'locked'
  },

  knowledge_awakens: {
    id: 'knowledge_awakens',
    chapter: 2,
    title: 'Bilginin Uyanışı',
    description: 'Eski bilgileri toplamak için kütüphane inşa et.',

    requirements: {
      buildings: {
        temple: 2,
        workshop: 2
      },
      previousQuests: ['craft_and_trade']
    },

    objectives: [
      {
        id: 'build_library',
        description: 'Kütüphaneyi 2. seviyeye yükselt',
        type: 'building_level',
        building: 'library',
        level: 2,
        completed: false
      },
      {
        id: 'reach_population',
        description: '20 nüfusa ulaş',
        type: 'population',
        target: 20,
        completed: false
      }
    ],

    rewards: {
      unlockStoryScenes: ['ancient_knowledge_revealed'],
      resources: {
        essence: 300,
        materials: 150,
        rare_materials: 50,
        legendary_materials: 5
      }
    },

    status: 'locked'
  },

  the_dark_secret: {
    id: 'the_dark_secret',
    chapter: 3,
    title: 'Karanlık Sır',
    description: 'Tapınağın derinliklerindeki sırrı keşfet.',

    requirements: {
      buildings: {
        temple: 3,
        library: 2
      },
      previousQuests: ['knowledge_awakens']
    },

    objectives: [
      {
        id: 'temple_level_3',
        description: 'Tapınağı 3. seviyeye yükselt',
        type: 'building_level',
        building: 'temple',
        level: 3,
        completed: false
      },
      {
        id: 'all_buildings_level_2',
        description: 'Tüm binaları en az 2. seviyeye yükselt',
        type: 'all_buildings_min_level',
        level: 2,
        completed: false
      }
    ],

    rewards: {
      unlockStoryScenes: ['dark_secret_revealed', 'final_choice'],
      resources: {
        essence: 500,
        materials: 250,
        rare_materials: 100,
        legendary_materials: 20
      }
    },

    status: 'locked'
  },

  restoration_complete: {
    id: 'restoration_complete',
    chapter: 3,
    title: 'Restorasyonun Tamamlanması',
    description: 'Köyü eski ihtişamına kavuştur.',

    requirements: {
      buildings: {
        town_hall: 5,
        temple: 4,
        barracks: 4,
        workshop: 4,
        market: 4,
        library: 4
      },
      previousQuests: ['the_dark_secret']
    },

    objectives: [
      {
        id: 'max_all_buildings',
        description: 'Tüm binaları maksimum seviyeye yükselt',
        type: 'all_buildings_max_level',
        completed: false
      },
      {
        id: 'reach_max_population',
        description: '50 nüfusa ulaş',
        type: 'population',
        target: 50,
        completed: false
      }
    ],

    rewards: {
      unlockStoryScenes: ['epilogue', 'true_ending'],
      resources: {
        essence: 1000,
        materials: 500,
        rare_materials: 200,
        legendary_materials: 50
      }
    },

    status: 'locked'
  }
};

/**
 * Görev bölümleri (hikaye akışı için)
 */
export const questChapters = {
  1: {
    id: 1,
    title: 'Bölüm I: Yeni Umutlar',
    description: 'Köyün yeniden inşası başlar.',
    quests: ['discovery_begins', 'military_foundation']
  },
  2: {
    id: 2,
    title: 'Bölüm II: Büyüme ve Gelişme',
    description: 'Köy gelişir ve bilgi arayışı başlar.',
    quests: ['craft_and_trade', 'knowledge_awakens']
  },
  3: {
    id: 3,
    title: 'Bölüm III: Gerçeğin Peşinde',
    description: 'Eski sırlar ortaya çıkar ve nihai karar verilir.',
    quests: ['the_dark_secret', 'restoration_complete']
  }
};

/**
 * Başlangıçta kilidi açık olan binalar
 */
export const initialUnlockedBuildings = [
  'town_hall',
  'temple'
];

/**
 * Tüm binalar ve kilit durumları
 */
export const buildingUnlockMap = {
  town_hall: {
    unlocked: true, // Başlangıçta açık
    unlockedBy: null
  },
  temple: {
    unlocked: true, // Başlangıçta açık
    unlockedBy: null
  },
  barracks: {
    unlocked: false,
    unlockedBy: 'discovery_begins' // Bu görev tamamlanınca açılır
  },
  workshop: {
    unlocked: false,
    unlockedBy: 'military_foundation'
  },
  market: {
    unlocked: false,
    unlockedBy: 'military_foundation'
  },
  library: {
    unlocked: false,
    unlockedBy: 'craft_and_trade'
  }
};
