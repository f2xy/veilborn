using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Kaynak göstergelerini yöneten UI
/// </summary>
public class ResourceDisplayUI : MonoBehaviour
{
    [Header("Resource Text Elements")]
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI ironText;
    [SerializeField] private TextMeshProUGUI foodText;

    [Header("Storage Bar Elements (Optional)")]
    [SerializeField] private Image woodBar;
    [SerializeField] private Image stoneBar;
    [SerializeField] private Image ironBar;
    [SerializeField] private Image foodBar;

    [Header("Population")]
    [SerializeField] private TextMeshProUGUI populationText;

    [Header("Settings")]
    [SerializeField] private bool showMaxCapacity = true;
    [SerializeField] private bool showStorageBars = true;
    [SerializeField] private Color fullStorageColor = Color.red;
    [SerializeField] private Color normalStorageColor = Color.green;

    private void Start()
    {
        // Event'leri dinle
        if (VillageManager.Instance != null)
        {
            VillageManager.Instance.OnWoodChanged.AddListener(UpdateWood);
            VillageManager.Instance.OnStoneChanged.AddListener(UpdateStone);
            VillageManager.Instance.OnIronChanged.AddListener(UpdateIron);
            VillageManager.Instance.OnFoodChanged.AddListener(UpdateFood);
            VillageManager.Instance.OnPopulationChanged.AddListener(UpdatePopulation);
        }

        // İlk güncelleme
        UpdateAllDisplays();
    }

    private void UpdateAllDisplays()
    {
        if (VillageManager.Instance == null) return;

        UpdateWood(VillageManager.Instance.Wood);
        UpdateStone(VillageManager.Instance.Stone);
        UpdateIron(VillageManager.Instance.Iron);
        UpdateFood(VillageManager.Instance.Food);
        UpdatePopulation(VillageManager.Instance.CurrentPopulation, VillageManager.Instance.MaxPopulation);
    }

    private void UpdateWood(int amount)
    {
        if (woodText != null)
        {
            if (showMaxCapacity)
                woodText.text = $"{amount}/{VillageManager.Instance.MaxWood}";
            else
                woodText.text = amount.ToString();
        }

        if (showStorageBars && woodBar != null)
        {
            float percentage = VillageManager.Instance.WoodStoragePercentage;
            woodBar.fillAmount = percentage;
            woodBar.color = Color.Lerp(normalStorageColor, fullStorageColor, percentage);
        }
    }

    private void UpdateStone(int amount)
    {
        if (stoneText != null)
        {
            if (showMaxCapacity)
                stoneText.text = $"{amount}/{VillageManager.Instance.MaxStone}";
            else
                stoneText.text = amount.ToString();
        }

        if (showStorageBars && stoneBar != null)
        {
            float percentage = VillageManager.Instance.StoneStoragePercentage;
            stoneBar.fillAmount = percentage;
            stoneBar.color = Color.Lerp(normalStorageColor, fullStorageColor, percentage);
        }
    }

    private void UpdateIron(int amount)
    {
        if (ironText != null)
        {
            if (showMaxCapacity)
                ironText.text = $"{amount}/{VillageManager.Instance.MaxIron}";
            else
                ironText.text = amount.ToString();
        }

        if (showStorageBars && ironBar != null)
        {
            float percentage = VillageManager.Instance.IronStoragePercentage;
            ironBar.fillAmount = percentage;
            ironBar.color = Color.Lerp(normalStorageColor, fullStorageColor, percentage);
        }
    }

    private void UpdateFood(int amount)
    {
        if (foodText != null)
        {
            if (showMaxCapacity)
                foodText.text = $"{amount}/{VillageManager.Instance.MaxFood}";
            else
                foodText.text = amount.ToString();
        }

        if (showStorageBars && foodBar != null)
        {
            float percentage = VillageManager.Instance.FoodStoragePercentage;
            foodBar.fillAmount = percentage;
            foodBar.color = Color.Lerp(normalStorageColor, fullStorageColor, percentage);
        }
    }

    private void UpdatePopulation(int current, int max)
    {
        if (populationText != null)
        {
            populationText.text = $"{current}/{max}";
        }
    }
}
