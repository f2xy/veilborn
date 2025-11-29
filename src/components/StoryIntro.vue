<template>
  <div class="story-intro">
    <div class="stars"></div>

    <div class="story-container">
      <div class="story-content">
        <h2 class="story-title">{{ storyIntro.title }}</h2>

        <div class="story-text">
          <p
            v-for="(paragraph, index) in storyIntro.paragraphs"
            :key="index"
            class="story-paragraph"
            :class="{ 'visible': visibleParagraphs[index] }"
          >
            {{ paragraph }}
          </p>
        </div>

        <div class="story-choices" v-if="showChoices">
          <button
            v-for="choice in storyIntro.choices"
            :key="choice.id"
            class="choice-button"
            @click="handleChoice(choice.action)"
          >
            {{ choice.text }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { getStoryIntro } from '../data/story.js'

export default {
  name: 'StoryIntro',
  emits: ['navigate'],
  setup(props, { emit }) {
    const storyIntro = ref(getStoryIntro())
    const visibleParagraphs = ref({})
    const showChoices = ref(false)

    // Animate paragraphs appearing one by one
    const animateParagraphs = () => {
      storyIntro.value.paragraphs.forEach((_, index) => {
        setTimeout(() => {
          visibleParagraphs.value[index] = true

          // Show choices after last paragraph
          if (index === storyIntro.value.paragraphs.length - 1) {
            setTimeout(() => {
              showChoices.value = true
            }, 500)
          }
        }, index * 1000) // 1 second delay between each paragraph
      })
    }

    const handleChoice = (action) => {
      if (action === 'main_menu') {
        emit('navigate', 'menu')
      } else if (action === 'character_creation') {
        emit('navigate', 'gameplay')
      }
    }

    onMounted(() => {
      animateParagraphs()
    })

    return {
      storyIntro,
      visibleParagraphs,
      showChoices,
      handleChoice
    }
  }
}
</script>

<style scoped>
.story-intro {
  width: 100%;
  height: 100vh;
  position: relative;
  background: linear-gradient(135deg, #0a0e1a 0%, #1a1430 25%, #0f1a2e 50%, #0d1a28 75%, #0a0e1a 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  overflow-y: auto;
  padding: 2rem;
}

/* Animated stars background */
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

.story-container {
  position: relative;
  z-index: 1;
  max-width: 800px;
  width: 100%;
  margin: 0 auto;
}

.story-content {
  background: rgba(15, 20, 35, 0.7);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 12px;
  padding: 3rem;
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.5);
}

.story-title {
  font-family: 'Cinzel', serif;
  font-size: 2.5rem;
  font-weight: 600;
  color: #e8d5f2;
  text-align: center;
  margin-bottom: 2.5rem;
  letter-spacing: 0.15em;
  text-shadow: 0 0 20px rgba(168, 85, 247, 0.4);
}

.story-text {
  margin-bottom: 3rem;
}

.story-paragraph {
  font-family: 'Lato', sans-serif;
  font-size: 1.1rem;
  line-height: 1.8;
  color: rgba(255, 255, 255, 0.85);
  margin-bottom: 1.5rem;
  text-align: justify;
  opacity: 0;
  transform: translateY(20px);
  transition: opacity 0.8s ease, transform 0.8s ease;
}

.story-paragraph.visible {
  opacity: 1;
  transform: translateY(0);
}

.story-paragraph:last-child {
  margin-bottom: 0;
}

.story-choices {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
  opacity: 0;
  animation: fadeIn 0.5s ease forwards;
}

@keyframes fadeIn {
  to {
    opacity: 1;
  }
}

.choice-button {
  font-family: 'Lato', sans-serif;
  font-size: 1rem;
  font-weight: 400;
  padding: 0.9rem 2rem;
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

.choice-button:hover {
  background: linear-gradient(135deg, rgba(126, 58, 242, 0.8), rgba(147, 51, 234, 0.8));
  border-color: rgba(168, 85, 247, 0.6);
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(126, 58, 242, 0.5);
}

.choice-button:active {
  transform: translateY(0);
}

/* Responsive design */
@media (max-width: 768px) {
  .story-content {
    padding: 2rem;
  }

  .story-title {
    font-size: 2rem;
  }

  .story-paragraph {
    font-size: 1rem;
    text-align: left;
  }

  .choice-button {
    font-size: 0.9rem;
    padding: 0.8rem 1.5rem;
  }
}

@media (max-width: 480px) {
  .story-intro {
    padding: 1rem;
  }

  .story-content {
    padding: 1.5rem;
  }

  .story-title {
    font-size: 1.75rem;
    letter-spacing: 0.1em;
  }

  .story-paragraph {
    font-size: 0.95rem;
  }

  .story-choices {
    flex-direction: column;
  }

  .choice-button {
    width: 100%;
  }
}
</style>
