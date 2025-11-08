using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Kaynak üreten yapılar için component
/// Örnek: Çiftlik (yiyecek), Demir Atölyesi (demir), Oduncu (odun)
/// </summary>
public class ResourceProducer : MonoBehaviour
{
    [Header("Production Settings")]
    [Tooltip("Üretilen kaynak tipi")]
    [SerializeField] private ResourceType producedResource = ResourceType.Wood;

    [Tooltip("Üretim miktarı (her üretim döngüsünde)")]
    [SerializeField] private int productionAmount = 10;

    [Tooltip("Üretim süresi (saniye)")]
    [SerializeField] private float productionTime = 30f;

    [Tooltip("Otomatik üretim (sürekli üretim yapar)")]
    [SerializeField] private bool autoProduction = true;

    [Tooltip("Üretim için gereken işçi sayısı")]
    [SerializeField] private int requiredWorkers = 1;

    [Header("Cost (Opsiyonel - Üretim için gerekli kaynaklar)")]
    [Tooltip("Üretim maliyeti - Odun")]
    [SerializeField] private int woodCost = 0;

    [Tooltip("Üretim maliyeti - Taş")]
    [SerializeField] private int stoneCost = 0;

    [Tooltip("Üretim maliyeti - Demir")]
    [SerializeField] private int ironCost = 0;

    [Header("References")]
    [Tooltip("Bağlı bina (durumu kontrol için)")]
    [SerializeField] private Building building;

    [Header("Visual Feedback")]
    [Tooltip("Üretim sırasında partikül efekti")]
    [SerializeField] private ParticleSystem productionEffect;

    [Tooltip("Üretim sırasında gösterilecek sprite")]
    [SerializeField] private Sprite productionSprite;

    [Tooltip("Boşta sprite")]
    [SerializeField] private Sprite idleSprite;

    [SerializeField] private SpriteRenderer spriteRenderer;

    // Private
    private bool isProducing = false;
    private float productionTimer = 0f;
    private int assignedWorkers = 0;

    // Events
    public UnityEvent<ResourceType, int> OnProductionComplete;
    public UnityEvent OnProductionStarted;
    public UnityEvent OnProductionStopped;

    // Properties
    public ResourceType ProducedResource => producedResource;
    public int ProductionAmount => productionAmount;
    public float ProductionTime => productionTime;
    public bool IsProducing => isProducing;
    public float ProductionProgress => productionTime > 0 ? productionTimer / productionTime : 0f;
    public int AssignedWorkers => assignedWorkers;
    public int RequiredWorkers => requiredWorkers;
    public bool HasEnoughWorkers => assignedWorkers >= requiredWorkers;
    public bool CanProduce => building == null || building.CanBeUsed();

    private void Awake()
    {
        if (building == null)
            building = GetComponent<Building>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (autoProduction && CanProduce && HasEnoughWorkers)
        {
            StartProduction();
        }
    }

    private void Update()
    {
        if (!isProducing) return;

        // Bina kullanılamaz duruma geldiyse üretimi durdur
        if (!CanProduce)
        {
            StopProduction();
            return;
        }

        // Yeteri kadar işçi yoksa durdur
        if (!HasEnoughWorkers)
        {
            StopProduction();
            return;
        }

        productionTimer += Time.deltaTime;

        if (productionTimer >= productionTime)
        {
            CompleteProduction();
        }
    }

    /// <summary>
    /// Üretimi başlat
    /// </summary>
    public bool StartProduction()
    {
        if (isProducing) return false;
        if (!CanProduce) return false;
        if (!HasEnoughWorkers) return false;

        // Maliyet kontrolü
        if (!CanAffordProduction())
        {
            Debug.Log($"[ResourceProducer] Not enough resources to start production");
            return false;
        }

        // Maliyeti öde
        PayProductionCost();

        isProducing = true;
        productionTimer = 0f;
        OnProductionStarted?.Invoke();
        UpdateVisuals();

        if (productionEffect != null)
            productionEffect.Play();

        return true;
    }

