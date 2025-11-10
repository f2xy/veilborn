using UnityEngine;

/// <summary>
/// Isometric görüş açısı için kamera kontrolcüsü.
/// Kamerayı hareket ettirir, zoom yapar ve hedefe takip edebilir.
/// </summary>
public class IsometricCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Kameranın hareket hızı")]
    [SerializeField] private float moveSpeed = 10f;

    [Tooltip("Kameranın zoom hızı")]
    [SerializeField] private float zoomSpeed = 2f;

    [Tooltip("Minimum zoom değeri (orthographic size)")]
    [SerializeField] private float minZoom = 2f;

    [Tooltip("Maximum zoom değeri (orthographic size)")]
    [SerializeField] private float maxZoom = 15f;

    [Header("Target Following")]
    [Tooltip("Kameranın takip edeceği hedef (opsiyonel)")]
    [SerializeField] private Transform target;

    [Tooltip("Hedefe takip hızı")]
    [SerializeField] private float followSpeed = 5f;

    [Tooltip("Hedefe takip edilecek mi?")]
    [SerializeField] private bool followTarget = false;

    [Header("Boundaries")]
    [Tooltip("Kamera sınırlarını kullan")]
    [SerializeField] private bool useBoundaries = false;

    [Tooltip("Minimum kamera pozisyonu")]
    [SerializeField] private Vector2 minBoundary = new Vector2(-50, -50);

    [Tooltip("Maximum kamera pozisyonu")]
    [SerializeField] private Vector2 maxBoundary = new Vector2(50, 50);

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Orthographic modu aktif değilse uyarı ver
        if (!cam.orthographic)
        {
            Debug.LogWarning("Isometric oyunlar için kamera orthographic modda olmalıdır!");
            cam.orthographic = true;
        }
    }

    private void Update()
    {
        HandleMouseDrag();
        HandleKeyboardMovement();
        HandleZoom();

        if (followTarget && target != null)
        {
            FollowTarget();
        }
    }

    /// <summary>
    /// Fare ile sürükleyerek kamerayı hareket ettir
    /// </summary>
    private void HandleMouseDrag()
    {
        // Orta fare tuşu ile sürükleme
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(2) && isDragging)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = transform.position + difference;

            if (useBoundaries)
            {
                newPosition = ClampPosition(newPosition);
            }

            transform.position = newPosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }
    }

    /// <summary>
    /// Klavye ile kamerayı hareket ettir (WASD veya Arrow keys)
    /// </summary>
    private void HandleKeyboardMovement()
    {
        if (followTarget && target != null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;

        if (useBoundaries)
        {
            newPosition = ClampPosition(newPosition);
        }

        transform.position = newPosition;
    }

    /// <summary>
    /// Mouse wheel ile zoom yap
    /// </summary>
    private void HandleZoom()
    {
        // Input.mouseScrollDelta kullanarak Input Manager'a bağımlılığı kaldır
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - scroll * zoomSpeed,
                minZoom,
                maxZoom
            );
        }
    }

    /// <summary>
    /// Hedefi takip et
    /// </summary>
    private void FollowTarget()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (useBoundaries)
        {
            targetPosition = ClampPosition(targetPosition);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Kamera pozisyonunu sınırlar içinde tut
    /// </summary>
    private Vector3 ClampPosition(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, minBoundary.x, maxBoundary.x);
        float clampedY = Mathf.Clamp(position.y, minBoundary.y, maxBoundary.y);
        return new Vector3(clampedX, clampedY, position.z);
    }

    /// <summary>
    /// Takip hedefini değiştir
    /// </summary>
    public void SetTarget(Transform newTarget, bool follow = true)
    {
        target = newTarget;
        followTarget = follow;
    }

    /// <summary>
    /// Kamera sınırlarını ayarla
    /// </summary>
    public void SetBoundaries(Vector2 min, Vector2 max)
    {
        minBoundary = min;
        maxBoundary = max;
        useBoundaries = true;
    }
}
