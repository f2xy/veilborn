using UnityEngine;

/// <summary>
/// Köydeki yapı tiplerini tanımlar
/// </summary>
public enum BuildingType
{
    // Konut yapıları
    House,              // Ev
    Tent,               // Çadır (geçici)

    // Üretim yapıları
    Blacksmith,         // Demir atölyesi
    SwordWorkshop,      // Kılıç atölyesi

    // Depolama
    Warehouse,          // Ambar
    Barn,               // Ahır

    // Özel yapılar
    HealerHut,          // Şifacı kulübesi
    MonumentTower,      // Anıt kule/kaide
    WatchTower,         // Gözlem kulesi

    // Savunma
    Wall,               // Duvar
    Gate                // Kapı
}

/// <summary>
/// Yapının mevcut durumunu tanımlar
/// </summary>
public enum BuildingCondition
{
    Destroyed,          // Tamamen yıkık
    Ruined,             // Harabe/kullanılamaz
    Burned,             // Yanmış
    Damaged,            // Hasarlı ama kullanılabilir
    UnderConstruction,  // Yapım aşamasında
    Good,               // İyi durumda
    Excellent           // Mükemmel durumda
}

/// <summary>
/// Yapının kategorisini tanımlar
/// </summary>
public enum BuildingCategory
{
    Residential,        // Konut
    Production,         // Üretim
    Storage,            // Depolama
    Defense,            // Savunma
    Special             // Özel
}
