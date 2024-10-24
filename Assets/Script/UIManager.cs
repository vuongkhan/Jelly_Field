using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public LevelManager levelManager;
    public TextMeshProUGUI conditionText;
    public TextMeshProUGUI scoreText;
    public GameObject nextLevelPanel;
    private GameManager gameManager;
    public GameObject gameOverPanel;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateUI();
        nextLevelPanel.SetActive(false);
    }

    public void UpdateUI()
    {
        conditionText.text = GetConditionsText();
        scoreText.text = GetScoreText();
        CheckConditionsAndShowNextLevel();
    }

    private string GetConditionsText()
    {
        string conditions = "Conditions:\n";
        foreach (var condition in levelManager.levelConditions)
        {
            conditions += $"{condition.color}: {condition.requiredAmount}\n";
        }
        return conditions;
    }

    private string GetScoreText()
    {
        string scoreInfo = "Current Score:\n";
        foreach (var condition in levelManager.levelConditions)
        {
            if (condition.color == JellyColor.Red) continue;
            int score = gameManager.GetScoreForColor(condition.color);
            scoreInfo += $"{condition.color}: {score}\n";
        }
        return scoreInfo;
    }

    private void CheckConditionsAndShowNextLevel()
    {
        foreach (var condition in levelManager.levelConditions)
        {
            if (condition.color == JellyColor.Red) continue;
            int score = gameManager.GetScoreForColor(condition.color);
            if (score < condition.requiredAmount)
            {
                nextLevelPanel.SetActive(false);
                return;
            }
        }
        nextLevelPanel.SetActive(true);
    }

    public void LoadNextLevel(string nextLevelName)
    {
        SceneManager.LoadScene(nextLevelName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
