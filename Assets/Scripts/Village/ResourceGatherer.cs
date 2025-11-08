using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Kaynak toplayan köylü/işçi
/// ResourceNode'lardan kaynak toplar ve VillageManager'a teslim eder
/// </summary>
public class ResourceGatherer : MonoBehaviour
{
    [Header("Gatherer Settings")]
    [Tooltip("Toplayıcı tipi (hangi kaynağı toplar)")]
    [SerializeField] private ResourceType gatherType = ResourceType.Wood;

    [Tooltip("Hareket hızı")]
    [SerializeField] private float moveSpeed = 3f;

    [Tooltip("Taşıma kapasitesi")]
    [SerializeField] private int carryCapacity = 20;

    [Tooltip("Otomatik mod (kendisi kaynak bulur)")]
    [SerializeField] private bool autoMode = true;

    [Tooltip("Boşta bekleme süresi (saniye)")]
    [SerializeField] private float idleTime = 1f;

    [Header("References")]
    [Tooltip("Karakter controller (opsiyonel)")]
    [SerializeField] private IsometricCharacterController characterController;

    [Tooltip("Sprite renderer")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Visual Feedback")]
    [Tooltip("Kaynak taşırken gösterilecek sprite")]
    [SerializeField] private Sprite carryingSprite;

    [Tooltip("Normal sprite")]
    [SerializeField] private Sprite normalSprite;

    // Private
    private GathererState currentState = GathererState.Idle;
    private ResourceNode targetNode;
    private Vector3 storagePosition;
    private int carriedAmount = 0;
    private float idleTimer = 0f;
    private float gatherTimer = 0f;

    // Events
    public UnityEvent<ResourceType, int> OnResourceGathered;
    public UnityEvent<ResourceType, int> OnResourceDelivered;
    public UnityEvent<GathererState> OnStateChanged;

    // Properties
    public GathererState State => currentState;
    public ResourceType GatherType => gatherType;
    public int CarriedAmount => carriedAmount;
    public bool IsCarrying => carriedAmount > 0;
    public bool IsFull => carriedAmount >= carryCapacity;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<IsometricCharacterController>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Varsayılan depo pozisyonu (köy merkezi)
        storagePosition = Vector3.zero;
    }

    private void Start()
    {
        ChangeState(GathererState.Idle);
    }

    private void Update()
    {
        if (!autoMode) return;

        switch (currentState)
        {
            case GathererState.Idle:
                UpdateIdle();
                break;
            case GathererState.MovingToResource:
                UpdateMovingToResource();
                break;
            case GathererState.Gathering:
                UpdateGathering();
                break;
            case GathererState.MovingToStorage:
                UpdateMovingToStorage();
                break;
            case GathererState.Delivering:
                UpdateDelivering();
                break;
        }
    }

    /// <summary>
    /// Boşta durumu
    /// </summary>
    private void UpdateIdle()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;

