using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    // Modern UI Colors
    private Color bgDark = new Color(0.06f, 0.09f, 0.16f);
    private Color btnNormal = new Color(0.12f, 0.16f, 0.23f);
    private Color btnHighlight = new Color(0.23f, 0.51f, 0.96f);
    private Color btnPressed = new Color(0.15f, 0.39f, 0.92f);
    private Color textLight = new Color(0.97f, 0.98f, 0.99f);
    private Color accentBlue = new Color(0.22f, 0.74f, 0.97f);
    private Color accentYellow = new Color(0.98f, 0.75f, 0.14f);

    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGO.AddComponent<GraphicRaycaster>();
            canvasGO.transform.SetParent(transform);
        }

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
        }

        Font font = Font.CreateDynamicFontFromOSFont("Arial", 24);
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial");
            if (font == null)
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        GameObject scoreLabel = CreateText(canvas, "\u2605 SCORE", 28, new Color(0.6f, 0.6f, 0.7f), TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(0, 1), new Vector3(30, -15, 0), new Vector2(180, 30));
        scoreLabel.GetComponent<Text>().font = font;
        scoreLabel.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject scoreText = CreateText(canvas, "0", 64, Color.white, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(0, 1), new Vector3(30, -50, 0), new Vector2(200, 76));
        scoreText.GetComponent<Text>().font = font;
        scoreText.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject comboText = CreateText(canvas, "", 32, Color.yellow, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(0, 1), new Vector3(30, -120, 0), new Vector2(300, 40));
        comboText.GetComponent<Text>().font = font;
        comboText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        comboText.SetActive(false);

        GameObject qCountPanel = new GameObject("QCountPanel");
        qCountPanel.transform.SetParent(canvas.transform, false);
        Image qcImg = qCountPanel.AddComponent<Image>();
        qcImg.color = new Color(0.04f, 0.08f, 0.15f, 0.85f);
        RectTransform qcrt = qCountPanel.GetComponent<RectTransform>();
        qcrt.anchorMin = new Vector2(1, 1);
        qcrt.anchorMax = new Vector2(1, 1);
        qcrt.pivot = new Vector2(1, 1);
        qcrt.sizeDelta = new Vector2(140, 76);
        qcrt.anchoredPosition = new Vector3(-15, -12, 0);

        GameObject qcIcon = CreateTextInParent(qCountPanel, "\u25C9", 24, new Color(0.4f, 0.8f, 1f, 0.7f), TextAnchor.MiddleCenter, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector3(0, -20, 0), new Vector2(50, 28));
        qcIcon.GetComponent<Text>().font = font;
        GameObject questionCounter = CreateTextInParent(qCountPanel, "", 36, new Color(0.4f, 0.8f, 1f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector3(0, 24, 0), new Vector2(90, 42));
        questionCounter.GetComponent<Text>().font = font;
        questionCounter.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject timerGO = new GameObject("TimerBar");
        timerGO.transform.SetParent(canvas.transform, false);
        Image timerBar = timerGO.AddComponent<Image>();
        timerBar.color = new Color(0.2f, 0.6f, 1f);
        RectTransform trt = timerGO.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0, 1);
        trt.anchorMax = new Vector2(1, 1);
        trt.pivot = new Vector2(0, 1);
        trt.sizeDelta = new Vector2(0, 6);
        trt.anchoredPosition = Vector2.zero;
        timerBar.type = Image.Type.Filled;
        timerBar.fillMethod = Image.FillMethod.Horizontal;

        GameObject timerBg = new GameObject("TimerBg");
        timerBg.transform.SetParent(canvas.transform, false);
        Image tbg = timerBg.AddComponent<Image>();
        tbg.color = new Color(0.1f, 0.1f, 0.15f);
        RectTransform tbRt = timerBg.GetComponent<RectTransform>();
        tbRt.anchorMin = new Vector2(0, 1);
        tbRt.anchorMax = new Vector2(1, 1);
        tbRt.pivot = new Vector2(0, 1);
        tbRt.sizeDelta = new Vector2(0, 6);
        tbRt.anchoredPosition = Vector2.zero;
        timerBg.transform.SetAsFirstSibling();

        GameObject timerTextGO = new GameObject("TimerTextPanel");
        timerTextGO.transform.SetParent(canvas.transform, false);
        Image timerBgImg = timerTextGO.AddComponent<Image>();
        timerBgImg.color = new Color(0.12f, 0.06f, 0.03f, 0.85f);
        RectTransform ttrt = timerTextGO.GetComponent<RectTransform>();
        ttrt.anchorMin = new Vector2(0.5f, 1);
        ttrt.anchorMax = new Vector2(0.5f, 1);
        ttrt.pivot = new Vector2(0.5f, 1);
        ttrt.sizeDelta = new Vector2(150, 86);
        ttrt.anchoredPosition = new Vector3(0, -14, 0);

        GameObject timerIcon = CreateTextInParent(timerTextGO, "\u231B", 26, new Color(1f, 0.7f, 0.15f, 0.7f), TextAnchor.MiddleCenter, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector3(0, -22, 0), new Vector2(54, 30));
        timerIcon.GetComponent<Text>().font = font;
        GameObject timerText = CreateTextInParent(timerTextGO, "12", 44, new Color(1f, 0.75f, 0.1f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector3(0, 28, 0), new Vector2(80, 50));
        timerText.GetComponent<Text>().font = font;
        timerText.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject questionGO = CreateText(canvas, "Question", 44, textLight, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, 310, 0), new Vector2(900, 120));
        questionGO.GetComponent<Text>().font = font;
        questionGO.GetComponent<Text>().fontStyle = FontStyle.Bold;
        questionGO.GetComponent<Text>().resizeTextForBestFit = true;
        questionGO.GetComponent<Text>().resizeTextMinSize = 20;
        questionGO.GetComponent<Text>().resizeTextMaxSize = 44;
        
        Shadow qShadow = questionGO.AddComponent<Shadow>();
        qShadow.effectColor = new Color(0, 0, 0, 0.5f);
        qShadow.effectDistance = new Vector2(2, -2);

        GameObject[] btnGOs = new GameObject[4];
        Button[] buttons = new Button[4];
        Text[] btnTexts = new Text[4];

        float[] btnY = { -80, 10, 100, 190 };

        for (int i = 0; i < 4; i++)
        {
            btnGOs[i] = new GameObject("AnswerBtn_" + i);
            btnGOs[i].transform.SetParent(canvas.transform, false);

            Image img = btnGOs[i].AddComponent<Image>();
            img.sprite = MakeRoundedRectSprite(64, 64);
            img.type = Image.Type.Sliced;

            Button btn = btnGOs[i].AddComponent<Button>();
            ColorBlock colors = btn.colors;
            colors.normalColor = btnNormal;
            colors.highlightedColor = btnHighlight;
            colors.pressedColor = btnPressed;
            colors.selectedColor = new Color(0.2f, 0.24f, 0.32f);
            colors.fadeDuration = 0.15f;
            btn.colors = colors;
            
            btnGOs[i].AddComponent<ButtonHoverAnimator>();
            Shadow btnShadow = btnGOs[i].AddComponent<Shadow>();
            btnShadow.effectColor = new Color(0, 0, 0, 0.3f);
            btnShadow.effectDistance = new Vector2(0, -3);

            RectTransform brt = btnGOs[i].GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0.5f);
            brt.anchorMax = new Vector2(0.5f, 0.5f);
            brt.pivot = new Vector2(0.5f, 0.5f);
            brt.sizeDelta = new Vector2(750, 75);
            brt.anchoredPosition = new Vector3(0, btnY[i], 0);

            GameObject answerText = CreateTextInParent(btnGOs[i], "", 24, textLight, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            answerText.GetComponent<Text>().font = font;
            answerText.GetComponent<Text>().fontStyle = FontStyle.Bold;
            answerText.GetComponent<Text>().resizeTextForBestFit = true;
            answerText.GetComponent<Text>().resizeTextMinSize = 14;
            answerText.GetComponent<Text>().resizeTextMaxSize = 28;

            buttons[i] = btn;
            btnTexts[i] = answerText.GetComponent<Text>();
        }

        GameObject gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        Image panelImg = gameOverPanel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.75f);
        RectTransform prt = gameOverPanel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.sizeDelta = Vector2.zero;
        gameOverPanel.SetActive(false);

        GameObject goTitle = CreateTextInParent(gameOverPanel, "QUIZ COMPLETE!", 56, new Color(1, 0.8f, 0.2f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, 80, 0), new Vector2(600, 60));
        goTitle.GetComponent<Text>().font = font;
        goTitle.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject finalScoreTitle = CreateTextInParent(gameOverPanel, "FINAL SCORE", 20, new Color(0.6f, 0.6f, 0.7f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, 20, 0), new Vector2(300, 25));
        finalScoreTitle.GetComponent<Text>().font = font;

        GameObject finalScore = CreateTextInParent(gameOverPanel, "0", 72, Color.white, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, -35, 0), new Vector2(300, 70));
        finalScore.GetComponent<Text>().font = font;
        finalScore.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject finalCorrect = CreateTextInParent(gameOverPanel, "", 22, Color.white, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, -90, 0), new Vector2(400, 30));
        finalCorrect.GetComponent<Text>().font = font;

        GameObject restartBtnGO = new GameObject("RestartBtn");
        restartBtnGO.transform.SetParent(gameOverPanel.transform, false);
        Image rImg = restartBtnGO.AddComponent<Image>();
        rImg.sprite = MakeRoundedRectSprite(64, 64);
        rImg.type = Image.Type.Sliced;
        rImg.color = accentBlue;
        Button rBtn = restartBtnGO.AddComponent<Button>();
        ColorBlock rColors = rBtn.colors;
        rColors.highlightedColor = new Color(0.4f, 0.8f, 1f);
        rColors.pressedColor = new Color(0.1f, 0.5f, 0.8f);
        rBtn.colors = rColors;
        
        restartBtnGO.AddComponent<ButtonHoverAnimator>();
        Shadow rShadow = restartBtnGO.AddComponent<Shadow>();
        rShadow.effectColor = new Color(0, 0, 0, 0.3f);
        rShadow.effectDistance = new Vector2(0, -3);

        RectTransform rrt = restartBtnGO.GetComponent<RectTransform>();
        rrt.anchorMin = new Vector2(0.5f, 0.5f);
        rrt.anchorMax = new Vector2(0.5f, 0.5f);
        rrt.pivot = new Vector2(0.5f, 0.5f);
        rrt.sizeDelta = new Vector2(240, 65);
        rrt.anchoredPosition = new Vector3(0, -160, 0);

        GameObject restartText = CreateTextInParent(restartBtnGO, "\u21BB PLAY AGAIN", 26, textLight, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        restartText.GetComponent<Text>().font = font;
        restartText.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvas.transform, false);
        Image ppImg = pausePanel.AddComponent<Image>();
        ppImg.color = new Color(0, 0, 0, 0.85f);
        RectTransform pprt = pausePanel.GetComponent<RectTransform>();
        pprt.anchorMin = Vector2.zero;
        pprt.anchorMax = Vector2.one;
        pprt.sizeDelta = Vector2.zero;
        pausePanel.SetActive(false);

        GameObject pauseTitle = CreateTextInParent(pausePanel, "PAUSED", 48, new Color(1, 0.8f, 0.2f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector3(0, 160, 0), new Vector2(400, 60));
        pauseTitle.GetComponent<Text>().font = font;
        pauseTitle.GetComponent<Text>().fontStyle = FontStyle.Bold;

        System.Action<string, Vector3, System.Action> addPauseBtn = (string label, Vector3 pos, System.Action cb) =>
        {
            GameObject b = new GameObject("PauseBtn_" + label.Replace(" ", ""));
            b.transform.SetParent(pausePanel.transform, false);
            Image bi = b.AddComponent<Image>();
            bi.sprite = MakeRoundedRectSprite(64, 64);
            bi.type = Image.Type.Sliced;
            Button bb = b.AddComponent<Button>();
            ColorBlock bc = bb.colors;
            bc.normalColor = btnNormal;
            bc.highlightedColor = btnHighlight;
            bc.pressedColor = btnPressed;
            bc.fadeDuration = 0.15f;
            bb.colors = bc;
            
            b.AddComponent<ButtonHoverAnimator>();
            Shadow btnShadow = b.AddComponent<Shadow>();
            btnShadow.effectColor = new Color(0, 0, 0, 0.3f);
            btnShadow.effectDistance = new Vector2(0, -3);

            RectTransform br = b.GetComponent<RectTransform>();
            br.anchorMin = new Vector2(0.5f, 0.5f);
            br.anchorMax = new Vector2(0.5f, 0.5f);
            br.pivot = new Vector2(0.5f, 0.5f);
            br.sizeDelta = new Vector2(300, 60);
            br.anchoredPosition = pos;
            GameObject bt = CreateTextInParent(b, label, 24, textLight, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            bt.GetComponent<Text>().font = font;
            bt.GetComponent<Text>().fontStyle = FontStyle.Bold;
            bb.onClick.AddListener(() => cb());
        };

        addPauseBtn("\u25B6  RESUME", new Vector3(0, 70, 0), () =>
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        });
        addPauseBtn("\u21BB  PLAY AGAIN", new Vector3(0, -10, 0), () =>
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            if (QuizGameManager.Instance != null) QuizGameManager.Instance.RestartGame();
        });
        addPauseBtn("\u2699  SETTINGS", new Vector3(0, -90, 0), () => { });
        addPauseBtn("\u2715  EXIT GAME", new Vector3(0, -170, 0), () =>
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            if (MenuManager.Instance != null) MenuManager.Instance.ReturnToMenu();
        });

        GameObject exitBtnGO = new GameObject("ExitBtn");
        exitBtnGO.transform.SetParent(canvas.transform, false);
        Image eImg = exitBtnGO.AddComponent<Image>();
        eImg.sprite = MakeRoundedRectSprite(32, 32);
        eImg.type = Image.Type.Sliced;
        eImg.color = new Color(0.8f, 0.2f, 0.2f);
        Button eBtn = exitBtnGO.AddComponent<Button>();
        ColorBlock eColors = eBtn.colors;
        eColors.normalColor = new Color(0.8f, 0.2f, 0.2f);
        eColors.highlightedColor = new Color(0.9f, 0.3f, 0.3f);
        eColors.pressedColor = new Color(0.6f, 0.1f, 0.1f);
        eColors.fadeDuration = 0.15f;
        eBtn.colors = eColors;
        
        exitBtnGO.AddComponent<ButtonHoverAnimator>();
        
        RectTransform ert = exitBtnGO.GetComponent<RectTransform>();
        ert.anchorMin = new Vector2(1, 0);
        ert.anchorMax = new Vector2(1, 0);
        ert.pivot = new Vector2(1, 0);
        ert.sizeDelta = new Vector2(200, 70);
        ert.anchoredPosition = new Vector3(-25, 25, 0);
        GameObject exitText = CreateTextInParent(exitBtnGO, "\u2630 MENU", 32, textLight, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        exitText.GetComponent<Text>().font = font;
        exitText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        eBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        });

        GameObject soundBtnGO = new GameObject("SoundBtn");
        soundBtnGO.transform.SetParent(canvas.transform, false);
        Image sImg = soundBtnGO.AddComponent<Image>();
        sImg.sprite = MakeRoundedRectSprite(64, 64);
        sImg.type = Image.Type.Sliced;
        sImg.color = btnNormal;
        Button sBtn = soundBtnGO.AddComponent<Button>();
        ColorBlock sColors = sBtn.colors;
        sColors.normalColor = btnNormal;
        sColors.highlightedColor = btnHighlight;
        sColors.pressedColor = btnPressed;
        sColors.fadeDuration = 0.15f;
        sBtn.colors = sColors;
        
        soundBtnGO.AddComponent<ButtonHoverAnimator>();

        RectTransform srt = soundBtnGO.GetComponent<RectTransform>();
        srt.anchorMin = new Vector2(0, 0);
        srt.anchorMax = new Vector2(0, 0);
        srt.pivot = new Vector2(0, 0);
        srt.sizeDelta = new Vector2(240, 70);
        srt.anchoredPosition = new Vector3(25, 25, 0);
        GameObject soundText = CreateTextInParent(soundBtnGO, AudioListener.volume > 0 ? "\u266B SOUND ON" : "\u266B SOUND OFF", 28, textLight, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        soundText.GetComponent<Text>().font = font;
        soundText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        sBtn.onClick.AddListener(() =>
        {
            AudioListener.volume = AudioListener.volume > 0 ? 0f : 1f;
            soundText.GetComponent<Text>().text = AudioListener.volume > 0 ? "\u266B SOUND ON" : "\u266B SOUND OFF";
        });

        QuizGameManager qgm = gameObject.AddComponent<QuizGameManager>();
        qgm.questionText = questionGO.GetComponent<Text>();
        qgm.answerButtons = buttons;
        qgm.answerTexts = btnTexts;
        qgm.scoreText = scoreText.GetComponent<Text>();
        qgm.comboText = comboText.GetComponent<Text>();
        qgm.questionCounterText = questionCounter.GetComponent<Text>();
        qgm.timerBar = timerBar;
        qgm.timerText = timerText.GetComponent<Text>();
        qgm.gameOverPanel = gameOverPanel;
        qgm.finalScoreText = finalScore.GetComponent<Text>();
        qgm.finalCorrectText = finalCorrect.GetComponent<Text>();
        qgm.restartBtn = rBtn;

        gameObject.AddComponent<QuestionGenerator>();

        GameObject prefab = Resources.Load<GameObject>("CharacterPS-01");
        if (prefab != null)
        {
            GameObject charInst = Instantiate(prefab);
            charInst.name = "Character";
            // Parent to the GameUI root (NOT the Canvas) so it gets destroyed on exit
            charInst.transform.SetParent(transform);
            
            charInst.transform.localScale = new Vector3(0.18f, 0.18f, 1);
            charInst.transform.position = new Vector3(-5.5f, 0.2f, 0);
            
            // Add a SortingGroup so the character renders as a cohesive unit above the background
            UnityEngine.Rendering.SortingGroup sg = charInst.AddComponent<UnityEngine.Rendering.SortingGroup>();
            sg.sortingOrder = 5;

            charInst.AddComponent<CharacterAnchor>();
            charInst.AddComponent<CharacterEmotion>();
        }
        else
        {
            GameObject charGO = new GameObject("Character");
            // Parent to the GameUI root
            charGO.transform.SetParent(transform);
            charGO.transform.localScale = new Vector3(3.5f, 3.5f, 1);
            SpriteRenderer sr = charGO.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 5;

            Texture2D charTex = Resources.Load<Texture2D>("ELMACHHOUNE");
            if (charTex == null) charTex = Resources.Load<Texture2D>("character");
            if (charTex != null)
                sr.sprite = Sprite.Create(charTex, new Rect(0, 0, charTex.width, charTex.height), new Vector2(0.5f, 0.5f), Mathf.Max(charTex.width, charTex.height));
            else
            {
                Texture2D tex = new Texture2D(24, 24);
                for (int x = 0; x < 24; x++)
                    for (int y = 0; y < 24; y++)
                    {
                        if (x < 2 || x > 21 || y < 2 || y > 21) tex.SetPixel(x, y, Color.black);
                        else if ((x == 8 || x == 15) && (y == 15 || y == 16)) tex.SetPixel(x, y, Color.white);
                        else tex.SetPixel(x, y, new Color(0, 0.7f, 1));
                    }
                tex.Apply();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, 24, 24), new Vector2(0.5f, 0.5f), 24);
            }

            charGO.transform.position = new Vector3(-5.5f, 0.2f, 0);
            charGO.AddComponent<CharacterAnchor>();
            charGO.AddComponent<CharacterEmotion>();
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            cam = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bgDark;
        cam.transform.position = new Vector3(0, 0, -10);

        CreateBackground(canvas);
    }

    void CreateBackground(Canvas canvas)
    {
        GameObject bgGO = new GameObject("InteractiveBackground");
        bgGO.transform.SetParent(transform);
        InteractiveBackground ib = bgGO.AddComponent<InteractiveBackground>();
        ib.canvas = canvas;
    }

    GameObject CreateText(Canvas canvas, string text, int fontSize, Color color, TextAnchor align, Vector2 anchorMin, Vector2 anchorMax, Vector3 pos, Vector2 size)
    {
        GameObject go = new GameObject("Text_" + text.Replace(" ", ""));
        go.transform.SetParent(canvas.transform, false);
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

    GameObject CreateTextInParent(GameObject parent, string text, int fontSize, Color color, TextAnchor align, Vector2 anchorMin, Vector2 anchorMax, Vector3 pos, Vector2 size)
    {
        GameObject go = new GameObject("Text_" + text.Replace(" ", ""));
        go.transform.SetParent(parent.transform, false);
        Text txt = go.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.alignment = align;
        txt.raycastTarget = false;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    Sprite MakeRoundedRectSprite(int w, int h)
    {
        int radius = 16;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                float dx = Mathf.Min(x, w - 1 - x);
                float dy = Mathf.Min(y, h - 1 - y);
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
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
    }
}
