// WordLetter.cs
using UnityEngine;
using System.Collections.Generic;

public class WordLetter : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Ссылка на компонент SpriteRenderer
    public Sprite baseSprite;  // Спрайт для скрытой буквы (линия)
    public Sprite letterSprite;  // Спрайт для открытой буквы
    public char letter;  // Символ буквы
    private Dictionary<char, Sprite> letterSprites;  // Словарь спрайтов букв

    void Start()
    {
        spriteRenderer.sprite = baseSprite;  // Изначально устанавливаем спрайт линии
    }

    public void Initialize(char letter, Dictionary<char, Sprite> letterSprites)
    {
        this.letter = char.ToUpper(letter); // Приведение к верхнему регистру
        this.letterSprites = letterSprites;
        letterSprite = GetSpriteForLetter(this.letter);
        if (letterSprite == null)
        {
            Debug.LogError("Failed to load sprite for letter: " + this.letter);
        }
    }

    public void Reveal()
    {
        spriteRenderer.sprite = letterSprite;  // Переключаем на спрайт буквы
    }

    private Sprite GetSpriteForLetter(char letter)
    {
        if (letterSprites.TryGetValue(letter, out Sprite sprite))
        {
            return sprite;
        }
        Debug.LogError("Sprite not found for letter: " + letter);
        return null;
    }
}
