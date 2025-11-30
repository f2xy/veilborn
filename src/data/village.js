// Village data structure for Veilborn
// Buildings can be upgraded over time as the player progresses

export const villageData = {
  name: "Forgotten Haven",
  description: "A ruined village at the edge of the Veil, waiting to be restored",

  // Building types and their upgrade levels
  buildings: {
    town_hall: {
      id: "town_hall",
      name: "Town Hall",
      description: "The heart of the village. Restoring it unlocks new possibilities.",
      maxLevel: 5,
      levels: {
        0: {
          name: "Collapsed Ruins",
          description: "Nothing but rubble and broken stone.",
          visual: "ruins",
          unlocks: []
        },
        1: {
          name: "Makeshift Shelter",
          description: "A basic structure providing minimal shelter.",
          visual: "basic",
          unlocks: ["Basic storage", "Village council"],
          cost: { essence: 100 }
        },
        2: {
          name: "Restored Hall",
          description: "A proper hall where villagers can gather.",
          visual: "restored",
          unlocks: ["Quest board", "Trading post"],
          cost: { essence: 250, materials: 50 }
        },
        3: {
          name: "Fortified Hall",
          description: "Strong walls provide protection and comfort.",
          visual: "fortified",
          unlocks: ["Defense planning", "Advanced crafting"],
          cost: { essence: 500, materials: 100, rare_materials: 10 }
        },
        4: {
          name: "Grand Hall",
          description: "A magnificent structure that inspires hope.",
          visual: "grand",
          unlocks: ["Ancient rituals", "Master craftsmen"],
          cost: { essence: 1000, materials: 200, rare_materials: 50 }
        },
        5: {
          name: "Citadel of Hope",
          description: "A beacon of light in the darkness.",
          visual: "citadel",
          unlocks: ["All features"],
          cost: { essence: 2000, materials: 500, rare_materials: 100, legendary_materials: 10 }
        }
      }
    },

    temple: {
      id: "temple",
      name: "Veil Temple",
      description: "A place of power where the Veil grows thin.",
      maxLevel: 5,
      levels: {
        0: {
          name: "Broken Shrine",
          description: "Ancient stones mark what once was sacred ground.",
          visual: "ruins",
          unlocks: []
        },
        1: {
          name: "Cleansed Altar",
          description: "The altar is cleansed and ready for use.",
          visual: "basic",
          unlocks: ["Basic prayers", "Essence gathering"],
          cost: { essence: 150 }
        },
        2: {
          name: "Active Temple",
          description: "Power flows through this sacred place once more.",
          visual: "restored",
          unlocks: ["Advanced prayers", "Veil manipulation"],
          cost: { essence: 300, materials: 75 }
        },
        3: {
          name: "Sanctified Temple",
          description: "The Veil responds to your presence here.",
          visual: "fortified",
          unlocks: ["Rituals", "Blessing chamber"],
          cost: { essence: 600, materials: 150, rare_materials: 20 }
        },
        4: {
          name: "Grand Temple",
          description: "A magnificent place of worship and power.",
          visual: "advanced",
          unlocks: ["Divine rituals", "Mass blessings"],
          cost: { essence: 1200, materials: 300, rare_materials: 60 }
        },
        5: {
          name: "Celestial Sanctuary",
          description: "Where the mortal and divine realms meet.",
          visual: "masterwork",
          unlocks: ["Ultimate prayers", "Veil mastery"],
          cost: { essence: 2500, materials: 600, rare_materials: 120, legendary_materials: 15 }
        }
      }
    },

    barracks: {
      id: "barracks",
      name: "Barracks",
      description: "Training grounds for those who defend the village.",
      maxLevel: 4,
      levels: {
        0: {
          name: "Abandoned Training Ground",
          description: "Weathered training dummies stand forgotten.",
          visual: "ruins",
          unlocks: []
        },
        1: {
          name: "Basic Training Area",
          description: "Simple facilities for combat practice.",
          visual: "basic",
          unlocks: ["Combat training", "Weapon storage"],
          cost: { essence: 100, materials: 50 }
        },
        2: {
          name: "Proper Barracks",
          description: "Equipped for serious training.",
          visual: "restored",
          unlocks: ["Advanced training", "Armory"],
          cost: { essence: 250, materials: 100 }
        },
        3: {
          name: "Fortified Barracks",
          description: "Elite warriors train here.",
          visual: "advanced",
          unlocks: ["Elite training", "War tactics"],
          cost: { essence: 500, materials: 200, rare_materials: 30 }
        },
        4: {
          name: "Military Academy",
          description: "The finest warriors in the realm train here.",
          visual: "masterwork",
          unlocks: ["Legendary training", "Strategic warfare"],
          cost: { essence: 1000, materials: 400, rare_materials: 80, legendary_materials: 5 }
        }
      }
    },

    workshop: {
      id: "workshop",
      name: "Workshop",
      description: "Where materials are transformed into useful items.",
      maxLevel: 4,
      levels: {
        0: {
          name: "Broken Forge",
          description: "Cold ashes in a crumbling forge.",
          visual: "ruins",
          unlocks: []
        },
        1: {
          name: "Working Forge",
          description: "The forge burns once more.",
          visual: "basic",
          unlocks: ["Basic crafting", "Item repair"],
          cost: { essence: 120, materials: 60 }
        },
        2: {
          name: "Master Workshop",
          description: "Advanced tools enable complex crafting.",
          visual: "restored",
          unlocks: ["Advanced crafting", "Enchanting"],
          cost: { essence: 300, materials: 150, rare_materials: 15 }
        },
        3: {
          name: "Artisan's Guild",
          description: "Master craftsmen create wonders here.",
          visual: "advanced",
          unlocks: ["Expert crafting", "Rare enchantments"],
          cost: { essence: 700, materials: 350, rare_materials: 50 }
        },
        4: {
          name: "Legendary Forge",
          description: "Artifacts of immense power are forged here.",
          visual: "masterwork",
          unlocks: ["Legendary crafting", "Mythic items"],
          cost: { essence: 1500, materials: 700, rare_materials: 100, legendary_materials: 8 }
        }
      }
    },

    market: {
      id: "market",
      name: "Market Square",
      description: "Where traders gather and goods are exchanged.",
      maxLevel: 3,
      levels: {
        0: {
          name: "Empty Square",
          description: "A desolate plaza where no one comes.",
          visual: "ruins",
          unlocks: []
        },
        1: {
          name: "Small Market",
          description: "A few brave traders set up stalls.",
          visual: "basic",
          unlocks: ["Basic trading", "Resource exchange"],
          cost: { essence: 80, materials: 40 }
        },
        2: {
          name: "Bustling Market",
          description: "Traders from distant lands visit regularly.",
          visual: "restored",
          unlocks: ["Rare goods", "Special merchants"],
          cost: { essence: 200, materials: 100, rare_materials: 10 }
        },
        3: {
          name: "Grand Bazaar",
          description: "A legendary trading hub known across the realm.",
          visual: "advanced",
          unlocks: ["Exotic goods", "Master merchants"],
          cost: { essence: 500, materials: 250, rare_materials: 40, legendary_materials: 3 }
        }
      }
    },

    library: {
      id: "library",
      name: "Ancient Library",
      description: "Repository of forgotten knowledge.",
      maxLevel: 3,
      levels: {
        0: {
          name: "Burned Archives",
          description: "Charred scrolls and broken shelves.",
          visual: "ruins",
          unlocks: []
        },
        1: {
          name: "Salvaged Collection",
          description: "Some books have been recovered and preserved.",
          visual: "basic",
          unlocks: ["Lore discovery", "Basic research"],
          cost: { essence: 150, materials: 30 }
        },
        2: {
          name: "Restored Library",
          description: "Knowledge flows freely once more.",
          visual: "restored",
          unlocks: ["Advanced research", "Spell learning"],
          cost: { essence: 350, materials: 80, rare_materials: 25 }
        },
        3: {
          name: "Grand Archive",
          description: "The greatest collection of knowledge in the realm.",
          visual: "advanced",
          unlocks: ["Ancient secrets", "Forbidden knowledge"],
          cost: { essence: 800, materials: 200, rare_materials: 60, legendary_materials: 5 }
        }
      }
    }
  },

  // Initial population and growth
  population: {
    initial: 0,
    growthFactors: ["town_hall_level", "available_housing", "village_safety"]
  },

  // Village upgrades unlock new story content
  storyUnlocks: {
    town_hall_1: "village_elder_arrives",
    temple_1: "veil_mysteries_begin",
    barracks_1: "defense_training_available",
    workshop_1: "master_craftsman_arrives",
    market_1: "first_traders_arrive",
    library_1: "scholar_arrives"
  }
}

// Utility functions
export const getBuilding = (buildingId) => {
  return villageData.buildings[buildingId] || null
}

export const getBuildingLevel = (buildingId, level) => {
  const building = getBuilding(buildingId)
  if (!building || !building.levels[level]) return null
  return building.levels[level]
}

export const getAllBuildings = () => {
  return Object.values(villageData.buildings)
}

export const getVillageName = () => {
  return villageData.name
}
