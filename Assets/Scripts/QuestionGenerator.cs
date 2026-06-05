using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QuizQuestion
{
    public string question;
    public string[] answers;
    public int correctIndex;
    public int difficulty;
}

[System.Serializable]
public class QuizData
{
    public string quizName;
    public List<QuizQuestion> questions;
}

[System.Serializable]
public class QuizCategory
{
    public string categoryName;
    public List<QuizData> quizzes;
}

public class QuestionGenerator : MonoBehaviour
{
    public int currentDifficulty = 1;
    
    // The loaded category data
    public static List<QuizCategory> categories;
    public static string selectedQuizName;
    
    private List<QuizQuestion> currentQuizQuestions;
    private List<QuizQuestion> unusedQuestions;

    void Awake()
    {
        InitializeData();
    }

    public void InitializeData()
    {
        if (categories != null) return;
        
        categories = new List<QuizCategory>();

        // ---- CODE CATEGORY ----
        QuizCategory codeCat = new QuizCategory { categoryName = "Code", quizzes = new List<QuizData>() };

        // Git/GitHub Quiz
        QuizData gitQuiz = new QuizData { quizName = "Git/GitHub", questions = new List<QuizQuestion>() };
        gitQuiz.questions.Add(new QuizQuestion { question = "What command initializes a git repository?", answers = new string[] { "git init", "git start", "git make", "git create" }, correctIndex = 0, difficulty = 1 });
        gitQuiz.questions.Add(new QuizQuestion { question = "How do you check the state of the repository?", answers = new string[] { "git state", "git info", "git status", "git check" }, correctIndex = 2, difficulty = 1 });
        gitQuiz.questions.Add(new QuizQuestion { question = "Which command adds files to the staging area?", answers = new string[] { "git push", "git commit", "git add", "git stage" }, correctIndex = 2, difficulty = 1 });
        gitQuiz.questions.Add(new QuizQuestion { question = "How do you save changes to the local repository?", answers = new string[] { "git save", "git commit", "git push", "git record" }, correctIndex = 1, difficulty = 1 });
        gitQuiz.questions.Add(new QuizQuestion { question = "Which command is used to upload commits to a remote repo?", answers = new string[] { "git send", "git upload", "git push", "git commit" }, correctIndex = 2, difficulty = 2 });
        gitQuiz.questions.Add(new QuizQuestion { question = "How do you download changes from a remote repository?", answers = new string[] { "git pull", "git download", "git fetch", "git get" }, correctIndex = 0, difficulty = 2 });
        gitQuiz.questions.Add(new QuizQuestion { question = "What command creates a new branch?", answers = new string[] { "git branch <name>", "git create <name>", "git new <name>", "git make <name>" }, correctIndex = 0, difficulty = 2 });
        gitQuiz.questions.Add(new QuizQuestion { question = "How do you switch to an existing branch?", answers = new string[] { "git switch", "git checkout", "Both A and B", "git change" }, correctIndex = 2, difficulty = 2 });
        gitQuiz.questions.Add(new QuizQuestion { question = "What does git clone do?", answers = new string[] { "Copies a local file", "Creates a remote repo", "Copies a remote repo", "Deletes a repo" }, correctIndex = 2, difficulty = 1 });
        gitQuiz.questions.Add(new QuizQuestion { question = "Which command shows the commit history?", answers = new string[] { "git history", "git log", "git commits", "git show" }, correctIndex = 1, difficulty = 1 });
        codeCat.quizzes.Add(gitQuiz);

        // HTML Course Quiz
        QuizData htmlQuiz = new QuizData { quizName = "HTML Course", questions = new List<QuizQuestion>() };
        htmlQuiz.questions.Add(new QuizQuestion { question = "What does HTML stand for?", answers = new string[] { "Hyper Text Markup Language", "Home Tool Markup Language", "Hyperlinks and Text Markup Language", "Hyper Tool Markup Language" }, correctIndex = 0, difficulty = 1 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "Who is making the Web standards?", answers = new string[] { "Google", "Mozilla", "Microsoft", "The World Wide Web Consortium" }, correctIndex = 3, difficulty = 1 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "Choose the correct HTML element for the largest heading:", answers = new string[] { "<h1>", "<h6>", "<heading>", "<head>" }, correctIndex = 0, difficulty = 1 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "What is the correct HTML element for inserting a line break?", answers = new string[] { "<br>", "<break>", "<lb>", "<newline>" }, correctIndex = 0, difficulty = 1 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "What is the correct HTML for adding a background color?", answers = new string[] { "<body bg=\"yellow\">", "<background>yellow</background>", "<body style=\"background-color:yellow;\">", "<body color=\"yellow\">" }, correctIndex = 2, difficulty = 2 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "Choose the correct HTML element to define important text", answers = new string[] { "<important>", "<strong>", "<b>", "<i>" }, correctIndex = 1, difficulty = 2 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "Choose the correct HTML element to define emphasized text", answers = new string[] { "<i>", "<italic>", "<em>", "<emp>" }, correctIndex = 2, difficulty = 2 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "Which character is used to indicate an end tag?", answers = new string[] { "<", "/", "*", "^" }, correctIndex = 1, difficulty = 1 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "How can you make a numbered list?", answers = new string[] { "<ul>", "<dl>", "<ol>", "<list>" }, correctIndex = 2, difficulty = 2 });
        htmlQuiz.questions.Add(new QuizQuestion { question = "How can you make a bulleted list?", answers = new string[] { "<ol>", "<dl>", "<ul>", "<list>" }, correctIndex = 2, difficulty = 2 });
        codeCat.quizzes.Add(htmlQuiz);

        categories.Add(codeCat);

        // ---- DESIGN CATEGORY ----
        QuizCategory designCat = new QuizCategory { categoryName = "Design", quizzes = new List<QuizData>() };
        
        // Basic Design Quiz
        QuizData designQuiz = new QuizData { quizName = "Design Basics", questions = new List<QuizQuestion>() };
        designQuiz.questions.Add(new QuizQuestion { question = "What does UI stand for?", answers = new string[] { "User Integration", "User Interface", "Unified Interface", "User Interaction" }, correctIndex = 1, difficulty = 1 });
        designQuiz.questions.Add(new QuizQuestion { question = "What does UX stand for?", answers = new string[] { "User Experience", "User Execution", "Unified Experience", "User Expansion" }, correctIndex = 0, difficulty = 1 });
        designQuiz.questions.Add(new QuizQuestion { question = "Which color mode is used for screens?", answers = new string[] { "CMYK", "RGB", "Pantone", "Grayscale" }, correctIndex = 1, difficulty = 1 });
        designQuiz.questions.Add(new QuizQuestion { question = "Which color mode is best for printing?", answers = new string[] { "RGB", "HEX", "CMYK", "HSB" }, correctIndex = 2, difficulty = 1 });
        designQuiz.questions.Add(new QuizQuestion { question = "What is kerning?", answers = new string[] { "Space between lines", "Space between characters", "Font weight", "Font size" }, correctIndex = 1, difficulty = 2 });
        designQuiz.questions.Add(new QuizQuestion { question = "What is leading?", answers = new string[] { "Space between lines", "Space between characters", "Paragraph spacing", "Indent size" }, correctIndex = 0, difficulty = 2 });
        designQuiz.questions.Add(new QuizQuestion { question = "Which tool is standard for UI design?", answers = new string[] { "Figma", "Word", "Excel", "Notepad" }, correctIndex = 0, difficulty = 1 });
        designQuiz.questions.Add(new QuizQuestion { question = "What is a vector image?", answers = new string[] { "Made of pixels", "Made of paths/math", "A photograph", "A 3D model" }, correctIndex = 1, difficulty = 2 });
        designQuiz.questions.Add(new QuizQuestion { question = "What is a raster image?", answers = new string[] { "Made of pixels", "Made of paths", "Scalable without quality loss", "A text file" }, correctIndex = 0, difficulty = 2 });
        designQuiz.questions.Add(new QuizQuestion { question = "What does a wireframe do?", answers = new string[] { "Adds colors to UI", "Shows basic layout structure", "Writes the code", "Animates the UI" }, correctIndex = 1, difficulty = 1 });
        designCat.quizzes.Add(designQuiz);

        categories.Add(designCat);
    }

