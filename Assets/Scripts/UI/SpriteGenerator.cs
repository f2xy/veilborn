using UnityEngine;

/// <summary>
/// Runtime'da basit placeholder sprite'lar oluşturur
/// </summary>
public static class SpriteGenerator
{
    /// <summary>
    /// Kare sprite oluştur
    /// </summary>
    public static Sprite CreateSquareSprite(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    /// <summary>
    /// Daire sprite oluştur
    /// </summary>
    public static Sprite CreateCircleSprite(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        float radius = size / 2f;
        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);

                if (distance <= radius)
                {
                    pixels[y * size + x] = color;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    /// <summary>
    /// Çerçeveli kare sprite oluştur
    /// </summary>
    public static Sprite CreateBorderedSquareSprite(int size, Color fillColor, Color borderColor, int borderWidth = 2)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isBorder = x < borderWidth || x >= size - borderWidth ||
                                y < borderWidth || y >= size - borderWidth;

                pixels[y * size + x] = isBorder ? borderColor : fillColor;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    /// <summary>
    /// Üçgen sprite oluştur (isometric için)
    /// </summary>
    public static Sprite CreateIsometricTileSprite(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        // Başlangıçta transparan
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        // Isometric elmas şekli çiz
        int centerX = width / 2;

        for (int y = 0; y < height; y++)
        {
            float normalizedY = (float)y / height;
            int halfWidth;

            if (normalizedY < 0.5f)
            {
                // Üst yarı - genişleyen
                halfWidth = Mathf.RoundToInt(centerX * (normalizedY * 2));
            }
            else
            {
                // Alt yarı - daralan
                halfWidth = Mathf.RoundToInt(centerX * ((1 - normalizedY) * 2));
            }

            for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
            {
                if (x >= 0 && x < width)
                {
                    pixels[y * width + x] = color;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), width);
    }

    /// <summary>
    /// Ev sprite'ı (basit kare ev şekli)
    /// </summary>
    public static Sprite CreateHouseSprite(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        // Başlangıçta transparan
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        Color roofColor = AdjustBrightness(color, 0.7f);
        Color doorColor = AdjustBrightness(color, 0.5f);

        // Ev gövdesi (alt 2/3)
        int houseHeight = size * 2 / 3;
        for (int y = 0; y < houseHeight; y++)
        {
            for (int x = size / 6; x < size * 5 / 6; x++)
            {
                pixels[y * size + x] = color;
            }
        }

        // Çatı (üst 1/3, üçgen)
        int roofStart = houseHeight;
        int centerX = size / 2;
        for (int y = roofStart; y < size; y++)
        {
            int width = (size * 2 / 3) * (size - y) / (size - roofStart);
            for (int x = centerX - width / 2; x <= centerX + width / 2; x++)
            {
                if (x >= 0 && x < size)
                {
                    pixels[y * size + x] = roofColor;
                }
            }
        }

        // Kapı (orta alt)
        int doorWidth = size / 5;
        int doorHeight = size / 3;
        for (int y = 0; y < doorHeight; y++)
        {
            for (int x = centerX - doorWidth / 2; x <= centerX + doorWidth / 2; x++)
            {
                if (x >= 0 && x < size)
                {
                    pixels[y * size + x] = doorColor;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0f), size);
    }

    /// <summary>
    /// Ağaç sprite'ı (kahverengi gövde + yeşil tepe)
    /// </summary>
    public static Sprite CreateTreeSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        // Başlangıçta transparan
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        int centerX = size / 2;
        int trunkWidth = size / 6;
        int trunkHeight = size / 3;
        int crownRadius = size / 3;

        Color brown = new Color(0.6f, 0.3f, 0f);
        Color green = new Color(0f, 0.6f, 0f);

        // Gövde (alt kısım)
        for (int y = 0; y < trunkHeight; y++)
        {
            for (int x = centerX - trunkWidth / 2; x <= centerX + trunkWidth / 2; x++)
            {
                if (x >= 0 && x < size)
                {
                    pixels[y * size + x] = brown;
                }
            }
        }

        // Taç (daire)
        Vector2 crownCenter = new Vector2(centerX, size - crownRadius);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                if (Vector2.Distance(pos, crownCenter) <= crownRadius)
                {
                    pixels[y * size + x] = green;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0f), size);
    }

    /// <summary>
    /// Renk tonunu değiştir (lighter/darker)
    /// </summary>
    public static Color AdjustBrightness(Color color, float factor)
    {
        return new Color(
            Mathf.Clamp01(color.r * factor),
            Mathf.Clamp01(color.g * factor),
            Mathf.Clamp01(color.b * factor),
            color.a
        );
    }
}
