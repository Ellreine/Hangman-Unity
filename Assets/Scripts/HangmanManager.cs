using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class HangmanManager : MonoBehaviour
{
    public CurrentScoreManager currentScoreManager;
    public BestScoreManager bestScoreManager;
    public GameObject[] hangmanParts;
    public GameObject gameOverPanel;
    public Button restartButton;
    public Text timerText;

    private int livesRemaining;
    private bool isGameOver;
    private PlayerData playerData;
    private string playerName;
    private string filePath;
    private float gameTime;
    private bool isTimerRunning;

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        playerName = PlayerPrefs.GetString("CurrentPlayerName", "Player1");
        filePath = Path.Combine(Application.dataPath, "playerData.json");
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
        currentScoreManager.ResetScore();
        bestScoreManager.LoadBestScores();
        gameTime = 0f;
        isTimerRunning = true;
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
        isTimerRunning = false;
        playerData.currentTime = gameTime;
        gameOverPanel.SetActive(true);
        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager != null)
        {
            wordManager.DisableAllLetters();
        }

        UpdatePlayerDataIfNeeded();
    }

    public void RestartGame()
    {
        currentScoreManager.ResetScore();

        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager != null)
        {
            wordManager.ResetAllLetters();
            wordManager.ResetLetterSprites();
            wordManager.InitializeNewWord();
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
            SavePlayerData();
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
                    player.ResetCurrentScore();
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

    public void SaveGameState()
    {
        SavePlayerData();
        Debug.Log("Game state saved.");
    }
}
