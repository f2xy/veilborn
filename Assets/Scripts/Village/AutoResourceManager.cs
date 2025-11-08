using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Otomatik kaynak toplama ve üretim yönetimi
/// Köylüleri ve üretim yapılarını yönetir
/// </summary>
public class AutoResourceManager : MonoBehaviour
{
    [Header("Gatherer Settings")]
    [Tooltip("Köylü prefabı")]
    [SerializeField] private GameObject gathererPrefab;

    [Tooltip("Köylülerin spawn olacağı pozisyon")]
    [SerializeField] private Transform gathererSpawnPoint;

    [Tooltip("Her kaynak tipi için köylü sayısı")]
    [SerializeField] private int gatherersPerResourceType = 2;

    [Tooltip("Otomatik köylü oluşturma")]
    [SerializeField] private bool autoCreateGatherers = true;

    [Header("Production Settings")]
    [Tooltip("Otomatik üretim yapılarına işçi ata")]
    [SerializeField] private bool autoAssignWorkers = true;

    [Tooltip("Üretim yapısı başına işçi sayısı")]
    [SerializeField] private int workersPerProducer = 1;

    [Header("Balance Settings")]
    [Tooltip("Kaynak dengesi kontrolü (otomatik önceliklendirme)")]
    [SerializeField] private bool autoBalance = true;

    [Tooltip("Kaynak dengesini kontrol aralığı (saniye)")]
    [SerializeField] private float balanceCheckInterval = 10f;

    // Private
    private List<ResourceGatherer> allGatherers = new List<ResourceGatherer>();
    private List<ResourceProducer> allProducers = new List<ResourceProducer>();
    private float balanceTimer = 0f;

    private void Start()
    {
        if (autoCreateGatherers)
        {
            CreateInitialGatherers();
        }

        if (autoAssignWorkers)
        {
            AssignWorkersToProducers();
        }
    }

    private void Update()
    {
        if (autoBalance)
        {
            balanceTimer += Time.deltaTime;

            if (balanceTimer >= balanceCheckInterval)
            {
                balanceTimer = 0f;
                BalanceResources();
            }
        }
    }

    /// <summary>
    /// Başlangıç köylülerini oluştur
    /// </summary>
    private void CreateInitialGatherers()
    {
        if (gathererPrefab == null)
        {
            Debug.LogWarning("[AutoResourceManager] Gatherer prefab is not assigned!");
            return;
        }

        Vector3 spawnPos = gathererSpawnPoint != null ? gathererSpawnPoint.position : Vector3.zero;

        // Her kaynak tipi için köylü oluştur
        CreateGatherers(ResourceType.Wood, gatherersPerResourceType, spawnPos);
        CreateGatherers(ResourceType.Stone, gatherersPerResourceType, spawnPos + Vector3.right * 2);
        CreateGatherers(ResourceType.Iron, gatherersPerResourceType, spawnPos + Vector3.right * 4);
        CreateGatherers(ResourceType.Food, gatherersPerResourceType, spawnPos + Vector3.right * 6);
    }

    /// <summary>
    /// Belirli kaynak tipi için köylü oluştur
    /// </summary>
    public void CreateGatherers(ResourceType type, int count, Vector3 position)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            GameObject gathererObj = Instantiate(gathererPrefab, position + offset, Quaternion.identity, transform);

