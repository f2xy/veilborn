using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Oyun sahnesini runtime'da test objeleriyle doldurur.
/// İlk başlatmada otomatik olarak binalar, kaynaklar ve köylüler oluşturur.
/// </summary>
public class RuntimeSceneSetup : MonoBehaviour
{
    [Header("Setup Options")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool setupOnlyIfEmpty = true;

    [Header("Test Objects")]
    [SerializeField] private int housesToCreate = 5;
    [SerializeField] private int treeNodesToCreate = 8;
    [SerializeField] private int stoneNodesToCreate = 5;
    [SerializeField] private int ironNodesToCreate = 3;
    [SerializeField] private int villagersToCreate = 3;

    [Header("Placement Area")]
    [SerializeField] private Vector2 placementAreaMin = new Vector2(-15, -10);
    [SerializeField] private Vector2 placementAreaMax = new Vector2(15, 10);

    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupScene();
        }
    }

    private void Awake()
    {
        // BuildingSelector'ı kameraya ekle (eğer yoksa)
        SetupBuildingSelector();
    }

    /// <summary>
    /// Sahneyi test objeleriyle doldur
    /// </summary>
    public void SetupScene()
    {
        // Eğer zaten objeler varsa ve sadece boşsa setup etmek isteniyorsa, çık
        if (setupOnlyIfEmpty && GameObject.FindObjectsOfType<Building>().Length > 0)
        {
            Debug.Log("Sahne zaten objelerle dolu, setup atlandı.");
            return;
        }

        Debug.Log("Sahne setup başlatılıyor...");

        CreateBuildings();
        CreateResourceNodes();
        CreateVillagers();
        CreateUI();
        SetupQuestSystem();

        Debug.Log("Sahne setup tamamlandı!");
    }

    /// <summary>
    /// Test binaları oluştur
    /// </summary>
    private void CreateBuildings()
    {
        Debug.Log($"{housesToCreate} bina oluşturuluyor...");

        for (int i = 0; i < housesToCreate; i++)
        {
            Vector3 position = GetRandomPosition();
            GameObject buildingObj = new GameObject($"House_{i + 1}");
            buildingObj.transform.position = position;

            // Sprite Renderer ekle
            SpriteRenderer sr = buildingObj.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Objects";

            // BuildingData ScriptableObject oluştur (runtime)
            BuildingData data = ScriptableObject.CreateInstance<BuildingData>();
            data.buildingType = BuildingType.House;
            data.maxHealth = 100;
            data.housingCapacity = 4;
            data.woodCost = 50;
            data.stoneCost = 30;
            data.ironCost = 0;
            data.buildTime = 10f;

            // Sprite'ları oluştur (placeholder)
            data.goodSprite = SpriteGenerator.CreateHouseSprite(64, new Color(0.8f, 0.6f, 0.4f));
            data.damagedSprite = SpriteGenerator.CreateHouseSprite(64, new Color(0.6f, 0.5f, 0.3f));
            data.ruinedSprite = SpriteGenerator.CreateSquareSprite(64, new Color(0.4f, 0.3f, 0.2f));
            data.burnedSprite = SpriteGenerator.CreateSquareSprite(64, new Color(0.2f, 0.1f, 0.1f));
            data.destroyedSprite = SpriteGenerator.CreateSquareSprite(64, new Color(0.3f, 0.2f, 0.1f));

            // Building component ekle
            Building building = buildingObj.AddComponent<Building>();
            building.Initialize(data);

            // Bazı binalar harabe olsun
            if (i < housesToCreate / 2)
            {
                BuildingCondition[] conditions = {
                    BuildingCondition.Damaged,
                    BuildingCondition.Ruined,
                    BuildingCondition.Burned
                };
                building.SetCondition(conditions[Random.Range(0, conditions.Length)]);
            }

            // Collider ekle
            BoxCollider2D collider = buildingObj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(2f, 2f);

            // Sprite sorter ekle
            IsometricSpriteSorter sorter = buildingObj.AddComponent<IsometricSpriteSorter>();

            // VillageManager'a kaydet
            if (VillageManager.Instance != null)
            {
                VillageManager.Instance.RegisterBuilding(building);
            }
        }
    }

    /// <summary>
    /// Kaynak düğümleri oluştur
    /// </summary>
    private void CreateResourceNodes()
    {
        Debug.Log("Kaynak düğümleri oluşturuluyor...");

        // Ağaçlar
        CreateResourceNodeGroup(ResourceType.Wood, treeNodesToCreate,
            SpriteGenerator.CreateTreeSprite(48), new Color(0.2f, 0.5f, 0.2f));

        // Taş düğümleri
        CreateResourceNodeGroup(ResourceType.Stone, stoneNodesToCreate,
            SpriteGenerator.CreateSquareSprite(48, new Color(0.5f, 0.5f, 0.5f)), new Color(0.6f, 0.6f, 0.6f));

        // Demir düğümleri
        CreateResourceNodeGroup(ResourceType.Iron, ironNodesToCreate,
            SpriteGenerator.CreateSquareSprite(48, new Color(0.3f, 0.3f, 0.4f)), new Color(0.4f, 0.4f, 0.5f));
    }

    private void CreateResourceNodeGroup(ResourceType type, int count, Sprite sprite, Color emptyColor)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 position = GetRandomPosition();
            GameObject nodeObj = new GameObject($"{type}Node_{i + 1}");
            nodeObj.transform.position = position;

            // Sprite Renderer ekle
            SpriteRenderer sr = nodeObj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingLayerName = "Objects";

            // ResourceNode component ekle
            ResourceNode node = nodeObj.AddComponent<ResourceNode>();
            // ResourceNode'un serialized field'larını reflection ile set et
            var typeField = typeof(ResourceNode).GetField("resourceType",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (typeField != null) typeField.SetValue(node, type);

            var amountField = typeof(ResourceNode).GetField("currentAmount",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (amountField != null) amountField.SetValue(node, Random.Range(50, 200));

            var maxAmountField = typeof(ResourceNode).GetField("maxAmount",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (maxAmountField != null) maxAmountField.SetValue(node, 200);

            // Collider ekle
            CircleCollider2D collider = nodeObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;

            // Sprite sorter ekle
            nodeObj.AddComponent<IsometricSpriteSorter>();
        }
    }

    /// <summary>
    /// Köylüler oluştur
    /// </summary>
    private void CreateVillagers()
    {
        Debug.Log($"{villagersToCreate} köylü oluşturuluyor...");

        for (int i = 0; i < villagersToCreate; i++)
        {
            Vector3 position = GetRandomPosition();
            GameObject villagerObj = new GameObject($"Villager_{i + 1}");
            villagerObj.transform.position = position;
            villagerObj.tag = "NPC";

            // Sprite Renderer ekle
            SpriteRenderer sr = villagerObj.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteGenerator.CreateCircleSprite(32, new Color(0.9f, 0.8f, 0.6f));
            sr.sortingLayerName = "Characters";

            // ResourceGatherer component ekle
            ResourceGatherer gatherer = villagerObj.AddComponent<ResourceGatherer>();

            // İlk kaynak tipini ata
            ResourceType[] types = { ResourceType.Wood, ResourceType.Stone, ResourceType.Iron };
            var targetResourceField = typeof(ResourceGatherer).GetField("targetResourceType",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (targetResourceField != null)
                targetResourceField.SetValue(gatherer, types[i % types.Length]);

            // Collider ekle
            CircleCollider2D collider = villagerObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;

            // Sprite sorter ekle
            villagerObj.AddComponent<IsometricSpriteSorter>();
        }
    }

    /// <summary>
    /// UI sistemini oluştur
    /// </summary>
    private void CreateUI()
    {
        // UI Canvas var mı kontrol et
        Canvas existingCanvas = GameObject.FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            Debug.Log("Canvas zaten mevcut, UI setup atlandı.");
            return;
        }

        Debug.Log("UI sistemi oluşturuluyor...");

        // Canvas oluştur
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // EventSystem oluştur (eğer yoksa)
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Resource Display UI
        GameObject resourceDisplayObj = new GameObject("ResourceDisplay");
        resourceDisplayObj.transform.SetParent(canvasObj.transform, false);
        ResourceDisplayUI resourceUI = resourceDisplayObj.AddComponent<ResourceDisplayUI>();

        // Building Menu UI
        GameObject buildingMenuObj = new GameObject("BuildingMenu");
        buildingMenuObj.transform.SetParent(canvasObj.transform, false);
        buildingMenuObj.AddComponent<BuildingMenuUI>();

        // Building Info UI
        GameObject buildingInfoObj = new GameObject("BuildingInfo");
        buildingInfoObj.transform.SetParent(canvasObj.transform, false);
        buildingInfoObj.AddComponent<BuildingInfoUI>();

        // Quest UI
        GameObject questUIObj = new GameObject("QuestUI");
        questUIObj.transform.SetParent(canvasObj.transform, false);
        questUIObj.AddComponent<QuestUI>();

        // Game Speed Controller
        GameObject speedControllerObj = new GameObject("GameSpeedController");
        speedControllerObj.AddComponent<GameSpeedController>();

        Debug.Log("UI sistemi oluşturuldu.");
    }

    /// <summary>
    /// Quest sistemini başlat
    /// </summary>
    private void SetupQuestSystem()
    {
        // QuestSystem zaten singleton olarak kendi başlatıyor
        if (QuestSystem.Instance != null)
        {
            Debug.Log("Quest sistemi hazır.");
        }
        else
        {
            GameObject questSystemObj = new GameObject("QuestSystem");
            questSystemObj.AddComponent<QuestSystem>();
        }
    }

    /// <summary>
    /// BuildingSelector sistemini kur
    /// </summary>
    private void SetupBuildingSelector()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.GetComponent<BuildingSelector>() == null)
        {
            mainCamera.gameObject.AddComponent<BuildingSelector>();
            Debug.Log("BuildingSelector kameraya eklendi.");
        }
    }

    /// <summary>
    /// Rastgele pozisyon üret
    /// </summary>
    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(placementAreaMin.x, placementAreaMax.x);
        float y = Random.Range(placementAreaMin.y, placementAreaMax.y);
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Sahneyi temizle (tüm test objelerini sil)
    /// </summary>
    public void ClearScene()
    {
        // Tüm binaları sil
        foreach (Building building in GameObject.FindObjectsOfType<Building>())
        {
            DestroyImmediate(building.gameObject);
        }

        // Tüm kaynak düğümlerini sil
        foreach (ResourceNode node in GameObject.FindObjectsOfType<ResourceNode>())
        {
            DestroyImmediate(node.gameObject);
        }

        // Tüm köylüleri sil
        foreach (ResourceGatherer gatherer in GameObject.FindObjectsOfType<ResourceGatherer>())
        {
            DestroyImmediate(gatherer.gameObject);
        }

        Debug.Log("Sahne temizlendi.");
    }
}
