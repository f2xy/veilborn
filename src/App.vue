<template>
  <div class="app">
    <MainMenu v-if="currentScreen === 'menu'" @navigate="handleNavigation" />
    <StoryIntro v-else-if="currentScreen === 'newGame'" @navigate="handleNavigation" />

    <!-- Placeholder for other screens -->
    <div v-else-if="currentScreen === 'characterCreation'" class="placeholder-screen">
      <h2>Character Creation</h2>
      <p>Character creation screen coming soon...</p>
      <button @click="handleNavigation('menu')">Back to Menu</button>
    </div>

    <div v-else-if="currentScreen === 'continue'" class="placeholder-screen">
      <h2>Continue Game</h2>
      <p>Continue functionality coming soon...</p>
      <button @click="handleNavigation('menu')">Back to Menu</button>
    </div>

    <div v-else-if="currentScreen === 'loadGame'" class="placeholder-screen">
      <h2>Load Game</h2>
      <p>Load game functionality coming soon...</p>
      <button @click="handleNavigation('menu')">Back to Menu</button>
    </div>

    <div v-else-if="currentScreen === 'options'" class="placeholder-screen">
      <h2>Options</h2>
      <p>Options screen coming soon...</p>
      <button @click="handleNavigation('menu')">Back to Menu</button>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue'
import MainMenu from './components/MainMenu.vue'
import StoryIntro from './components/StoryIntro.vue'

export default {
  name: 'App',
  components: {
    MainMenu,
    StoryIntro
  },
  setup() {
    const currentScreen = ref('menu')

    const handleNavigation = (screen) => {
      currentScreen.value = screen
      console.log('Navigating to:', screen)
    }

    return {
      currentScreen,
      handleNavigation
    }
  }
}
</script>

<style scoped>
.app {
  width: 100%;
  height: 100vh;
  position: relative;
}

.placeholder-screen {
  width: 100%;
  height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #0f1829 0%, #1a1430 50%, #0d3d4a 100%);
  color: white;
  padding: 2rem;
  text-align: center;
}

.placeholder-screen h2 {
  font-family: 'Cinzel', serif;
  font-size: 2.5rem;
  color: #e8d5f2;
  margin-bottom: 1rem;
}

.placeholder-screen p {
  font-family: 'Lato', sans-serif;
  font-size: 1.2rem;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 2rem;
}

.placeholder-screen button {
  font-family: 'Lato', sans-serif;
  font-size: 1rem;
  padding: 0.9rem 2rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.6), rgba(126, 58, 242, 0.6));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.placeholder-screen button:hover {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  border-color: rgba(168, 85, 247, 0.6);
  transform: translateY(-2px);
}
</style>