            // Eğer kaynak taşıyorsa depoya git
            if (IsCarrying)
            {
                GoToStorage();
            }
            else
            {
                // Yeni kaynak bul
                FindAndGatherResource();
            }
        }
    }

    /// <summary>
    /// Kaynağa gitme durumu
    /// </summary>
    private void UpdateMovingToResource()
    {
        if (targetNode == null || targetNode.IsEmpty)
        {
            // Kaynak yok olmuş, yeni kaynak bul
            ChangeState(GathererState.Idle);
            return;
        }

        // Hedefe ulaştı mı?
        float distance = Vector3.Distance(transform.position, targetNode.transform.position);
        if (distance <= targetNode.InteractionRange)
        {
            // Toplama başlat
            if (targetNode.RegisterGatherer())
            {
                ChangeState(GathererState.Gathering);
                gatherTimer = 0f;
            }
            else
            {
                // Kaynak dolu, başka kaynak bul
                targetNode = null;
                ChangeState(GathererState.Idle);
            }
        }
    }

    /// <summary>
    /// Toplama durumu
    /// </summary>
    private void UpdateGathering()
    {
        if (targetNode == null || targetNode.IsEmpty)
        {
            ChangeState(GathererState.Idle);
            return;
        }

        gatherTimer += Time.deltaTime;

        if (gatherTimer >= targetNode.HarvestTime)
        {
            gatherTimer = 0f;

            // Kaynak topla
            int amountToGather = Mathf.Min(targetNode.HarvestAmount, carryCapacity - carriedAmount);

            if (targetNode.Harvest(amountToGather))
            {
                carriedAmount += amountToGather;
                OnResourceGathered?.Invoke(gatherType, amountToGather);
                UpdateVisuals();

                // Dolu mu?
                if (IsFull)
                {
                    targetNode.UnregisterGatherer();
                    targetNode = null;
                    GoToStorage();
                }
            }
            else
            {
                // Kaynak tükendi
                targetNode.UnregisterGatherer();
                targetNode = null;
                ChangeState(GathererState.Idle);
            }
        }
    }

    /// <summary>
    /// Depoya gitme durumu
    /// </summary>
    private void UpdateMovingToStorage()
    {
        float distance = Vector3.Distance(transform.position, storagePosition);

        if (distance <= 1f)
        {
            ChangeState(GathererState.Delivering);
        }
    }

    /// <summary>
    /// Teslim etme durumu
    /// </summary>
    private void UpdateDelivering()
    {
        if (IsCarrying)
        {
            // Kaynağı köye teslim et
            VillageManager.Instance.AddResource(gatherType, carriedAmount);
            OnResourceDelivered?.Invoke(gatherType, carriedAmount);
            carriedAmount = 0;
            UpdateVisuals();
        }

        ChangeState(GathererState.Idle);
    }

    /// <summary>
    /// Kaynak bul ve toplamaya başla
    /// </summary>
    private void FindAndGatherResource()
    {
        targetNode = ResourceNode.FindNearest(transform.position, gatherType);

        if (targetNode != null)
        {
            MoveTo(targetNode.transform.position);
            ChangeState(GathererState.MovingToResource);
        }
    }

    /// <summary>
    /// Depoya git
    /// </summary>
    private void GoToStorage()
    {
        MoveTo(storagePosition);
        ChangeState(GathererState.MovingToStorage);
    }

    /// <summary>
    /// Belirli bir pozisyona hareket et
    /// </summary>
    private void MoveTo(Vector3 position)
    {
        if (characterController != null)
        {
            characterController.MoveTo(position);
        }
        else
        {
            // Basit hareket
            StartCoroutine(MoveToCoroutine(position));
        }
    }

    /// <summary>
    /// Basit hareket coroutine
    /// </summary>
    private System.Collections.IEnumerator MoveToCoroutine(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Durum değiştir
    /// </summary>
    private void ChangeState(GathererState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        OnStateChanged?.Invoke(newState);

        // Durağa geçtiğinde karakteri durdur
        if (newState == GathererState.Idle || newState == GathererState.Gathering || newState == GathererState.Delivering)
        {
            if (characterController != null)
                characterController.Stop();
        }
    }

    /// <summary>
    /// Görünümü güncelle
    /// </summary>
    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        if (IsCarrying && carryingSprite != null)
        {
            spriteRenderer.sprite = carryingSprite;
        }
        else if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    /// <summary>
    /// Depo pozisyonunu ayarla
    /// </summary>
    public void SetStoragePosition(Vector3 position)
    {
        storagePosition = position;
    }

    /// <summary>
    /// Manuel olarak kaynak toplamayı başlat
    /// </summary>
    public void StartGathering(ResourceNode node)
    {
        if (node == null || node.ResourceType != gatherType) return;

        targetNode = node;
        MoveTo(node.transform.position);
        ChangeState(GathererState.MovingToResource);
    }

    /// <summary>
    /// Toplayıcıyı durdur
    /// </summary>
    public void Stop()
    {
        if (targetNode != null)
        {
            targetNode.UnregisterGatherer();
            targetNode = null;
        }

        ChangeState(GathererState.Idle);

        if (characterController != null)
            characterController.Stop();
    }

    private void OnDestroy()
    {
        // Kayıt silme
        if (targetNode != null)
        {
            targetNode.UnregisterGatherer();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Mevcut hedefi göster
        if (targetNode != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetNode.transform.position);
        }

        // Depo pozisyonunu göster
        if (currentState == GathererState.MovingToStorage)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, storagePosition);
        }

        // Durum ve taşınan miktar
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.5f,
            $"{currentState}\n{carriedAmount}/{carryCapacity}"
        );
    }
#endif
}

/// <summary>
/// Toplayıcı durumları
/// </summary>
public enum GathererState
{
    Idle,               // Boşta
    MovingToResource,   // Kaynağa gidiyor
    Gathering,          // Toplama yapıyor
    MovingToStorage,    // Depoya gidiyor
    Delivering          // Teslim ediyor
}
