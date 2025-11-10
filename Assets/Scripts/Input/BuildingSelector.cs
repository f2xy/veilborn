using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Yapı seçme sistemi
/// Farelekle tıklayarak yapıları seçer
/// </summary>
public class BuildingSelector : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private LayerMask selectableLayer = -1;
    [SerializeField] private float selectionRadius = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject selectionIndicatorPrefab;
    [SerializeField] private Color selectionColor = Color.yellow;

    [Header("References")]
    [SerializeField] private BuildingInfoUI buildingInfoUI;

    private Building selectedBuilding;
    private GameObject selectionIndicator;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (buildingInfoUI == null)
            buildingInfoUI = FindObjectOfType<BuildingInfoUI>();
    }

    private void Update()
    {
        HandleMouseInput();
    }

    /// <summary>
    /// Fare inputunu işle
    /// </summary>
    private void HandleMouseInput()
    {
        // UI üzerindeyse işlem yapma
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Sol tıklama
        if (Input.GetMouseButtonDown(0))
        {
            SelectBuildingAtMousePosition();
        }

        // Sağ tıklama - seçimi kaldır
        if (Input.GetMouseButtonDown(1))
        {
            DeselectBuilding();
        }
    }

    /// <summary>
    /// Fare pozisyonundaki yapıyı seç
    /// </summary>
    private void SelectBuildingAtMousePosition()
    {
        if (mainCamera == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        // Yapı ara
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, selectionRadius, selectableLayer);

        Building closestBuilding = null;
        float closestDistance = float.MaxValue;

        foreach (var col in colliders)
        {
            Building building = col.GetComponent<Building>();
            if (building != null)
            {
                float distance = Vector2.Distance(worldPos, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBuilding = building;
                }
            }
        }

        if (closestBuilding != null)
        {
            SelectBuilding(closestBuilding);
        }
        else
        {
            DeselectBuilding();
        }
    }

    /// <summary>
    /// Yapıyı seç
    /// </summary>
    public void SelectBuilding(Building building)
    {
        if (selectedBuilding == building) return;

        selectedBuilding = building;

        // UI'yı güncelle
        if (buildingInfoUI != null)
        {
            buildingInfoUI.ShowBuildingInfo(building);
        }

        // Selection indicator
        UpdateSelectionIndicator();
    }

    /// <summary>
    /// Seçimi kaldır
    /// </summary>
    public void DeselectBuilding()
    {
        selectedBuilding = null;

        // UI'yı kapat
        if (buildingInfoUI != null)
        {
            buildingInfoUI.HideInfo();
        }

        // Indicator'ı kaldır
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Selection indicator'ı güncelle
    /// </summary>
    private void UpdateSelectionIndicator()
    {
        if (selectedBuilding == null)
        {
            if (selectionIndicator != null)
                selectionIndicator.SetActive(false);
            return;
        }

        // Indicator oluştur
        if (selectionIndicator == null)
        {
            if (selectionIndicatorPrefab != null)
            {
                selectionIndicator = Instantiate(selectionIndicatorPrefab);
            }
            else
            {
                // Basit bir indicator oluştur
                selectionIndicator = GameObject.CreatePrimitive(PrimitiveType.Quad);
                selectionIndicator.name = "SelectionIndicator";

                // Renderer ayarları
                var renderer = selectionIndicator.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = new Material(Shader.Find("Sprites/Default"));
                    renderer.material.color = new Color(selectionColor.r, selectionColor.g, selectionColor.b, 0.3f);
                    renderer.sortingOrder = -1; // Yapının altında
                }

                // Collider kaldır
                Destroy(selectionIndicator.GetComponent<Collider>());
            }
        }

        // Pozisyonu ayarla
        selectionIndicator.SetActive(true);
        selectionIndicator.transform.position = selectedBuilding.transform.position + Vector3.back * 0.01f;
        selectionIndicator.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    }

    private void LateUpdate()
    {
        // Indicator'ı sürekli güncelle
        if (selectedBuilding != null && selectionIndicator != null)
        {
            selectionIndicator.transform.position = selectedBuilding.transform.position + Vector3.back * 0.01f;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, selectionRadius);
    }
#endif
}
