using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public InputField nameInputField;
    public Button playButton;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "Resources/playerData.json");
        Debug.Log("Player data file path: " + filePath); // Вывод пути к файлу в консоль
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    void OnPlayButtonClicked()
    {
        string playerName = nameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            PlayerData playerData = LoadPlayerData(playerName);
            if (playerData == null)
            {
                playerData = new PlayerData(playerName);
                SavePlayerData(playerData);
            }
            PlayerPrefs.SetString("CurrentPlayerName", playerName);
            SceneManager.LoadScene("GameScene"); // Замените "GameScene" на название вашей основной сцены
        }
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

    PlayerData LoadPlayerData(string playerName)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
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
        }
        return null;
    }
}
