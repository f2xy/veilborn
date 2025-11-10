using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Seçili yapının bilgilerini gösteren UI paneli
/// </summary>
public class BuildingInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI buildingConditionText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Button repairButton;
    [SerializeField] private Button demolishButton;
    [SerializeField] private TextMeshProUGUI repairCostText;

    private Building selectedBuilding;

    private void Start()
    {
        // Buton event'lerini bağla
        if (repairButton != null)
            repairButton.onClick.AddListener(OnRepairClicked);

        if (demolishButton != null)
            demolishButton.onClick.AddListener(OnDemolishClicked);

        // Başlangıçta kapat
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    /// <summary>
    /// Yapıyı seç ve bilgilerini göster
    /// </summary>
    public void ShowBuildingInfo(Building building)
    {
        selectedBuilding = building;

        if (building == null)
        {
            HideInfo();
            return;
        }

        // Paneli aç
        if (infoPanel != null)
            infoPanel.SetActive(true);

        UpdateInfo();
    }

    /// <summary>
    /// Bilgileri güncelle
    /// </summary>
    private void UpdateInfo()
    {
        if (selectedBuilding == null || selectedBuilding.Data == null) return;

        // İsim
        if (buildingNameText != null)
            buildingNameText.text = selectedBuilding.Data.buildingName;

        // Durum
        if (buildingConditionText != null)
        {
            string conditionStr = GetConditionString(selectedBuilding.Condition);
            buildingConditionText.text = $"Durum: {conditionStr}";

            // Renk kodlama
            buildingConditionText.color = GetConditionColor(selectedBuilding.Condition);
        }

        // Can
        if (healthText != null)
            healthText.text = $"{selectedBuilding.CurrentHealth}/{selectedBuilding.MaxHealth}";

        if (healthBar != null)
            healthBar.fillAmount = selectedBuilding.HealthPercentage;

        // Onarım maliyeti
        if (repairCostText != null && selectedBuilding.Data != null)
        {
            int woodCost = Mathf.RoundToInt(selectedBuilding.Data.woodCost * selectedBuilding.Data.repairCostMultiplier);
            int stoneCost = Mathf.RoundToInt(selectedBuilding.Data.stoneCost * selectedBuilding.Data.repairCostMultiplier);
            int ironCost = Mathf.RoundToInt(selectedBuilding.Data.ironCost * selectedBuilding.Data.repairCostMultiplier);

            repairCostText.text = $"Onarım: {woodCost}W {stoneCost}S {ironCost}I";
        }

        // Buton durumları
        if (repairButton != null)
            repairButton.interactable = selectedBuilding.HealthPercentage < 1f && !selectedBuilding.IsDestroyed;

        if (demolishButton != null)
            demolishButton.interactable = !selectedBuilding.IsDestroyed;
    }

    /// <summary>
    /// Bilgileri gizle
    /// </summary>
    public void HideInfo()
    {
        selectedBuilding = null;

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    /// <summary>
    /// Onarım butonuna tıklandı
    /// </summary>
    private void OnRepairClicked()
    {
        if (selectedBuilding == null) return;

        // Can miktarını tam doldur
        int repairAmount = selectedBuilding.MaxHealth - selectedBuilding.CurrentHealth;
        selectedBuilding.Repair(repairAmount);

        // Maliyeti düş (basitleştirilmiş - daha sonra geliştirilebilir)
        if (selectedBuilding.Data != null)
        {
            int woodCost = Mathf.RoundToInt(selectedBuilding.Data.woodCost * selectedBuilding.Data.repairCostMultiplier);
            int stoneCost = Mathf.RoundToInt(selectedBuilding.Data.stoneCost * selectedBuilding.Data.repairCostMultiplier);
            int ironCost = Mathf.RoundToInt(selectedBuilding.Data.ironCost * selectedBuilding.Data.repairCostMultiplier);

            VillageManager.Instance.SpendResource(ResourceType.Wood, woodCost);
            VillageManager.Instance.SpendResource(ResourceType.Stone, stoneCost);
            VillageManager.Instance.SpendResource(ResourceType.Iron, ironCost);
        }

        UpdateInfo();
    }

    /// <summary>
    /// Yıkım butonuna tıklandı
    /// </summary>
    private void OnDemolishClicked()
    {
        if (selectedBuilding == null) return;

        // Yapıyı yok et
        VillageManager.Instance.UnregisterBuilding(selectedBuilding);
        Destroy(selectedBuilding.gameObject);

        HideInfo();
    }

    /// <summary>
    /// Durum string'ini al
    /// </summary>
    private string GetConditionString(BuildingCondition condition)
    {
        return condition switch
        {
            BuildingCondition.Destroyed => "Yıkık",
            BuildingCondition.Ruined => "Harabe",
            BuildingCondition.Burned => "Yanmış",
            BuildingCondition.Damaged => "Hasarlı",
            BuildingCondition.UnderConstruction => "İnşa Halinde",
            BuildingCondition.Good => "İyi",
            BuildingCondition.Excellent => "Mükemmel",
            _ => "Bilinmiyor"
        };
    }

    /// <summary>
    /// Durum rengini al
    /// </summary>
    private Color GetConditionColor(BuildingCondition condition)
    {
        return condition switch
        {
            BuildingCondition.Destroyed => Color.red,
            BuildingCondition.Ruined => new Color(1f, 0.5f, 0f), // Orange
            BuildingCondition.Burned => new Color(0.5f, 0.25f, 0f), // Dark orange
            BuildingCondition.Damaged => Color.yellow,
            BuildingCondition.UnderConstruction => Color.cyan,
            BuildingCondition.Good => Color.green,
            BuildingCondition.Excellent => new Color(0f, 1f, 0.5f), // Bright green
            _ => Color.white
        };
    }

    private void Update()
    {
        // Seçili yapı varsa bilgileri güncelle
        if (selectedBuilding != null && infoPanel != null && infoPanel.activeSelf)
        {
            UpdateInfo();
        }
    }
}
