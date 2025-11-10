using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Oyun hızını kontrol eden UI
/// </summary>
public class GameSpeedController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button normalSpeedButton;
    [SerializeField] private Button fastSpeedButton;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Speed Settings")]
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private float fastSpeed = 2f;

    [Header("Visual Feedback")]
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color normalColor = Color.white;

    private bool isPaused = false;
    private float currentSpeed = 1f;

    private void Start()
    {
        // Buton event'lerini bağla
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        if (normalSpeedButton != null)
            normalSpeedButton.onClick.AddListener(() => SetSpeed(normalSpeed));

        if (fastSpeedButton != null)
            fastSpeedButton.onClick.AddListener(() => SetSpeed(fastSpeed));

        // Klavye kısayolları için
        UpdateSpeedDisplay();
    }

    private void Update()
    {
        // Klavye kısayolları
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSpeed(normalSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetSpeed(fastSpeed);
        }
    }

    /// <summary>
    /// Pause/unpause
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = currentSpeed;
        }

        UpdateSpeedDisplay();
        UpdateButtonColors();
    }

    /// <summary>
    /// Oyun hızını ayarla
    /// </summary>
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;

        if (!isPaused)
        {
            Time.timeScale = currentSpeed;
        }

        UpdateSpeedDisplay();
        UpdateButtonColors();
    }

    /// <summary>
    /// Hız göstergesini güncelle
    /// </summary>
    private void UpdateSpeedDisplay()
    {
        if (speedText == null) return;

        if (isPaused)
        {
            speedText.text = "PAUSED";
        }
        else
        {
            speedText.text = $"{currentSpeed}x";
        }
    }

    /// <summary>
    /// Buton renklerini güncelle
    /// </summary>
    private void UpdateButtonColors()
    {
        // Pause button
        if (pauseButton != null)
        {
            var colors = pauseButton.colors;
            colors.normalColor = isPaused ? selectedColor : normalColor;
            pauseButton.colors = colors;
        }

        // Normal speed button
        if (normalSpeedButton != null)
        {
            var colors = normalSpeedButton.colors;
            colors.normalColor = (!isPaused && Mathf.Approximately(currentSpeed, normalSpeed)) ? selectedColor : normalColor;
            normalSpeedButton.colors = colors;
        }

        // Fast speed button
        if (fastSpeedButton != null)
        {
            var colors = fastSpeedButton.colors;
            colors.normalColor = (!isPaused && Mathf.Approximately(currentSpeed, fastSpeed)) ? selectedColor : normalColor;
            fastSpeedButton.colors = colors;
        }
    }

    private void OnDestroy()
    {
        // Oyun bittiğinde timeScale'i sıfırla
        Time.timeScale = 1f;
    }
}
