using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private Canvas menuCanvas;
    private GameObject gameRoot;
    private Font font;
    private GameObject currentPanel;
    
    // Modern UI Colors
    private Color bgDark = new Color(0.06f, 0.09f, 0.16f); // #0F172A
    private Color btnNormal = new Color(0.12f, 0.16f, 0.23f); // #1E293B
    private Color btnHighlight = new Color(0.23f, 0.51f, 0.96f); // #3B82F6
    private Color btnPressed = new Color(0.15f, 0.39f, 0.92f); // #2563EB
    private Color textLight = new Color(0.97f, 0.98f, 0.99f); // #F8FAFC
    private Color textAccent = new Color(0.22f, 0.74f, 0.97f); // #38BDF8

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        font = Font.CreateDynamicFontFromOSFont("Arial", 24);
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial");
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        // Initialize question categories
        QuestionGenerator tempGen = gameObject.AddComponent<QuestionGenerator>();
        tempGen.InitializeData();
        Destroy(tempGen);

        CreateEventSystem();
        SetupAudio();
        CreateMenu();
    }

    void SetupAudio()
    {
        AudioListener.volume = 1f;
        AudioSource src = GetComponent<AudioSource>();
        if (src == null) src = gameObject.AddComponent<AudioSource>();
        AudioClip clip = Resources.Load<AudioClip>("sound-effect");
        if (clip != null)
        {
            src.clip = clip;
            src.loop = true;
            src.volume = 0.5f;
            src.Play();
        }
    }

    void CreateEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        GameObject esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<StandaloneInputModule>();
    }

    void CreateMenu()
    {
        GameObject canvasGO = new GameObject("MenuCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();
        menuCanvas = canvas;

        GameObject bgGO = new GameObject("MenuBg");
        bgGO.transform.SetParent(canvas.transform, false);
        Image bgImg = bgGO.AddComponent<Image>();
        bgImg.color = bgDark; // Modern Background
        RectTransform brt = bgGO.GetComponent<RectTransform>();
        brt.anchorMin = Vector2.zero;
        brt.anchorMax = Vector2.one;
        brt.sizeDelta = Vector2.zero;

        CreateSoundButton(canvas);

        ShowMainMenu();
    }

    void ClearCurrentPanel()
    {
        if (currentPanel != null)
        {
            Destroy(currentPanel);
        }
        currentPanel = new GameObject("Panel");
        currentPanel.transform.SetParent(menuCanvas.transform, false);
        RectTransform rt = currentPanel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    void ShowMainMenu()
    {
        ClearCurrentPanel();

        GameObject title = CreateText("QUIZ LOBBY", 72, textAccent, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, 180, 0), new Vector2(700, 90));
        title.transform.SetParent(currentPanel.transform, false);
        title.GetComponent<Text>().font = font;
        title.GetComponent<Text>().fontStyle = FontStyle.Bold;
        
        // Add subtle shadow to title
        Shadow shadow = title.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(2, -2);

        CreateMenuButton(currentPanel.transform, "\u25B6 SELECT CATEGORY", new Vector3(0, 30, 0), () => ShowCategories());
        CreateMenuButton(currentPanel.transform, "\u2715 EXIT GAME", new Vector3(0, -70, 0), () => Application.Quit());
    }

    void ShowCategories()
    {
        ClearCurrentPanel();

        GameObject title = CreateText("CATEGORIES", 60, textAccent, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, 250, 0), new Vector2(700, 90));
        title.transform.SetParent(currentPanel.transform, false);
        title.GetComponent<Text>().font = font;
        title.GetComponent<Text>().fontStyle = FontStyle.Bold;
        
        Shadow shadow = title.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(2, -2);

        float startY = 100;
        foreach (var cat in QuestionGenerator.categories)
        {
            string catName = cat.categoryName;
            string icon = GetCategoryIcon(catName);
            CreateMenuButton(currentPanel.transform, icon + "  " + catName, new Vector3(0, startY, 0), () => ShowQuizzes(cat));
            startY -= 100;
        }

        CreateMenuButton(currentPanel.transform, "\u2190 BACK", new Vector3(0, startY - 50, 0), () => ShowMainMenu());
    }

    void ShowQuizzes(QuizCategory category)
    {
        ClearCurrentPanel();

        GameObject title = CreateText(category.categoryName.ToUpper() + " QUIZZES", 60, textAccent, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, 250, 0), new Vector2(800, 90));
        title.transform.SetParent(currentPanel.transform, false);
        title.GetComponent<Text>().font = font;
        title.GetComponent<Text>().fontStyle = FontStyle.Bold;
        
        Shadow shadow = title.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(2, -2);

        float startY = 100;
        foreach (var quiz in category.quizzes)
        {
            string qName = quiz.quizName;
            CreateMenuButton(currentPanel.transform, qName, new Vector3(0, startY, 0), () => PlayGame(qName));
            startY -= 100;
        }

        CreateMenuButton(currentPanel.transform, "\u2190 BACK", new Vector3(0, startY - 50, 0), () => ShowCategories());
    }

    void CreateSoundButton(Canvas canvas)
    {
        GameObject btnGO = new GameObject("SoundBtn");
        btnGO.transform.SetParent(canvas.transform, false);
        Image img = btnGO.AddComponent<Image>();
        img.sprite = MakeRoundedSprite();
        img.type = Image.Type.Sliced;
        Button btn = btnGO.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = btnNormal;
        colors.highlightedColor = btnHighlight;
        colors.pressedColor = btnPressed;
        colors.fadeDuration = 0.15f;
        btn.colors = colors;
        
        btnGO.AddComponent<ButtonHoverAnimator>();
        
        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(240, 70);
        rt.anchoredPosition = new Vector3(25, 25, 0);
        GameObject txt = CreateText(AudioListener.volume > 0 ? "\u266B SOUND ON" : "\u266B SOUND OFF", 28, textLight, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        txt.transform.SetParent(btnGO.transform, false);
        txt.GetComponent<Text>().font = font;
        txt.GetComponent<Text>().fontStyle = FontStyle.Bold;
        btn.onClick.AddListener(() =>
        {
            AudioListener.volume = AudioListener.volume > 0 ? 0f : 1f;
            txt.GetComponent<Text>().text = AudioListener.volume > 0 ? "\u266B SOUND ON" : "\u266B SOUND OFF";
        });
    }

    GameObject CreateText(string text, int fontSize, Color color, TextAnchor align, Vector2 anchorMin, Vector2 anchorMax, Vector3 pos, Vector2 size)
    {
        GameObject go = new GameObject("Text_" + text.Replace(" ", ""));
        Text txt = go.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.alignment = align;
        txt.raycastTarget = false;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = anchorMin;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    Sprite MakeRoundedSprite()
    {
        int size = 64;
        int radius = 16;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dx = Mathf.Min(x, size - 1 - x);
                float dy = Mathf.Min(y, size - 1 - y);
                if (dx < radius && dy < radius)
                {
                    float dist = Mathf.Sqrt((radius - dx) * (radius - dx) + (radius - dy) * (radius - dy));
                    tex.SetPixel(x, y, dist > radius ? Color.clear : Color.white);
                }
                else
                {
                    tex.SetPixel(x, y, Color.white);
                }
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
    }

    void CreateMenuButton(Transform parent, string text, Vector3 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGO = new GameObject("Btn_" + text.Replace(" ", ""));
        btnGO.transform.SetParent(parent, false);

        Image img = btnGO.AddComponent<Image>();
        img.sprite = MakeRoundedSprite();
        img.type = Image.Type.Sliced;

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = btnNormal;
        colors.highlightedColor = btnHighlight;
        colors.pressedColor = btnPressed;
        colors.fadeDuration = 0.15f;
        btn.colors = colors;
        
        btnGO.AddComponent<ButtonHoverAnimator>();
        
        // Add subtle shadow to button
        Shadow shadow = btnGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.3f);
        shadow.effectDistance = new Vector2(0, -3);

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(400, 75); // Made wider and taller for a modern look
        rt.anchoredPosition = position;

        GameObject btnText = CreateText(text, 28, textLight, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        btnText.transform.SetParent(btnGO.transform, false);
        btnText.GetComponent<Text>().font = font;
        btnText.GetComponent<Text>().fontStyle = FontStyle.Bold;

        btn.onClick.AddListener(onClick);
    }

    void PlayGame(string quizName)
    {
        QuestionGenerator.selectedQuizName = quizName;
        menuCanvas.gameObject.SetActive(false);
        GameObject go = new GameObject("GameUI");
        go.AddComponent<GameUI>();
        gameRoot = go;
    }

    string GetCategoryIcon(string catName)
    {
        switch (catName)
        {
            case "Code": return "\uD83D\uDCBB";
            case "Design": return "\uD83C\uDFA8";
            case "Mathematics": return "\uD83D\uDD22";
            default: return "\uD83D\uDCCA";
        }
    }

    public void ReturnToMenu()
    {
        if (gameRoot != null)
        {
            Destroy(gameRoot);
            gameRoot = null;
        }
        menuCanvas.gameObject.SetActive(true);
        ShowMainMenu();
    }
}

public class ButtonHoverAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private float targetScale = 1f;
    private float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, originalScale * targetScale, ref velocity, smoothTime);
    }

    public void OnPointerEnter(PointerEventData eventData) { targetScale = 1.03f; }
    public void OnPointerExit(PointerEventData eventData) { targetScale = 1f; }
    public void OnPointerDown(PointerEventData eventData) { targetScale = 0.95f; }
    public void OnPointerUp(PointerEventData eventData) { targetScale = 1.03f; }
}