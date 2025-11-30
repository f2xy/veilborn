<template>
  <div class="gameplay">
    <div class="stars"></div>

    <div class="game-container">
      <!-- Header with player stats -->
      <div class="game-header">
        <div class="player-info">
          <div class="trait-info" v-if="gameState.dominantTrait.value">
            <span class="label">Trait:</span>
            <span class="value">{{ formatTrait(gameState.dominantTrait.value) }}</span>
          </div>
          <div class="alignment-info">
            <span class="label">Path:</span>
            <span class="value" :class="`alignment-${gameState.dominantAlignment.value}`">
              {{ formatAlignment(gameState.dominantAlignment.value) }}
            </span>
          </div>
        </div>

        <div class="game-actions">
          <button class="action-btn" @click="handleSave" title="Save Game">
            <span>💾</span>
          </button>
          <button class="action-btn" @click="handleMenu" title="Menu">
            <span>☰</span>
          </button>
        </div>
      </div>

      <!-- Available Story Scenes from Quest Completion -->
      <div class="available-scenes" v-if="gameState.availableStoryScenes.value.length > 0">
        <h3 class="available-scenes-title">📜 Yeni Hikaye Sahneleri</h3>
        <div class="scene-list">
          <button
            v-for="sceneId in gameState.availableStoryScenes.value"
            :key="sceneId"
            class="scene-button"
            @click="navigateToAvailableScene(sceneId)"
          >
            <span class="scene-icon">📖</span>
            <span class="scene-name">{{ getSceneTitle(sceneId) }}</span>
            <span class="scene-arrow">→</span>
          </button>
        </div>
      </div>

      <!-- Main content area -->
      <div class="game-content">
        <div class="scene-container" v-if="currentScene">
          <h2 class="scene-title">{{ currentScene.title }}</h2>

          <div class="scene-text">
            <p
              v-for="(paragraph, index) in currentScene.text"
              :key="index"
              class="scene-paragraph"
              :class="{ 'visible': visibleParagraphs[index] }"
            >
              {{ paragraph }}
            </p>
          </div>

          <div class="scene-choices" v-if="showChoices && currentScene.choices">
            <button
              v-for="choice in currentScene.choices"
              :key="choice.id"
              class="choice-button"
              @click="handleChoice(choice)"
            >
              <span class="choice-text">{{ choice.text }}</span>
              <span class="choice-arrow">→</span>
            </button>
          </div>
        </div>

        <div v-else class="error-message">
          <p>Scene not found. The veil has grown too thick...</p>
          <button class="choice-button" @click="$emit('navigate', 'menu')">
            Return to Menu
          </button>
        </div>
      </div>

      <!-- Choice history (optional - can be toggled) -->
      <div class="history-toggle" v-if="gameState.playerChoices.value.length > 0">
        <button @click="showHistory = !showHistory" class="toggle-btn">
          {{ showHistory ? 'Hide' : 'View' }} History ({{ gameState.playerChoices.value.length }})
        </button>
      </div>

      <div class="choice-history" v-if="showHistory">
        <h3>Your Journey</h3>
        <div class="history-items">
          <div
            v-for="(choice, index) in gameState.playerChoices.value"
            :key="index"
            class="history-item"
          >
            <span class="history-number">{{ index + 1 }}.</span>
            <span class="history-choice">{{ choice.choiceId }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, watch } from 'vue'
import { useGameState } from '../composables/useGameState.js'
import { getScene } from '../data/story.js'

