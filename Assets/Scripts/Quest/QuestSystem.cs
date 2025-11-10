using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Basit görev/hedef sistemi
/// </summary>
public class QuestSystem : MonoBehaviour
{
    [Header("Active Quests")]
    [SerializeField] private List<Quest> activeQuests = new List<Quest>();

    [Header("UI")]
    [SerializeField] private QuestUI questUI;

    // Events
    public UnityEvent<Quest> OnQuestCompleted;
    public UnityEvent<Quest> OnQuestAdded;

    // Singleton
    private static QuestSystem instance;
    public static QuestSystem Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (questUI == null)
            questUI = FindObjectOfType<QuestUI>();

        // Başlangıç görevlerini ekle
        AddStartingQuests();
    }

    private void Update()
    {
        // Görevleri kontrol et
        CheckQuestProgress();
    }

    /// <summary>
    /// Başlangıç görevlerini ekle
    /// </summary>
    private void AddStartingQuests()
    {
        // Görev 1: Kaynak topla
        AddQuest(new Quest
        {
            id = "collect_wood",
            title = "Odun Topla",
            description = "100 odun toplayın",
            type = QuestType.CollectResource,
            targetResourceType = ResourceType.Wood,
            targetAmount = 100,
            currentAmount = 0
        });

        // Görev 2: Ev onar
        AddQuest(new Quest
        {
            id = "repair_houses",
            title = "Evleri Onar",
            description = "3 evi onara veya yeniden inşa edin",
            type = QuestType.RepairBuildings,
            targetBuildingType = BuildingType.House,
            targetAmount = 3,
            currentAmount = 0
        });
    }

    /// <summary>
    /// Görev ekle
    /// </summary>
    public void AddQuest(Quest quest)
    {
        if (activeQuests.Exists(q => q.id == quest.id))
            return;

        activeQuests.Add(quest);
        OnQuestAdded?.Invoke(quest);

        if (questUI != null)
            questUI.UpdateQuests(activeQuests);
    }

    /// <summary>
    /// Görev ilerlemesini kontrol et
    /// </summary>
    private void CheckQuestProgress()
    {
        if (VillageManager.Instance == null) return;

        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            Quest quest = activeQuests[i];

            if (quest.isCompleted) continue;

            // Görev tipine göre ilerlemeyi kontrol et
            switch (quest.type)
            {
                case QuestType.CollectResource:
                    quest.currentAmount = GetResourceAmount(quest.targetResourceType);
                    break;

                case QuestType.RepairBuildings:
                    quest.currentAmount = CountGoodBuildings(quest.targetBuildingType);
                    break;

                case QuestType.BuildStructure:
                    quest.currentAmount = CountGoodBuildings(quest.targetBuildingType);
                    break;

                case QuestType.ReachPopulation:
                    quest.currentAmount = VillageManager.Instance.CurrentPopulation;
                    break;
            }

            // Tamamlandı mı kontrol et
            if (quest.currentAmount >= quest.targetAmount)
            {
                CompleteQuest(quest);
            }
        }

        // UI güncelle
        if (questUI != null)
            questUI.UpdateQuests(activeQuests);
    }

    /// <summary>
    /// Kaynak miktarını al
    /// </summary>
    private int GetResourceAmount(ResourceType type)
    {
        return type switch
        {
            ResourceType.Wood => VillageManager.Instance.Wood,
            ResourceType.Stone => VillageManager.Instance.Stone,
            ResourceType.Iron => VillageManager.Instance.Iron,
            ResourceType.Food => VillageManager.Instance.Food,
            _ => 0
        };
    }

    /// <summary>
    /// İyi durumdaki yapıları say
    /// </summary>
    private int CountGoodBuildings(BuildingType type)
    {
        int count = 0;
        foreach (var building in VillageManager.Instance.AllBuildings)
        {
            if (building.Data.buildingType == type && building.CanBeUsed())
                count++;
        }
        return count;
    }

    /// <summary>
    /// Görevi tamamla
    /// </summary>
    private void CompleteQuest(Quest quest)
    {
        quest.isCompleted = true;
        OnQuestCompleted?.Invoke(quest);

        Debug.Log($"[QuestSystem] Quest completed: {quest.title}");

        // Ödül ver (varsa)
        if (quest.rewardWood > 0)
            VillageManager.Instance.AddResource(ResourceType.Wood, quest.rewardWood);
        if (quest.rewardStone > 0)
            VillageManager.Instance.AddResource(ResourceType.Stone, quest.rewardStone);
        if (quest.rewardIron > 0)
            VillageManager.Instance.AddResource(ResourceType.Iron, quest.rewardIron);
        if (quest.rewardFood > 0)
            VillageManager.Instance.AddResource(ResourceType.Food, quest.rewardFood);
    }

    /// <summary>
    /// Tamamlanan görevleri temizle
    /// </summary>
    public void ClearCompletedQuests()
    {
        activeQuests.RemoveAll(q => q.isCompleted);

        if (questUI != null)
            questUI.UpdateQuests(activeQuests);
    }
}

/// <summary>
/// Görev yapısı
/// </summary>
[System.Serializable]
public class Quest
{
    public string id;
    public string title;
    [TextArea] public string description;
    public QuestType type;

    // Hedefler
    public ResourceType targetResourceType;
    public BuildingType targetBuildingType;
    public int targetAmount;
    public int currentAmount;

    // Durum
    public bool isCompleted;

    // Ödüller
    public int rewardWood;
    public int rewardStone;
    public int rewardIron;
    public int rewardFood;
}

/// <summary>
/// Görev tipleri
/// </summary>
public enum QuestType
{
    CollectResource,    // Kaynak topla
    BuildStructure,     // Yapı inşa et
    RepairBuildings,    // Yapıları onar
    ReachPopulation     // Nüfusa ulaş
}
