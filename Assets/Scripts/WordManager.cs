using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class WordManager : MonoBehaviour
{
    public GameObject letterPrefab;  // Префаб буквы для слова
    public GameObject letterGridPrefab;  // Префаб буквы для сетки
    public GameObject victoryPanel; // Панель победы
    public GameObject gameOverPanel; // Панель проигрыша
    public Button nextWordButton; // Кнопка "следующее слово"
    public string[] wordList = { "Борщ", "Суп", "Каша", "Кот", "Собака" };  // Массив слов
    public float spacing = 1f;  // Расстояние между буквами
    public Sprite[] letterSprites;  // Массив спрайтов букв
    public HangmanManager hangmanManager; // Ссылка на HangmanManager
    public LeaderboardManager leaderboardManager; // Ссылка на LeaderboardManager

    private Dictionary<char, Sprite> letterSpriteDict;  // Словарь спрайтов букв
    private HashSet<char> guessedLetters;  // Множество угаданных букв
    private List<GameObject> letterObjects = new List<GameObject>();  // Список объектов букв
    private string currentWord;  // Текущее загаданное слово
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
            Debug.LogError("LeaderboardManager не найден в сцене.");
        }

        leaderboardManager.UpdateLeaderboard();
        startTime = Time.time;
        InitializeNewWord();
    }


    void InitializeLetterSprites()
    {
        letterSpriteDict = new Dictionary<char, Sprite>();

        string letters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
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
        currentWord = wordList[Random.Range(0, wordList.Length)];  // Выбираем случайное слово
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
        Vector3 nextPosition = transform.position;  // Начальная позиция для первой буквы

        foreach (char letter in currentWord)
        {
            GameObject letterObj = Instantiate(letterPrefab, nextPosition, Quaternion.identity, transform);
            WordLetter wordLetter = letterObj.GetComponent<WordLetter>();
            wordLetter.Initialize(letter, letterSpriteDict);
            nextPosition.x += spacing;  // Увеличиваем позицию для следующей буквы
            letterObjects.Add(letterObj);
            Debug.Log("Added letter object: " + letterObj.name); // Добавляем отладочное сообщение
        }
    }

    public bool CheckLetter(string letter)
    {
        char upperLetter = char.ToUpper(letter[0]); // Приведение к верхнему регистру
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
                return; // Если есть неугаданные буквы, выход из метода
            }
        }

        Debug.Log("Victory!");
        playerData.currentScore++; // Увеличиваем текущий счет при угаданном слове
        ShowVictoryPanel();

        // Обновить текущий счет игрока (но не bestScore) в лидерборде
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
        hangmanManager.ResetHangman(); // Сброс виселицы и жизней

        // Обновляем текущий счет игрока, но не обновляем bestScore
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
                clickHandler.enabled = false;  // Отключаем обработчик кликов
            }
        }

        // Отключение букв в сетке
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
                feedback.ResetFeedback(); // Сбросить состояние меток
            }
            else
            {
                Debug.LogError("LetterFeedback component not found on: " + letterObject.name);
            }

            LetterClickHandler clickHandler = letterObject.GetComponent<LetterClickHandler>();
            if (clickHandler != null)
            {
                clickHandler.enabled = true;  // Включаем обработчик кликов
                Debug.Log("Enabling click handler for letter: " + letterObject.name);
            }
            else
            {
                Debug.LogError("LetterClickHandler component not found on: " + letterObject.name);
            }
        }

        // Обновление букв в сетке
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
                spriteRenderer.sprite = wordLetter.baseSprite; // Устанавливаем спрайт линии
            }
        }
    }

    void OnGameOver()
    {
        if (playerData == null)
        {
            Debug.LogError("playerData не инициализирован.");
            return;
        }

        if (leaderboardManager == null)
        {
            Debug.LogError("leaderboardManager не инициализирован.");
            return;
        }

        playerData.currentTime = Time.time - startTime;

        bool isNewBest = false;
        // Обновляем bestScore и bestTime только если текущий счет и время лучше
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

        // Сброс текущего счета и времени
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
