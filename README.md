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

## Geliştirme İpuçları

1. **Sprite Sıralama**: Hareketli tüm objelere `IsometricSpriteSorter` ekleyin
2. **Koordinat Sistemi**: `IsometricHelper` sınıfını kullanarak koordinat dönüşümleri yapın
3. **Fizik**: Proje 2D fizik için yapılandırılmıştır (gravity = 0)
4. **Tile Boyutu**: Standart isometric tile boyutu 1:0.5 oranındadır

## Yeni Özellikler Ekleme

1. `Assets/Scripts/` klasöründe yeni C# scriptleri oluşturun
2. `Assets/Scenes/` klasöründe yeni sahneler oluşturun
3. `Assets/Sprites/` klasörüne isometric grafik dosyalarınızı ekleyin
4. `Assets/Prefabs/` klasöründe tekrar kullanılabilir objeler oluşturun

## Lisans

Tüm hakları saklıdır." 
