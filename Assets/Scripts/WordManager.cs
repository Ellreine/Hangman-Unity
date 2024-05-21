using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class WordManager : MonoBehaviour
{
    public GameObject letterPrefab;
    public GameObject victoryPanel;
    public Button nextWordButton;
    public float spacing = 1f;
    public Sprite[] letterSprites;
    public HangmanManager hangmanManager;
    public CurrentScoreManager currentScoreManager;

    private Dictionary<char, Sprite> letterSpriteDict;
    private HashSet<char> guessedLetters;
    private List<GameObject> letterObjects = new List<GameObject>();
    private string currentWord;
    private List<string> wordList;

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

        string letters = "¿¡¬√ƒ≈®∆«»… ÀÃÕŒœ–—“”‘’÷◊ÿŸ⁄€‹›ﬁﬂ";
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
        string filePath = Path.Combine(Application.dataPath, "words.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            WordList wordListContainer = JsonUtility.FromJson<WordList>(json);
            wordList = wordListContainer.words;
        }
        else
        {
            Debug.LogError("Words JSON file not found at path: " + filePath);
            wordList = new List<string>();
        }
    }

    public void InitializeNewWord()
    {
        ClearPreviousWord();
        currentWord = wordList[Random.Range(0, wordList.Count)];
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
        Vector3 nextPosition = transform.position;

        foreach (char letter in currentWord)
        {
            GameObject letterObj = Instantiate(letterPrefab, nextPosition, Quaternion.identity, transform);
            WordLetter wordLetter = letterObj.GetComponent<WordLetter>();
            wordLetter.Initialize(letter, letterSpriteDict);
            nextPosition.x += spacing;
            letterObjects.Add(letterObj);
        }
    }

    public bool CheckLetter(string letter)
    {
        char upperLetter = char.ToUpper(letter[0]);
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
                return;
            }
        }

        currentScoreManager.IncrementScore(1);
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
        hangmanManager.ResetHangman();

        victoryPanel.SetActive(false);
    }

    public void DisableAllLetters()
    {
        foreach (GameObject letterObject in letterObjects)
        {
            LetterClickHandler clickHandler = letterObject.GetComponent<LetterClickHandler>();
            if (clickHandler != null)
            {
                clickHandler.enabled = false;
            }
        }

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
                feedback.ResetFeedback();
            }

            LetterClickHandler clickHandler = letterObject.GetComponent<LetterClickHandler>();
            if (clickHandler != null)
            {
                clickHandler.enabled = true;
            }
        }

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
                spriteRenderer.sprite = wordLetter.baseSprite;
            }
        }
    }
}
