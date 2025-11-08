"# Veilborn

Unity 2D Isometric oyun projesi.

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
