// Story data structure for Veilborn
// This file contains all narrative content that can be easily updated

export const storyData = {
  intro: {
    title: "The Awakening",
    paragraphs: [
      "In the depths of the forgotten realm, where shadows dance between reality and dreams, you find yourself awakening from a slumber that has lasted centuries.",

      "The veil between worlds grows thin, and whispers of ancient power echo through the darkness. You are neither fully alive nor truly dead - you are Veilborn.",

      "Memories fragment and scatter like dust in the wind. Who were you? What brought you to this liminal space between existence and oblivion?",

      "The only certainty is the pull - an inexorable force drawing you forward into the unknown. Your journey begins not with answers, but with questions that pierce the very fabric of your being."
    ],
    choices: [
      {
        id: "continue",
        text: "Begin your journey",
        action: "character_creation"
      },
      {
        id: "back",
        text: "Return to menu",
        action: "main_menu"
      }
    ]
  },

  // Additional story chapters can be added here
  chapters: {
    chapter1: {
      title: "The First Steps",
      content: "Your story continues..."
    }
    // More chapters will be added as the game develops
  }
}

// Story utility functions
export const getStoryIntro = () => storyData.intro

export const getChapter = (chapterId) => {
  return storyData.chapters[chapterId] || null
}
