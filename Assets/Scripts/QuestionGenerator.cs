using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    public string question;
    public int[] answers;
    public int correctIndex;
    public int difficulty;
}

public class QuestionGenerator : MonoBehaviour
{
    public int currentDifficulty = 1;

    public QuizQuestion GenerateQuestion()
    {
        int a, b, correctAnswer;
        string op;
        int opType = Random.Range(0, 3);

        switch (opType)
        {
            case 0:
                a = Random.Range(1, 5 + currentDifficulty * 2);
                b = Random.Range(1, 5 + currentDifficulty);
                correctAnswer = a + b;
                op = "+";
                break;
            case 1:
                a = Random.Range(currentDifficulty * 2, 10 + currentDifficulty * 3);
                b = Random.Range(1, a);
                correctAnswer = a - b;
                op = "-";
                break;
            default:
                a = Random.Range(1, 3 + currentDifficulty);
                b = Random.Range(1, 3 + currentDifficulty);
                correctAnswer = a * b;
                op = "\u00d7";
                break;
        }

        string question = $"What is {a} {op} {b}?";

        int[] answers = new int[4];
        int correctPos = Random.Range(0, 4);
        answers[correctPos] = correctAnswer;

        for (int i = 0; i < 4; i++)
        {
            if (i == correctPos) continue;
            int wrong;
            do
            {
                wrong = correctAnswer + Random.Range(-5 - currentDifficulty, 6 + currentDifficulty);
            } while (wrong == correctAnswer || wrong < 0 || System.Array.IndexOf(answers, wrong) != -1);
            answers[i] = wrong;
        }

        return new QuizQuestion
        {
            question = question,
            answers = answers,
            correctIndex = correctPos,
            difficulty = currentDifficulty
        };
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
