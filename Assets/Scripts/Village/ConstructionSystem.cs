using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// İnşaat ve onarım sistemini yönetir
/// Yapıların inşa edilmesini ve onarılmasını otomatik veya manuel olarak yönetir
/// </summary>
public class ConstructionSystem : MonoBehaviour
{
    [Header("Construction Settings")]
    [Tooltip("Otomatik inşaat modu")]
    [SerializeField] private bool autoConstruct = true;

    [Tooltip("İnşaat hızı çarpanı")]
    [SerializeField] private float constructionSpeedMultiplier = 1f;

    [Tooltip("Aynı anda kaç inşaat yapılabilir")]
    [SerializeField] private int maxConcurrentConstructions = 3;

    [Header("Repair Settings")]
    [Tooltip("Otomatik onarım modu")]
    [SerializeField] private bool autoRepair = false;

    [Tooltip("Onarım hızı çarpanı")]
    [SerializeField] private float repairSpeedMultiplier = 0.5f;

    [Tooltip("Onarım için minimum can yüzdesi (bu değerin altındakiler otomatik onarılır)")]
    [Range(0f, 1f)]
    [SerializeField] private float autoRepairThreshold = 0.5f;

    // Active constructions
    private List<Building> activeConstructions = new List<Building>();
    private List<Building> repairQueue = new List<Building>();

    // Properties
    public int ActiveConstructionCount => activeConstructions.Count;
    public int RepairQueueCount => repairQueue.Count;

    private void Update()
    {
        if (autoConstruct)
        {
            UpdateConstructions();
        }

        if (autoRepair)
        {
            UpdateRepairs();
        }
    }

    /// <summary>
    /// Aktif inşaatları güncelle
    /// </summary>
    private void UpdateConstructions()
    {
        // Tamamlananları temizle
        activeConstructions.RemoveAll(b => b == null || !b.IsUnderConstruction);

        // İnşaatları ilerlet
        foreach (var building in activeConstructions)
        {
            if (building.Data == null) continue;

            // İlerleme hesapla (saniye başına yüzde)
            float progressPerSecond = 1f / building.Data.buildTime;
            float deltaProgress = progressPerSecond * constructionSpeedMultiplier * Time.deltaTime;

            building.UpdateConstruction(deltaProgress);
        }
    }

    /// <summary>
    /// Onarımları güncelle
    /// </summary>
    private void UpdateRepairs()
    {
        // Tamamlananları ve yok olanları temizle
        repairQueue.RemoveAll(b => b == null || b.HealthPercentage >= 1f || b.IsDestroyed);

        // Otomatik onarım - hasarlı yapıları bul
        if (repairQueue.Count < maxConcurrentConstructions)
        {
            FindBuildingsNeedingRepair();
        }

        // Onarımları ilerlet
        for (int i = 0; i < Mathf.Min(repairQueue.Count, maxConcurrentConstructions); i++)
        {
            Building building = repairQueue[i];
            if (building == null || building.Data == null) continue;

            // Onarım hızı hesapla
            int repairPerSecond = Mathf.CeilToInt(building.MaxHealth / building.Data.buildTime);
            int repairAmount = Mathf.CeilToInt(repairPerSecond * repairSpeedMultiplier * Time.deltaTime);

            building.Repair(repairAmount);
        }
    }

    /// <summary>
    /// Onarıma ihtiyacı olan yapıları bul
    /// </summary>
    private void FindBuildingsNeedingRepair()
    {
        foreach (var building in VillageManager.Instance.AllBuildings)
        {
            if (building == null || repairQueue.Contains(building)) continue;

            if (building.HealthPercentage < autoRepairThreshold && !building.IsUnderConstruction)
            {
                AddToRepairQueue(building);
            }
        }
    }

    /// <summary>
    /// Yapıyı inşaat listesine ekle
    /// </summary>
    public bool StartConstruction(Building building)
    {
        if (building == null || activeConstructions.Contains(building)) return false;

        if (activeConstructions.Count >= maxConcurrentConstructions)
        {
            Debug.LogWarning("Max concurrent construction limit reached!");
            return false;
        }

        activeConstructions.Add(building);
        return true;
    }

    /// <summary>
    /// Yapıyı onarım kuyruğuna ekle
    /// </summary>
    public bool AddToRepairQueue(Building building)
    {
        if (building == null || repairQueue.Contains(building)) return false;
        if (building.IsDestroyed || building.HealthPercentage >= 1f) return false;

        repairQueue.Add(building);
        return true;
    }

    /// <summary>
    /// Yapıyı onarım kuyruğundan çıkar
    /// </summary>
    public void RemoveFromRepairQueue(Building building)
    {
        repairQueue.Remove(building);
    }

    /// <summary>
    /// İnşaattan kaldır
    /// </summary>
    public void CancelConstruction(Building building)
    {
        activeConstructions.Remove(building);
    }

    /// <summary>
    /// Belirli bir yapıyı anında tamamla (cheat/debug için)
    /// </summary>
    public void CompleteConstructionInstantly(Building building)
    {
        if (building != null && building.IsUnderConstruction)
        {
            building.UpdateConstruction(1f);
        }
    }

    /// <summary>
    /// Belirli bir yapıyı anında onar (cheat/debug için)
    /// </summary>
    public void RepairInstantly(Building building)
    {
        if (building != null && !building.IsDestroyed)
        {
            building.Repair(building.MaxHealth);
            RemoveFromRepairQueue(building);
        }
    }

    /// <summary>
    /// Tüm yapıları onar
    /// </summary>
    public void RepairAllBuildings()
    {
        foreach (var building in VillageManager.Instance.AllBuildings)
        {
            if (building != null && !building.IsDestroyed)
            {
                building.Repair(building.MaxHealth);
            }
        }
        repairQueue.Clear();
    }

    /// <summary>
    /// Maksimum eşzamanlı inşaat sayısını ayarla
    /// </summary>
    public void SetMaxConcurrentConstructions(int max)
    {
        maxConcurrentConstructions = Mathf.Max(1, max);
    }

    /// <summary>
    /// İnşaat hızını ayarla
    /// </summary>
    public void SetConstructionSpeed(float multiplier)
    {
        constructionSpeedMultiplier = Mathf.Max(0.1f, multiplier);
    }

    /// <summary>
    /// Onarım hızını ayarla
    /// </summary>
    public void SetRepairSpeed(float multiplier)
    {
        repairSpeedMultiplier = Mathf.Max(0.1f, multiplier);
    }

    /// <summary>
    /// Otomatik inşaatı aç/kapat
    /// </summary>
    public void SetAutoConstruct(bool enabled)
    {
        autoConstruct = enabled;
    }

    /// <summary>
    /// Otomatik onarımı aç/kapat
    /// </summary>
    public void SetAutoRepair(bool enabled)
    {
        autoRepair = enabled;
    }
}
