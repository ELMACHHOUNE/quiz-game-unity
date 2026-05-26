using UnityEngine;
using System.Collections;

public class CharacterEmotion : MonoBehaviour
{
    public enum Emotion { Idle, Thinking, Happy, Angry, Celebrating }

    private SpriteRenderer[] srs;
    private Vector3 originalScale;
    private Color[] originalColors;
    private Emotion currentEmotion = Emotion.Idle;
    private Coroutine activeRoutine;

    private Transform head;
    private Transform leftArm;
    private Transform rightArm;

    [Header("Effects")]
    public GameObject effectPrefab;

    void Start()
    {
        srs = GetComponentsInChildren<SpriteRenderer>();
        originalScale = transform.localScale;
        originalColors = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++)
        {
            originalColors[i] = srs[i].color;
        }

        // Find body parts by name
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allChildren)
        {
            string n = t.name.ToLower();
            if (n == "head" && head == null) head = t;
            else if (n == "left-hand" && leftArm == null) leftArm = t;
            else if (n == "right-hand" && rightArm == null) rightArm = t;
        }

        if (head != null) head = CreatePivot(head, -0.85f);      // Pivot near bottom of head (neck)
        if (leftArm != null) leftArm = CreatePivot(leftArm, 0.85f);  // Pivot near top of left arm (shoulder)
        if (rightArm != null) rightArm = CreatePivot(rightArm, 0.85f); // Pivot near top of right arm (shoulder)

        IdleAnimation();
    }

    Transform CreatePivot(Transform part, float yDir)
    {
        if (part == null || part.name.EndsWith("_Pivot")) return part;

        // PSD layer groups might not have a SpriteRenderer directly, so check children
        SpriteRenderer[] groupSrs = part.GetComponentsInChildren<SpriteRenderer>();
        if (groupSrs.Length == 0) return part;

        // Calculate the aggregate bounding box for the entire part
        Bounds b = groupSrs[0].bounds;
        for (int i = 1; i < groupSrs.Length; i++)
        {
            b.Encapsulate(groupSrs[i].bounds);
        }

        // Create a pivot object
        GameObject pivot = new GameObject(part.name + "_Pivot");
        pivot.transform.SetParent(part.parent, false);

        // Move pivot to the target bound location (e.g. top of arm, or bottom of head)
        // yDir = 1f means the absolute top edge. We use ~0.85f for shoulders so it's slightly inside.
        Vector3 pivotWorldPos = b.center + new Vector3(0, b.extents.y * yDir, 0);
        
        pivot.transform.position = pivotWorldPos;
        pivot.transform.localRotation = part.localRotation;
        
        // Parent the part to the new pivot
        part.SetParent(pivot.transform, true);

        return pivot.transform;
    }

    void SetColor(Color c)
    {
        foreach (var sr in srs)
        {
            if (sr != null) sr.color = c;
        }
    }

    void RestoreColors()
    {
        for (int i = 0; i < srs.Length; i++)
        {
            if (srs[i] != null) srs[i].color = originalColors[i];
        }
    }

    void IdleAnimation()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        // Reset all body parts to ensure clean state before idle animation
        if (head != null) head.localRotation = Quaternion.identity;
        if (leftArm != null) leftArm.localRotation = Quaternion.identity;
        if (rightArm != null) rightArm.localRotation = Quaternion.identity;

        activeRoutine = StartCoroutine(IdleLoop());
    }

    IEnumerator IdleLoop()
    {
        while (true)
        {
            float t = Time.time;

            if (head != null) head.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * 1.5f) * 2f);
            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * 2f) * 3f);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(0, 0, Mathf.Cos(t * 2f) * 3f);

            yield return null;
        }
    }

    public void SetThinking()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        // Ensure all body parts are initialized before starting thinking
        if (head == null && leftArm == null && rightArm == null)
        {
            // Attempt to find them again in case they weren't found in Start
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                string n = t.name.ToLower();
                if (n == "head" && head == null) head = t;
                else if (n == "left-hand" && leftArm == null) leftArm = t;
                else if (n == "right-hand" && rightArm == null) rightArm = t;
            }
            if (head != null) head = CreatePivot(head, -0.85f);
            if (leftArm != null) leftArm = CreatePivot(leftArm, 0.85f);
            if (rightArm != null) rightArm = CreatePivot(rightArm, 0.85f);
        }

        activeRoutine = StartCoroutine(ThinkingAnim());
    }

    IEnumerator ThinkingAnim()
    {
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;

            if (head != null) head.localRotation = Quaternion.Euler(0, 0, 15f + Mathf.Sin(timer * 2f) * 3f);
            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(0, 0, -20f + Mathf.Sin(timer * 4f) * 2f);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(0, 0, 20f + Mathf.Cos(timer * 4f) * 2f);

            yield return null;
        }
    }

    public void SetHappy(System.Action onComplete = null)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        // Reset body parts to ensure clean animation start
        if (head != null) head.localRotation = Quaternion.identity;
        if (leftArm != null) leftArm.localRotation = Quaternion.identity;
        if (rightArm != null) rightArm.localRotation = Quaternion.identity;

        activeRoutine = StartCoroutine(HappyAnim(onComplete));
    }

    IEnumerator HappyAnim(System.Action onComplete)
    {
        SetColor(new Color(0.5f, 1f, 0.5f));
        for (int i = 0; i < 3; i++)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * 6f;

                if (leftArm != null) leftArm.localRotation = Quaternion.Euler(0, 0, 100f * Mathf.Sin(t * Mathf.PI));
                if (rightArm != null) rightArm.localRotation = Quaternion.Euler(0, 0, -100f * Mathf.Sin(t * Mathf.PI));
                if (head != null) head.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * Mathf.PI * 2) * 10f);

                yield return null;
            }
            if (leftArm != null) leftArm.localRotation = Quaternion.identity;
            if (rightArm != null) rightArm.localRotation = Quaternion.identity;
            if (head != null) head.localRotation = Quaternion.identity;

            SpawnParticle(Color.green);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.3f);
        RestoreColors();
        IdleAnimation();
        onComplete?.Invoke();
    }

    public void SetAngry(System.Action onComplete = null)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        // Reset body parts to ensure clean animation start
        if (head != null) head.localRotation = Quaternion.identity;
        if (leftArm != null) leftArm.localRotation = Quaternion.identity;
        if (rightArm != null) rightArm.localRotation = Quaternion.identity;

        activeRoutine = StartCoroutine(AngryAnim(onComplete));
    }

    IEnumerator AngryAnim(System.Action onComplete)
    {
        SetColor(new Color(1f, 0.3f, 0.3f));

        for (int i = 0; i < 4; i++)
        {
            if (head != null) head.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(0, 0, Random.Range(35f, 55f));
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(0, 0, Random.Range(-35f, -55f));

            SpawnParticle(Color.red);
            yield return new WaitForSeconds(0.06f);
        }

        if (head != null) head.localRotation = Quaternion.identity;
        if (leftArm != null) leftArm.localRotation = Quaternion.identity;
        if (rightArm != null) rightArm.localRotation = Quaternion.identity;

        float flashTimer = 0;
        while (flashTimer < 0.8f)
        {
            if (flashTimer % 0.15f < 0.075f) SetColor(new Color(1f, 0.3f, 0.3f));
            else RestoreColors();
            flashTimer += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.identity;
        transform.localScale = originalScale;
        RestoreColors();
        IdleAnimation();
        onComplete?.Invoke();
    }

    public void SetCelebrating(System.Action onComplete = null)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        // Reset body parts to ensure clean animation start
        if (head != null) head.localRotation = Quaternion.identity;
        if (leftArm != null) leftArm.localRotation = Quaternion.identity;
        if (rightArm != null) rightArm.localRotation = Quaternion.identity;

        activeRoutine = StartCoroutine(CelebrateAnim(onComplete));
    }

    IEnumerator CelebrateAnim(System.Action onComplete)
    {
        for (int i = 0; i < 5; i++)
        {
            float duration = 0.3f;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;

                if (head != null) head.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * Mathf.PI * 4f) * 15f);
                if (leftArm != null) leftArm.localRotation = Quaternion.Euler(0, 0, 150f * Mathf.Sin(t * Mathf.PI));
                if (rightArm != null) rightArm.localRotation = Quaternion.Euler(0, 0, -150f * Mathf.Sin(t * Mathf.PI));

                yield return null;
            }
            if (head != null) head.localRotation = Quaternion.identity;
            if (leftArm != null) leftArm.localRotation = Quaternion.identity;
            if (rightArm != null) rightArm.localRotation = Quaternion.identity;
        }
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