    /// <summary>
    /// Üretimi durdur
    /// </summary>
    public void StopProduction()
    {
        if (!isProducing) return;

        isProducing = false;
        productionTimer = 0f;
        OnProductionStopped?.Invoke();
        UpdateVisuals();

        if (productionEffect != null)
            productionEffect.Stop();
    }

    /// <summary>
    /// Üretimi tamamla
    /// </summary>
    private void CompleteProduction()
    {
        // Kaynağı üret
        VillageManager.Instance.AddResource(producedResource, productionAmount);
        OnProductionComplete?.Invoke(producedResource, productionAmount);

        productionTimer = 0f;

        // Otomatik üretim aktifse devam et
        if (autoProduction && HasEnoughWorkers && CanAffordProduction())
        {
            PayProductionCost();
            // Üretim devam ediyor, timer sıfırlandı
        }
        else
        {
            StopProduction();
        }
    }

    /// <summary>
    /// Üretim maliyetini karşılayabiliyor mu?
    /// </summary>
    private bool CanAffordProduction()
    {
        if (woodCost == 0 && stoneCost == 0 && ironCost == 0)
            return true;

        return VillageManager.Instance.Wood >= woodCost &&
               VillageManager.Instance.Stone >= stoneCost &&
               VillageManager.Instance.Iron >= ironCost;
    }

    /// <summary>
    /// Üretim maliyetini öde
    /// </summary>
    private void PayProductionCost()
    {
        if (woodCost > 0)
            VillageManager.Instance.SpendResource(ResourceType.Wood, woodCost);
        if (stoneCost > 0)
            VillageManager.Instance.SpendResource(ResourceType.Stone, stoneCost);
        if (ironCost > 0)
            VillageManager.Instance.SpendResource(ResourceType.Iron, ironCost);
    }

    /// <summary>
    /// İşçi ata
    /// </summary>
    public bool AssignWorker()
    {
        assignedWorkers++;

        // Yeterli işçi olduysa ve otomatik mod aktifse üretimi başlat
        if (autoProduction && !isProducing && HasEnoughWorkers && CanProduce)
        {
            StartProduction();
        }

        return true;
    }

    /// <summary>
    /// İşçiyi kaldır
    /// </summary>
    public void RemoveWorker()
    {
        assignedWorkers = Mathf.Max(0, assignedWorkers - 1);

        // Yeteri kadar işçi kalmadıysa durdur
        if (!HasEnoughWorkers && isProducing)
        {
            StopProduction();
        }
    }

    /// <summary>
    /// Üretim hızını ayarla
    /// </summary>
    public void SetProductionSpeed(float multiplier)
    {
        productionTime = productionTime / Mathf.Max(0.1f, multiplier);
    }

    /// <summary>
    /// Üretim miktarını ayarla
    /// </summary>
    public void SetProductionAmount(int amount)
    {
        productionAmount = Mathf.Max(1, amount);
    }

    /// <summary>
    /// Görünümü güncelle
    /// </summary>
    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        if (isProducing && productionSprite != null)
        {
            spriteRenderer.sprite = productionSprite;
        }
        else if (idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    /// <summary>
    /// Otomatik üretimi aç/kapat
    /// </summary>
    public void SetAutoProduction(bool enabled)
    {
        autoProduction = enabled;

        if (enabled && !isProducing && HasEnoughWorkers && CanProduce)
        {
            StartProduction();
        }
        else if (!enabled && isProducing)
        {
            StopProduction();
        }
    }

    /// <summary>
    /// Anında üret (cheat/debug için)
    /// </summary>
    public void ProduceInstantly()
    {
        if (!CanProduce) return;

        VillageManager.Instance.AddResource(producedResource, productionAmount);
        OnProductionComplete?.Invoke(producedResource, productionAmount);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Üretim durumu
        Color gizmoColor = isProducing ? Color.green : Color.yellow;
        if (!CanProduce) gizmoColor = Color.red;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // Bilgi göster
        string info = $"{producedResource} Producer\n" +
                      $"Amount: {productionAmount}\n" +
                      $"Time: {productionTime}s\n" +
                      $"Workers: {assignedWorkers}/{requiredWorkers}\n" +
                      $"Progress: {ProductionProgress:P0}";

        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, info);
    }
#endif
}
