using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InteractiveBackground : MonoBehaviour
{
    public int starCount = 60;
    public int layerCount = 3;
    public Color[] nebulaColors;
    public Canvas canvas;

    private List<StarData> stars = new List<StarData>();
    private List<NebulaDrift> nebulaBlobs = new List<NebulaDrift>();
    private float nebulaTime;
    private Camera cam;
    private Color baseColor;

    class StarData
    {
        public Transform t;
        public float twinkleSpeed;
        public float twinkleOffset;
        public float orbitSpeed;
        public float orbitAngle;
        public Vector3 basePos;
        public float depth;
        public float parallaxFactor;
        public SpriteRenderer sr;
        public float baseAlpha;
    }

    class NebulaDrift
    {
        public Transform t;
        public Vector3 driftDir;
        public float driftSpeed;
        public float rotationSpeed;
        public SpriteRenderer sr;
        public Vector3 boundsMin;
        public Vector3 boundsMax;
    }

    void Start()
    {
        cam = Camera.main;
        if (cam == null) cam = FindObjectOfType<Camera>();
        baseColor = cam.backgroundColor;

        if (nebulaColors == null || nebulaColors.Length == 0)
            nebulaColors = new Color[] {
                new Color(0.06f, 0.07f, 0.15f),
                new Color(0.10f, 0.04f, 0.18f),
                new Color(0.04f, 0.10f, 0.20f),
                new Color(0.12f, 0.06f, 0.10f)
            };

        CreateStars();
        CreateNebulaBlobs();
    }

    void CreateStars()
    {
        Texture2D starTex = GenerateCircleTexture(8, Color.white);

        for (int layer = 0; layer < layerCount; layer++)
        {
            float depth = 3f + layer * 4f;
            float sizeMul = 0.06f + layer * 0.04f;
            float alphaMul = 1f - layer * 0.2f;
            starsPerLayer = starCount / layerCount;

            for (int i = 0; i < starsPerLayer; i++)
            {
                GameObject go = new GameObject("Star" + layer + "_" + i);
                go.transform.SetParent(transform);

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = Sprite.Create(starTex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
                float b = Random.Range(0.6f, 1f);
                sr.color = new Color(b, b, b, alphaMul * Random.Range(0.4f, 1f));
                sr.sortingOrder = -10 + layer;

                float spread = 2f + layer * 2f;
                float x = Random.Range(-spread * 3f, spread * 3f);
                float y = Random.Range(-spread * 2f, spread * 2f);
                float size = Random.Range(0.8f, 2f) * sizeMul;
                go.transform.localScale = Vector3.one * size;
                go.transform.position = new Vector3(x, y, depth);

                stars.Add(new StarData
                {
                    t = go.transform,
                    twinkleSpeed = Random.Range(0.8f, 3f),
                    twinkleOffset = Random.Range(0f, Mathf.PI * 2f),
                    orbitSpeed = Random.Range(-0.3f, 0.3f),
                    orbitAngle = Random.Range(0f, Mathf.PI * 2f),
                    basePos = go.transform.position,
                    depth = depth,
                    parallaxFactor = 0.3f + layer * 0.3f,
                    sr = sr,
                    baseAlpha = sr.color.a
                });
            }
        }
    }

    int starsPerLayer;

    void CreateNebulaBlobs()
    {
        Color[] blobColors = new Color[] {
            new Color(0.3f, 0.2f, 0.6f, 0.05f),
            new Color(0.15f, 0.3f, 0.5f, 0.04f),
            new Color(0.5f, 0.15f, 0.3f, 0.03f)
        };

        for (int i = 0; i < 5; i++)
        {
            GameObject go = new GameObject("Nebula_" + i);
            go.transform.SetParent(transform);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            Texture2D tex = GenerateCircleTexture(32, Color.white);
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            sr.color = blobColors[i % blobColors.Length];
            sr.sortingOrder = -15;

            float size = Random.Range(4f, 10f);
            go.transform.localScale = Vector3.one * size;
            go.transform.position = new Vector3(
                Random.Range(-6f, 6f),
                Random.Range(-4f, 4f),
                Random.Range(8f, 15f)
            );

            nebulaBlobs.Add(new NebulaDrift
            {
                t = go.transform,
                driftDir = Random.insideUnitSphere.normalized,
                driftSpeed = Random.Range(0.1f, 0.3f),
                rotationSpeed = Random.Range(-2f, 2f),
                sr = sr,
                boundsMin = new Vector3(-8f, -5f, 8f),
                boundsMax = new Vector3(8f, 5f, 15f)
            });
        }
    }

    Texture2D GenerateCircleTexture(int size, Color color)
    {
        Texture2D tex = new Texture2D(size, size);
        float half = size / 2f;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float dx = (x - half + 0.5f) / half;
                float dy = (y - half + 0.5f) / half;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(1f - d);
                tex.SetPixel(x, y, new Color(color.r, color.g, color.b, a * a));
            }
        tex.Apply();
        return tex;
    }

    void Update()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2 mouseDir = new Vector2(mouseWorld.x, mouseWorld.y) * 0.1f;

        nebulaTime += Time.deltaTime;
        UpdateNebulaColor();

        foreach (var star in stars)
        {
            float twinkle = Mathf.Sin(Time.time * star.twinkleSpeed + star.twinkleOffset) * 0.4f + 0.6f;

            star.orbitAngle += star.orbitSpeed * Time.deltaTime;
            Vector3 orbitOffset = new Vector3(
                Mathf.Cos(star.orbitAngle) * 0.3f,
                Mathf.Sin(star.orbitAngle) * 0.3f,
                0
            );

            Vector3 parallaxOffset = new Vector3(
                -mouseDir.x * star.parallaxFactor,
                -mouseDir.y * star.parallaxFactor * 0.5f,
                0
            );

            star.t.position = star.basePos + orbitOffset + parallaxOffset;

            Color c = star.sr.color;
            star.sr.color = new Color(c.r, c.g, c.b, star.baseAlpha * twinkle);
        }

        foreach (var blob in nebulaBlobs)
        {
            blob.t.position += blob.driftDir * blob.driftSpeed * Time.deltaTime;
            blob.t.Rotate(0, 0, blob.rotationSpeed * Time.deltaTime);

            Vector3 pos = blob.t.position;
            if (pos.x < blob.boundsMin.x || pos.x > blob.boundsMax.x)
                blob.driftDir.x *= -1;
            if (pos.y < blob.boundsMin.y || pos.y > blob.boundsMax.y)
                blob.driftDir.y *= -1;
            blob.t.position = new Vector3(
                Mathf.Clamp(pos.x, blob.boundsMin.x, blob.boundsMax.x),
                Mathf.Clamp(pos.y, blob.boundsMin.y, blob.boundsMax.y),
                pos.z
            );
        }

        if (Input.GetMouseButtonDown(0))
            SpawnClickBurst(cam.ScreenToWorldPoint(Input.mousePosition));
    }

    void SpawnClickBurst(Vector3 worldPos)
    {
        worldPos.z = 5;
        Texture2D dotTex = GenerateCircleTexture(6, Color.white);
        for (int i = 0; i < 12; i++)
        {
            GameObject p = new GameObject("ClickParticle_" + i);
            SpriteRenderer psr = p.AddComponent<SpriteRenderer>();
            psr.sprite = Sprite.Create(dotTex, new Rect(0, 0, 6, 6), new Vector2(0.5f, 0.5f), 6);
            float hue = Random.Range(0.5f, 0.7f);
            psr.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1f, 0.8f);
            psr.sortingOrder = 20;
            p.transform.position = worldPos;
            p.transform.localScale = Vector3.one * Random.Range(0.04f, 0.1f);
            Rigidbody2D rb = p.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.linearVelocity = Random.insideUnitCircle * 4f;
            StartCoroutine(FadeAndDestroy(p, psr, 0.6f));
        }
    }

    void UpdateNebulaColor()
    {
        float t = nebulaTime * 0.015f;
        int i = Mathf.FloorToInt(t) % nebulaColors.Length;
        float f = t - Mathf.Floor(t);
        Color target = Color.Lerp(nebulaColors[i], nebulaColors[(i + 1) % nebulaColors.Length], f);
        cam.backgroundColor = Color.Lerp(cam.backgroundColor, target, Time.deltaTime * 0.3f);
    }

    public void SpawnUIBurst(Vector2 screenPos)
    {
        if (canvas == null) return;
        for (int i = 0; i < 10; i++)
        {
            GameObject p = new GameObject("UIBurst_" + i);
            p.transform.SetParent(canvas.transform, false);
            Image img = p.AddComponent<Image>();
            Texture2D tex = new Texture2D(8, 8);
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    float dx = (x - 3.5f) / 3.5f;
                    float dy = (y - 3.5f) / 3.5f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    tex.SetPixel(x, y, d < 1 ? Color.white : Color.clear);
                }
            tex.Apply();
            img.sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
            img.raycastTarget = false;
            img.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1f, 0.9f);
            RectTransform rt = p.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = screenPos;
            float size = Random.Range(12f, 24f);
            rt.sizeDelta = new Vector2(size, size);
            StartCoroutine(FadeAndDestroyUI(p, img, Random.insideUnitCircle * 120f, 0.5f));
        }
    }

    IEnumerator FadeAndDestroyUI(GameObject go, Image img, Vector2 velocity, float duration)
    {
        float t = 0;
        RectTransform rt = go.GetComponent<RectTransform>();
        Color original = img.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            rt.anchoredPosition += velocity * Time.deltaTime;
            velocity *= 0.95f;
            img.color = Color.Lerp(original, new Color(original.r, original.g, original.b, 0), t / duration);
            yield return null;
        }
        Destroy(go);
    }

    IEnumerator FadeAndDestroy(GameObject go, SpriteRenderer sr, float duration)
    {
        float t = 0;
        Color original = sr.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            sr.color = Color.Lerp(original, new Color(original.r, original.g, original.b, 0), t / duration);
            sr.transform.localScale *= 0.97f;
            yield return null;
        }
        Destroy(go);
    }

    public void Flash(Color color, float duration = 0.6f)
    {
        StartCoroutine(FlashRoutine(color, duration));
    }

    IEnumerator FlashRoutine(Color color, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            cam.backgroundColor = Color.Lerp(color, cam.backgroundColor, t / duration);
            yield return null;
        }
    }
}
