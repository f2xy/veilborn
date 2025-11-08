using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Köy yönetim sistemi - Singleton
/// Tüm yapıları ve köy kaynaklarını yönetir
/// </summary>
public class VillageManager : MonoBehaviour
{
    private static VillageManager instance;
    public static VillageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VillageManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("VillageManager");
                    instance = go.AddComponent<VillageManager>();
                }
            }
            return instance;
        }
    }

    [Header("Village Settings")]
    [Tooltip("Köyün adı")]
    [SerializeField] private string villageName = "Veilborn Köyü";

    [Tooltip("Köy seviyesi")]
    [SerializeField] private int villageLevel = 1;

    [Header("Resources")]
    [Tooltip("Odun")]
    [SerializeField] private int wood = 100;

    [Tooltip("Taş")]
    [SerializeField] private int stone = 50;

    [Tooltip("Demir")]
    [SerializeField] private int iron = 20;

    [Tooltip("Yiyecek")]
    [SerializeField] private int food = 50;

    [Header("Population")]
    [Tooltip("Mevcut nüfus")]
    [SerializeField] private int currentPopulation = 10;

    [Tooltip("Maksimum nüfus (konut kapasitesi)")]
    [SerializeField] private int maxPopulation = 0;

    [Header("Buildings")]
    [Tooltip("Sahne başlangıcında varolan yapılar")]
    [SerializeField] private List<Building> existingBuildings = new List<Building>();

    // Runtime data
    private List<Building> allBuildings = new List<Building>();
    private Dictionary<BuildingType, List<Building>> buildingsByType = new Dictionary<BuildingType, List<Building>>();

    // Events
    public UnityEvent<int> OnWoodChanged;
    public UnityEvent<int> OnStoneChanged;
    public UnityEvent<int> OnIronChanged;
    public UnityEvent<int> OnFoodChanged;
    public UnityEvent<int, int> OnPopulationChanged; // current, max
    public UnityEvent<Building> OnBuildingAdded;
    public UnityEvent<Building> OnBuildingRemoved;

    // Properties
    public string VillageName => villageName;
    public int VillageLevel => villageLevel;
    public int Wood => wood;
    public int Stone => stone;
    public int Iron => iron;
    public int Food => food;
    public int CurrentPopulation => currentPopulation;
    public int MaxPopulation => maxPopulation;
    public List<Building> AllBuildings => allBuildings;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeVillage();
    }

    /// <summary>
    /// Köyü başlat
    /// </summary>
    private void InitializeVillage()
    {
        // Sahnede varolan yapıları bul
        if (existingBuildings.Count == 0)
        {
            existingBuildings.AddRange(FindObjectsOfType<Building>());
        }

        // Tüm yapıları kaydet
        foreach (var building in existingBuildings)
        {
            RegisterBuilding(building);
        }

        UpdateMaxPopulation();
    }

    /// <summary>
    /// Yapıyı sisteme kaydet
    /// </summary>
    public void RegisterBuilding(Building building)
    {
        if (building == null || allBuildings.Contains(building)) return;

        allBuildings.Add(building);

        // Tipe göre gruplama
        BuildingType type = building.Data.buildingType;
        if (!buildingsByType.ContainsKey(type))
        {
            buildingsByType[type] = new List<Building>();
        }
        buildingsByType[type].Add(building);

        // Event'leri dinle
        building.OnBuildingDestroyed.AddListener(() => OnBuildingDestroyedHandler(building));
        building.OnBuildingCompleted.AddListener(() => UpdateMaxPopulation());

        OnBuildingAdded?.Invoke(building);
        UpdateMaxPopulation();
    }

    /// <summary>
    /// Yapıyı sistemden kaldır
    /// </summary>
    public void UnregisterBuilding(Building building)
    {
        if (building == null || !allBuildings.Contains(building)) return;

        allBuildings.Remove(building);

        BuildingType type = building.Data.buildingType;
        if (buildingsByType.ContainsKey(type))
        {
            buildingsByType[type].Remove(building);
        }

        OnBuildingRemoved?.Invoke(building);
        UpdateMaxPopulation();
    }

    /// <summary>
    /// Yapı yıkıldığında çağrılır
    /// </summary>
    private void OnBuildingDestroyedHandler(Building building)
    {
        UpdateMaxPopulation();
    }

    /// <summary>
    /// Maksimum nüfusu güncelle
    /// </summary>
    private void UpdateMaxPopulation()
    {
        int oldMax = maxPopulation;
        maxPopulation = 0;

        foreach (var building in allBuildings)
        {
            if (building.CanBeUsed())
            {
                maxPopulation += building.Data.housingCapacity;
            }
        }

        if (oldMax != maxPopulation)
        {
            OnPopulationChanged?.Invoke(currentPopulation, maxPopulation);
        }
    }

    /// <summary>
    /// Kaynak ekle
    /// </summary>
    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                wood += amount;
                OnWoodChanged?.Invoke(wood);
                break;
            case ResourceType.Stone:
                stone += amount;
                OnStoneChanged?.Invoke(stone);
                break;
            case ResourceType.Iron:
                iron += amount;
                OnIronChanged?.Invoke(iron);
                break;
            case ResourceType.Food:
                food += amount;
                OnFoodChanged?.Invoke(food);
                break;
        }
    }

    /// <summary>
    /// Kaynak harca
    /// </summary>
    public bool SpendResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                if (wood >= amount)
                {
                    wood -= amount;
                    OnWoodChanged?.Invoke(wood);
                    return true;
                }
                break;
            case ResourceType.Stone:
                if (stone >= amount)
                {
                    stone -= amount;
                    OnStoneChanged?.Invoke(stone);
                    return true;
                }
                break;
            case ResourceType.Iron:
                if (iron >= amount)
                {
                    iron -= amount;
                    OnIronChanged?.Invoke(iron);
                    return true;
                }
                break;
            case ResourceType.Food:
                if (food >= amount)
                {
                    food -= amount;
                    OnFoodChanged?.Invoke(food);
                    return true;
                }
                break;
        }
        return false;
    }

    /// <summary>
    /// İnşa maliyetini kontrol et
    /// </summary>
    public bool CanAffordBuilding(BuildingData data)
    {
        return wood >= data.woodCost &&
               stone >= data.stoneCost &&
               iron >= data.ironCost;
    }

    /// <summary>
    /// İnşa maliyetini öde
    /// </summary>
    public bool PayForBuilding(BuildingData data)
    {
        if (!CanAffordBuilding(data)) return false;

        SpendResource(ResourceType.Wood, data.woodCost);
        SpendResource(ResourceType.Stone, data.stoneCost);
        SpendResource(ResourceType.Iron, data.ironCost);

        return true;
    }

    /// <summary>
    /// Belirli tipteki yapıları al
    /// </summary>
    public List<Building> GetBuildingsByType(BuildingType type)
    {
        if (buildingsByType.ContainsKey(type))
            return buildingsByType[type];
        return new List<Building>();
    }

    /// <summary>
    /// Kullanılabilir yapı sayısını al
    /// </summary>
    public int GetUsableBuildingCount(BuildingType type)
    {
        return GetBuildingsByType(type).Count(b => b.CanBeUsed());
    }

    /// <summary>
    /// Nüfus ekle/çıkar
    /// </summary>
    public void ChangePopulation(int delta)
    {
        currentPopulation = Mathf.Clamp(currentPopulation + delta, 0, maxPopulation);
        OnPopulationChanged?.Invoke(currentPopulation, maxPopulation);
    }

    /// <summary>
    /// Köy seviyesini artır
    /// </summary>
    public void LevelUp()
    {
        villageLevel++;
    }
}

/// <summary>
/// Kaynak tipleri
/// </summary>
public enum ResourceType
{
    Wood,
    Stone,
    Iron,
    Food
}
