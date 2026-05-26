using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private Canvas menuCanvas;
    private GameObject gameRoot;
    private Font font;

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
        bgImg.color = new Color(0.06f, 0.07f, 0.12f);
        RectTransform brt = bgGO.GetComponent<RectTransform>();
        brt.anchorMin = Vector2.zero;
        brt.anchorMax = Vector2.one;
        brt.sizeDelta = Vector2.zero;

        GameObject title = CreateText("MATH QUIZ", 72, new Color(1, 0.8f, 0.2f), TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, 180, 0), new Vector2(700, 90));
        title.transform.SetParent(canvas.transform, false);
        title.GetComponent<Text>().font = font;
        title.GetComponent<Text>().fontStyle = FontStyle.Bold;

        CreateMenuButton(canvas, "\u25B6 PLAY GAME", new Vector3(0, 30, 0), () => PlayGame());
        CreateMenuButton(canvas, "\u2699 SETTINGS", new Vector3(0, -70, 0), () => ShowSettings());
        CreateMenuButton(canvas, "\u2715 EXIT GAME", new Vector3(0, -170, 0), () => Application.Quit());
        CreateSoundButton(canvas);
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
        colors.normalColor = new Color(0.15f, 0.18f, 0.25f);
        colors.highlightedColor = new Color(0.35f, 0.4f, 0.55f);
        colors.pressedColor = new Color(0.55f, 0.6f, 0.8f);
        colors.fadeDuration = 0.1f;
        btn.colors = colors;
        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(120, 40);
        rt.anchoredPosition = new Vector3(15, 15, 0);
        GameObject txt = CreateText(AudioListener.volume > 0 ? "\u266B SOUND ON" : "\u266B SOUND OFF", 18, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
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
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
            {
                float dx = Mathf.Min(x, 15 - x);
                float dy = Mathf.Min(y, 15 - y);
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                tex.SetPixel(x, y, dist < 3 ? Color.clear : Color.white);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(3, 3, 3, 3));
    }

    void CreateMenuButton(Canvas canvas, string text, Vector3 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGO = new GameObject("Btn_" + text.Replace(" ", ""));
        btnGO.transform.SetParent(canvas.transform, false);

        Image img = btnGO.AddComponent<Image>();
        img.sprite = MakeRoundedSprite();
        img.type = Image.Type.Sliced;

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.15f, 0.18f, 0.25f);
        colors.highlightedColor = new Color(0.35f, 0.4f, 0.55f);
        colors.pressedColor = new Color(0.55f, 0.6f, 0.8f);
        colors.fadeDuration = 0.1f;
        btn.colors = colors;

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(260, 65);
        rt.anchoredPosition = position;

        GameObject btnText = CreateText(text, 28, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        btnText.transform.SetParent(btnGO.transform, false);
        btnText.GetComponent<Text>().font = font;
        btnText.GetComponent<Text>().fontStyle = FontStyle.Bold;

        btn.onClick.AddListener(onClick);
    }

    void PlayGame()
    {
        menuCanvas.gameObject.SetActive(false);
        GameObject go = new GameObject("GameUI");
        go.AddComponent<GameUI>();
        gameRoot = go;
    }

    void ShowSettings()
    {
        Debug.Log("Settings - placeholder");
    }

    public void ReturnToMenu()
    {
        if (gameRoot != null)
        {
            Destroy(gameRoot);
            gameRoot = null;
        }
        menuCanvas.gameObject.SetActive(true);
    }
}