            ResourceGatherer gatherer = gathererObj.GetComponent<ResourceGatherer>();
            if (gatherer != null)
            {
                // Gatherer tipini ayarla
                gatherer.GetType().GetField("gatherType",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(gatherer, type);

                allGatherers.Add(gatherer);
            }
        }
    }

    /// <summary>
    /// Üretim yapılarına işçi ata
    /// </summary>
    private void AssignWorkersToProducers()
    {
        allProducers.Clear();
        allProducers.AddRange(FindObjectsOfType<ResourceProducer>());

        foreach (var producer in allProducers)
        {
            // İşçi ata
            for (int i = 0; i < workersPerProducer; i++)
            {
                producer.AssignWorker();
            }
        }
    }

    /// <summary>
    /// Kaynak dengesini kontrol et ve köylüleri yeniden dağıt
    /// </summary>
    private void BalanceResources()
    {
        if (allGatherers.Count == 0) return;

        // Her kaynak tipinin storage doluluk oranını kontrol et
        Dictionary<ResourceType, float> storageLevels = new Dictionary<ResourceType, float>
        {
            { ResourceType.Wood, VillageManager.Instance.WoodStoragePercentage },
            { ResourceType.Stone, VillageManager.Instance.StoneStoragePercentage },
            { ResourceType.Iron, VillageManager.Instance.IronStoragePercentage },
            { ResourceType.Food, VillageManager.Instance.FoodStoragePercentage }
        };

        // En düşük storage seviyesine sahip kaynağı bul
        ResourceType lowestResource = ResourceType.Wood;
        float lowestPercentage = 1f;

        foreach (var kvp in storageLevels)
        {
            if (kvp.Value < lowestPercentage)
            {
                lowestPercentage = kvp.Value;
                lowestResource = kvp.Key;
            }
        }

        // Eğer bir kaynak %30'un altındaysa, onu toplayan köylü sayısını artır
        if (lowestPercentage < 0.3f)
        {
            ReassignGatherers(lowestResource);
        }
    }

    /// <summary>
    /// Köylüleri yeniden ata
    /// </summary>
    private void ReassignGatherers(ResourceType priorityResource)
    {
        // Öncelik kaynağı dışındaki köylüleri say
        int othersCount = 0;
        ResourceGatherer gathererToReassign = null;

        foreach (var gatherer in allGatherers)
        {
            if (gatherer.GatherType != priorityResource)
            {
                othersCount++;
                if (gathererToReassign == null)
                {
                    gathererToReassign = gatherer;
                }
            }
        }

        // Eğer başka kaynak toplayan köylü varsa, birini yeniden ata
        if (gathererToReassign != null && othersCount > 1)
        {
            // Köylünün tipini değiştir (reflection ile)
            gathererToReassign.GetType().GetField("gatherType",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gathererToReassign, priorityResource);

            // Mevcut işini durdur
            gathererToReassign.Stop();

            Debug.Log($"[AutoResourceManager] Reassigned gatherer to {priorityResource}");
        }
    }

    /// <summary>
    /// Yeni köylü ekle
    /// </summary>
    public ResourceGatherer AddGatherer(ResourceType type)
    {
        if (gathererPrefab == null) return null;

        Vector3 spawnPos = gathererSpawnPoint != null ? gathererSpawnPoint.position : Vector3.zero;
        Vector3 offset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);

        GameObject gathererObj = Instantiate(gathererPrefab, spawnPos + offset, Quaternion.identity, transform);
        ResourceGatherer gatherer = gathererObj.GetComponent<ResourceGatherer>();

        if (gatherer != null)
        {
            gatherer.GetType().GetField("gatherType",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gatherer, type);

            allGatherers.Add(gatherer);
        }

        return gatherer;
    }

    /// <summary>
    /// Köylü kaldır
    /// </summary>
    public void RemoveGatherer(ResourceGatherer gatherer)
    {
        if (gatherer != null && allGatherers.Contains(gatherer))
        {
            allGatherers.Remove(gatherer);
            Destroy(gatherer.gameObject);
        }
    }

    /// <summary>
    /// Belirli tipteki köylü sayısını al
    /// </summary>
    public int GetGathererCount(ResourceType type)
    {
        int count = 0;
        foreach (var gatherer in allGatherers)
        {
            if (gatherer.GatherType == type)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Tüm köylüleri durdur
    /// </summary>
    public void StopAllGatherers()
    {
        foreach (var gatherer in allGatherers)
        {
            if (gatherer != null)
                gatherer.Stop();
        }
    }

    /// <summary>
    /// Tüm üretimi durdur
    /// </summary>
    public void StopAllProduction()
    {
        foreach (var producer in allProducers)
        {
            if (producer != null)
                producer.StopProduction();
        }
    }

    /// <summary>
    /// Yeni üretim yapısı kaydet
    /// </summary>
    public void RegisterProducer(ResourceProducer producer)
    {
        if (producer != null && !allProducers.Contains(producer))
        {
            allProducers.Add(producer);

            if (autoAssignWorkers)
            {
                for (int i = 0; i < workersPerProducer; i++)
                {
                    producer.AssignWorker();
                }
            }
        }
    }

    /// <summary>
    /// Otomatik dengelemeyi aç/kapat
    /// </summary>
    public void SetAutoBalance(bool enabled)
    {
        autoBalance = enabled;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Köylüleri göster
        if (allGatherers != null)
        {
            foreach (var gatherer in allGatherers)
            {
                if (gatherer != null)
                {
                    Color color = gatherer.GatherType switch
                    {
                        ResourceType.Wood => new Color(0.6f, 0.3f, 0f),
                        ResourceType.Stone => Color.gray,
                        ResourceType.Iron => new Color(0.5f, 0.5f, 0.5f),
                        ResourceType.Food => Color.green,
                        _ => Color.white
                    };

                    Gizmos.color = color;
                    Gizmos.DrawWireSphere(gatherer.transform.position, 0.3f);
                }
            }
        }

        // Spawn noktası
        if (gathererSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gathererSpawnPoint.position, 1f);
        }
    }
#endif
}
