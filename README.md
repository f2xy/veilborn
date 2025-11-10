"# Veilborn

Unity 2D Isometric köy inşa ve yönetim oyunu projesi.

## 🎮 Oyunu Başlatma (MVP - Minimum Viable Product)

Oyun şu anda oynanabilir MVP durumunda! Temel sistemler hazır ve test edilebilir.

### Hızlı Başlangıç

1. Unity Hub'da projeyi açın (Unity 2022.3+)
2. `VillageScene` sahnesini açın
3. Play butonuna basın
4. Oyunu test edin!

### Kontroller

- **B** - İnşa menüsünü aç/kapat
- **Fare Sol Tık** - Yapı seç / Yapı yerleştir
- **Fare Sağ Tık** - İptal / Seçimi kaldır
- **WASD / Ok Tuşları** - Kamera hareket
- **Mouse Wheel** - Zoom
- **Orta Fare Tuşu** - Kamera sürükle
- **Space** - Pause/Unpause
- **1** - Normal hız
- **2** - Hızlı mod (2x)
- **Q** - Görev listesini aç/kapat
- **ESC** - Menüleri kapat

### Başlangıç Görevleri

Oyun şu görevlerle başlar:
1. ✅ **100 Odun Topla** - Köylüler otomatik olarak ağaçlardan odun toplar
2. ✅ **3 Ev Onar** - Harabe evleri seçip "Onar" butonuna basın

## Proje Yapısı

```
veilborn/
├── Assets/
│   ├── Scenes/          # Oyun sahneleri
│   ├── Scripts/         # C# scriptleri
│   ├── Sprites/         # 2D grafikler ve sprite'lar
│   ├── Prefabs/         # Prefab dosyaları
│   ├── Materials/       # Materyal dosyaları
│   ├── Audio/           # Ses dosyaları
│   └── Resources/       # Runtime yüklenebilir kaynaklar
├── ProjectSettings/     # Unity proje ayarları
└── Packages/            # Unity paket bağımlılıkları
```

## Gereksinimler

- Unity 2022.3.0f1 veya daha yeni bir sürüm
- 2D oyun geliştirme için gerekli Unity paketleri (otomatik yüklenecektir)

## Kurulum

1. Unity Hub'ı açın
2. "Add" butonuna tıklayın ve bu proje klasörünü seçin
3. Unity 2022.3 veya daha yeni bir sürüm ile projeyi açın
4. Unity Package Manager otomatik olarak gerekli paketleri indirecektir

## Kullanılan Unity Paketleri

- 2D Animation
- 2D Pixel Perfect
- 2D Sprite
- 2D Tilemap
- TextMeshPro
- Visual Scripting
- ve diğerleri...

## Isometric Özellikler

Bu proje isometric görüş açısı için önceden yapılandırılmıştır:

### Hazır Script'ler

- **IsometricCameraController.cs**: Kamera kontrolü, zoom ve hedefe takip
  - WASD/Arrow keys ile hareket
  - Mouse wheel ile zoom
  - Orta fare tuşu ile sürükleme
  - Hedef takip sistemi

- **IsometricCharacterController.cs**: Karakter hareketi
  - Klavye ve fare kontrolü
  - Smooth hareket
  - Animasyon desteği

- **IsometricSpriteSorter.cs**: Otomatik sprite sıralama
  - Y pozisyonuna göre depth illusion
  - Hareketli objeler için dinamik güncelleme

- **IsometricHelper.cs**: Yardımcı fonksiyonlar
  - Koordinat dönüşümleri (Cartesian ↔ Isometric)
  - Grid sistemleri
  - Komşu hesaplama

### Sorting Layer'lar

Proje aşağıdaki sorting layer'lar ile gelir:
- Background (Arka plan)
- Ground (Zemin)
- Objects (Objeler)
- Characters (Karakterler)
- Effects (Efektler)
- UI (Kullanıcı arayüzü)

### Tag ve Layer'lar

Önceden tanımlanmış tag'ler:
- Player, Enemy, NPC, Interactive, Ground

### Örnek Sahne

`Assets/Scenes/SampleScene.unity` isometric grid ile önceden yapılandırılmış örnek bir sahnedir.

## Geliştirme

### Isometric Karakter Oluşturma

1. Yeni bir GameObject oluşturun
2. `SpriteRenderer` ekleyin
3. `IsometricCharacterController` script'ini ekleyin
4. `IsometricSpriteSorter` script'ini ekleyin (otomatik sıralama için)
5. Sorting Layer'ı "Characters" olarak ayarlayın

