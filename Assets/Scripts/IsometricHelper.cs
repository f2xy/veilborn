using UnityEngine;

/// <summary>
/// Isometric koordinat dönüşümleri ve yardımcı fonksiyonlar.
/// Kartezyen koordinatları isometric'e ve tam tersine çevirir.
/// </summary>
public static class IsometricHelper
{
    // Standart isometric açılar
    public const float ISOMETRIC_ANGLE = 30f; // Derece cinsinden
    public const float TILE_WIDTH = 1f;
    public const float TILE_HEIGHT = 0.5f;

    /// <summary>
    /// Kartezyen (düz) koordinatları isometric koordinatlara çevir
    /// </summary>
    /// <param name="cartesian">Kartezyen koordinat</param>
    /// <returns>Isometric koordinat</returns>
    public static Vector2 CartesianToIsometric(Vector2 cartesian)
    {
        float isoX = (cartesian.x - cartesian.y) * TILE_WIDTH / 2f;
        float isoY = (cartesian.x + cartesian.y) * TILE_HEIGHT / 2f;
        return new Vector2(isoX, isoY);
    }

    /// <summary>
    /// Isometric koordinatları kartezyen (düz) koordinatlara çevir
    /// </summary>
    /// <param name="isometric">Isometric koordinat</param>
    /// <returns>Kartezyen koordinat</returns>
    public static Vector2 IsometricToCartesian(Vector2 isometric)
    {
        float cartX = (isometric.x / (TILE_WIDTH / 2f) + isometric.y / (TILE_HEIGHT / 2f)) / 2f;
        float cartY = (isometric.y / (TILE_HEIGHT / 2f) - isometric.x / (TILE_WIDTH / 2f)) / 2f;
        return new Vector2(cartX, cartY);
    }

    /// <summary>
    /// 3D kartezyen koordinatları 2D isometric koordinatlara çevir
    /// </summary>
    /// <param name="cartesian3D">3D Kartezyen koordinat</param>
    /// <returns>2D Isometric koordinat</returns>
    public static Vector2 Cartesian3DToIsometric(Vector3 cartesian3D)
    {
        float isoX = cartesian3D.x - cartesian3D.z;
        float isoY = (cartesian3D.x + cartesian3D.z) / 2f - cartesian3D.y;
        return new Vector2(isoX, isoY);
    }

    /// <summary>
    /// World pozisyonunu grid koordinatına çevir
    /// </summary>
    /// <param name="worldPosition">Dünya pozisyonu</param>
    /// <returns>Grid koordinatı</returns>
    public static Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        Vector2 cartesian = IsometricToCartesian(worldPosition);
        return new Vector2Int(
            Mathf.FloorToInt(cartesian.x),
            Mathf.FloorToInt(cartesian.y)
        );
    }

    /// <summary>
    /// Grid koordinatını world pozisyonuna çevir
    /// </summary>
    /// <param name="gridPosition">Grid koordinatı</param>
    /// <returns>Dünya pozisyonu</returns>
    public static Vector2 GridToWorld(Vector2Int gridPosition)
    {
        return CartesianToIsometric(new Vector2(gridPosition.x, gridPosition.y));
    }

    /// <summary>
    /// Grid koordinatını world pozisyonuna çevir (merkez)
    /// </summary>
    /// <param name="gridPosition">Grid koordinatı</param>
    /// <returns>Grid hücresinin merkez dünya pozisyonu</returns>
    public static Vector2 GridToWorldCenter(Vector2Int gridPosition)
    {
        Vector2 offset = new Vector2(0.5f, 0.5f);
        return CartesianToIsometric(new Vector2(gridPosition.x, gridPosition.y) + offset);
    }

    /// <summary>
    /// İki grid koordinatı arasındaki manhattan mesafesini hesapla
    /// </summary>
    /// <param name="from">Başlangıç koordinatı</param>
    /// <param name="to">Hedef koordinat</param>
    /// <returns>Manhattan mesafesi</returns>
    public static int ManhattanDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }

    /// <summary>
    /// Tile'ın komşularını al (4 yön)
    /// </summary>
    /// <param name="gridPosition">Grid koordinatı</param>
    /// <returns>4 komşu koordinat</returns>
    public static Vector2Int[] GetNeighbors4(Vector2Int gridPosition)
    {
        return new Vector2Int[]
        {
            gridPosition + Vector2Int.up,       // Kuzey
            gridPosition + Vector2Int.right,    // Doğu
            gridPosition + Vector2Int.down,     // Güney
            gridPosition + Vector2Int.left      // Batı
        };
    }

    /// <summary>
    /// Tile'ın komşularını al (8 yön)
    /// </summary>
    /// <param name="gridPosition">Grid koordinatı</param>
    /// <returns>8 komşu koordinat</returns>
    public static Vector2Int[] GetNeighbors8(Vector2Int gridPosition)
    {
        return new Vector2Int[]
        {
            gridPosition + Vector2Int.up,                           // Kuzey
            gridPosition + Vector2Int.up + Vector2Int.right,       // Kuzey-Doğu
            gridPosition + Vector2Int.right,                        // Doğu
            gridPosition + Vector2Int.down + Vector2Int.right,     // Güney-Doğu
            gridPosition + Vector2Int.down,                         // Güney
            gridPosition + Vector2Int.down + Vector2Int.left,      // Güney-Batı
            gridPosition + Vector2Int.left,                         // Batı
            gridPosition + Vector2Int.up + Vector2Int.left         // Kuzey-Batı
        };
    }

    /// <summary>
    /// Mouse pozisyonunu world pozisyonuna çevir
    /// </summary>
    /// <param name="mousePosition">Mouse screen pozisyonu</param>
    /// <param name="camera">Kullanılacak kamera (null ise main camera)</param>
    /// <returns>World pozisyonu</returns>
    public static Vector2 ScreenToWorld(Vector3 mousePosition, Camera camera = null)
    {
        if (camera == null)
            camera = Camera.main;

        Vector3 worldPos = camera.ScreenToWorldPoint(mousePosition);
        return new Vector2(worldPos.x, worldPos.y);
    }

    /// <summary>
    /// Mouse pozisyonunu grid koordinatına çevir
    /// </summary>
    /// <param name="mousePosition">Mouse screen pozisyonu</param>
    /// <param name="camera">Kullanılacak kamera (null ise main camera)</param>
    /// <returns>Grid koordinatı</returns>
    public static Vector2Int ScreenToGrid(Vector3 mousePosition, Camera camera = null)
    {
        Vector2 worldPos = ScreenToWorld(mousePosition, camera);
        return WorldToGrid(worldPos);
    }

    /// <summary>
    /// İki nokta arasındaki yönü hesapla (8 yön)
    /// </summary>
    /// <param name="from">Başlangıç noktası</param>
    /// <param name="to">Hedef nokta</param>
    /// <returns>Yön vektörü (-1, 0, 1 değerleri)</returns>
    public static Vector2Int GetDirection8(Vector2Int from, Vector2Int to)
    {
        Vector2Int delta = to - from;
        return new Vector2Int(
            Mathf.Clamp(delta.x, -1, 1),
            Mathf.Clamp(delta.y, -1, 1)
        );
    }
}
