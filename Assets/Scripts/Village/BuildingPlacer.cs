using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Yapı yerleştirme sistemi
/// Oyuncunun yeni yapılar inşa etmesini sağlar
/// </summary>
public class BuildingPlacer : MonoBehaviour
{
    [Header("Placement Settings")]
    [Tooltip("Yerleştirme modunda mı?")]
    [SerializeField] private bool isPlacementMode = false;

    [Tooltip("Yerleştirilecek yapı prefabı")]
    [SerializeField] private GameObject buildingPrefab;

    [Tooltip("Önizleme rengi - Yerleştirilebilir")]
    [SerializeField] private Color validPlacementColor = new Color(0f, 1f, 0f, 0.5f);

    [Tooltip("Önizleme rengi - Yerleştirilemez")]
    [SerializeField] private Color invalidPlacementColor = new Color(1f, 0f, 0f, 0.5f);

    [Header("Grid Settings")]
    [Tooltip("Grid snap kullan")]
    [SerializeField] private bool useGridSnap = true;

    [Tooltip("Grid boyutu")]
    [SerializeField] private float gridSize = 1f;

    [Header("Collision Check")]
    [Tooltip("Çarpışma kontrolü için layer mask")]
    [SerializeField] private LayerMask collisionMask = -1;

    [Tooltip("Çarpışma kontrol yarıçapı")]
    [SerializeField] private float collisionCheckRadius = 0.5f;

    // Private
    private GameObject previewObject;
    private SpriteRenderer previewRenderer;
    private Camera mainCamera;
    private bool canPlace = false;
    private Vector3 currentPlacementPosition;

    // Events
    public UnityEvent<Building> OnBuildingPlaced;
    public UnityEvent OnPlacementCancelled;

    // Properties
    public bool IsPlacementMode => isPlacementMode;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isPlacementMode)
        {
            UpdatePlacementPreview();
            HandlePlacementInput();
        }
    }

    /// <summary>
    /// Yerleştirme modunu başlat
    /// </summary>
    public void StartPlacement(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("BuildingPlacer: Prefab is null!");
            return;
        }

        buildingPrefab = prefab;
        isPlacementMode = true;

        // Önizleme objesi oluştur
        CreatePreview();
    }

    /// <summary>
    /// Yerleştirme modunu iptal et
    /// </summary>
    public void CancelPlacement()
    {
        isPlacementMode = false;

        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
            previewRenderer = null;
        }

        OnPlacementCancelled?.Invoke();
    }

    /// <summary>
    /// Önizleme objesi oluştur
    /// </summary>
    private void CreatePreview()
    {
        if (buildingPrefab == null) return;

        previewObject = Instantiate(buildingPrefab);

        // Building component'ini devre dışı bırak (sadece önizleme için)
        Building building = previewObject.GetComponent<Building>();
        if (building != null)
        {
            building.enabled = false;
        }

        // Collider'ları devre dışı bırak
        Collider2D[] colliders = previewObject.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Sprite renderer'ı bul
        previewRenderer = previewObject.GetComponent<SpriteRenderer>();
        if (previewRenderer != null)
        {
            previewRenderer.color = validPlacementColor;
        }

        // Sorting order'ı artır
        if (previewRenderer != null)
        {
            previewRenderer.sortingOrder = 1000; // En üstte görünsün
        }
    }

    /// <summary>
    /// Önizlemeyi güncelle
    /// </summary>
    private void UpdatePlacementPreview()
    {
        if (previewObject == null || mainCamera == null) return;

        // Mouse pozisyonunu world koordinatına çevir
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        // Grid snap
        if (useGridSnap)
        {
            worldPos = SnapToGrid(worldPos);
        }

        currentPlacementPosition = worldPos;
        previewObject.transform.position = currentPlacementPosition;

        // Yerleştirilebilir mi kontrol et
        canPlace = CheckPlacementValidity(currentPlacementPosition);

        // Rengi güncelle
        if (previewRenderer != null)
        {
            previewRenderer.color = canPlace ? validPlacementColor : invalidPlacementColor;
        }
    }

    /// <summary>
    /// Grid'e snap yap
    /// </summary>
    private Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Yerleştirmenin geçerli olup olmadığını kontrol et
    /// </summary>
    private bool CheckPlacementValidity(Vector3 position)
    {
        // Çarpışma kontrolü
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, collisionCheckRadius, collisionMask);

        // Önizleme objesi hariç başka bir şeyle çarpışıyor mu?
        foreach (var col in colliders)
        {
            if (col.gameObject != previewObject)
            {
                return false;
            }
        }

        // Maliyet kontrolü
        Building buildingComponent = buildingPrefab.GetComponent<Building>();
        if (buildingComponent != null && buildingComponent.Data != null)
        {
            if (!VillageManager.Instance.CanAffordBuilding(buildingComponent.Data))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Yerleştirme inputlarını işle
    /// </summary>
    private void HandlePlacementInput()
    {
        // Sol tık - Yerleştir
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            PlaceBuilding();
        }

        // Sağ tık veya ESC - İptal
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }

        // R tuşu - Döndür (ileride eklenebilir)
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotatePreview();
        }
    }

    /// <summary>
    /// Yapıyı yerleştir
    /// </summary>
    private void PlaceBuilding()
    {
        if (buildingPrefab == null || !canPlace) return;

        // Gerçek yapıyı oluştur
        GameObject newBuilding = Instantiate(buildingPrefab, currentPlacementPosition, Quaternion.identity);

        Building building = newBuilding.GetComponent<Building>();
        if (building != null)
        {
            // İnşa durumuna ayarla
            building.SetCondition(BuildingCondition.UnderConstruction);

            // Maliyeti öde
            if (building.Data != null)
            {
                VillageManager.Instance.PayForBuilding(building.Data);
            }

            // VillageManager'a kaydet
            VillageManager.Instance.RegisterBuilding(building);

            OnBuildingPlaced?.Invoke(building);
        }

        // Yerleştirme modunu sonlandır
        CancelPlacement();
    }

    /// <summary>
    /// Önizlemeyi döndür (ileride kullanım için)
    /// </summary>
    private void RotatePreview()
    {
        if (previewObject != null)
        {
            previewObject.transform.Rotate(0, 0, 90f);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (isPlacementMode)
        {
            Gizmos.color = canPlace ? Color.green : Color.red;
            Gizmos.DrawWireSphere(currentPlacementPosition, collisionCheckRadius);
        }
    }
#endif
}
