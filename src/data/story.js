// Veilborn için hikaye veri yapısı
// Bu dosya kolayca güncellenebilecek tüm anlatı içeriğini barındırır

export const storyData = {
  intro: {
    title: "Uyanış",
    paragraphs: [
      "Unutulmuş diyarın derinliklerinde, gölgelerin gerçeklik ve düşler arasında dans ettiği yerde, yüzyıllardır süren bir uykudan uyanıyorsun.",

      "Dünyalar arasındaki perde inceliyor ve kadim gücün fısıltıları karanlıkta yankılanıyor. Ne tamamen canlısın ne de gerçekten ölü - sen Peçe Doğumlususn.",

      "Anılar parçalanıyor ve rüzgardaki toz gibi dağılıyor. Kim miydin? Seni varoluş ile unutuş arasındaki bu sınır alanına ne getirdi?",

      "Tek kesin olan çekiş - seni bilinmeze doğru iten kaçınılmaz bir güç. Yolculuğun cevaplarla değil, varlığının dokusunu delen sorularla başlıyor."
    ],
    choices: [
      {
        id: "continue",
        text: "Yolculuğuna başla",
        action: "character_creation"
      },
      {
        id: "back",
        text: "Menüye dön",
        action: "main_menu"
      }
    ]
  },

  // Hikaye sahneleri - her sahne farklı sonuçlara dallanabilir
  scenes: {
    // Giriş sonrası başlangıç sahnesi
    awakening_chamber: {
      id: "awakening_chamber",
      title: "Uyanış Odası",
      text: [
        "Gözlerini açtığında kendini loş ışıklı bir odada buluyorsun. Kadim taş duvarlar seni çevreliyor, öte dünyadan gelen bir ışıkla nabız gibi atan parlayan runlarla kaplılar.",

        "Bilinç geri dönerken, soğuk bir taş sunak üzerinde yattığını fark ediyorsun. Hava tütsü kokusu ve başka bir şeyle - kadim ve güçlü bir şeyle - dolu.",

        "Solunda, gümüş çerçeveli süslü bir ayna fark ediyorsun. Sağında ise yarı açık duran bir ahşap kapı, öteden gelen zayıf fısıltılarla yankılanıyor."
      ],
      choices: [
        {
          id: "examine_mirror",
          text: "Aynayı incele",
          nextScene: "mirror_reflection",
          effects: {
            trait: "curious",
            value: 1
          }
        },
        {
          id: "approach_door",
          text: "Kapıya yaklaş",
          nextScene: "mysterious_corridor",
          effects: {
            trait: "brave",
            value: 1
          }
        },
        {
          id: "study_runes",
          text: "Parlayan runları incele",
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
      title: "Peçe Doğumlunun Yansıması",
      text: [
        "Aynaya temkinli yaklaşıyorsun. Sana bakan yansıma... farklı. Yüz hatların sanki iki varoluş hali arasında yakalanmış gibi kayıyor ve titreşiyor.",

        "Aynanın derinliklerinde anıların parçalarını görüyorsun - bir zamanlar tanıdığın yüzler, bir zamanlar evim dediğin yerler. Ama soluyorlar, güneş altındaki sabah sisi gibi.",

        "Aynadan bir ses fısıldıyor: 'Hatırlamak mı istiyorsun, yoksa unutup yeniden doğmayı mı?'"
      ],
      choices: [
        {
          id: "remember",
          text: "Kim olduğumu hatırlamak istiyorum",
          nextScene: "path_of_memories",
          effects: {
            trait: "nostalgic",
            value: 2,
            unlockMemories: true
          }
        },
        {
          id: "forget",
          text: "Geçmiş gitti. Yeniden doğuşu seçiyorum",
          nextScene: "path_of_rebirth",
          effects: {
            trait: "determined",
            value: 2,
            clearMemories: true
          }
        },
        {
          id: "reject_mirror",
          text: "Aynadan uzaklaş",
          nextScene: "awakening_chamber"
        }
      ]
    },

    mysterious_corridor: {
      id: "mysterious_corridor",
      title: "Fısıldayan Koridor",
      text: [
        "Kapıyı itip uzun, kıvrımlı bir koridora giriyorsun. Meşaleler duvarlarda titreşiyor, neredeyse canlı görünen dans eden gölgeler atıyor.",

        "Fısıltılar burada daha yüksek. Anlamamanın gereken dillerde konuşuyorlar, ama bir şekilde anlıyorsun. Güçten, fedakarlıktan, sonsuzlukta yankılanan seçimlerden bahsediyorlar.",

        "İleride, koridor üçe ayrılıyor. Biri ışığa doğru yukarı çıkıyor, diğeri karanlığa iniyor ve üçüncüsü sise doğru düz devam ediyor."
      ],
      choices: [
        {
          id: "path_light",
          text: "Işık yolunu seç",
          nextScene: "celestial_garden",
          effects: {
            alignment: "light",
            value: 1
          }
        },
        {
          id: "path_darkness",
          text: "Karanlığa in",
          nextScene: "shadow_depths",
          effects: {
            alignment: "dark",
            value: 1
          }
        },
        {
          id: "path_mist",
          text: "Sisin içinden yürü",
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
      title: "Kadimler'in Sırları",
      text: [
        "Uzanıp parlayan runlardan birine dokunuyorsun. Hemen zihnin geçmiş çağlardan gelen vizyonlar ve bilgiyle dolup taşıyor.",

        "Uygarlıkların yükselişini ve çöküşünü, büyünün doğuşunu ve Perde'nin yaratılışını görüyorsun. Şimdi anlıyorsun - kendinden çok daha büyük bir şeyin parçasısın.",

        "Runlar sana bir seçim sunuyor: Güçlerini ve beraberinde gelen yükü kabul et, ya da uzaklaş ve kendi yolunu ara."
      ],
      choices: [
        {
          id: "accept_power",
          text: "Kadim gücü kabul et",
          nextScene: "path_of_power",
          effects: {
            trait: "knowledgeable",
            value: 2,
            gainPower: "ancient_magic"
          }
        },
        {
          id: "reject_power",
          text: "Gücü reddet, kendi yolunu aç",
          nextScene: "path_of_independence",
          effects: {
            trait: "independent",
            value: 2
          }
        }
      ]
    },

    // Dallanan yollar devam ediyor...
    path_of_memories: {
      id: "path_of_memories",
      title: "Dünün Yankıları",
      text: [
        "Ayna gümüş bir ışık patlamasıyla paramparça oluyor ve anıların sel gibi geri geliyor. İsmini, hayatını, sevdiklerini hatırlıyorsun...",

        "Ve nasıl öldüğünü hatırlıyorsun.",

        "Bu bilginin ağırlığı hem bir armağan hem de bir lanet. Geçmişine bağlısın, ama belki de bu senin gücün."
      ],
      choices: [
        {
          id: "continue_journey",
          text: "Anılarınla birlikte devam et",
          nextScene: "awakening_chamber"
        }
      ]
    },

    path_of_rebirth: {
      id: "path_of_rebirth",
      title: "Yeniden Doğuş",
      text: [
        "Gözlerini kapatıp bırakıyorsun. Anılar duman gibi dağılıyor, seni özgür ama demirsiz bırakıyor.",

        "Gözlerini tekrar açtığında, artık eskiden olduğun kişi değilsin. Hayat ile ölüm arasındaki boşlukta dövülmüş yeni bir şeysin.",

        "Ayna çatlıyor ve sessizleşiyor. Geçmişin gitti, ama geleceğin yazmak için senin."
      ],
      choices: [
        {
          id: "embrace_newself",
          text: "Yeni kimliğini kucakla",
          nextScene: "awakening_chamber"
        }
      ]
    },

    celestial_garden: {
      id: "celestial_garden",
      title: "Sonsuz Işık Bahçesi",
      text: [
        "Nefes kesici bir bahçeye çıkıyorsun, yumuşak altın ışıkla yıkanmış. Etrafında imkansız renklerde çiçekler açıyor ve hava ölümlü dünyada hiç var olmamış kuşların şarkısıyla dolu.",

        "Bahçenin ortasında beyaz cüppeler giymiş bir figür duruyor, yüzü saf ışıktan bir başlıkla gizlenmiş.",

        "'Hoş geldin, Peçe Doğumlu,' diyorlar. 'Işık yolunu seçtin. Ama unutma - en parlak ışık bile en karanlık gölgeleri yaratır.'"
      ],
      choices: [
        {
          id: "speak_to_figure",
          text: "Cüppeli figürle konuş",
          nextScene: "meeting_light_guardian"
        }
      ]
    },

    shadow_depths: {
      id: "shadow_depths",
      title: "Aşağının Derinlikleri",
      text: [
        "İnerken karanlık seni sarıyor. Ama bu ışığın yokluğu değil - canlı, nefes alan ve izleyen bir şey.",

        "Burada kötü niyet hissetmiyorsun, sadece güç ve olasılık. Karanlıkta, yargıdan özgürsün, yukarıdaki dünyanın beklentilerinden.",

        "İpek gibi yumuşak ve gece yarısı kadar karanlık bir ses fısıldıyor: 'Gölgede, gerçekten ne olduğumuzuz. Maske yok, yapmacık yok. Sadece gerçek.'"
      ],
      choices: [
        {
          id: "embrace_shadow",
          text: "Gölgeyi kucakla",
          nextScene: "meeting_shadow_guardian"
        }
      ]
    },

    veiled_realm: {
      id: "veiled_realm",
      title: "Dünyalar Arası",
      text: [
        "Sis etrafında dönerken, ne sıcak ne soğuk, ne rahatlatıcı ne de tehdit edici. Burada, aradaki bu boşlukta... dengeli hissediyorsun.",

        "Şimdi neden Peçe Doğumlu olarak adlandırıldığını anlıyorsun. Hayat ile ölüm, ışık ile gölge, geçmiş ile gelecek arasındaki boşlukta varsin.",

        "Ne erkek ne kadın yumuşak bir ses konuşuyor: 'Orta yolda yürüyorsun. En zor yol, ama belki de en bilge olanı.'"
      ],
      choices: [
        {
          id: "continue_middle_path",
          text: "Orta yolda devam et",
          nextScene: "meeting_veil_guardian"
        }
      ]
    },

    // Koruyucu buluşmaları - bunlar köye götürür
    meeting_light_guardian: {
      id: "meeting_light_guardian",
      title: "Işık Koruyucusu",
      text: [
        "Cüppeli figür yaklaşıyor ve yumuşak güneş ışığı gibi ondan yayılan sıcaklığı hissediyorsun.",

        "'Ben Işık Koruyucusuyum,' diyorlar. 'Aydınlanmada yürümeyi seçtin. Ama yolun burada bitmiyor.'",

        "'Bir yer var - zaman ve gölgeye kaybolan bir köy. Senin gibi birine ihtiyacı var, unutulmuşlara umudu geri getirebilecek birine. Bu yükü kabul eder misin?'"
      ],
      choices: [
        {
          id: "accept_quest",
          text: "Köyü restore edeceğim",
          nextScene: "journey_to_village",
          effects: {
            quest: "restore_village"
          }
        }
      ]
    },

    meeting_shadow_guardian: {
      id: "meeting_shadow_guardian",
      title: "Gölge Koruyucusu",
      text: [
        "Ses değişen karanlıktan bir figüre dönüşüyor, ne tehdit edici ne de rahatlatıcı.",

        "'Ben Gölge Koruyucusuyum,' diyor. 'Başkalarının korktuğunu kucaklıyorsun. Bu güce ihtiyaç var.'",

        "'Terk edilmiş ve unutulmuş bir köy var. Karanlıkta, yeni bir amaç bulabilir. Onu alan olarak hak eder misin?'"
      ],
      choices: [
        {
          id: "accept_quest",
          text: "Köyü hak edeceğim",
          nextScene: "journey_to_village",
          effects: {
            quest: "restore_village"
          }
        }
      ]
    },

    meeting_veil_guardian: {
      id: "meeting_veil_guardian",
      title: "Perde Koruyucusu",
      text: [
        "Sis toplanıp haller arasında var olan bir figür oluşturuyor, ne tamamen katı ne de tamamen ruhani.",

        "'Ben Perde Koruyucusuyum,' diyorlar. 'Benim gibi dünyalar arasında yürüyorsun. Bu hem armağan hem de lanet.'",

        "'Harabeler içinde bir köy yatıyor, geçmiş ihtişamı ile gelecek olasılığı arasında sıkışmış. Sadece orta yolda yürüyen biri onu dengeye götürebilir. Bu görevi üstlenir misin?'"
      ],
      choices: [
        {
          id: "accept_quest",
          text: "Köye rehberlik edeceğim",
          nextScene: "journey_to_village",
          effects: {
            quest: "restore_village"
          }
        }
      ]
    },

    path_of_power: {
      id: "path_of_power",
      title: "Kadim Sırların Bekçisi",
      text: [
        "Kadim güç içinden akıyor, varlığının her lifini bilgi ve güçle dolduruyor.",

        "Ne olduğunun ve ne olabileceğinin vizyonlarını görüyorsun. Bunlar arasında bir köy görüyorsun - terk edilmiş, ama büyüklük potansiyeli olan.",

        "Güç sana fısıldıyor: bu yer senin güç merkezin olabilir, dünya üzerindeki iraden için bir üs."
      ],
      choices: [
        {
          id: "go_to_village",
          text: "Köyü ara",
          nextScene: "journey_to_village"
        }
      ]
    },

    path_of_independence: {
      id: "path_of_independence",
      title: "Kendi Yolun",
      text: [
        "Gücü reddedip uzaklaşıyorsun, kendi gücüne ve muhakemene güveniyorsun.",

        "Yürürken, sana seslenen bir şey hissediyorsun - bir ses değil, bir his. Sana ihtiyaç duyan bir yer, gücün için değil, iraden için.",

        "Kendini unutulmuş bir köye doğru çekilmiş buluyorsun, kendi kaderini şekillendirebileceğin bir yer."
      ],
      choices: [
        {
          id: "follow_feeling",
          text: "Hissi takip et",
          nextScene: "journey_to_village"
        }
      ]
    },

    // Köye yolculuk - son geçiş sahnesi
    journey_to_village: {
      id: "journey_to_village",
      title: "Yolculuğun Sonu",
      text: [
        "Saatler - ya da belki de günler - gibi hissettiren bir süre yolculuk ediyorsun. Zaman bu diyarlar arası diyarda garip hareket ediyor.",

        "Sonunda bir tepeye tırmanıyorsun ve görüyorsun: bir vadiye yuvalanmış, sise bürünmüş bir köy. Binalar çeşitli çürüme hallerinde duruyor, sokaklar yabani otlarla kaplanmış.",

        "Ama burada potansiyel hissediyorsun. Bu yer restore edilebilir. Bu senin evin, sığınağın, mirasın olabilir.",

        "Tepeden iniyorsun, yolculuğunun yeni bir bölümünü başlatmaya hazır."
      ],
      choices: [
        {
          id: "enter_village",
          text: "Köye gir",
          action: "enter_village"
        }
      ]
    }
  }
}

// Hikaye yardımcı fonksiyonları
export const getStoryIntro = () => storyData.intro

export const getScene = (sceneId) => {
  return storyData.scenes[sceneId] || null
}

export const getFirstScene = () => {
  return storyData.scenes.awakening_chamber
}
