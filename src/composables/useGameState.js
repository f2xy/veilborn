import { ref, computed, watch } from 'vue'
import { useQuestState } from './useQuestState.js'

// Game state management using Vue composables
// This provides a simple state management solution without external dependencies

const currentSceneId = ref(null)
const sceneHistory = ref([])
const playerTraits = ref({
  curious: 0,
  brave: 0,
  wise: 0,
  nostalgic: 0,
  determined: 0,
  knowledgeable: 0,
  independent: 0
})
const playerAlignment = ref({
  light: 0,
  dark: 0,
  neutral: 0
})
const playerChoices = ref([])
const gameFlags = ref({
  hasMemories: false,
  hasPower: false
})
const unlockedStoryScenes = ref([])

export function useGameState() {
  // Quest system integration
  const questState = useQuestState()

  // Start a new game
  const startNewGame = () => {
    currentSceneId.value = 'awakening_chamber'
    sceneHistory.value = []
    playerTraits.value = {
      curious: 0,
      brave: 0,
      wise: 0,
      nostalgic: 0,
      determined: 0,
      knowledgeable: 0,
      independent: 0
    }
    playerAlignment.value = {
      light: 0,
      dark: 0,
      neutral: 0
    }
    playerChoices.value = []
    gameFlags.value = {
      hasMemories: false,
      hasPower: false
    }
    unlockedStoryScenes.value = []
  }

  // Navigate to a new scene
  const navigateToScene = (sceneId) => {
    if (currentSceneId.value) {
      sceneHistory.value.push(currentSceneId.value)
    }
    currentSceneId.value = sceneId
  }

  // Record a player choice and apply its effects
  const makeChoice = (choice, sceneId) => {
    // Record the choice
    playerChoices.value.push({
      sceneId,
      choiceId: choice.id,
      timestamp: Date.now()
    })

    // Apply effects if any
    if (choice.effects) {
      const effects = choice.effects

      // Apply trait changes
      if (effects.trait && effects.value) {
        if (playerTraits.value.hasOwnProperty(effects.trait)) {
          playerTraits.value[effects.trait] += effects.value
        }
      }

      // Apply alignment changes
      if (effects.alignment && effects.value) {
        if (playerAlignment.value.hasOwnProperty(effects.alignment)) {
          playerAlignment.value[effects.alignment] += effects.value
        }
      }

      // Apply special flags
      if (effects.unlockMemories) {
        gameFlags.value.hasMemories = true
      }
      if (effects.clearMemories) {
        gameFlags.value.hasMemories = false
      }
      if (effects.gainPower) {
        gameFlags.value.hasPower = effects.gainPower
      }
    }

    // Navigate to next scene
    if (choice.nextScene) {
      navigateToScene(choice.nextScene)
    }
  }

  // Go back to previous scene (if available)
  const goBack = () => {
    if (sceneHistory.value.length > 0) {
      currentSceneId.value = sceneHistory.value.pop()
    }
  }

  // Get dominant trait
  const dominantTrait = computed(() => {
    const traits = Object.entries(playerTraits.value)
    if (traits.length === 0) return null

    const max = traits.reduce((prev, current) =>
      current[1] > prev[1] ? current : prev
    )

    return max[1] > 0 ? max[0] : null
  })

  // Get dominant alignment
  const dominantAlignment = computed(() => {
    const alignments = Object.entries(playerAlignment.value)
    if (alignments.length === 0) return 'neutral'

    const max = alignments.reduce((prev, current) =>
      current[1] > prev[1] ? current : prev
    )

    return max[1] > 0 ? max[0] : 'neutral'
  })

  // Save game state to localStorage
  const saveGame = (slotName = 'autosave') => {
    const gameState = {
      currentSceneId: currentSceneId.value,
      sceneHistory: sceneHistory.value,
      playerTraits: playerTraits.value,
      playerAlignment: playerAlignment.value,
      playerChoices: playerChoices.value,
      gameFlags: gameFlags.value,
      unlockedStoryScenes: unlockedStoryScenes.value,
      timestamp: Date.now()
    }

    localStorage.setItem(`veilborn_save_${slotName}`, JSON.stringify(gameState))
    return true
  }

  // Load game state from localStorage
  const loadGame = (slotName = 'autosave') => {
    const savedData = localStorage.getItem(`veilborn_save_${slotName}`)

    if (!savedData) return false

    try {
      const gameState = JSON.parse(savedData)

      currentSceneId.value = gameState.currentSceneId
      sceneHistory.value = gameState.sceneHistory || []
      playerTraits.value = gameState.playerTraits || {}
      playerAlignment.value = gameState.playerAlignment || {}
      playerChoices.value = gameState.playerChoices || []
      gameFlags.value = gameState.gameFlags || {}
      unlockedStoryScenes.value = gameState.unlockedStoryScenes || []

      return true
    } catch (error) {
      console.error('Failed to load game:', error)
      return false
    }
  }

  // Check if a save exists
  const hasSave = (slotName = 'autosave') => {
    return localStorage.getItem(`veilborn_save_${slotName}`) !== null
  }

  // Check if a story scene is unlocked
  const isStorySceneUnlocked = (sceneId) => {
    return unlockedStoryScenes.value.includes(sceneId)
  }

  // Unlock a story scene
  const unlockStoryScene = (sceneId) => {
    if (!unlockedStoryScenes.value.includes(sceneId)) {
      unlockedStoryScenes.value.push(sceneId)
      console.log(`📖 Yeni hikaye sahnesi açıldı: ${sceneId}`)
      saveGame('autosave')
    }
  }

  // Get available story scenes (unlocked but not yet visited)
  const availableStoryScenes = computed(() => {
    return unlockedStoryScenes.value.filter(
      sceneId => !sceneHistory.value.includes(sceneId) && sceneId !== currentSceneId.value
    )
  })

  // Watch for quest completion and unlock story scenes
  watch(() => questState.completedQuests.value, (newCompleted, oldCompleted) => {
    if (newCompleted.length > oldCompleted.length) {
      const newQuestId = newCompleted[newCompleted.length - 1]
      const quest = questState.getQuestById(newQuestId)

      if (quest && quest.rewards && quest.rewards.unlockStoryScenes) {
        quest.rewards.unlockStoryScenes.forEach(sceneId => {
          unlockStoryScene(sceneId)
        })
      }
    }
  })

  return {
    // State
    currentSceneId,
    sceneHistory,
    playerTraits,
    playerAlignment,
    playerChoices,
    gameFlags,
    unlockedStoryScenes,

    // Computed
    dominantTrait,
    dominantAlignment,
    availableStoryScenes,

    // Quest integration
    questState,

    // Methods
    startNewGame,
    navigateToScene,
    makeChoice,
    goBack,
    saveGame,
    loadGame,
    hasSave,
    isStorySceneUnlocked,
    unlockStoryScene
  }
}
