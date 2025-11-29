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

  // Story scenes - each scene can branch to different outcomes
  scenes: {
    // Starting scene after intro
    awakening_chamber: {
      id: "awakening_chamber",
      title: "The Awakening Chamber",
      text: [
        "You open your eyes to find yourself in a dimly lit chamber. Ancient stone walls surround you, covered in glowing runes that pulse with an otherworldly light.",

        "As consciousness returns, you realize you're lying on a cold stone altar. The air is thick with the scent of incense and something else - something ancient and powerful.",

        "To your left, you notice a ornate mirror with a silver frame. To your right, a wooden door stands slightly ajar, with faint whispers echoing from beyond."
      ],
      choices: [
        {
          id: "examine_mirror",
          text: "Examine the mirror",
          nextScene: "mirror_reflection",
          effects: {
            trait: "curious",
            value: 1
          }
        },
        {
          id: "approach_door",
          text: "Approach the door",
          nextScene: "mysterious_corridor",
          effects: {
            trait: "brave",
            value: 1
          }
        },
        {
          id: "study_runes",
          text: "Study the glowing runes",
          nextScene: "ancient_knowledge",
          effects: {
            trait: "wise",
            value: 1
          }
        }
      ]
    },

    mirror_reflection: {
      id: "mirror_reflection",
      title: "Reflection of the Veilborn",
      text: [
        "You approach the mirror cautiously. The reflection staring back at you is... different. Your features seem to shift and shimmer, as if caught between two states of being.",

        "In the mirror's depths, you see glimpses of memories - faces you once knew, places you once called home. But they're fading, like morning mist under the sun.",

        "A voice whispers from the mirror: 'Do you wish to remember, or to forget and be reborn?'"
      ],
      choices: [
        {
          id: "remember",
          text: "I want to remember who I was",
          nextScene: "path_of_memories",
          effects: {
            trait: "nostalgic",
            value: 2,
            unlockMemories: true
          }
        },
        {
          id: "forget",
          text: "The past is gone. I choose rebirth",
          nextScene: "path_of_rebirth",
          effects: {
            trait: "determined",
            value: 2,
            clearMemories: true
          }
        },
        {
          id: "reject_mirror",
          text: "Step away from the mirror",
          nextScene: "awakening_chamber"
        }
      ]
    },

    mysterious_corridor: {
      id: "mysterious_corridor",
      title: "The Whispering Corridor",
      text: [
        "You push through the door and enter a long, winding corridor. Torches flicker along the walls, casting dancing shadows that seem almost alive.",

        "The whispers grow louder here. They speak in languages you shouldn't understand, yet somehow you do. They speak of power, of sacrifice, of choices that echo through eternity.",

        "Ahead, the corridor splits into three paths. One leads upward into light, another descends into darkness, and the third continues straight ahead into mist."
      ],
      choices: [
        {
          id: "path_light",
          text: "Take the path of light",
          nextScene: "celestial_garden",
          effects: {
            alignment: "light",
            value: 1
          }
        },
        {
          id: "path_darkness",
          text: "Descend into darkness",
          nextScene: "shadow_depths",
          effects: {
            alignment: "dark",
            value: 1
          }
        },
        {
          id: "path_mist",
          text: "Walk through the mist",
          nextScene: "veiled_realm",
          effects: {
            alignment: "neutral",
            value: 1
          }
        }
      ]
    },

    ancient_knowledge: {
      id: "ancient_knowledge",
      title: "Secrets of the Ancients",
      text: [
        "You reach out and touch one of the glowing runes. Immediately, your mind is flooded with visions and knowledge from ages past.",

        "You see the rise and fall of civilizations, the birth of magic, and the creation of the Veil itself. You understand now - you are part of something far greater than yourself.",

        "The runes offer you a choice: Accept their power and the burden that comes with it, or walk away and seek your own path."
      ],
      choices: [
        {
          id: "accept_power",
          text: "Accept the ancient power",
          nextScene: "path_of_power",
          effects: {
            trait: "knowledgeable",
            value: 2,
            gainPower: "ancient_magic"
          }
        },
        {
          id: "reject_power",
          text: "Reject the power, forge your own way",
          nextScene: "path_of_independence",
          effects: {
            trait: "independent",
            value: 2
          }
        }
      ]
    },

    // Branching paths continue...
    path_of_memories: {
      id: "path_of_memories",
      title: "Echoes of Yesterday",
      text: [
        "The mirror shatters in a burst of silver light, and your memories come flooding back. You remember your name, your life, your loved ones...",

        "And you remember how you died.",

        "The weight of this knowledge is both a gift and a curse. You are bound to your past, but perhaps that is your strength."
      ],
      choices: [
        {
          id: "continue_journey",
          text: "Continue with your memories intact",
          nextScene: "awakening_chamber"
        }
      ]
    },

    path_of_rebirth: {
      id: "path_of_rebirth",
      title: "Born Anew",
      text: [
        "You close your eyes and let go. The memories dissolve like smoke, leaving you free but unmoored.",

        "When you open your eyes again, you are no longer who you were. You are something new, something forged in the space between life and death.",

        "The mirror cracks and falls silent. Your past is gone, but your future is yours to write."
      ],
      choices: [
        {
          id: "embrace_newself",
          text: "Embrace your new identity",
          nextScene: "awakening_chamber"
        }
      ]
    },

    celestial_garden: {
      id: "celestial_garden",
      title: "Garden of Eternal Light",
      text: [
        "You emerge into a breathtaking garden bathed in gentle, golden light. Flowers of impossible colors bloom around you, and the air is filled with the song of birds that never existed in the mortal world.",

        "In the center of the garden stands a figure clad in white robes, their face obscured by a hood of pure light.",

        "'Welcome, Veilborn,' they say. 'You have chosen the path of light. But remember - even the brightest light casts the darkest shadows.'"
      ],
      choices: [
        {
          id: "speak_to_figure",
          text: "Speak to the robed figure",
          nextScene: "meeting_light_guardian"
        }
      ]
    },

    shadow_depths: {
      id: "shadow_depths",
      title: "The Depths Below",
      text: [
        "Darkness envelops you as you descend. But it's not the absence of light - it's something alive, something that breathes and watches.",

        "You sense no malice here, only power and possibility. In the darkness, you are free from judgment, free from the expectations of the world above.",

        "A voice, smooth as silk and dark as midnight, whispers: 'In shadow, we are what we truly are. No masks, no pretense. Only truth.'"
      ],
      choices: [
        {
          id: "embrace_shadow",
          text: "Embrace the shadow",
          nextScene: "meeting_shadow_guardian"
        }
      ]
    },

    veiled_realm: {
      id: "veiled_realm",
      title: "Between the Worlds",
      text: [
        "The mist swirls around you, neither warm nor cold, neither comforting nor threatening. Here, in this space between, you feel... balanced.",

        "You understand now why you are called Veilborn. You exist in the space between life and death, light and shadow, past and future.",

        "A gentle voice, neither male nor female, speaks: 'You walk the middle path. The hardest path, but perhaps the wisest.'"
      ],
      choices: [
        {
          id: "continue_middle_path",
          text: "Continue on the middle path",
          nextScene: "meeting_veil_guardian"
        }
      ]
    }
  }
}

// Story utility functions
export const getStoryIntro = () => storyData.intro

export const getScene = (sceneId) => {
  return storyData.scenes[sceneId] || null
}

export const getFirstScene = () => {
  return storyData.scenes.awakening_chamber
}
