<template>
  <div class="app">
    <MainMenu v-if="currentScreen === 'menu'" @navigate="handleNavigation" />
  </div>
</template>

<script>
import { ref } from 'vue'
import MainMenu from './components/MainMenu.vue'

export default {
  name: 'App',
  components: {
    MainMenu
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
</style>
