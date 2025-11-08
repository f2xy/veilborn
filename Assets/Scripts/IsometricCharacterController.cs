using UnityEngine;

/// <summary>
/// Isometric görüş açısı için karakter kontrolcüsü.
/// Karakteri hareket ettirir ve animasyonları yönetir.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class IsometricCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Karakter hareket hızı")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Smooth hareket için")]
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Input Settings")]
    [Tooltip("Fare ile hareket etsin mi?")]
    [SerializeField] private bool useMouseMovement = false;

    [Tooltip("Klavye ile hareket etsin mi?")]
    [SerializeField] private bool useKeyboardMovement = true;

    [Header("Components")]
    [Tooltip("Sprite Renderer bileşeni")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Animator bileşeni (opsiyonel)")]
    [SerializeField] private Animator animator;

    // Private değişkenler
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 currentVelocity;
    private Vector2 smoothVelocity;
    private Vector3 targetPosition;
    private bool isMovingToTarget = false;

    // Animator parametreleri (varsa)
    private readonly int moveXHash = Animator.StringToHash("MoveX");
    private readonly int moveYHash = Animator.StringToHash("MoveY");
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody2D ayarları
        rb.gravityScale = 0; // Isometric oyunlarda yerçekimi genellikle 0'dır
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // SpriteRenderer otomatik bul
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Animator otomatik bul
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    /// <summary>
    /// Kullanıcı girdilerini işle
    /// </summary>
    private void HandleInput()
    {
        if (useKeyboardMovement)
        {
            HandleKeyboardInput();
        }

        if (useMouseMovement)
        {
            HandleMouseInput();
        }
    }

    /// <summary>
    /// Klavye girdilerini işle
    /// </summary>
    private void HandleKeyboardInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontal, vertical).normalized;
        isMovingToTarget = false;
    }

    /// <summary>
    /// Fare girdilerini işle
    /// </summary>
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Sol tık
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            isMovingToTarget = true;
        }
    }

    /// <summary>
    /// Karakteri hareket ettir
    /// </summary>
    private void MoveCharacter()
    {
        Vector2 targetVelocity;

        if (isMovingToTarget)
        {
            // Hedefe doğru hareket
            Vector2 direction = ((Vector2)targetPosition - rb.position).normalized;
            float distance = Vector2.Distance(rb.position, targetPosition);

            if (distance < 0.1f)
            {
                isMovingToTarget = false;
                targetVelocity = Vector2.zero;
                movement = Vector2.zero;
            }
            else
            {
                targetVelocity = direction * moveSpeed;
                movement = direction;
            }
        }
        else
        {
            // Klavye ile hareket
            targetVelocity = movement * moveSpeed;
        }

        // Smooth hareket
        currentVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref smoothVelocity, smoothTime);
        rb.velocity = currentVelocity;

        // Animasyon ve sprite yönlendirme
        UpdateAnimation();
        UpdateSpriteDirection();
    }

    /// <summary>
    /// Animasyonları güncelle
    /// </summary>
    private void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetFloat(moveXHash, movement.x);
        animator.SetFloat(moveYHash, movement.y);
        animator.SetBool(isMovingHash, movement.magnitude > 0.1f);
    }

    /// <summary>
    /// Sprite yönünü güncelle
    /// </summary>
    private void UpdateSpriteDirection()
    {
        if (spriteRenderer == null) return;

        // Karakterin baktığı yöne göre sprite'ı çevir
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// Belirli bir pozisyona hareket et
    /// </summary>
    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
        isMovingToTarget = true;
    }

    /// <summary>
    /// Hareketi durdur
    /// </summary>
    public void Stop()
    {
        movement = Vector2.zero;
        isMovingToTarget = false;
        rb.velocity = Vector2.zero;
        currentVelocity = Vector2.zero;
    }

    /// <summary>
    /// Hareket hızını değiştir
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    /// <summary>
    /// Karakter hareket ediyor mu?
    /// </summary>
    public bool IsMoving()
    {
        return rb.velocity.magnitude > 0.1f;
    }

    /// <summary>
    /// Mevcut hareket yönünü al
    /// </summary>
    public Vector2 GetMovementDirection()
    {
        return movement;
    }
}
