using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Yapı inşa menüsünü yöneten UI
/// </summary>
public class BuildingMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buildingButtonPrefab;

    [Header("Building Prefabs")]
    [SerializeField] private List<BuildingPrefabEntry> buildingPrefabs = new List<BuildingPrefabEntry>();

    [Header("References")]
    [SerializeField] private BuildingPlacer buildingPlacer;

    private bool isMenuOpen = false;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    private void Start()
    {
        if (buildingPlacer == null)
            buildingPlacer = FindObjectOfType<BuildingPlacer>();

        // Menüyü kapat
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // Butonları oluştur
        CreateBuildingButtons();
    }

    private void Update()
    {
        // B tuşu ile menüyü aç/kapat
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleMenu();
        }

        // ESC ile kapat
        if (Input.GetKeyDown(KeyCode.Escape) && isMenuOpen)
        {
            CloseMenu();
        }
    }

    /// <summary>
    /// Yapı butonlarını oluştur
    /// </summary>
    private void CreateBuildingButtons()
    {
        if (buttonContainer == null || buildingButtonPrefab == null) return;

        // Eski butonları temizle
        foreach (var btn in spawnedButtons)
        {
            if (btn != null) Destroy(btn);
        }
        spawnedButtons.Clear();

        // Her yapı için buton oluştur
        foreach (var entry in buildingPrefabs)
        {
            if (entry.prefab == null) continue;

            GameObject buttonObj = Instantiate(buildingButtonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();

            if (button != null)
            {
                // Buton tıklama eventi
                GameObject prefab = entry.prefab; // Local copy for closure
                button.onClick.AddListener(() => StartBuildingPlacement(prefab));

                // Buton metnini ayarla
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    Building building = prefab.GetComponent<Building>();
                    if (building != null && building.Data != null)
                    {
                        string costText = $"\n{building.Data.woodCost}W {building.Data.stoneCost}S {building.Data.ironCost}I";
                        buttonText.text = $"{entry.displayName}{costText}";
                    }
                    else
                    {
                        buttonText.text = entry.displayName;
                    }
                }

                spawnedButtons.Add(buttonObj);
            }
        }
    }

    /// <summary>
    /// Yapı yerleştirmeyi başlat
    /// </summary>
    private void StartBuildingPlacement(GameObject prefab)
    {
        if (buildingPlacer == null) return;

        buildingPlacer.StartPlacement(prefab);
        CloseMenu();
    }

    /// <summary>
    /// Menüyü aç/kapat
    /// </summary>
    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (menuPanel != null)
            menuPanel.SetActive(isMenuOpen);
    }

    /// <summary>
    /// Menüyü aç
    /// </summary>
    public void OpenMenu()
    {
        isMenuOpen = true;
        if (menuPanel != null)
            menuPanel.SetActive(true);
    }

    /// <summary>
    /// Menüyü kapat
    /// </summary>
    public void CloseMenu()
    {
        isMenuOpen = false;
        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

    /// <summary>
    /// Yapı ekle
    /// </summary>
    public void AddBuilding(GameObject prefab, string displayName)
    {
        buildingPrefabs.Add(new BuildingPrefabEntry { prefab = prefab, displayName = displayName });
        CreateBuildingButtons();
    }
}

[System.Serializable]
public class BuildingPrefabEntry
{
    public GameObject prefab;
    public string displayName;
}
