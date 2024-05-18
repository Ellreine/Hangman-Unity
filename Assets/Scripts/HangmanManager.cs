using UnityEngine;
using UnityEngine.UI;

public class HangmanManager : MonoBehaviour
{
    public GameObject[] hangmanParts; // Массив частей виселицы
    private int livesRemaining;
    public GameObject gameOverPanel; // Ссылка на панель GameOver
    public Button restartButton; // Ссылка на кнопку Restart

    private bool isGameOver;

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        InitializeGame();
    }

    void InitializeGame()
    {
        isGameOver = false;
        livesRemaining = hangmanParts.Length;
        HideAllParts();
        gameOverPanel.SetActive(false); // Скрыть панель GameOver
    }

    void HideAllParts()
    {
        foreach (GameObject part in hangmanParts)
        {
            part.SetActive(false);
        }
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        int currentPart = hangmanParts.Length - livesRemaining;
        if (livesRemaining > 0)
        {
            hangmanParts[currentPart].SetActive(true);
            livesRemaining--;
        }

        if (livesRemaining == 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true); // Показать панель GameOver
        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager != null)
        {
            wordManager.DisableAllLetters(); // Заблокировать нажатия на буквы
        }
    }

    void RestartGame()
    {
        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager != null)
        {
            wordManager.ResetAllLetters(); // Сбросить состояния всех букв
            wordManager.ResetLetterSprites(); // Сбросить спрайты букв
            wordManager.InitializeNewWord(); // Начать новую игру
        }
        InitializeGame();
    }

    public void ResetHangman()
    {
        isGameOver = false;
        livesRemaining = hangmanParts.Length;
        HideAllParts();
    }

    public bool HasLivesRemaining()
    {
        return livesRemaining > 0;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
