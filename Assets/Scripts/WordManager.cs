using UnityEngine;
using System.Collections.Generic;

public class WordManager : MonoBehaviour
{
    public GameObject letterPrefab;  // ������ ����� ��� �����
    public GameObject letterGridPrefab;  // ������ ����� ��� �����
    public string[] wordList = { "����", "���", "����", "���", "������" };  // ������ ����
    public float spacing = 1f;  // ���������� ����� �������
    public Sprite[] letterSprites;  // ������ �������� ����
    public HangmanManager hangmanManager; // ������ �� HangmanManager

    private Dictionary<char, Sprite> letterSpriteDict;  // ������� �������� ����
    private HashSet<char> guessedLetters;  // ��������� ��������� ����
    private List<GameObject> letterObjects = new List<GameObject>();  // ������ �������� ����
    private string currentWord;  // ������� ���������� �����

    void Start()
    {
        guessedLetters = new HashSet<char>();
        InitializeLetterSprites();
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
        }
        else
        {
            Debug.Log("Letter not guessed: " + upperLetter);
            hangmanManager.LoseLife();

            if (!hangmanManager.HasLivesRemaining())
            {
                Debug.Log("Game Over!");
            }
        }

        return letterFound;
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
}
