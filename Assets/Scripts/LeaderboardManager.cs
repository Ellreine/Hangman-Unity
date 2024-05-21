using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject playerScorePrefab;
    public Transform leaderboardContent;
    public Text currentScoreText;
    private string filePath;

    private List<PlayerScore> playerScores = new List<PlayerScore>();

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "playerData.json");
        LoadPlayerScores();
        UpdateLeaderboard();
        UpdateCurrentScore();
    }

    void LoadPlayerScores()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
            if (playerDataList != null)
            {
                foreach (var playerData in playerDataList.players)
                {
                    playerScores.Add(new PlayerScore(playerData.playerName, playerData.bestScore, playerData.bestTime));
                }
            }
        }
        else
        {
            Debug.LogError("Файл с данными игроков не найден: " + filePath);
        }
    }

    public void AddPlayerScore(string playerName, int score, float time)
    {
        var existingPlayer = playerScores.FirstOrDefault(p => p.playerName == playerName);
        if (existingPlayer != null)
        {
            if (score > existingPlayer.score)
            {
                existingPlayer.score = score;
                existingPlayer.time = time;
            }
        }
        else
        {
            playerScores.Add(new PlayerScore(playerName, score, time));
        }
        UpdateLeaderboard();
        UpdateCurrentScore();
    }

    public void UpdateLeaderboard()
    {
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        var sortedScores = playerScores.OrderByDescending(p => p.score).ThenBy(p => p.time).ToList();

        for (int i = 0; i < Mathf.Min(3, sortedScores.Count); i++)
        {
            AddPlayerScoreToUI(sortedScores[i]);
        }
    }

    void AddPlayerScoreToUI(PlayerScore playerScore)
    {
        GameObject playerScoreObj = Instantiate(playerScorePrefab, leaderboardContent);

        Text playerNameText = playerScoreObj.transform.Find("PlayerName").GetComponent<Text>();
        Text scoreText = playerScoreObj.transform.Find("Score").GetComponent<Text>();
        Text timeText = playerScoreObj.transform.Find("Time").GetComponent<Text>();

        if (playerNameText == null || scoreText == null || timeText == null)
        {
            Debug.LogError("Один из компонентов Text не найден на префабе PlayerScorePrefab.");
            return;
        }

        playerNameText.text = playerScore.playerName;
        scoreText.text = playerScore.score.ToString();
        timeText.text = playerScore.time.ToString("F2") + "s";
    }

    public void UpdateCurrentPlayerScore(string playerName, int currentScore)
    {
        var player = playerScores.FirstOrDefault(p => p.playerName == playerName);
        if (player != null)
        {
            player.score = currentScore;
        }
        else
        {
            playerScores.Add(new PlayerScore(playerName, currentScore, 0f));
        }
        UpdateLeaderboard();
        UpdateCurrentScore();
    }

    void UpdateCurrentScore()
    {
        string currentPlayerName = PlayerPrefs.GetString("CurrentPlayerName");
        var currentPlayerScore = playerScores.FirstOrDefault(p => p.playerName == currentPlayerName);

        if (currentPlayerScore != null)
        {
            currentScoreText.text = "Current Score: " + currentPlayerScore.score;
        }
    }

    public void UpdateBestScore(string playerName, int bestScore, float bestTime)
    {
        var player = playerScores.FirstOrDefault(p => p.playerName == playerName);
        if (player != null)
        {
            player.score = bestScore;
            player.time = bestTime;
        }
        else
        {
            playerScores.Add(new PlayerScore(playerName, bestScore, bestTime));
        }
        UpdateLeaderboard();
    }
}
