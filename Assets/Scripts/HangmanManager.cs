using UnityEngine;
using UnityEngine.UI;

public class HangmanManager : MonoBehaviour
{
    public GameObject[] hangmanParts; // ������ ������ ��������
    private int livesRemaining;
    public GameObject gameOverPanel; // ������ �� ������ GameOver
    public Button restartButton; // ������ �� ������ Restart

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
        gameOverPanel.SetActive(false); // ������ ������ GameOver
    }

    // ������ ��� ����� ��������
    void HideAllParts()
    {
        foreach (GameObject part in hangmanParts)
        {
            part.SetActive(false);
        }
    }

    // ������ �����
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
        gameOverPanel.SetActive(true); // �������� ������ GameOver
        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager != null)
        {
            wordManager.DisableAllLetters(); // ������������� ������� �� �����
        }
    }

    void RestartGame()
    {
        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager != null)
        {
            wordManager.ResetAllLetters(); // �������� ��������� ���� ����
            wordManager.ResetLetterSprites(); // �������� ������� ����
            wordManager.InitializeNewWord(); // ������ ����� ����
        }
        InitializeGame();
    }

    // ��������, ���� �� ���������� �����
    public bool HasLivesRemaining()
    {
        return livesRemaining > 0;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
