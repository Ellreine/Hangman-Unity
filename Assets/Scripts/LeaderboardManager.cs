using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject playerScorePrefab; // ������ ��� ����������� ����� ������
    public Transform leaderboardContent; // �������, ���� ����� ����������� ���� �������
    private string filePath;

    private List<PlayerScore> playerScores = new List<PlayerScore>();

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "Resources/playerData.json");
        LoadPlayerScores();
        UpdateLeaderboard(); // �������� ������� ������� ��� ������
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
            Debug.LogError("���� � ������� ������� �� ������: " + filePath);
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
    }

    public void UpdateLeaderboard()
    {
        // �������� ������� ������
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // ����������� ������ �� ����� � �������
        var sortedScores = playerScores.OrderByDescending(p => p.score).ThenBy(p => p.time).ToList();

        // �������� ������� �������
        for (int i = 0; i < Mathf.Min(3, sortedScores.Count); i++)
        {
            AddPlayerScoreToUI(sortedScores[i]);
        }

        // ���� ������� ����� �� � ���-3, �������� ��� � ������
        string currentPlayerName = PlayerPrefs.GetString("CurrentPlayerName");
        var currentPlayerScore = playerScores.FirstOrDefault(p => p.playerName == currentPlayerName);
        if (currentPlayerScore != null && !sortedScores.Take(3).Contains(currentPlayerScore))
        {
            AddPlayerScoreToUI(currentPlayerScore, true);
        }
    }

    void AddPlayerScoreToUI(PlayerScore playerScore, bool isCurrentPlayer = false)
    {
        GameObject playerScoreObj = Instantiate(playerScorePrefab, leaderboardContent);

        // �������� � ������������� �����������
        Text playerNameText = playerScoreObj.transform.Find("PlayerName").GetComponent<Text>();
        Text scoreText = playerScoreObj.transform.Find("Score").GetComponent<Text>();
        Text timeText = playerScoreObj.transform.Find("Time").GetComponent<Text>();

        if (playerNameText == null || scoreText == null || timeText == null)
        {
            Debug.LogError("���� �� ����������� Text �� ������ �� ������� PlayerScorePrefab.");
            return;
        }

        playerNameText.text = playerScore.playerName;
        scoreText.text = playerScore.score.ToString();
        timeText.text = playerScore.time.ToString("F2") + "s";

        if (isCurrentPlayer)
        {
            Image playerScoreImage = playerScoreObj.GetComponent<Image>();
            if (playerScoreImage != null)
            {
                playerScoreImage.color = Color.yellow; // �������� ���� ��� �������� ������
            }
            else
            {
                Debug.LogError("��������� Image �� ������ �� ������� PlayerScorePrefab.");
            }
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
        SavePlayerScores();
        UpdateLeaderboard();
    }

    public void UpdateCurrentPlayerScore(string playerName, int currentScore)
    {
        var player = playerScores.FirstOrDefault(p => p.playerName == playerName);
        if (player != null)
        {
            // ��������� ������ ������� ����, �� ������� ������ ����
            player.score = currentScore;
        }
        else
        {
            playerScores.Add(new PlayerScore(playerName, currentScore, 0f));
        }
        UpdateLeaderboard();
    }

    void SavePlayerScores()
    {
        PlayerDataList playerDataList = new PlayerDataList();
        playerDataList.players = playerScores.Select(ps => new PlayerData(ps.playerName)
        {
            bestScore = ps.score,
            bestTime = ps.time
        }).ToArray();

        string json = JsonUtility.ToJson(playerDataList);
        File.WriteAllText(filePath, json);
    }

    void SavePlayerData(PlayerData playerData)
    {
        List<PlayerData> allPlayers = new List<PlayerData>();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
            if (playerDataList != null)
            {
                allPlayers = new List<PlayerData>(playerDataList.players);
            }
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
}
