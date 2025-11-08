using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tüm yapılar için temel sınıf
/// Her yapı prefab'ına bu component eklenmeli
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(IsometricSpriteSorter))]
public class Building : MonoBehaviour
{
    [Header("Building Configuration")]
    [Tooltip("Bu yapının data'sı")]
    [SerializeField] private BuildingData buildingData;

    [Tooltip("Başlangıç durumu")]
    [SerializeField] private BuildingCondition startingCondition = BuildingCondition.Good;

    [Header("Runtime Info")]
    [Tooltip("Mevcut durum")]
    [SerializeField] private BuildingCondition currentCondition;

    [Tooltip("Mevcut can")]
    [SerializeField] private int currentHealth;

    [Tooltip("İnşa ilerleme durumu (0-1)")]
    [SerializeField] private float constructionProgress = 0f;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private IsometricSpriteSorter spriteSorter;

    // Events
    public UnityEvent<BuildingCondition> OnConditionChanged;
    public UnityEvent<float> OnConstructionProgress;
    public UnityEvent OnBuildingDestroyed;
    public UnityEvent OnBuildingRepaired;
    public UnityEvent OnBuildingCompleted;

    // Properties
    public BuildingData Data => buildingData;
    public BuildingCondition Condition => currentCondition;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => buildingData != null ? buildingData.maxHealth : 100;
    public float HealthPercentage => MaxHealth > 0 ? (float)currentHealth / MaxHealth : 0f;
    public bool IsDestroyed => currentCondition == BuildingCondition.Destroyed;
    public bool IsUsable => currentCondition == BuildingCondition.Good ||
                             currentCondition == BuildingCondition.Excellent ||
                             currentCondition == BuildingCondition.Damaged;
    public bool IsUnderConstruction => currentCondition == BuildingCondition.UnderConstruction;
    public Vector2Int GridPosition { get; set; }

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteSorter == null)
            spriteSorter = GetComponent<IsometricSpriteSorter>();

        if (buildingData == null)
        {
            Debug.LogError($"Building {gameObject.name} has no BuildingData assigned!");
            return;
        }
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Yapıyı başlat
    /// </summary>
    public void Initialize()
    {
        currentCondition = startingCondition;
        currentHealth = MaxHealth;

        // Eğer yıkık veya harabe durumundaysa can azalt
        switch (currentCondition)
        {
            case BuildingCondition.Destroyed:
                currentHealth = 0;
                break;
            case BuildingCondition.Ruined:
            case BuildingCondition.Burned:
                currentHealth = Mathf.RoundToInt(MaxHealth * 0.1f);
                break;
            case BuildingCondition.Damaged:
                currentHealth = Mathf.RoundToInt(MaxHealth * 0.5f);
                break;
            case BuildingCondition.UnderConstruction:
                currentHealth = 0;
                constructionProgress = 0f;
                break;
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Görünümü güncelle
    /// </summary>
    private void UpdateVisuals()
    {
        if (buildingData == null || spriteRenderer == null) return;

        spriteRenderer.sprite = buildingData.GetSpriteForCondition(currentCondition);

        // İnşa sırasındaysa, ilerlemeye göre sprite seç
        if (IsUnderConstruction && buildingData.constructionSprites != null &&
            buildingData.constructionSprites.Length > 0)
        {
            int spriteIndex = Mathf.FloorToInt(constructionProgress * (buildingData.constructionSprites.Length - 1));
            spriteIndex = Mathf.Clamp(spriteIndex, 0, buildingData.constructionSprites.Length - 1);
            spriteRenderer.sprite = buildingData.constructionSprites[spriteIndex];
        }
    }

    /// <summary>
    /// Yapıya hasar ver
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (IsDestroyed) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        // Can durumuna göre condition güncelle
        UpdateConditionByHealth();
        UpdateVisuals();

        if (currentHealth <= 0)
        {
            Destroy();
        }
    }

    /// <summary>
    /// Yapıyı onar
    /// </summary>
    public void Repair(int amount)
    {
        if (IsDestroyed) return;

        currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
        UpdateConditionByHealth();
        UpdateVisuals();

        if (currentHealth >= MaxHealth)
        {
            OnBuildingRepaired?.Invoke();
        }
    }

    /// <summary>
    /// Can durumuna göre condition'ı güncelle
    /// </summary>
    private void UpdateConditionByHealth()
    {
        BuildingCondition oldCondition = currentCondition;

        float healthPercent = HealthPercentage;

        if (healthPercent <= 0f)
            currentCondition = BuildingCondition.Destroyed;
        else if (healthPercent < 0.25f)
            currentCondition = BuildingCondition.Ruined;
        else if (healthPercent < 0.5f)
            currentCondition = BuildingCondition.Damaged;
        else if (healthPercent < 0.9f)
            currentCondition = BuildingCondition.Good;
        else
            currentCondition = BuildingCondition.Excellent;

        if (oldCondition != currentCondition)
        {
            OnConditionChanged?.Invoke(currentCondition);
        }
    }

    /// <summary>
    /// Yapıyı yok et
    /// </summary>
    private void Destroy()
    {
        currentCondition = BuildingCondition.Destroyed;
        currentHealth = 0;
        UpdateVisuals();
        OnBuildingDestroyed?.Invoke();
    }

    /// <summary>
    /// İnşa ilerlemesini güncelle
    /// </summary>
    public void UpdateConstruction(float deltaProgress)
    {
        if (!IsUnderConstruction) return;

        constructionProgress = Mathf.Clamp01(constructionProgress + deltaProgress);
        OnConstructionProgress?.Invoke(constructionProgress);
        UpdateVisuals();

        if (constructionProgress >= 1f)
        {
            CompleteConstruction();
        }
    }

    /// <summary>
    /// İnşayı tamamla
    /// </summary>
    private void CompleteConstruction()
    {
        currentCondition = BuildingCondition.Good;
        currentHealth = MaxHealth;
        constructionProgress = 1f;
        UpdateVisuals();
        OnBuildingCompleted?.Invoke();
    }

    /// <summary>
    /// Yapıyı belirli bir duruma ayarla
    /// </summary>
    public void SetCondition(BuildingCondition condition)
    {
        BuildingCondition oldCondition = currentCondition;
        currentCondition = condition;

        switch (condition)
        {
            case BuildingCondition.Destroyed:
                currentHealth = 0;
                break;
            case BuildingCondition.Ruined:
            case BuildingCondition.Burned:
                currentHealth = Mathf.RoundToInt(MaxHealth * 0.1f);
                break;
            case BuildingCondition.Damaged:
                currentHealth = Mathf.RoundToInt(MaxHealth * 0.5f);
                break;
            case BuildingCondition.UnderConstruction:
                currentHealth = 0;
                constructionProgress = 0f;
                break;
            case BuildingCondition.Good:
                currentHealth = Mathf.RoundToInt(MaxHealth * 0.8f);
                break;
            case BuildingCondition.Excellent:
                currentHealth = MaxHealth;
                break;
        }

        UpdateVisuals();

        if (oldCondition != currentCondition)
        {
            OnConditionChanged?.Invoke(currentCondition);
        }
    }

    /// <summary>
    /// Yapının kullanılabilir olup olmadığını kontrol et
    /// </summary>
    public bool CanBeUsed()
    {
        return IsUsable && !IsUnderConstruction;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (buildingData == null) return;

        // Grid boyutunu göster
        Gizmos.color = Color.yellow;
        Vector3 size = new Vector3(buildingData.width, buildingData.height, 0);
        Gizmos.DrawWireCube(transform.position, size);

        // Savunma menzilini göster
        if (buildingData.isDefense && buildingData.defenseRange > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, buildingData.defenseRange);
        }
    }
#endif
}
