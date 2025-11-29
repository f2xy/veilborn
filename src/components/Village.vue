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
            <span class="resource-amount">{{ amount }}</span>
          </div>
        </div>
      </div>

      <!-- Village Actions -->
      <div class="village-actions">
        <button class="action-btn" @click="$emit('navigate', 'gameplay')" title="Continue Journey">
          <span>⚔️ Continue Journey</span>
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
          :class="getBuildingClass(building)"
        >
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
          <p>You stand before the ruins of a once-great village. The buildings are crumbling, the streets overgrown with weeds, and an eerie silence hangs in the air.</p>
          <p>But you sense potential here - with time and effort, this place could rise again. Your journey has brought you here for a reason.</p>
          <p>As you explore, you find some basic resources scattered among the ruins. This will be your new home, a sanctuary in the darkness.</p>
          <button class="welcome-btn" @click="closeWelcome">Begin Restoration</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useVillageState } from '../composables/useVillageState.js'
import { getAllBuildings, getBuildingLevel } from '../data/village.js'

export default {
  name: 'Village',
  emits: ['navigate'],
  setup() {
    const villageState = useVillageState()
    const buildings = ref(getAllBuildings())
    const showWelcome = ref(false)
    const villageName = ref("Forgotten Haven")

    const villageTierText = computed(() => {
      const tier = villageState.villageTier.value
      const tierNames = {
        ruins: 'Ruins',
        settlement: 'Small Settlement',
        village: 'Growing Village',
        town: 'Prosperous Town',
        stronghold: 'Fortified Stronghold',
        citadel: 'Grand Citadel'
      }
      return tierNames[tier] || 'Unknown'
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
      if (level === 0) return 'Abandoned Ruins'
      if (level === building.maxLevel) return 'Fully Restored'
      return `Level ${level} of ${building.maxLevel}`
    }

    const getBuildingIcon = (buildingId) => {
      const icons = {
        town_hall: '🏛️',
        temple: '⛩️',
        barracks: '⚔️',
        workshop: '🔨',
        market: '🏪',
        library: '📚'
      }
      return icons[buildingId] || '🏗️'
    }

    const getResourceIcon = (resourceType) => {
      const icons = {
        essence: '✨',
        materials: '🪨',
        rare_materials: '💎',
        legendary_materials: '⭐'
      }
      return icons[resourceType] || '📦'
    }

    const handleUpgrade = (building) => {
      const nextLevel = nextLevelData(building)
      if (nextLevel && villageState.upgradeBuilding(building.id, nextLevel.cost)) {
        // Success feedback
        console.log(`Upgraded ${building.name} to level ${currentLevel(building.id)}`)
      }
    }

    const closeWelcome = () => {
      showWelcome.value = false
      villageState.saveVillageState()
    }

    onMounted(() => {
      // Check if this is first visit
      if (!villageState.villageDiscovered.value) {
        villageState.discoverVillage()
        showWelcome.value = true
      }
    })

    return {
      villageState,
      buildings,
      showWelcome,
      villageName,
      villageTierText,
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
      closeWelcome
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
  padding: 2rem;
}

.stars {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image:
    radial-gradient(2px 2px at 20% 30%, white, transparent),
    radial-gradient(2px 2px at 60% 70%, white, transparent),
    radial-gradient(1px 1px at 50% 50%, white, transparent);
  background-size: 200% 200%;
  animation: twinkle 8s ease-in-out infinite;
  opacity: 0.3;
  pointer-events: none;
}

@keyframes twinkle {
  0%, 100% { opacity: 0.3; }
  50% { opacity: 0.15; }
}

.village-container {
  position: relative;
  z-index: 1;
  max-width: 1400px;
  margin: 0 auto;
}

.village-header {
  background: rgba(20, 15, 10, 0.8);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 12px;
  padding: 2rem;
  margin-bottom: 2rem;
  backdrop-filter: blur(10px);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1.5rem;
}

.village-name {
  font-family: 'Cinzel', serif;
  font-size: 2.5rem;
  color: #e8d5f2;
  margin-bottom: 0.5rem;
  text-shadow: 0 0 20px rgba(168, 85, 247, 0.4);
}

.village-tier {
  font-family: 'Lato', sans-serif;
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 1rem;
}

.progress-bar {
  width: 300px;
  height: 24px;
  background: rgba(0, 0, 0, 0.5);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 12px;
  position: relative;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #6d28d9, #7e3af2);
  transition: width 0.5s ease;
}

.progress-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-family: 'Lato', sans-serif;
  font-size: 0.85rem;
  color: white;
  font-weight: 600;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.8);
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
  padding: 0.75rem 1.25rem;
  background: rgba(0, 0, 0, 0.4);
  border: 1px solid rgba(139, 92, 246, 0.2);
  border-radius: 8px;
}

.resource-icon {
  font-size: 1.5rem;
}

.resource-amount {
  font-family: 'Lato', sans-serif;
  font-size: 1.1rem;
  color: white;
  font-weight: 600;
}

.village-actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  justify-content: center;
}

