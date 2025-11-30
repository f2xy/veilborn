import { ref, computed } from 'vue'
import { getAllBuildings } from '../data/village.js'

// Village state management
const buildingLevels = ref({
  town_hall: 0,
  temple: 0,
  barracks: 0,
  workshop: 0,
  market: 0,
  library: 0
})

const resources = ref({
  essence: 0,
  materials: 0,
  rare_materials: 0,
  legendary_materials: 0
})

const population = ref(0)
const villageDiscovered = ref(false)

// Population assignment to buildings
const buildingAssignments = ref({
  town_hall: 0,
  temple: 0,
  barracks: 0,
  workshop: 0,
  market: 0,
  library: 0
})

// Resource production timer
let productionInterval = null
const lastProductionTime = ref(Date.now())

// Building capacity and production configuration
const buildingConfig = {
  town_hall: {
    capacity: [0, 2, 4, 6, 8, 10],
    production: { essence: 0.5, materials: 1 }
  },
  temple: {
    capacity: [0, 2, 4, 6, 8],
    production: { essence: 2, rare_materials: 0.1 }
  },
  barracks: {
    capacity: [0, 3, 6, 9],
    production: { materials: 0.5 }
  },
  workshop: {
    capacity: [0, 2, 4, 6],
    production: { materials: 2, rare_materials: 0.3 }
  },
  market: {
    capacity: [0, 2, 4, 6],
    production: { essence: 1, materials: 1, rare_materials: 0.2 }
  },
  library: {
    capacity: [0, 2, 4],
    production: { essence: 1.5, rare_materials: 0.2 }
  }
}

// Production multipliers based on building level
const levelMultipliers = [1, 1.5, 2, 2.5, 3, 4]

