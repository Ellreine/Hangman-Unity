// WordManager.cs
using UnityEngine;
using System.Collections.Generic;

public class WordManager : MonoBehaviour
{
    public GameObject letterPrefab;  // ������ �����
    public string currentWord = "����";  // ���������� �����
    public float spacing = 1f;  // ���������� ����� �������
    public Sprite[] letterSprites;  // ������ �������� ����
    public HangmanManager hangmanManager; // ������ �� HangmanManager

    private Dictionary<char, Sprite> letterSpriteDict;  // ������� �������� ����
    private HashSet<char> guessedLetters;  // ��������� ��������� ����

    void Start()
    {
        guessedLetters = new HashSet<char>();
        InitializeLetterSprites();
        InitializeWord();
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

    void InitializeWord()
    {
        Vector3 nextPosition = transform.position;  // ��������� ������� ��� ������ �����

        foreach (char letter in currentWord)
        {
            GameObject letterObj = Instantiate(letterPrefab, nextPosition, Quaternion.identity, transform);
            WordLetter wordLetter = letterObj.GetComponent<WordLetter>();
            wordLetter.Initialize(letter, letterSpriteDict);
            nextPosition.x += spacing;  // ����������� ������� ��� ��������� �����
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
}
