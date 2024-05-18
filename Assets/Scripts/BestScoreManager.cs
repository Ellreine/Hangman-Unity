using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class BestScoreManager : MonoBehaviour
{
    public GameObject playerScorePrefab;
    public Transform leaderboardContent;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "Resources", "playerData.json");
        Debug.Log("Player data file path: " + filePath); // Вывод пути к файлу в консоль для проверки
        LoadBestScores();
    }

    public void LoadBestScores()
    {
        // Очистить текущий список
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        PlayerDataList playerDataList = LoadPlayerDataList();
        if (playerDataList != null)
        {
            // Сортируем игроков по лучшему счету в порядке убывания и берем топ-3
            var topPlayers = playerDataList.players
                .OrderByDescending(p => p.bestScore)
                .ThenBy(p => p.bestTime)
                .Take(3);

            foreach (var playerData in topPlayers)
            {
                AddPlayerScoreToUI(playerData.playerName, playerData.bestScore, playerData.bestTime);
            }
        }
    }

    void AddPlayerScoreToUI(string playerName, int score, float time)
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

        playerNameText.text = playerName;
        scoreText.text = score.ToString();
        timeText.text = time.ToString("F2") + "s";
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
}
