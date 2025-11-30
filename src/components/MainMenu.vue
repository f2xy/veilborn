<template>
  <div class="main-menu">
    <div class="stars"></div>
    <div class="menu-container">
      <h1 class="game-title">VEILBORN</h1>

      <div class="menu-buttons">
        <button class="menu-button" @click="$emit('navigate', 'newGame')">
          New Game
        </button>
        <button class="menu-button" @click="$emit('navigate', 'continue')">
          Continue
        </button>
        <button class="menu-button" @click="$emit('navigate', 'loadGame')">
          Load Game
        </button>
        <button class="menu-button" @click="$emit('navigate', 'options')">
          Options
        </button>
        <button class="menu-button exit-button" @click="handleExit">
          Exit
        </button>
      </div>

      <div class="footer">
        <div class="version">v1.0.0</div>
        <div class="copyright">© 2024 Veilborn Studios</div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'MainMenu',
  emits: ['navigate'],
  methods: {
    handleExit() {
      if (confirm('Are you sure you want to exit?')) {
        window.close()
      }
    }
  }
}
</script>

<style scoped>
.main-menu {
  width: 100%;
  height: 100vh;
  position: relative;
  background: linear-gradient(135deg, #0f1829 0%, #1a1430 25%, #1e2838 50%, #0d3d4a 75%, #1a1430 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

/* Animated stars background */
.stars {
  position: absolute;
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
  opacity: 0.6;
}

@keyframes twinkle {
  0%, 100% { opacity: 0.6; }
  50% { opacity: 0.3; }
}

.menu-container {
  position: relative;
  z-index: 1;
  text-align: center;
  max-width: 500px;
  width: 100%;
  padding: 0 20px;
}

.game-title {
  font-family: 'Cinzel', serif;
  font-size: 4rem;
  font-weight: 700;
  color: #e8d5f2;
  letter-spacing: 0.3em;
  margin-bottom: 4rem;
  text-shadow: 0 0 20px rgba(168, 85, 247, 0.4),
               0 0 40px rgba(139, 92, 246, 0.2);
  animation: glow 3s ease-in-out infinite;
}

@keyframes glow {
  0%, 100% {
    text-shadow: 0 0 20px rgba(168, 85, 247, 0.4),
                 0 0 40px rgba(139, 92, 246, 0.2);
  }
  50% {
    text-shadow: 0 0 30px rgba(168, 85, 247, 0.6),
                 0 0 60px rgba(139, 92, 246, 0.4);
  }
}

.menu-buttons {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 3rem;
}

.menu-button {
  font-family: 'Lato', sans-serif;
  font-size: 1.1rem;
  font-weight: 400;
  padding: 1rem 2rem;
  background: linear-gradient(135deg, rgba(109, 40, 217, 0.6), rgba(126, 58, 242, 0.6));
  color: white;
  border: 2px solid rgba(168, 85, 247, 0.3);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 15px rgba(109, 40, 217, 0.3);
}

.menu-button:hover {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  border-color: rgba(168, 85, 247, 0.6);
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(126, 58, 242, 0.5);
}

.menu-button:active {
  transform: translateY(0);
}

.exit-button {
  background: transparent;
  border: none;
  box-shadow: none;
  margin-top: 1rem;
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.95rem;
}

.exit-button:hover {
  background: transparent;
  color: rgba(255, 255, 255, 0.9);
  transform: none;
  box-shadow: none;
}

.footer {
  font-family: 'Lato', sans-serif;
  color: rgba(255, 255, 255, 0.5);
  font-size: 0.85rem;
  margin-top: 2rem;
}

.version {
  margin-bottom: 0.25rem;
}

.copyright {
  font-size: 0.8rem;
}

/* Responsive design */
@media (max-width: 768px) {
  .game-title {
    font-size: 3rem;
    letter-spacing: 0.2em;
  }

  .menu-button {
    font-size: 1rem;
    padding: 0.9rem 1.5rem;
  }
}

@media (max-width: 480px) {
  .game-title {
    font-size: 2.5rem;
  }

  .menu-button {
    font-size: 0.95rem;
  }
}
</style>