.action-btn {
  font-family: 'Lato', sans-serif;
  padding: 0.9rem 2rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.6), rgba(126, 58, 242, 0.6));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  font-size: 0.95rem;
}

.action-btn:hover {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  transform: translateY(-2px);
}

.buildings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 1.5rem;
}

.building-card {
  background: rgba(20, 15, 10, 0.7);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 12px;
  padding: 1.5rem;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.building-card:hover {
  border-color: rgba(168, 85, 247, 0.5);
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(109, 40, 217, 0.3);
}

.building-card.visual-ruins {
  border-color: rgba(100, 100, 100, 0.3);
  opacity: 0.8;
}

.building-card.visual-basic {
  border-color: rgba(139, 92, 246, 0.4);
}

.building-card.visual-restored {
  border-color: rgba(168, 85, 247, 0.5);
}

.building-card.visual-fortified {
  border-color: rgba(147, 51, 234, 0.6);
  box-shadow: 0 4px 16px rgba(147, 51, 234, 0.2);
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
  font-family: 'Lato', sans-serif;
  font-size: 0.85rem;
  padding: 0.4rem 0.8rem;
  background: rgba(109, 40, 217, 0.5);
  border-radius: 12px;
  color: white;
  font-weight: 600;
}

.building-name {
  font-family: 'Cinzel', serif;
  font-size: 1.5rem;
  color: #e8d5f2;
  margin-bottom: 0.5rem;
}

.building-status {
  font-family: 'Lato', sans-serif;
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.6);
  margin-bottom: 0.75rem;
}

.building-description {
  font-family: 'Lato', sans-serif;
  font-size: 0.95rem;
  color: rgba(255, 255, 255, 0.75);
  line-height: 1.6;
  margin-bottom: 1rem;
}

.level-info {
  background: rgba(0, 0, 0, 0.3);
  border-radius: 8px;
  padding: 1rem;
  margin-bottom: 1rem;
}

.level-name {
  font-family: 'Lato', sans-serif;
  font-size: 1rem;
  color: #a78bfa;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.level-desc {
  font-family: 'Lato', sans-serif;
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 0.75rem;
}

.unlocks {
  margin-top: 0.75rem;
}

.unlocks-title {
  font-family: 'Lato', sans-serif;
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.6);
  margin-bottom: 0.4rem;
}

.unlocks-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.unlocks-list li {
  font-family: 'Lato', sans-serif;
  font-size: 0.85rem;
  color: #60a5fa;
  margin-bottom: 0.25rem;
}

.upgrade-section {
  border-top: 1px solid rgba(139, 92, 246, 0.2);
  padding-top: 1rem;
  margin-top: 1rem;
}

.cost-title {
  font-family: 'Lato', sans-serif;
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.6);
  margin-bottom: 0.5rem;
}

.cost-items {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1rem;
}

.cost-item {
  font-family: 'Lato', sans-serif;
  font-size: 0.9rem;
  color: white;
  padding: 0.4rem 0.8rem;
  background: rgba(109, 40, 217, 0.3);
  border-radius: 6px;
}

.cost-item.insufficient {
  background: rgba(217, 40, 40, 0.3);
  color: #ff6b6b;
}

.upgrade-btn {
  font-family: 'Lato', sans-serif;
  width: 100%;
  padding: 0.9rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.6), rgba(126, 58, 242, 0.6));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 0.95rem;
  font-weight: 600;
}

.upgrade-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  transform: translateY(-2px);
}

.upgrade-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

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
  padding: 2rem;
}

.welcome-content {
  max-width: 600px;
  background: rgba(20, 15, 10, 0.95);
  border: 2px solid rgba(139, 92, 246, 0.5);
  border-radius: 12px;
  padding: 3rem;
  text-align: center;
}

.welcome-content h2 {
  font-family: 'Cinzel', serif;
  font-size: 2.5rem;
  color: #e8d5f2;
  margin-bottom: 1.5rem;
  text-shadow: 0 0 20px rgba(168, 85, 247, 0.4);
}

.welcome-content p {
  font-family: 'Lato', sans-serif;
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.85);
  line-height: 1.8;
  margin-bottom: 1.5rem;
}

.welcome-btn {
  font-family: 'Lato', sans-serif;
  padding: 1rem 2.5rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.6), rgba(126, 58, 242, 0.6));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 1.1rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.welcome-btn:hover {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  transform: translateY(-2px);
}

/* Responsive */
@media (max-width: 768px) {
  .buildings-grid {
    grid-template-columns: 1fr;
  }

  .village-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .progress-bar {
    width: 100%;
  }
}
</style>
