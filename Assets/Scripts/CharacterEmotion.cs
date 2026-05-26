using UnityEngine;
using System.Collections;

public class CharacterEmotion : MonoBehaviour
{
    public enum Emotion { Idle, Thinking, Happy, Angry, Celebrating }

    private SpriteRenderer sr;
    private Vector3 originalScale;
    private Color originalColor;
    private Emotion currentEmotion = Emotion.Idle;
    private Coroutine activeRoutine;

    [Header("Effects")]
    public GameObject effectPrefab;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        originalColor = sr.color;
        IdleAnimation();
    }

    void IdleAnimation()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(IdleLoop());
    }

    IEnumerator IdleLoop()
    {
        while (true)
        {
            float t = Mathf.Sin(Time.time * 2f) * 0.03f;
            transform.localScale = originalScale + Vector3.one * t;
            yield return null;
        }
    }

    public void SetThinking()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ThinkingAnim());
    }

    IEnumerator ThinkingAnim()
    {
        float timer = 0;
        Vector3 targetScale = originalScale * 0.95f;
        while (true)
        {
            timer += Time.deltaTime;
            float tilt = Mathf.Sin(timer * 1.5f) * 5f;
            transform.rotation = Quaternion.Euler(0, 0, tilt);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.PingPong(timer * 0.5f, 1));
            yield return null;
        }
    }

    public void SetHappy(System.Action onComplete = null)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(HappyAnim(onComplete));
    }

    IEnumerator HappyAnim(System.Action onComplete)
    {
        sr.color = new Color(0.5f, 1f, 0.5f);
        for (int i = 0; i < 3; i++)
        {
            float jumpH = 0.3f;
            Vector3 basePos = transform.position;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * 6f;
                float y = Mathf.Sin(t * Mathf.PI) * jumpH;
                transform.position = basePos + new Vector3(0, y, 0);
                transform.localScale = originalScale + Vector3.one * Mathf.Lerp(0, 0.1f, Mathf.PingPong(t * 2, 1));
                yield return null;
            }
            transform.position = basePos;
            SpawnParticle(Color.green);
        }
        transform.rotation = Quaternion.identity;
        transform.localScale = originalScale;
        yield return new WaitForSeconds(0.3f);
        sr.color = originalColor;
        IdleAnimation();
        onComplete?.Invoke();
    }

    public void SetAngry(System.Action onComplete = null)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(AngryAnim(onComplete));
    }

    IEnumerator AngryAnim(System.Action onComplete)
    {
        sr.color = new Color(1f, 0.3f, 0.3f);
        Vector3 basePos = transform.position;

        for (int i = 0; i < 4; i++)
        {
            float shakeX = Random.Range(-0.15f, 0.15f);
            float shakeY = Random.Range(-0.1f, 0.1f);
            transform.position = basePos + new Vector3(shakeX, shakeY, 0);
            transform.localScale = originalScale + Vector3.one * 0.05f;
            SpawnParticle(Color.red);
            yield return new WaitForSeconds(0.06f);
        }
        transform.position = basePos;

        float flashTimer = 0;
        while (flashTimer < 0.8f)
        {
            sr.color = flashTimer % 0.15f < 0.075f ? new Color(1f, 0.3f, 0.3f) : originalColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.identity;
        transform.localScale = originalScale;
        sr.color = originalColor;
        IdleAnimation();
        onComplete?.Invoke();
    }

    public void SetCelebrating(System.Action onComplete = null)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(CelebrateAnim(onComplete));
    }

    IEnumerator CelebrateAnim(System.Action onComplete)
    {
        for (int i = 0; i < 5; i++)
        {
            float rotation = 360f * (i + 1) / 5f;
            float duration = 0.3f;
            float t = 0;
            Quaternion startRot = transform.rotation;
            Vector3 basePos = transform.position;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, rotation, t));
                transform.position = basePos + new Vector3(0, Mathf.Sin(t * Mathf.PI) * 0.4f, 0);
                transform.localScale = originalScale + Vector3.one * Mathf.Lerp(0, 0.15f, Mathf.PingPong(t * 2, 1));
                yield return null;
            }
            SpawnParticle(Color.yellow);
            SpawnParticle(new Color(1, 0.5f, 0));
        }
        transform.rotation = Quaternion.identity;
        transform.localScale = originalScale;
        yield return new WaitForSeconds(0.5f);
        IdleAnimation();
        onComplete?.Invoke();
    }

    void SpawnParticle(Color color)
    {
        if (effectPrefab == null)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject p = new GameObject("Particle");
                SpriteRenderer psr = p.AddComponent<SpriteRenderer>();
                Texture2D tex = new Texture2D(4, 4);
                for (int x = 0; x < 4; x++)
                    for (int y = 0; y < 4; y++)
                        tex.SetPixel(x, y, Color.white);
                tex.Apply();
                psr.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
                psr.color = color;
                psr.sortingOrder = 10;
                p.transform.position = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f), 0);
                p.transform.localScale = Vector3.one * Random.Range(0.1f, 0.2f);
                Rigidbody2D rb = p.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0.5f;
                rb.linearVelocity = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f));
                Destroy(p, 0.8f);
            }
        }
    }
}
