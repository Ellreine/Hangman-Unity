using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class HangmanManager : MonoBehaviour
{
    public CurrentScoreManager currentScoreManager; // Ссылка на менеджер текущего счета
    public BestScoreManager bestScoreManager; // Ссылка на менеджер таблицы лидеров
    public GameObject[] hangmanParts; // Массив частей виселицы
    public GameObject gameOverPanel; // Ссылка на панель GameOver
    public Button restartButton; // Ссылка на кнопку Restart
    public Text timerText; // Ссылка на текст таймера

    private int livesRemaining;
    private bool isGameOver;
    private PlayerData playerData;
    private string playerName;
    private string filePath;
    private float gameTime; // Время игры
    private bool isTimerRunning;

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        playerName = PlayerPrefs.GetString("CurrentPlayerName", "Player1");
        filePath = Path.Combine(Application.dataPath, "Resources", "playerData.json");
        playerData = LoadPlayerData(playerName);

        if (playerData == null)
        {
            playerData = new PlayerData(playerName);
            SavePlayerData();
        }

        InitializeGame();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            gameTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void InitializeGame()
    {
        isGameOver = false;
        livesRemaining = hangmanParts.Length;
        HideAllParts();
        gameOverPanel.SetActive(false);
        currentScoreManager.ResetScore(); // Сброс текущего счета
        bestScoreManager.LoadBestScores(); // Обновить таблицу лидеров
        gameTime = 0f; // Сброс времени игры
        isTimerRunning = true; // Запуск таймера
    }

    void HideAllParts()
    {
        foreach (GameObject part in hangmanParts)
        {
            part.SetActive(false);
        }
    }

    void UpdateTimerUI()
    {
        timerText.text = "Time: " + gameTime.ToString("F2") + "s";
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

    public void GameOver()
    {
        isGameOver = true;
        isTimerRunning = false; // Остановка таймера
        playerData.currentTime = gameTime; // Установка текущего времени
        gameOverPanel.SetActive(true);
        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager != null)
        {
            wordManager.DisableAllLetters(); // Заблокировать нажатия на буквы
        }

        // Проверяем и обновляем данные игрока
        UpdatePlayerDataIfNeeded();
    }

    public void RestartGame()
    {
        currentScoreManager.ResetScore();

        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager != null)
        {
            wordManager.ResetAllLetters(); // Сбросить состояния всех букв
            wordManager.ResetLetterSprites(); // Сбросить спрайты букв
            wordManager.InitializeNewWord(); // Начать новую игру
        }

        InitializeGame();
    }

    private void UpdatePlayerDataIfNeeded()
    {
        int currentScore = currentScoreManager.GetCurrentScore();
        float currentTime = playerData.currentTime;
        if (currentScore > playerData.bestScore || (currentScore == playerData.bestScore && currentTime < playerData.bestTime))
        {
            playerData.bestScore = currentScore;
            playerData.bestTime = currentTime;
            SavePlayerData(); // Обновляем данные в JSON файле
            Debug.Log("Player data updated in JSON file.");
        }
        else
        {
            Debug.Log("Current score is not higher than best score. No update needed.");
        }
    }

    PlayerData LoadPlayerData(string playerName)
    {
        PlayerDataList playerDataList = LoadPlayerDataList();
        if (playerDataList != null)
        {
            foreach (var player in playerDataList.players)
            {
                if (player.playerName == playerName)
                {
                    player.ResetCurrentScore(); // Сброс текущего счета при загрузке
                    return player;
                }
            }
        }
        return null;
    }

    void SavePlayerData()
    {
        List<PlayerData> allPlayers = new List<PlayerData>();
        PlayerDataList playerDataList = LoadPlayerDataList();
        if (playerDataList != null)
        {
            allPlayers = new List<PlayerData>(playerDataList.players);
        }

        bool playerFound = false;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].playerName == playerData.playerName)
            {
                allPlayers[i] = playerData;
                playerFound = true;
                break;
            }
        }

        if (!playerFound)
        {
            allPlayers.Add(playerData);
        }

        PlayerDataList updatedPlayerDataList = new PlayerDataList { players = allPlayers.ToArray() };
        string updatedJson = JsonUtility.ToJson(updatedPlayerDataList);
        File.WriteAllText(filePath, updatedJson);
    }

    PlayerDataList LoadPlayerDataList()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<PlayerDataList>(json);
        }
        else
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("playerData");
            if (jsonFile != null)
            {
                return JsonUtility.FromJson<PlayerDataList>(jsonFile.text);
            }
        }
        return null;
    }

    public bool HasLivesRemaining()
    {
        return livesRemaining > 0;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void ResetHangman()
    {
        isGameOver = false;
        livesRemaining = hangmanParts.Length;
        HideAllParts();
    }

    // Добавляем метод SaveGameState
    public void SaveGameState()
    {
        SavePlayerData();
        Debug.Log("Game state saved.");
    }
}
