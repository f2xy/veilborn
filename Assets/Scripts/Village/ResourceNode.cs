using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Haritada bulunan kaynak noktaları (ağaç, taş, demir, vb.)
/// Köylüler bu noktalardan kaynak toplar
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(IsometricSpriteSorter))]
public class ResourceNode : MonoBehaviour
{
    [Header("Resource Settings")]
    [Tooltip("Bu kaynağın tipi")]
    [SerializeField] private ResourceType resourceType = ResourceType.Wood;

    [Tooltip("Başlangıç kaynak miktarı")]
    [SerializeField] private int initialAmount = 100;

    [Tooltip("Maksimum kaynak miktarı")]
    [SerializeField] private int maxAmount = 100;

    [Tooltip("Kaynak yenilenme hızı (saniyede)")]
    [SerializeField] private float regenerationRate = 0f;

    [Tooltip("Her toplama işleminde alınacak miktar")]
    [SerializeField] private int harvestAmount = 10;

    [Tooltip("Toplama süresi (saniye)")]
    [SerializeField] private float harvestTime = 2f;

    [Tooltip("Kaynak tükendiğinde yok olsun mu?")]
    [SerializeField] private bool destroyWhenEmpty = false;

    [Header("Visual Settings")]
    [Tooltip("Dolu durumda sprite")]
    [SerializeField] private Sprite fullSprite;

    [Tooltip("Yarı dolu durumda sprite")]
    [SerializeField] private Sprite halfSprite;

    [Tooltip("Neredeyse boş durumda sprite")]
    [SerializeField] private Sprite emptySprite;

    [Header("Interaction")]
    [Tooltip("Toplama mesafesi")]
    [SerializeField] private float interactionRange = 1.5f;

    [Tooltip("Aynı anda kaç kişi toplayabilir")]
    [SerializeField] private int maxGatherers = 3;

    // Private
    private int currentAmount;
    private SpriteRenderer spriteRenderer;
    private int activeGatherers = 0;

    // Events
    public UnityEvent<int> OnAmountChanged;
    public UnityEvent OnResourceDepleted;
    public UnityEvent<int> OnResourceHarvested;

    // Properties
    public ResourceType ResourceType => resourceType;
    public int CurrentAmount => currentAmount;
    public int MaxAmount => maxAmount;
    public float HarvestTime => harvestTime;
    public int HarvestAmount => harvestAmount;
    public float InteractionRange => interactionRange;
    public bool IsEmpty => currentAmount <= 0;
    public bool IsFull => currentAmount >= maxAmount;
    public bool CanGather => !IsEmpty && activeGatherers < maxGatherers;
    public float AmountPercentage => maxAmount > 0 ? (float)currentAmount / maxAmount : 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentAmount = initialAmount;
    }

    private void Start()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        // Kaynak yenilenmesi
        if (regenerationRate > 0 && currentAmount < maxAmount)
        {
            float regeneration = regenerationRate * Time.deltaTime;
            AddAmount(Mathf.CeilToInt(regeneration));
        }
    }

    /// <summary>
    /// Kaynaktan topla
    /// </summary>
    public bool Harvest(int amount)
    {
        if (IsEmpty) return false;

        int actualAmount = Mathf.Min(amount, currentAmount);
        currentAmount -= actualAmount;

        OnResourceHarvested?.Invoke(actualAmount);
        OnAmountChanged?.Invoke(currentAmount);
        UpdateVisuals();

        if (IsEmpty)
        {
            OnResourceDepleted?.Invoke();

            if (destroyWhenEmpty)
            {
                Destroy(gameObject, 0.5f);
            }
        }

        return true;
    }

    /// <summary>
    /// Varsayılan miktarda topla
    /// </summary>
    public bool HarvestDefault()
    {
        return Harvest(harvestAmount);
    }

    /// <summary>
    /// Kaynak miktarı ekle
    /// </summary>
    public void AddAmount(int amount)
    {
        currentAmount = Mathf.Min(currentAmount + amount, maxAmount);
        OnAmountChanged?.Invoke(currentAmount);
        UpdateVisuals();
    }

    /// <summary>
    /// Kaynağı yenile (tam doldur)
    /// </summary>
    public void Refill()
    {
        currentAmount = maxAmount;
        OnAmountChanged?.Invoke(currentAmount);
        UpdateVisuals();
    }

    /// <summary>
    /// Toplayıcı kaydet
    /// </summary>
    public bool RegisterGatherer()
    {
        if (activeGatherers >= maxGatherers) return false;

        activeGatherers++;
        return true;
    }

    /// <summary>
    /// Toplayıcıyı kaldır
    /// </summary>
    public void UnregisterGatherer()
    {
        activeGatherers = Mathf.Max(0, activeGatherers - 1);
    }

    /// <summary>
    /// Görünümü güncelle
    /// </summary>
    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        float percentage = AmountPercentage;

        if (emptySprite != null && percentage < 0.25f)
        {
            spriteRenderer.sprite = emptySprite;
        }
        else if (halfSprite != null && percentage < 0.75f)
        {
            spriteRenderer.sprite = halfSprite;
        }
        else if (fullSprite != null)
        {
            spriteRenderer.sprite = fullSprite;
        }

        // Boşsa rengini soluklaştır
        if (IsEmpty)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    /// <summary>
    /// En yakın ResourceNode'u bul
    /// </summary>
    public static ResourceNode FindNearest(Vector3 position, ResourceType type)
    {
        ResourceNode[] allNodes = FindObjectsOfType<ResourceNode>();
        ResourceNode nearest = null;
        float minDistance = float.MaxValue;

        foreach (var node in allNodes)
        {
            if (node.ResourceType != type || node.IsEmpty || !node.CanGather)
                continue;

            float distance = Vector3.Distance(position, node.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = node;
            }
        }

        return nearest;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Toplama menzilini göster
        Gizmos.color = IsEmpty ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // Kaynak tipini göster
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.5f,
            $"{resourceType}\n{currentAmount}/{maxAmount}"
        );
    }
#endif
}
