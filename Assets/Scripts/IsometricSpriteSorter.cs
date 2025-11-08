using UnityEngine;

/// <summary>
/// Isometric görüş açısında sprite'ların doğru sırada render edilmesini sağlar.
/// Y pozisyonuna göre sorting order'ı otomatik olarak günceller.
/// Bu, derinlik illüzyonu yaratır (aşağıdaki objeler önde görünür).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class IsometricSpriteSorter : MonoBehaviour
{
    [Header("Sorting Settings")]
    [Tooltip("Sorting hesaplaması için kullanılacak precision (hassaslık)")]
    [SerializeField] private float sortingPrecision = 100f;

    [Tooltip("Y pozisyonuna göre offset ekle")]
    [SerializeField] private float yOffset = 0f;

    [Tooltip("Sabit bir offset ekle (tüm nesneler için)")]
    [SerializeField] private int baseOffset = 0;

    [Tooltip("Her frame güncellensin mi? (Hareketli objeler için true yapın)")]
    [SerializeField] private bool updateEveryFrame = true;

    [Header("Optional Settings")]
    [Tooltip("Pivot noktasını kullan (genellikle ayaklar için)")]
    [SerializeField] private bool usePivotPoint = false;

    [Tooltip("Pivot noktasının local pozisyonu")]
    [SerializeField] private Vector2 pivotPoint = Vector2.zero;

    private SpriteRenderer spriteRenderer;
    private Transform trans;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trans = transform;
    }

    private void Start()
    {
        UpdateSortingOrder();
    }

    private void LateUpdate()
    {
        if (updateEveryFrame)
        {
            UpdateSortingOrder();
        }
    }

    /// <summary>
    /// Sorting order'ı Y pozisyonuna göre günceller
    /// </summary>
    private void UpdateSortingOrder()
    {
        float yPosition = usePivotPoint ? trans.position.y + pivotPoint.y : trans.position.y;
        yPosition += yOffset;

        // Y pozisyonunu negatif yapıp precision ile çarparak sorting order hesapla
        // Daha aşağıdaki objeler (düşük Y) daha yüksek sorting order alır (önde görünür)
        int sortingOrder = Mathf.RoundToInt(-yPosition * sortingPrecision) + baseOffset;

        spriteRenderer.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// Sorting order'ı manuel olarak güncelle
    /// </summary>
    public void RefreshSortingOrder()
    {
        UpdateSortingOrder();
    }

    /// <summary>
    /// Base offset'i değiştir
    /// </summary>
    public void SetBaseOffset(int offset)
    {
        baseOffset = offset;
        UpdateSortingOrder();
    }

    /// <summary>
    /// Y offset'i değiştir
    /// </summary>
    public void SetYOffset(float offset)
    {
        yOffset = offset;
        UpdateSortingOrder();
    }

    /// <summary>
    /// Sorting precision'ı değiştir
    /// </summary>
    public void SetSortingPrecision(float precision)
    {
        sortingPrecision = precision;
        UpdateSortingOrder();
    }

    /// <summary>
    /// Otomatik güncellemeyi aç/kapat
    /// </summary>
    public void SetUpdateEveryFrame(bool update)
    {
        updateEveryFrame = update;
    }

#if UNITY_EDITOR
    // Editor'da debug için gizmos çiz
    private void OnDrawGizmosSelected()
    {
        if (usePivotPoint)
        {
            Gizmos.color = Color.yellow;
            Vector3 pivotWorldPos = transform.position + (Vector3)pivotPoint;
            Gizmos.DrawWireSphere(pivotWorldPos, 0.1f);
        }
    }
#endif
}