### Isometric Kamera Kurulumu

Main Camera'ya `IsometricCameraController` script'ini ekleyin ve aşağıdaki ayarları yapın:
- Projection: Orthographic
- Rotation: X: 45°, Y: 0°, Z: 0° (isometric görünüm için)

### Grid Kullanımı

Scene'e Grid objesi ekleyin ve ayarları yapın:
- Cell Layout: Isometric
- Cell Size: X: 1, Y: 0.5

### Sprite Ayarları

Isometric sprite'lar için önerilen ayarlar:
- Pixels Per Unit: 100 (veya sprite boyutunuza göre)
- Sprite Mode: Single veya Multiple
- Filter Mode: Point (pixel art için) veya Bilinear
- Compression: None (kalite için)

## Köy Yönetim Sistemi

Bu oyun bir köy inşa ve yönetim mekaniği içerir. Köy başlangıçta harabe durumdadır.

### Köy Özellikleri

- **Başlangıç Durumu**: Köy yıkık ve harabe durumunda
  - Bazı evler yıkık
  - Bazı evler yanmış
  - Köylüler çadırlarda yaşıyor
  - Duvarlar yıkık

### Yapı Tipleri

#### Konut Yapıları
- **Evler**: Normal evler (başlangıçta çoğu yıkık/yanmış)
- **Çadırlar**: Geçici konutlar

#### Üretim Yapıları
- **Demir Atölyesi**: Demir üretimi
- **Kılıç Atölyesi**: Silah üretimi

#### Depolama
- **Ambarlar**: Kaynak depolama
- **Ahırlar**: Hayvan barınağı

#### Özel Yapılar
- **Şifacı Kulübesi**: Tedavi merkezi
- **Anıt Kule**: Köyün merkezindeki önemli yapı

#### Savunma Yapıları
- **Gözlem Kuleleri**: 4 köşede (başlangıçta yıkık)
- **Duvarlar**: Köyü çevreler (başlangıçta yıkık)

### Yapı Durumları

Yapılar aşağıdaki durumlarda olabilir:
- **Destroyed**: Tamamen yıkık
- **Ruined**: Harabe (kullanılamaz)
- **Burned**: Yanmış
- **Damaged**: Hasarlı (kullanılabilir)
- **UnderConstruction**: İnşa halinde
- **Good**: İyi durumda
- **Excellent**: Mükemmel durumda

### Köy Yönetimi Scriptleri

#### VillageManager.cs
Köyün merkezi yönetim sistemi:
- Kaynak yönetimi (Odun, Taş, Demir, Yiyecek)
- Nüfus yönetimi
- Yapı kayıt sistemi
- Singleton pattern ile erişim

```csharp
// Örnek kullanım
VillageManager.Instance.AddResource(ResourceType.Wood, 50);
bool canBuild = VillageManager.Instance.CanAffordBuilding(buildingData);
```

#### Building.cs
Tüm yapılar için temel component:
- Can sistemi
- Durum yönetimi
- Görsel güncellemeler
- Event sistemi

#### ConstructionSystem.cs
İnşaat ve onarım yönetimi:
- Otomatik/manuel inşaat
- Eşzamanlı inşaat limiti
- Onarım kuyruğu

#### BuildingPlacer.cs
Yapı yerleştirme sistemi:
- Grid snap
- Çarpışma kontrolü
- Önizleme sistemi
- Maliyet kontrolü

### Yapı Oluşturma

