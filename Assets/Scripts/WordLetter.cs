// WordLetter.cs
using UnityEngine;
using System.Collections.Generic;

public class WordLetter : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // ������ �� ��������� SpriteRenderer
    public Sprite baseSprite;  // ������ ��� ������� ����� (�����)
    public Sprite letterSprite;  // ������ ��� �������� �����
    public char letter;  // ������ �����
    private Dictionary<char, Sprite> letterSprites;  // ������� �������� ����

    void Start()
    {
        spriteRenderer.sprite = baseSprite;  // ���������� ������������� ������ �����
    }

    public void Initialize(char letter, Dictionary<char, Sprite> letterSprites)
    {
        this.letter = char.ToUpper(letter); // ���������� � �������� ��������
        this.letterSprites = letterSprites;
        letterSprite = GetSpriteForLetter(this.letter);
        if (letterSprite == null)
        {
            Debug.LogError("Failed to load sprite for letter: " + this.letter);
        }
    }

    public void Reveal()
    {
        spriteRenderer.sprite = letterSprite;  // ����������� �� ������ �����
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
