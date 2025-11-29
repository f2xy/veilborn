import { ref, computed } from 'vue'
import { getAllBuildings } from '../data/village.js'
import {
  getAllVillagerTypes,
  getBuildingCapacity,
  calculateProduction,
  canVillagerWorkAt
} from '../data/villagers.js'

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

// Villager management
const villagers = ref([])
const nextVillagerId = ref(1)
const buildingAssignments = ref({
  town_hall: [],
  temple: [],
  barracks: [],
  workshop: [],
  market: [],
  library: []
})

// Resource production timer
let productionInterval = null
const lastProductionTime = ref(Date.now())

export function useVillageState() {
  // Initialize village (when player first discovers it)
  const discoverVillage = () => {
    villageDiscovered.value = true
    // Give some initial resources to get started
    resources.value.essence = 100
    resources.value.materials = 50

    // Add initial villagers
    addVillager('worker')
    addVillager('worker')
    addVillager('miner')

    // Start resource production
    startProduction()
  }

  // Villager management functions
  const addVillager = (typeId) => {
    const newVillager = {
      id: nextVillagerId.value++,
      type: typeId,
      assignedTo: null,
      happiness: 100
    }
    villagers.value.push(newVillager)
    population.value = villagers.value.length
    saveVillageState()
    return newVillager
  }

  const assignVillager = (villagerId, buildingId) => {
    const villager = villagers.value.find(v => v.id === villagerId)
    if (!villager) return false

    // Check if villager can work at this building
    if (!canVillagerWorkAt(villager.type, buildingId)) {
      return false
    }

    // Check building capacity
    const buildingLevel = getBuildingLevel(buildingId)
    const capacity = getBuildingCapacity(buildingId, buildingLevel)
    const currentAssigned = buildingAssignments.value[buildingId]?.length || 0

    if (currentAssigned >= capacity) {
      return false
    }

    // Unassign from previous building if any
    if (villager.assignedTo) {
      unassignVillager(villagerId)
    }

    // Assign to new building
    villager.assignedTo = buildingId
    if (!buildingAssignments.value[buildingId]) {
      buildingAssignments.value[buildingId] = []
    }
    buildingAssignments.value[buildingId].push(villagerId)

    saveVillageState()
    return true
  }

  const unassignVillager = (villagerId) => {
    const villager = villagers.value.find(v => v.id === villagerId)
    if (!villager || !villager.assignedTo) return false

    const buildingId = villager.assignedTo
    const assignments = buildingAssignments.value[buildingId]
    if (assignments) {
      const index = assignments.indexOf(villagerId)
      if (index > -1) {
        assignments.splice(index, 1)
      }
    }

    villager.assignedTo = null
    saveVillageState()
    return true
  }

  const getVillagersAtBuilding = (buildingId) => {
    const assignedIds = buildingAssignments.value[buildingId] || []
    return villagers.value.filter(v => assignedIds.includes(v.id))
  }

  const getUnassignedVillagers = () => {
    return villagers.value.filter(v => !v.assignedTo)
  }

  const getBuildingAssignmentCount = (buildingId) => {
    return buildingAssignments.value[buildingId]?.length || 0
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
    for (const [buildingId, assignedVillagerIds] of Object.entries(buildingAssignments.value)) {
      const buildingLevel = getBuildingLevel(buildingId)

      assignedVillagerIds.forEach(villagerId => {
        const villager = villagers.value.find(v => v.id === villagerId)
        if (villager) {
          const villagerProduction = calculateProduction(villager.type, buildingLevel)

          for (const [resource, amount] of Object.entries(villagerProduction)) {
            production[resource] = (production[resource] || 0) + amount
          }
        }
      })
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
      villagers: villagers.value,
      nextVillagerId: nextVillagerId.value,
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
      villagers.value = villageState.villagers || []
      nextVillagerId.value = villageState.nextVillagerId || 1
      buildingAssignments.value = villageState.buildingAssignments || {}
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
    villagers.value = []
    nextVillagerId.value = 1
    buildingAssignments.value = {
      town_hall: [],
      temple: [],
      barracks: [],
      workshop: [],
      market: [],
      library: []
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
    villagers,
    buildingAssignments,

    // Computed
    villageProgress,
    villageTier,
    getProductionRate,

    // Building Methods
    getBuildingLevel,
    canUpgrade,
    upgradeBuilding,
    addResources,

    // Villager Methods
    addVillager,
    assignVillager,
    unassignVillager,
    getVillagersAtBuilding,
    getUnassignedVillagers,
    getBuildingAssignmentCount,

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