1. **BuildingData ScriptableObject Oluştur**
   - Project'te sağ tık → Create → Veilborn → Building Data
   - Yapı bilgilerini doldurun (tip, sprite'lar, maliyet, vb.)

2. **Building Prefab Oluştur**
   - GameObject oluşturun
   - `SpriteRenderer` ekleyin
   - `Building` component'i ekleyin
   - `IsometricSpriteSorter` ekleyin
   - BuildingData'yı atayın
   - Sorting Layer ayarlayın (Objects veya Characters)

3. **Sahneye Yerleştir**
   - Prefab'ı sahneye sürükleyin
   - Başlangıç durumunu ayarlayın (Destroyed, Ruined, Burned, vb.)
   - VillageManager'ın `existingBuildings` listesine ekleyin

### Kod Örneği - Yapı İnşa Etme

```csharp
// BuildingPlacer ile yerleştirme başlat
BuildingPlacer placer = GetComponent<BuildingPlacer>();
placer.StartPlacement(buildingPrefab);

// Yapı tamamlandığında
building.OnBuildingCompleted.AddListener(() => {
    Debug.Log("Yapı tamamlandı!");
});
```

## Kaynak Toplama ve Üretim Sistemi

Köyde kaynak toplamak ve üretmek için gelişmiş bir sistem mevcuttur.

### Kaynak Tipleri

Oyunda 4 temel kaynak vardır:
- **Wood (Odun)**: Ağaçlardan toplanır, inşaat için
- **Stone (Taş)**: Taş ocaklarından toplanır, inşaat için
- **Iron (Demir)**: Demir madenlerinden toplanır, üretim ve inşaat için
- **Food (Yiyecek)**: Çiftliklerden üretilir veya toplanır

### Kaynak Sistemleri Scriptleri

#### ResourceNode.cs
Haritadaki kaynak noktaları:
- Ağaçlar, taş ocakları, demir madenleri
- Kaynak miktarı ve yenilenme
- Toplama limitleri (aynı anda max toplayıcı sayısı)
- Farklı durumlar için sprite'lar (dolu, yarı, boş)

#### ResourceGatherer.cs
Kaynak toplayan köylüler:
- Otomatik kaynak arama ve toplama
- Taşıma kapasitesi
- Depo'ya teslim etme
- 5 durum: Idle, MovingToResource, Gathering, MovingToStorage, Delivering

#### ResourceProducer.cs
Kaynak üreten yapılar:
- Sürekli üretim (çiftlik, oduncu, vb.)
- İşçi gereksinimleri
- Üretim maliyeti (opsiyonel)
- Üretim süresi ve miktarı

#### AutoResourceManager.cs
Otomatik kaynak yönetimi:
- Köylüleri otomatik oluştur ve ata
- Üretim yapılarına işçi ata
- Kaynak dengelemesi (düşük kaynağa öncelik)
- Dinamik köylü yeniden atama

### Depolama Sistemi

VillageManager kaynaklar için storage limitleri yönetir:
- Her kaynak için maksimum kapasite
- Ambar yapıları kapasiteyi artırır
- Kapasite doluysa kaynak toplanamaz

```csharp
// Depolama durumunu kontrol et
float woodPercentage = VillageManager.Instance.WoodStoragePercentage;
if (woodPercentage > 0.9f)
{
    Debug.Log("Odun deposu neredeyse dolu!");
}

// Maksimum kapasiteleri al
int maxWood = VillageManager.Instance.MaxWood;
```

### Kaynak Toplama Örneği

**1. ResourceNode Oluşturma**
```
1. GameObject oluştur (örn: Tree)
2. SpriteRenderer ekle
3. IsometricSpriteSorter ekle
4. ResourceNode component ekle
5. Ayarları yapılandır:
   - Resource Type: Wood
   - Initial Amount: 100
   - Harvest Amount: 10
   - Harvest Time: 2s
```

**2. Köylü Oluşturma**
```
1. GameObject oluştur (örn: Villager)
2. SpriteRenderer ekle
3. IsometricCharacterController ekle (opsiyonel)
4. ResourceGatherer component ekle
5. Ayarları yapılandır:
   - Gather Type: Wood
   - Carry Capacity: 20
   - Auto Mode: true
```

**3. Üretim Yapısı Yapılandırma**
```
1. Building prefab'ına ResourceProducer ekle
2. Ayarları yapılandır:
   - Produced Resource: Food
   - Production Amount: 10
   - Production Time: 30s
   - Required Workers: 1
   - Auto Production: true
```

### Kod Örnekleri

**Manuel Kaynak Toplama**
```csharp
// Kaynağı bul
ResourceNode node = ResourceNode.FindNearest(villagerPos, ResourceType.Wood);

// Köylüyü gönder
ResourceGatherer gatherer = GetComponent<ResourceGatherer>();
gatherer.StartGathering(node);
```

**Üretim Başlatma**
```csharp
// Üretim yapısı
ResourceProducer producer = building.GetComponent<ResourceProducer>();

// İşçi ata
producer.AssignWorker();

// Üretimi başlat
producer.StartProduction();

// Event dinle
producer.OnProductionComplete.AddListener((type, amount) => {
    Debug.Log($"Üretim tamamlandı: {amount} {type}");
});
```

**AutoResourceManager Kullanımı**
```csharp
// Otomatik yönetici
AutoResourceManager manager = FindObjectOfType<AutoResourceManager>();

// Yeni köylü ekle
manager.AddGatherer(ResourceType.Stone);

// Köylü sayısını kontrol et
int woodGatherers = manager.GetGathererCount(ResourceType.Wood);

// Dengelemeyi kapat
manager.SetAutoBalance(false);
```

## UI ve Gameplay Sistemleri

### UI Bileşenleri

#### ResourceDisplayUI
Kaynak göstergelerini yönetir:
- Wood, Stone, Iron, Food göstergeleri
- Storage doluluk barları
- Nüfus göstergesi
- Otomatik güncelleme (VillageManager event'leri ile)

#### BuildingMenuUI
İnşa menüsünü yönetir:
- B tuşu ile aç/kapat
- Yapı listesi (prefab'lar)
- Maliyet gösterimi
- BuildingPlacer ile entegrasyon

#### BuildingInfoUI
Seçili yapı bilgilerini gösterir:
- Yapı adı ve durumu
- Can göstergesi
- Onarım butonu ve maliyeti
- Yıkım butonu

#### GameSpeedController
Oyun hızını kontrol eder:
- Pause/Unpause (Space)
- Normal hız (1)
- Hızlı mod 2x (2)
- Görsel feedback

### Gameplay Sistemleri

#### BuildingSelector
Yapı seçme sistemi:
- Fare tıklama ile seçim
- Selection indicator (sarı highlight)
- BuildingInfoUI ile entegrasyon
- UI üzerindeyken seçim yapmaz (EventSystem check)

#### QuestSystem
Görev/hedef sistemi:
- Başlangıç görevleri (100 odun, 3 ev onar)
- Otomatik ilerleme kontrolü
- Quest tipleri: CollectResource, BuildStructure, RepairBuildings, ReachPopulation
- Ödül sistemi

#### SpriteGenerator
Runtime placeholder sprite oluşturma:
- Kare, daire, bordered kare
- Isometric tile şekli
- Ağaç sprite'ı
- Renk ayarlama utilities

### Test Sahnesi Kurulumu

MVP test sahnesi oluşturmak için:

**1. Sahne Hazırlığı**
```
- VillageScene.unity açın
- Main Camera'ya IsometricCameraController ekli olmalı
- EventSystem oluşturun (UI için)
```

**2. UI Canvas Oluştur**
```
1. Hierarchy → Create → UI → Canvas
2. Canvas ayarları:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler: Scale With Screen Size
   - Reference Resolution: 1920x1080
3. UI Script'leri ekle:
   - ResourceDisplayUI
   - BuildingMenuUI
   - BuildingInfoUI
   - GameSpeedController
   - QuestUI
```

**3. Kaynak Node'ları**
```
1. GameObject oluştur → Tree
2. Component ekle:
   - SpriteRenderer (SpriteGenerator ile sprite atayın)
   - CircleCollider2D
   - IsometricSpriteSorter
   - ResourceNode (Resource Type: Wood)
3. Birkaç kez duplicate edin
4. Stone ve Iron node'ları için tekrarlayın
```

**4. Harabe Evler**
```
1. Building Prefab oluşturun
2. BuildingData ScriptableObject oluşturun
3. Sahneye 3-5 ev yerleştirin
4. Condition: Ruined/Burned olarak ayarlayın
5. VillageManager'a kaydedin
```

**5. Köylüler**
```
1. Villager prefab oluşturun
2. Component'ler:
   - SpriteRenderer (sarı daire)
   - IsometricCharacterController
   - ResourceGatherer
3. AutoResourceManager ile spawn edin
```

**6. Managers**
```
VillageManager GameObject'ine ekle:
- VillageManager
- ConstructionSystem
- BuildingPlacer
- AutoResourceManager
- BuildingSelector
- QuestSystem
```

## Geliştirme İpuçları

1. **Sprite Sıralama**: Hareketli tüm objelere `IsometricSpriteSorter` ekleyin
2. **Koordinat Sistemi**: `IsometricHelper` sınıfını kullanarak koordinat dönüşümleri yapın
3. **Fizik**: Proje 2D fizik için yapılandırılmıştır (gravity = 0)
4. **Tile Boyutu**: Standart isometric tile boyutu 1:0.5 oranındadır
5. **Yapı Durumları**: Her yapıya farklı durumlar için sprite'lar atayın
6. **Kaynak Yönetimi**: VillageManager singleton'ı üzerinden tüm kaynakları yönetin

## Yeni Özellikler Ekleme

1. `Assets/Scripts/` klasöründe yeni C# scriptleri oluşturun
2. `Assets/Scenes/` klasöründe yeni sahneler oluşturun
3. `Assets/Sprites/` klasörüne isometric grafik dosyalarınızı ekleyin
4. `Assets/Prefabs/` klasöründe tekrar kullanılabilir objeler oluşturun

## Lisans

Tüm hakları saklıdır." 
