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

export function useVillageState() {
  // Initialize village (when player first discovers it)
  const discoverVillage = () => {
    villageDiscovered.value = true
    // Give some initial resources to get started
    resources.value.essence = 100
    resources.value.materials = 50
  }

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

      return true
    } catch (error) {
      console.error('Failed to load village state:', error)
      return false
    }
  }

  // Reset village (for new game)
  const resetVillage = () => {
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
    saveVillageState()
  }

  return {
    // State
    buildingLevels,
    resources,
    population,
    villageDiscovered,

    // Computed
    villageProgress,
    villageTier,

    // Methods
    discoverVillage,
    getBuildingLevel,
    canUpgrade,
    upgradeBuilding,
    addResources,
    saveVillageState,
    loadVillageState,
    resetVillage
  }
}
