// WordManager.cs
using UnityEngine;
using System.Collections.Generic;

public class WordManager : MonoBehaviour
{
    public GameObject letterPrefab;  // Префаб буквы
    public string currentWord = "Борщ";  // Загаданное слово
    public float spacing = 1f;  // Расстояние между буквами
    public Sprite[] letterSprites;  // Массив спрайтов букв
    public HangmanManager hangmanManager; // Ссылка на HangmanManager

    private Dictionary<char, Sprite> letterSpriteDict;  // Словарь спрайтов букв
    private HashSet<char> guessedLetters;  // Множество угаданных букв

    void Start()
    {
        guessedLetters = new HashSet<char>();
        InitializeLetterSprites();
        InitializeWord();
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

    void InitializeWord()
    {
        Vector3 nextPosition = transform.position;  // Начальная позиция для первой буквы

        foreach (char letter in currentWord)
        {
            GameObject letterObj = Instantiate(letterPrefab, nextPosition, Quaternion.identity, transform);
            WordLetter wordLetter = letterObj.GetComponent<WordLetter>();
            wordLetter.Initialize(letter, letterSpriteDict);
            nextPosition.x += spacing;  // Увеличиваем позицию для следующей буквы
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
