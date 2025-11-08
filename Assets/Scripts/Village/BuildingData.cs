using UnityEngine;

/// <summary>
/// Yapı bilgilerini tutan ScriptableObject
/// Her yapı tipi için bir tane oluşturulmalı
/// </summary>
[CreateAssetMenu(fileName = "BuildingData", menuName = "Veilborn/Building Data", order = 1)]
public class BuildingData : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Yapının tipi")]
    public BuildingType buildingType;

    [Tooltip("Yapının kategorisi")]
    public BuildingCategory category;

    [Tooltip("Yapının adı")]
    public string buildingName;

    [Tooltip("Yapının açıklaması")]
    [TextArea(3, 5)]
    public string description;

    [Header("Visual")]
    [Tooltip("Yapının iyi durumdaki sprite'ı")]
    public Sprite goodSprite;

    [Tooltip("Yapının hasarlı durumdaki sprite'ı")]
    public Sprite damagedSprite;

    [Tooltip("Yapının harabe durumdaki sprite'ı")]
    public Sprite ruinedSprite;

    [Tooltip("Yapının yanmış durumdaki sprite'ı")]
    public Sprite burnedSprite;

    [Tooltip("Yapının yıkık durumdaki sprite'ı")]
    public Sprite destroyedSprite;

    [Tooltip("İnşa sırasında gösterilecek sprite'lar")]
    public Sprite[] constructionSprites;

    [Header("Grid Settings")]
    [Tooltip("Yapının genişliği (grid hücreleri)")]
    public int width = 1;

    [Tooltip("Yapının yüksekliği (grid hücreleri)")]
    public int height = 1;

    [Header("Gameplay")]
    [Tooltip("İnşa maliyeti - Odun")]
    public int woodCost = 0;

    [Tooltip("İnşa maliyeti - Taş")]
    public int stoneCost = 0;

    [Tooltip("İnşa maliyeti - Demir")]
    public int ironCost = 0;

    [Tooltip("İnşa süresi (saniye)")]
    public float buildTime = 10f;

    [Tooltip("Onarım maliyeti (normal maliyetin yüzdesi)")]
    [Range(0f, 1f)]
    public float repairCostMultiplier = 0.5f;

    [Tooltip("Maksimum can")]
    public int maxHealth = 100;

    [Tooltip("Bu yapıyı inşa etmek için gereken yapılar")]
    public BuildingType[] prerequisites;

    [Header("Functionality")]
    [Tooltip("Bu yapı kaç kişiyi barındırabilir")]
    public int housingCapacity = 0;

    [Tooltip("Bu yapı kaç kaynak saklayabilir")]
    public int storageCapacity = 0;

    [Tooltip("Üretim yapısı mı?")]
    public bool isProduction = false;

    [Tooltip("Savunma yapısı mı?")]
    public bool isDefense = false;

    [Tooltip("Savunma menzili (savunma yapıları için)")]
    public float defenseRange = 0f;

    [Tooltip("Savunma hasarı (savunma yapıları için)")]
    public int defenseArmor = 0;

    /// <summary>
    /// Toplam inşa maliyetini hesapla
    /// </summary>
    public int GetTotalCost()
    {
        return woodCost + stoneCost + ironCost;
    }

    /// <summary>
    /// Belirli bir duruma göre sprite al
    /// </summary>
    public Sprite GetSpriteForCondition(BuildingCondition condition)
    {
        switch (condition)
        {
            case BuildingCondition.Destroyed:
                return destroyedSprite != null ? destroyedSprite : ruinedSprite;
            case BuildingCondition.Ruined:
                return ruinedSprite != null ? ruinedSprite : damagedSprite;
            case BuildingCondition.Burned:
                return burnedSprite != null ? burnedSprite : ruinedSprite;
            case BuildingCondition.Damaged:
                return damagedSprite != null ? damagedSprite : goodSprite;
            case BuildingCondition.UnderConstruction:
                return constructionSprites != null && constructionSprites.Length > 0
                    ? constructionSprites[0]
                    : goodSprite;
            case BuildingCondition.Good:
            case BuildingCondition.Excellent:
            default:
                return goodSprite;
        }
    }
}
