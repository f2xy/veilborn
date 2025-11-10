using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Görev listesi UI
/// </summary>
public class QuestUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private Transform questContainer;
    [SerializeField] private GameObject questEntryPrefab;

    [Header("Toggle")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Q;
    [SerializeField] private bool startVisible = true;

    private List<GameObject> questEntries = new List<GameObject>();
    private bool isVisible;

    private void Start()
    {
        isVisible = startVisible;
        if (questPanel != null)
            questPanel.SetActive(isVisible);
    }

    private void Update()
    {
        // Q tuşu ile aç/kapat
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
    }

    /// <summary>
    /// Görevleri güncelle
    /// </summary>
    public void UpdateQuests(List<Quest> quests)
    {
        // Eski entry'leri temizle
        foreach (var entry in questEntries)
        {
            if (entry != null) Destroy(entry);
        }
        questEntries.Clear();

        if (questContainer == null || questEntryPrefab == null) return;

        // Yeni entry'ler oluştur
        foreach (var quest in quests)
        {
            GameObject entryObj = Instantiate(questEntryPrefab, questContainer);
            questEntries.Add(entryObj);

            // Text'leri ayarla
            TextMeshProUGUI[] texts = entryObj.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length > 0)
            {
                // Başlık
                texts[0].text = quest.title;

                // İlerleme
                if (texts.Length > 1)
                {
                    texts[1].text = $"{quest.currentAmount}/{quest.targetAmount}";

                    // Renk (tamamlandıysa yeşil)
                    texts[1].color = quest.isCompleted ? Color.green : Color.white;
                }

                // Açıklama (varsa)
                if (texts.Length > 2)
                {
                    texts[2].text = quest.description;
                }
            }

            // İlerleme barı (varsa)
            Image progressBar = entryObj.GetComponentInChildren<Image>();
            if (progressBar != null && progressBar.name.Contains("Progress"))
            {
                float progress = quest.targetAmount > 0 ? (float)quest.currentAmount / quest.targetAmount : 0f;
                progressBar.fillAmount = progress;
            }

            // Tamamlanmış görevler için opacity azalt
            if (quest.isCompleted)
            {
                CanvasGroup canvasGroup = entryObj.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = entryObj.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0.5f;
            }
        }
    }

    /// <summary>
    /// Paneli aç/kapat
    /// </summary>
    public void TogglePanel()
    {
        isVisible = !isVisible;
        if (questPanel != null)
            questPanel.SetActive(isVisible);
    }

    /// <summary>
    /// Paneli aç
    /// </summary>
    public void ShowPanel()
    {
        isVisible = true;
        if (questPanel != null)
            questPanel.SetActive(true);
    }

    /// <summary>
    /// Paneli kapat
    /// </summary>
    public void HidePanel()
    {
        isVisible = false;
        if (questPanel != null)
            questPanel.SetActive(false);
    }
}