export function useVillageState() {
  // Initialize village (when player first discovers it)
  const discoverVillage = () => {
    villageDiscovered.value = true
    // Give some initial resources to get started
    resources.value.essence = 100
    resources.value.materials = 50

    // Add initial population
    population.value = 5

    // Start resource production
    startProduction()
  }

  // Helper functions
  const getBuildingCapacity = (buildingId) => {
    const level = getBuildingLevel(buildingId)
    const config = buildingConfig[buildingId]
    if (!config) return 0
    return config.capacity[level] || 0
  }

  const getAssignedPopulation = computed(() => {
    return Object.values(buildingAssignments.value).reduce((sum, count) => sum + count, 0)
  })

  const getUnassignedPopulation = computed(() => {
    return population.value - getAssignedPopulation.value
  })

  // Population assignment functions
  const assignPopulation = (buildingId, amount = 1) => {
    const capacity = getBuildingCapacity(buildingId)
    const currentAssigned = buildingAssignments.value[buildingId] || 0
    const available = getUnassignedPopulation.value

    if (available < amount) return false
    if (currentAssigned + amount > capacity) return false

    buildingAssignments.value[buildingId] = currentAssigned + amount
    saveVillageState()
    return true
  }

  const unassignPopulation = (buildingId, amount = 1) => {
    const currentAssigned = buildingAssignments.value[buildingId] || 0
    if (currentAssigned < amount) return false

    buildingAssignments.value[buildingId] = currentAssigned - amount
    saveVillageState()
    return true
  }

  const addPopulation = (amount = 1) => {
    population.value += amount
    saveVillageState()
  }

  // Resource production system
  const calculateTotalProduction = () => {
    const production = {
      essence: 0,
      materials: 0,
      rare_materials: 0,
      legendary_materials: 0
    }

    // Calculate production from each building
    for (const [buildingId, assignedPopCount] of Object.entries(buildingAssignments.value)) {
      if (assignedPopCount === 0) continue

      const buildingLevel = getBuildingLevel(buildingId)
      const config = buildingConfig[buildingId]

      if (!config) continue

      // Get level multiplier
      const multiplier = levelMultipliers[buildingLevel] || 1

      // Calculate production for this building
      for (const [resource, baseAmount] of Object.entries(config.production)) {
        const amount = baseAmount * multiplier * assignedPopCount
        production[resource] = (production[resource] || 0) + amount
      }
    }

    return production
  }

  const produceResources = () => {
    const now = Date.now()
    const timePassed = (now - lastProductionTime.value) / 1000 // seconds
    lastProductionTime.value = now

    const productionPerSecond = calculateTotalProduction()

    // Add resources based on time passed
    for (const [resource, perSecond] of Object.entries(productionPerSecond)) {
      const produced = perSecond * timePassed
      if (produced > 0) {
        addResources(resource, Math.floor(produced * 10) / 10) // Round to 1 decimal
      }
    }
  }

  const startProduction = () => {
    if (productionInterval) {
      clearInterval(productionInterval)
    }

    lastProductionTime.value = Date.now()

    // Produce resources every second
    productionInterval = setInterval(() => {
      produceResources()
    }, 1000)
  }

  const stopProduction = () => {
    if (productionInterval) {
      clearInterval(productionInterval)
      productionInterval = null
    }
  }

  // Get production rate per second
  const getProductionRate = computed(() => {
    return calculateTotalProduction()
  })

  // Get building level
  const getBuildingLevel = (buildingId) => {
    return buildingLevels.value[buildingId] || 0
  }

  // Check if building can be upgraded
  const canUpgrade = (buildingId, cost) => {
    if (!cost) return true // No cost means it's the initial ruins level

    return (
      resources.value.essence >= (cost.essence || 0) &&
      resources.value.materials >= (cost.materials || 0) &&
      resources.value.rare_materials >= (cost.rare_materials || 0) &&
      resources.value.legendary_materials >= (cost.legendary_materials || 0)
    )
  }

  // Upgrade a building
  const upgradeBuilding = (buildingId, cost) => {
    if (!canUpgrade(buildingId, cost)) {
      return false
    }

    // Deduct resources
    if (cost) {
      resources.value.essence -= (cost.essence || 0)
      resources.value.materials -= (cost.materials || 0)
      resources.value.rare_materials -= (cost.rare_materials || 0)
      resources.value.legendary_materials -= (cost.legendary_materials || 0)
    }

    // Increase building level
    buildingLevels.value[buildingId]++

    // Save state
    saveVillageState()

    return true
  }

  // Add resources (earned through gameplay)
  const addResources = (resourceType, amount) => {
    if (resources.value.hasOwnProperty(resourceType)) {
      resources.value[resourceType] += amount
      saveVillageState()
    }
  }

  // Get total village progress (0-100%)
  const villageProgress = computed(() => {
    const buildings = getAllBuildings()
    let totalLevels = 0
    let maxPossibleLevels = 0

    buildings.forEach(building => {
      const currentLevel = buildingLevels.value[building.id] || 0
      totalLevels += currentLevel
      maxPossibleLevels += building.maxLevel
    })

    return maxPossibleLevels > 0
      ? Math.round((totalLevels / maxPossibleLevels) * 100)
      : 0
  })

  // Get village tier based on progress
  const villageTier = computed(() => {
    const progress = villageProgress.value
    if (progress === 0) return 'ruins'
    if (progress < 20) return 'settlement'
    if (progress < 40) return 'village'
    if (progress < 60) return 'town'
    if (progress < 80) return 'stronghold'
    return 'citadel'
  })

  // Save village state to localStorage
  const saveVillageState = () => {
    const villageState = {
      buildingLevels: buildingLevels.value,
      resources: resources.value,
      population: population.value,
      villageDiscovered: villageDiscovered.value,
      buildingAssignments: buildingAssignments.value,
      lastProductionTime: lastProductionTime.value,
      timestamp: Date.now()
    }

    localStorage.setItem('veilborn_village', JSON.stringify(villageState))
  }

  // Load village state from localStorage
  const loadVillageState = () => {
    const savedData = localStorage.getItem('veilborn_village')

    if (!savedData) return false

    try {
      const villageState = JSON.parse(savedData)

      buildingLevels.value = villageState.buildingLevels || {}
      resources.value = villageState.resources || {}
      population.value = villageState.population || 0
      villageDiscovered.value = villageState.villageDiscovered || false
      buildingAssignments.value = villageState.buildingAssignments || {
        town_hall: 0,
        temple: 0,
        barracks: 0,
        workshop: 0,
        market: 0,
        library: 0
      }
      lastProductionTime.value = villageState.lastProductionTime || Date.now()

      // Restart production if village is discovered
      if (villageDiscovered.value) {
        startProduction()
      }

      return true
    } catch (error) {
      console.error('Failed to load village state:', error)
      return false
    }
  }

  // Reset village (for new game)
  const resetVillage = () => {
    stopProduction()

    buildingLevels.value = {
      town_hall: 0,
      temple: 0,
      barracks: 0,
      workshop: 0,
      market: 0,
      library: 0
    }
    resources.value = {
      essence: 0,
      materials: 0,
      rare_materials: 0,
      legendary_materials: 0
    }
    population.value = 0
    villageDiscovered.value = false
    buildingAssignments.value = {
      town_hall: 0,
      temple: 0,
      barracks: 0,
      workshop: 0,
      market: 0,
      library: 0
    }
    lastProductionTime.value = Date.now()

    saveVillageState()
  }

  return {
    // State
    buildingLevels,
    resources,
    population,
    villageDiscovered,
    buildingAssignments,

    // Computed
    villageProgress,
    villageTier,
    getProductionRate,
    getAssignedPopulation,
    getUnassignedPopulation,

    // Building Methods
    getBuildingLevel,
    getBuildingCapacity,
    canUpgrade,
    upgradeBuilding,
    addResources,

    // Population Methods
    addPopulation,
    assignPopulation,
    unassignPopulation,

    // Production Methods
    startProduction,
    stopProduction,

    // Persistence Methods
    discoverVillage,
    saveVillageState,
    loadVillageState,
    resetVillage
  }
}
