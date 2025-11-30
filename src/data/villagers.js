// Villager data structure for Veilborn
// Villagers can be assigned to buildings to generate resources

export const villagerData = {
  // Villager types and their capabilities
  types: {
    worker: {
      id: "worker",
      name: "Worker",
      description: "General laborers who can work in any building",
      icon: "👷",
      baseProduction: {
        essence: 0.5,
        materials: 1
      },
      canWorkAt: ["town_hall", "workshop", "barracks", "market"]
    },

    miner: {
      id: "miner",
      name: "Miner",
      description: "Specialists in gathering materials from the earth",
      icon: "⛏️",
      baseProduction: {
        materials: 3,
        rare_materials: 0.2
      },
      canWorkAt: ["workshop", "town_hall"]
    },

    farmer: {
      id: "farmer",
      name: "Farmer",
      description: "Cultivators who provide sustenance and materials",
      icon: "🌾",
      baseProduction: {
        materials: 2,
        essence: 0.3
      },
      canWorkAt: ["market", "town_hall"]
    },

    scholar: {
      id: "scholar",
      name: "Scholar",
      description: "Learned individuals who study the arcane",
      icon: "📖",
      baseProduction: {
        essence: 2,
        rare_materials: 0.1
      },
      canWorkAt: ["library", "temple"]
    },

    priest: {
      id: "priest",
      name: "Priest",
      description: "Devotees who channel the power of the Veil",
      icon: "⛪",
      baseProduction: {
        essence: 3
      },
      canWorkAt: ["temple"]
    },

    warrior: {
      id: "warrior",
      name: "Warrior",
      description: "Defenders who protect the village",
      icon: "⚔️",
      baseProduction: {
        materials: 0.5
      },
      canWorkAt: ["barracks"]
    },

    merchant: {
      id: "merchant",
      name: "Merchant",
      description: "Traders who bring wealth to the village",
      icon: "💰",
      baseProduction: {
        essence: 1,
        materials: 1,
        rare_materials: 0.3
      },
      canWorkAt: ["market"]
    },

    craftsman: {
      id: "craftsman",
      name: "Craftsman",
      description: "Skilled artisans who work wonders with materials",
      icon: "🔧",
      baseProduction: {
        materials: 1,
        rare_materials: 0.5
      },
      canWorkAt: ["workshop"]
    }
  },

  // Requirements for villagers to appear
  unlockRequirements: {
    worker: { town_hall: 1 },
    miner: { town_hall: 1 },
    farmer: { town_hall: 1 },
    scholar: { library: 1 },
    priest: { temple: 1 },
    warrior: { barracks: 1 },
    merchant: { market: 1 },
    craftsman: { workshop: 1 }
  },

  // How many villagers can work at each building based on level
  capacityByBuildingLevel: {
    town_hall: [0, 2, 4, 6, 8, 10],
    temple: [0, 2, 4, 6, 8],
    barracks: [0, 3, 6, 9],
    workshop: [0, 2, 4, 6],
    market: [0, 2, 4, 6],
    library: [0, 2, 4]
  },

  // Production multipliers based on building level
  buildingLevelMultiplier: [1, 1.5, 2, 2.5, 3, 4],

  // How villagers arrive
  arrivalMethods: {
    initial: "Some survivors gather when the village is first discovered",
    building_upgrade: "New arrivals come as the village grows",
    story_events: "Travelers join through your journey",
    recruitment: "Active recruitment from distant lands"
  }
}

// Utility functions
export const getVillagerType = (typeId) => {
  return villagerData.types[typeId] || null
}

export const getAllVillagerTypes = () => {
  return Object.values(villagerData.types)
}

export const canVillagerWorkAt = (villagerTypeId, buildingId) => {
  const villagerType = getVillagerType(villagerTypeId)
  if (!villagerType) return false
  return villagerType.canWorkAt.includes(buildingId)
}

export const getBuildingCapacity = (buildingId, buildingLevel) => {
  const capacities = villagerData.capacityByBuildingLevel[buildingId]
  if (!capacities) return 0
  return capacities[buildingLevel] || 0
}

export const getProductionMultiplier = (buildingLevel) => {
  return villagerData.buildingLevelMultiplier[buildingLevel] || 1
}

export const calculateProduction = (villagerTypeId, buildingLevel) => {
  const villagerType = getVillagerType(villagerTypeId)
  if (!villagerType) return {}

  const multiplier = getProductionMultiplier(buildingLevel)
  const production = {}

  for (const [resource, amount] of Object.entries(villagerType.baseProduction)) {
    production[resource] = amount * multiplier
  }

  return production
}