export default {
  name: 'GamePlay',
  emits: ['navigate'],
  setup(props, { emit }) {
    const gameState = useGameState()
    const visibleParagraphs = ref({})
    const showChoices = ref(false)
    const showHistory = ref(false)

    const currentScene = computed(() => {
      return getScene(gameState.currentSceneId.value)
    })

    // Animate paragraphs appearing
    const animateParagraphs = () => {
      visibleParagraphs.value = {}
      showChoices.value = false

      if (!currentScene.value) return

      currentScene.value.text.forEach((_, index) => {
        setTimeout(() => {
          visibleParagraphs.value[index] = true

          // Show choices after last paragraph
          if (index === currentScene.value.text.length - 1) {
            setTimeout(() => {
              showChoices.value = true
            }, 500)
          }
        }, index * 800) // 0.8 second delay between paragraphs
      })
    }

    // Handle choice selection
    const handleChoice = (choice) => {
      // Check if this choice has a special action
      if (choice.action === 'enter_village') {
        gameState.saveGame('autosave')
        emit('navigate', 'village')
        return
      }

      if (choice.action === 'return_to_village') {
        gameState.saveGame('autosave')
        emit('navigate', 'village')
        return
      }

      if (choice.action === 'main_menu') {
        gameState.saveGame('autosave')
        emit('navigate', 'menu')
        return
      }

      gameState.makeChoice(choice, gameState.currentSceneId.value)

      // Auto-save after each choice
      gameState.saveGame('autosave')
    }

    // Navigate to available story scene
    const navigateToAvailableScene = (sceneId) => {
      gameState.navigateToScene(sceneId)
      gameState.saveGame('autosave')
    }

    // Get scene title by ID
    const getSceneTitle = (sceneId) => {
      const scene = getScene(sceneId)
      return scene ? scene.title : 'Unknown Scene'
    }

    // Handle save
    const handleSave = () => {
      gameState.saveGame('manual')
      alert('Game saved!')
    }

    // Handle menu
    const handleMenu = () => {
      if (confirm('Return to menu? (Your progress will be auto-saved)')) {
        gameState.saveGame('autosave')
        emit('navigate', 'menu')
      }
    }

    // Format trait name
    const formatTrait = (trait) => {
      return trait.charAt(0).toUpperCase() + trait.slice(1)
    }

    // Format alignment name
    const formatAlignment = (alignment) => {
      const names = {
        light: 'Light',
        dark: 'Shadow',
        neutral: 'Balance'
      }
      return names[alignment] || 'Unknown'
    }

    // Watch for scene changes
    watch(() => gameState.currentSceneId.value, () => {
      animateParagraphs()
    })

    onMounted(() => {
      animateParagraphs()
    })

    return {
      gameState,
      currentScene,
      visibleParagraphs,
      showChoices,
      showHistory,
      handleChoice,
      navigateToAvailableScene,
      getSceneTitle,
      handleSave,
      handleMenu,
      formatTrait,
      formatAlignment
    }
  }
}
</script>

<style scoped>
.gameplay {
  width: 100%;
  min-height: 100vh;
  position: relative;
  background: linear-gradient(135deg, #0a0e1a 0%, #1a1430 25%, #0f1a2e 50%, #0d1a28 75%, #0a0e1a 100%);
  display: flex;
  align-items: flex-start;
  justify-content: center;
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
    radial-gradient(2px 2px at 20% 30%, white, transparent),
    radial-gradient(2px 2px at 60% 70%, white, transparent),
    radial-gradient(1px 1px at 50% 50%, white, transparent),
    radial-gradient(1px 1px at 80% 10%, white, transparent),
    radial-gradient(2px 2px at 90% 60%, white, transparent),
    radial-gradient(1px 1px at 33% 80%, white, transparent),
    radial-gradient(1px 1px at 15% 90%, white, transparent);
  background-size: 200% 200%;
  animation: twinkle 8s ease-in-out infinite;
  opacity: 0.4;
  pointer-events: none;
}

@keyframes twinkle {
  0%, 100% { opacity: 0.4; }
  50% { opacity: 0.2; }
}

.game-container {
  position: relative;
  z-index: 1;
  max-width: 900px;
  width: 100%;
  margin: 0 auto;
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding: 1rem 1.5rem;
  background: rgba(15, 20, 35, 0.7);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 8px;
  backdrop-filter: blur(10px);
}

.player-info {
  display: flex;
  gap: 2rem;
  font-family: 'Lato', sans-serif;
  font-size: 0.9rem;
}

.trait-info, .alignment-info {
  display: flex;
  gap: 0.5rem;
}

.label {
  color: rgba(255, 255, 255, 0.6);
}

.value {
  color: #e8d5f2;
  font-weight: 600;
}

.alignment-light { color: #fbbf24; }
.alignment-dark { color: #a78bfa; }
.alignment-neutral { color: #60a5fa; }

.game-actions {
  display: flex;
  gap: 0.5rem;
}

/* Available Story Scenes */
.available-scenes {
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: rgba(255, 193, 7, 0.1);
  border: 2px solid rgba(255, 193, 7, 0.4);
  border-radius: 12px;
  backdrop-filter: blur(10px);
}

.available-scenes-title {
  color: #FFC107;
  font-size: 1.3rem;
  margin: 0 0 1rem 0;
  font-weight: bold;
}

.scene-list {
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
}

.scene-button {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem 1.5rem;
  background: rgba(0, 0, 0, 0.4);
  border: 2px solid rgba(255, 193, 7, 0.3);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  text-align: left;
}

.scene-button:hover {
  background: rgba(255, 193, 7, 0.2);
  border-color: rgba(255, 193, 7, 0.6);
  transform: translateX(10px);
}

.scene-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.scene-name {
  flex: 1;
  color: #e8d5f2;
  font-size: 1.1rem;
  font-weight: 500;
}

.scene-arrow {
  color: #FFC107;
  font-size: 1.2rem;
  font-weight: bold;
  flex-shrink: 0;
}

.action-btn {
  width: 40px;
  height: 40px;
  background: rgba(109, 40, 217, 0.4);
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  font-size: 1.2rem;
}

.action-btn:hover {
  background: rgba(126, 58, 242, 0.6);
  border-color: rgba(168, 85, 247, 0.6);
  transform: translateY(-2px);
}

.game-content {
  margin-bottom: 2rem;
}

.scene-container {
  background: rgba(15, 20, 35, 0.7);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 12px;
  padding: 3rem;
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.5);
}

.scene-title {
  font-family: 'Cinzel', serif;
  font-size: 2.2rem;
  font-weight: 600;
  color: #e8d5f2;
  text-align: center;
  margin-bottom: 2rem;
  letter-spacing: 0.1em;
  text-shadow: 0 0 20px rgba(168, 85, 247, 0.4);
}

.scene-text {
  margin-bottom: 2.5rem;
}

.scene-paragraph {
  font-family: 'Lato', sans-serif;
  font-size: 1.1rem;
  line-height: 1.8;
  color: rgba(255, 255, 255, 0.85);
  margin-bottom: 1.5rem;
  text-align: justify;
  opacity: 0;
  transform: translateY(20px);
  transition: opacity 0.6s ease, transform 0.6s ease;
}

.scene-paragraph.visible {
  opacity: 1;
  transform: translateY(0);
}

.scene-paragraph:last-child {
  margin-bottom: 0;
}

.scene-choices {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  opacity: 0;
  animation: fadeIn 0.5s ease forwards;
}

@keyframes fadeIn {
  to { opacity: 1; }
}

.choice-button {
  font-family: 'Lato', sans-serif;
  font-size: 1rem;
  font-weight: 400;
  padding: 1.2rem 1.8rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.5), rgba(126, 58, 242, 0.5));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 15px rgba(109, 40, 217, 0.2);
  display: flex;
  justify-content: space-between;
  align-items: center;
  text-align: left;
}

.choice-button:hover {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.7), rgba(147, 51, 234, 0.7));
  border-color: rgba(168, 85, 247, 0.6);
  transform: translateX(8px);
  box-shadow: 0 6px 20px rgba(126, 58, 242, 0.4);
}

