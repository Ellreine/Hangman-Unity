using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class WordList
{
    public List<string> words;
}

public class WordManager : MonoBehaviour
{
    public GameObject letterPrefab;  // Префаб буквы для слова
    public GameObject victoryPanel; // Панель победы
    public Button nextWordButton; // Кнопка "следующее слово"
    public float spacing = 1f;  // Расстояние между буквами
    public Sprite[] letterSprites;  // Массив спрайтов букв
    public HangmanManager hangmanManager; // Ссылка на HangmanManager
    public CurrentScoreManager currentScoreManager; // Ссылка на CurrentScoreManager

    private Dictionary<char, Sprite> letterSpriteDict;  // Словарь спрайтов букв
    private HashSet<char> guessedLetters;  // Множество угаданных букв
    private List<GameObject> letterObjects = new List<GameObject>();  // Список объектов букв
    private string currentWord;  // Текущее загаданное слово
    private List<string> wordList;  // Список слов из JSON файла

    void Start()
    {
        guessedLetters = new HashSet<char>();
        InitializeLetterSprites();
        LoadWordsFromJson();
        nextWordButton.onClick.AddListener(NextWord);
        victoryPanel.SetActive(false);
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

    void LoadWordsFromJson()
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", "words.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            WordList wordListContainer = JsonUtility.FromJson<WordList>(json);
            wordList = wordListContainer.words;
        }
        else
        {
            Debug.LogError("Words JSON file not found at path: " + filePath);
            wordList = new List<string>();  // Создаем пустой список, чтобы избежать ошибок
        }
    }

    public void InitializeNewWord()
    {
        ClearPreviousWord();
        currentWord = wordList[Random.Range(0, wordList.Count)];  // Выбираем случайное слово
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
            CheckWinCondition();
        }
        else
        {
            hangmanManager.LoseLife();

            if (!hangmanManager.HasLivesRemaining())
            {
                hangmanManager.GameOver();
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

        currentScoreManager.IncrementScore(1); // Увеличиваем текущий счет при угаданном слове
        ShowVictoryPanel();
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
        foreach (GameObject letterObject in letterObjects)
        {
            LetterFeedback feedback = letterObject.GetComponent<LetterFeedback>();
            if (feedback != null)
            {
                feedback.ResetFeedback(); // Сбросить состояние меток
            }

            LetterClickHandler clickHandler = letterObject.GetComponent<LetterClickHandler>();
            if (clickHandler != null)
            {
                clickHandler.enabled = true;  // Включаем обработчик кликов
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
}
