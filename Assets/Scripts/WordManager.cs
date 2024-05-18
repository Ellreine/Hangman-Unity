using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class WordManager : MonoBehaviour
{
    public GameObject letterPrefab;  // ������ ����� ��� �����
    public GameObject letterGridPrefab;  // ������ ����� ��� �����
    public GameObject victoryPanel; // ������ ������
    public GameObject gameOverPanel; // ������ ���������
    public Button nextWordButton; // ������ "��������� �����"
    public string[] wordList = { "����", "���", "����", "���", "������" };  // ������ ����
    public float spacing = 1f;  // ���������� ����� �������
    public Sprite[] letterSprites;  // ������ �������� ����
    public HangmanManager hangmanManager; // ������ �� HangmanManager
    public LeaderboardManager leaderboardManager; // ������ �� LeaderboardManager

    private Dictionary<char, Sprite> letterSpriteDict;  // ������� �������� ����
    private HashSet<char> guessedLetters;  // ��������� ��������� ����
    private List<GameObject> letterObjects = new List<GameObject>();  // ������ �������� ����
    private string currentWord;  // ������� ���������� �����
    private PlayerData playerData;
    private string filePath;
    private float startTime;

    void Start()
    {
        guessedLetters = new HashSet<char>();
        InitializeLetterSprites();
        nextWordButton.onClick.AddListener(NextWord);
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        string playerName = PlayerPrefs.GetString("CurrentPlayerName", "Player1");
        filePath = Path.Combine(Application.dataPath, "Resources/playerData.json");
        playerData = LoadPlayerData(playerName);

        if (playerData == null)
        {
            playerData = new PlayerData(playerName);
        }

        leaderboardManager = FindObjectOfType<LeaderboardManager>();
        if (leaderboardManager == null)
        {
            Debug.LogError("LeaderboardManager �� ������ � �����.");
        }

        leaderboardManager.UpdateLeaderboard();
        startTime = Time.time;
        InitializeNewWord();
    }


    void InitializeLetterSprites()
    {
        letterSpriteDict = new Dictionary<char, Sprite>();

        string letters = "�����Ũ��������������������������";
        for (int i = 0; i < letters.Length; i++)
        {
            if (i < letterSprites.Length)
            {
                letterSpriteDict[letters[i]] = letterSprites[i];
            }
            else
            {
                Debug.LogError("Not enough sprites for all letters.");
            }
        }
    }

    public void InitializeNewWord()
    {
        ClearPreviousWord();
        currentWord = wordList[Random.Range(0, wordList.Length)];  // �������� ��������� �����
        InitializeWord();
    }

    void ClearPreviousWord()
    {
        foreach (GameObject letterObject in letterObjects)
        {
            Destroy(letterObject);
        }
        letterObjects.Clear();
        guessedLetters.Clear();
    }

    void InitializeWord()
    {
        Vector3 nextPosition = transform.position;  // ��������� ������� ��� ������ �����

        foreach (char letter in currentWord)
        {
            GameObject letterObj = Instantiate(letterPrefab, nextPosition, Quaternion.identity, transform);
            WordLetter wordLetter = letterObj.GetComponent<WordLetter>();
            wordLetter.Initialize(letter, letterSpriteDict);
            nextPosition.x += spacing;  // ����������� ������� ��� ��������� �����
            letterObjects.Add(letterObj);
            Debug.Log("Added letter object: " + letterObj.name); // ��������� ���������� ���������
        }
    }

    public bool CheckLetter(string letter)
    {
        char upperLetter = char.ToUpper(letter[0]); // ���������� � �������� ��������
        bool letterFound = false;

        foreach (Transform child in transform)
        {
            WordLetter wordLetter = child.GetComponent<WordLetter>();
            if (wordLetter.letter == upperLetter)
            {
                wordLetter.Reveal();
                letterFound = true;
                guessedLetters.Add(upperLetter);
            }
        }

        if (letterFound)
        {
            Debug.Log("Letter guessed: " + upperLetter);
            CheckWinCondition();
        }
        else
        {
            Debug.Log("Letter not guessed: " + upperLetter);
            hangmanManager.LoseLife();

            if (!hangmanManager.HasLivesRemaining())
            {
                Debug.Log("Game Over!");
                OnGameOver();
            }
        }

        return letterFound;
    }

    void CheckWinCondition()
    {
        foreach (char letter in currentWord)
        {
            if (!guessedLetters.Contains(char.ToUpper(letter)))
            {
                return; // ���� ���� ����������� �����, ����� �� ������
            }
        }

        Debug.Log("Victory!");
        playerData.currentScore++; // ����������� ������� ���� ��� ��������� �����
        ShowVictoryPanel();

        // �������� ������� ���� ������ (�� �� bestScore) � ����������
        leaderboardManager.UpdateCurrentPlayerScore(playerData.playerName, playerData.currentScore);
    }



    void ShowVictoryPanel()
    {
        victoryPanel.SetActive(true);
        DisableAllLetters();
    }

    public void NextWord()
    {
        InitializeNewWord();
        ResetGridLetters();
        hangmanManager.ResetHangman(); // ����� �������� � ������

        // ��������� ������� ���� ������, �� �� ��������� bestScore
        leaderboardManager.UpdateCurrentPlayerScore(playerData.playerName, playerData.currentScore);

        victoryPanel.SetActive(false);
    }


    public void DisableAllLetters()
    {
        foreach (GameObject letterObject in letterObjects)
        {
            LetterClickHandler clickHandler = letterObject.GetComponent<LetterClickHandler>();
            if (clickHandler != null)
            {
                clickHandler.enabled = false;  // ��������� ���������� ������
            }
        }

        // ���������� ���� � �����
        LetterClickHandler[] gridLetters = FindObjectsOfType<LetterClickHandler>();
        foreach (LetterClickHandler clickHandler in gridLetters)
        {
            clickHandler.enabled = false;
        }
    }

    public void ResetAllLetters()
    {
        Debug.Log("Resetting all letters.");
        foreach (GameObject letterObject in letterObjects)
        {
            LetterFeedback feedback = letterObject.GetComponent<LetterFeedback>();
            if (feedback != null)
            {
                Debug.Log("Resetting feedback for letter: " + letterObject.name);
                feedback.ResetFeedback(); // �������� ��������� �����
            }
            else
            {
                Debug.LogError("LetterFeedback component not found on: " + letterObject.name);
            }

            LetterClickHandler clickHandler = letterObject.GetComponent<LetterClickHandler>();
            if (clickHandler != null)
            {
                clickHandler.enabled = true;  // �������� ���������� ������
                Debug.Log("Enabling click handler for letter: " + letterObject.name);
            }
            else
            {
                Debug.LogError("LetterClickHandler component not found on: " + letterObject.name);
            }
        }

        // ���������� ���� � �����
        ResetGridLetters();
    }

    void ResetGridLetters()
    {
        LetterClickHandler[] gridLetters = FindObjectsOfType<LetterClickHandler>();
        foreach (LetterClickHandler clickHandler in gridLetters)
        {
            clickHandler.enabled = true;
            LetterFeedback feedback = clickHandler.GetComponent<LetterFeedback>();
            if (feedback != null)
            {
                feedback.ResetFeedback();
                Debug.Log("Resetting grid letter: " + clickHandler.gameObject.name);
            }
        }
    }

    public void ResetLetterSprites()
    {
        foreach (GameObject letterObject in letterObjects)
        {
            SpriteRenderer spriteRenderer = letterObject.GetComponent<SpriteRenderer>();
            WordLetter wordLetter = letterObject.GetComponent<WordLetter>();
            if (spriteRenderer != null && wordLetter != null)
            {
                spriteRenderer.sprite = wordLetter.baseSprite; // ������������� ������ �����
            }
        }
    }

    void OnGameOver()
    {
        if (playerData == null)
        {
            Debug.LogError("playerData �� ���������������.");
            return;
        }

        if (leaderboardManager == null)
        {
            Debug.LogError("leaderboardManager �� ���������������.");
            return;
        }

        playerData.currentTime = Time.time - startTime;

        bool isNewBest = false;
        // ��������� bestScore � bestTime ������ ���� ������� ���� � ����� �����
        if (playerData.currentScore > playerData.bestScore ||
            (playerData.currentScore == playerData.bestScore && playerData.currentTime < playerData.bestTime))
        {
            playerData.bestScore = playerData.currentScore;
            playerData.bestTime = playerData.currentTime;
            isNewBest = true;
        }

        SavePlayerData(playerData);

        if (isNewBest)
        {
            leaderboardManager.UpdateBestScore(playerData.playerName, playerData.bestScore, playerData.bestTime);
        }
        else
        {
            leaderboardManager.UpdateLeaderboard();
        }

        // ����� �������� ����� � �������
        playerData.currentScore = 0;
        playerData.currentTime = 0;
        startTime = Time.time;
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
