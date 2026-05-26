using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuizGameManager : MonoBehaviour
{
    public static QuizGameManager Instance { get; private set; }

    [Header("UI References")]
    public Text questionText;
    public Button[] answerButtons;
    public Text[] answerTexts;
    public Text scoreText;
    public Text comboText;
    public Text questionCounterText;
    public Image timerBar;
    public Text timerText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text finalCorrectText;
    public Button restartBtn;

    [Header("Game Settings")]
    public float timePerQuestion = 12f;
    public int questionsPerRound = 10;

    private QuestionGenerator qGen;
    private CharacterEmotion character;
    private InteractiveBackground bg;
    private QuizQuestion currentQuestion;
    private float timeRemaining;
    private int score;
    private int correctCount;
    private int questionCount;
    private int combo;
    private int maxCombo;
    private bool answering;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        qGen = GetComponent<QuestionGenerator>();
        gameOverPanel.SetActive(false);
        score = 0;
        combo = 0;
        maxCombo = 0;
        questionCount = 0;
        correctCount = 0;
        UpdateScoreUI();

        if (restartBtn != null)
            restartBtn.onClick.AddListener(RestartGame);

        Invoke("DelayedStart", 0.1f);
    }

    void DelayedStart()
    {
        character = GetComponent<CharacterEmotion>();
        if (character == null)
            character = FindObjectOfType<CharacterEmotion>();

        bg = FindObjectOfType<InteractiveBackground>();

        NextQuestion();
    }

    void Update()
    {
        if (answering)
        {
            timeRemaining -= Time.deltaTime;
            if (timerBar != null)
                timerBar.fillAmount = timeRemaining / timePerQuestion;
            if (timerText != null)
                timerText.text = Mathf.Ceil(timeRemaining).ToString();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                OnTimeUp();
            }
        }
    }

    void NextQuestion()
    {
        if (questionCount >= questionsPerRound)
        {
            EndGame();
            return;
        }

        answering = true;
        questionCount++;
        currentQuestion = qGen.GenerateQuestion();

        if (questionText != null)
            questionText.text = currentQuestion.question;

        for (int i = 0; i < answerButtons.Length && i < currentQuestion.answers.Length; i++)
        {
            int index = i;
            answerTexts[i].text = currentQuestion.answers[i].ToString();
            answerButtons[i].interactable = true;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswer(index));
        }

        timeRemaining = timePerQuestion;
        if (timerBar != null)
            timerBar.fillAmount = 1;

        if (questionCounterText != null)
            questionCounterText.text = $"{questionCount}/{questionsPerRound}";

        if (character != null)
            character.SetThinking();
    }

    private Color[] btnOriginalColors;

    void OnAnswer(int index)
    {
        if (!answering) return;
        answering = false;

        btnOriginalColors = new Color[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            btnOriginalColors[i] = answerButtons[i].GetComponent<Image>().color;
            answerButtons[i].interactable = false;
        }

        bool correct = index == currentQuestion.correctIndex;

        if (correct)
        {
            combo++;
            if (combo > maxCombo) maxCombo = combo;
            int bonus = Mathf.FloorToInt(combo / 3f);
            int points = 10 + currentQuestion.difficulty * 5 + bonus * 5;
            score += points;
            correctCount++;

            if (character != null)
                character.SetHappy(() => NextQuestion());
            if (bg != null)
            {
                bg.Flash(new Color(0.2f, 0.8f, 0.3f), 0.4f);
                bg.SpawnUIBurst(Input.mousePosition);
            }

            StartCoroutine(DelayedHighlight(index, new Color(0.3f, 1f, 0.3f)));
        }
        else
        {
            combo = 0;
            if (character != null)
                character.SetAngry(() => NextQuestion());
            if (bg != null)
            {
                bg.Flash(new Color(0.8f, 0.2f, 0.2f), 0.4f);
                bg.SpawnUIBurst(Input.mousePosition);
            }

            StartCoroutine(DelayedHighlight(currentQuestion.correctIndex, new Color(0.3f, 1f, 0.3f)));
            StartCoroutine(DelayedHighlight(index, new Color(1f, 0.3f, 0.3f)));
        }

        UpdateScoreUI();
    }

    IEnumerator DelayedHighlight(int index, Color color)
    {
        yield return new WaitForSeconds(0.15f);
        Image img = answerButtons[index].GetComponent<Image>();
        if (img != null)
        {
            img.color = color;
            StartCoroutine(FadeColor(img, color, btnOriginalColors[index], 0.4f));
        }
    }

    void OnTimeUp()
    {
        answering = false;
        combo = 0;

        btnOriginalColors = new Color[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            btnOriginalColors[i] = answerButtons[i].GetComponent<Image>().color;
            answerButtons[i].interactable = false;
        }

        StartCoroutine(DelayedHighlight(currentQuestion.correctIndex, new Color(0.3f, 1f, 0.3f)));

        if (character != null)
            character.SetAngry(() => NextQuestion());
        if (bg != null)
        {
            bg.Flash(new Color(0.8f, 0.2f, 0.2f), 0.4f);
            bg.SpawnUIBurst(Input.mousePosition);
        }

        UpdateScoreUI();
    }

    IEnumerator FadeColor(Image img, Color from, Color to, float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            img.color = Color.Lerp(from, to, t);
            yield return null;
        }
        img.color = to;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();

        if (comboText != null)
        {
            if (combo >= 3)
            {
                comboText.text = $"COMBO x{combo}";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    void EndGame()
    {
        answering = false;
        gameOverPanel.SetActive(true);

        float accuracy = questionCount > 0 ? (float)correctCount / questionCount * 100f : 0;

        if (finalScoreText != null)
            finalScoreText.text = score.ToString();

        if (finalCorrectText != null)
            finalCorrectText.text = $"{correctCount}/{questionCount} correct  ({accuracy:F0}%)";

        if (character != null)
            character.SetCelebrating();
        if (bg != null)
            bg.Flash(new Color(1f, 0.8f, 0.2f), 1f);
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        qGen.ResetDifficulty();
        score = 0;
        combo = 0;
        maxCombo = 0;
        questionCount = 0;
        correctCount = 0;
        UpdateScoreUI();
        NextQuestion();
    }

    public void OnCorrectAnswer()
    {
        qGen.IncreaseDifficulty();
    }
}
