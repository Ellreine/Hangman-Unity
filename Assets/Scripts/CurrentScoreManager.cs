using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class CurrentScoreManager : MonoBehaviour
{
    public Text currentScoreText;
    private PlayerData playerData;
    private string playerName;
    private string filePath;

    void Start()
    {
        playerName = PlayerPrefs.GetString("CurrentPlayerName", "Player1");
        filePath = Path.Combine(Application.dataPath, "Resources", "playerData.json");
        playerData = LoadPlayerData(playerName);

        if (playerData == null)
        {
            playerData = new PlayerData(playerName);
            SavePlayerData();
        }

        playerData.ResetCurrentScore(); // —брос текущего счета при загрузке
        UpdateCurrentScoreUI();
    }

    public void IncrementScore(int points)
    {
        playerData.currentScore += points;
        UpdateCurrentScoreUI();
    }

    public void ResetScore()
    {
        playerData.ResetCurrentScore();
        UpdateCurrentScoreUI();
    }

    void UpdateCurrentScoreUI()
    {
        currentScoreText.text = "Current Score: " + playerData.currentScore;
    }

    public int GetCurrentScore()
    {
        return playerData.currentScore;
    }

    public float GetCurrentTime()
    {
        return playerData.currentTime;
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
                    return player;
                }
            }
        }
        return null;
    }

    public void SavePlayerData()
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
}