    public void SetupQuiz()
    {
        InitializeData();
        currentQuizQuestions = new List<QuizQuestion>();
        
        foreach (var cat in categories)
        {
            foreach (var q in cat.quizzes)
            {
                if (q.quizName == selectedQuizName)
                {
                    currentQuizQuestions = new List<QuizQuestion>(q.questions);
                    break;
                }
            }
        }
        
        // If not found or not selected, fallback to some default
        if (currentQuizQuestions.Count == 0 && categories.Count > 0 && categories[0].quizzes.Count > 0)
        {
            currentQuizQuestions = new List<QuizQuestion>(categories[0].quizzes[0].questions);
        }

        unusedQuestions = new List<QuizQuestion>(currentQuizQuestions);
    }

    public QuizQuestion GenerateQuestion()
    {
        if (unusedQuestions == null || unusedQuestions.Count == 0)
        {
            SetupQuiz(); // Restart or re-fetch
        }

        int index = Random.Range(0, unusedQuestions.Count);
        QuizQuestion q = unusedQuestions[index];
        unusedQuestions.RemoveAt(index);
        
        return q;
    }

    public void IncreaseDifficulty()
    {
        currentDifficulty = Mathf.Min(currentDifficulty + 1, 10);
    }

    public void ResetDifficulty()
    {
        currentDifficulty = 1;
    }
}