.choice-button:active {
  transform: translateX(4px);
}

.choice-arrow {
  opacity: 0;
  transition: opacity 0.3s ease;
}

.choice-button:hover .choice-arrow {
  opacity: 1;
}

.history-toggle {
  text-align: center;
  margin-bottom: 1rem;
}

.toggle-btn {
  font-family: 'Lato', sans-serif;
  font-size: 0.85rem;
  padding: 0.5rem 1rem;
  background: rgba(109, 40, 217, 0.3);
  color: rgba(255, 255, 255, 0.7);
  border: 1px solid rgba(168, 85, 247, 0.3);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.toggle-btn:hover {
  background: rgba(126, 58, 242, 0.4);
  color: white;
}

.choice-history {
  background: rgba(15, 20, 35, 0.6);
  border: 2px solid rgba(139, 92, 246, 0.2);
  border-radius: 8px;
  padding: 1.5rem;
  backdrop-filter: blur(10px);
}

.choice-history h3 {
  font-family: 'Cinzel', serif;
  font-size: 1.2rem;
  color: #e8d5f2;
  margin-bottom: 1rem;
  text-align: center;
}

.history-items {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.history-item {
  font-family: 'Lato', sans-serif;
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.6);
  padding: 0.5rem;
}

.history-number {
  color: rgba(168, 85, 247, 0.8);
  margin-right: 0.5rem;
}

.error-message {
  text-align: center;
  padding: 3rem;
  color: rgba(255, 255, 255, 0.7);
}

.error-message p {
  font-family: 'Lato', sans-serif;
  font-size: 1.2rem;
  margin-bottom: 2rem;
}

/* Responsive design */
@media (max-width: 768px) {
  .gameplay {
    padding: 1rem;
  }

  .scene-container {
    padding: 2rem;
  }

  .scene-title {
    font-size: 1.8rem;
  }

  .scene-paragraph {
    font-size: 1rem;
    text-align: left;
  }

  .game-header {
    flex-direction: column;
    gap: 1rem;
  }

  .player-info {
    flex-direction: column;
    gap: 0.5rem;
  }
}

@media (max-width: 480px) {
  .scene-container {
    padding: 1.5rem;
  }

  .scene-title {
    font-size: 1.5rem;
  }

  .choice-button {
    padding: 1rem 1.2rem;
    font-size: 0.95rem;
  }
}
</style>
