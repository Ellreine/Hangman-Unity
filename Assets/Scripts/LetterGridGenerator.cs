using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LetterGridGenerator : MonoBehaviour
{
    public GameObject letterPrefab;
    public float letterWidth = 1.0f;  // Фиксированная ширина для каждой буквы
    public float offset = 0.2f;  // Фиксированный отступ между буквами
    public float yOffset = 1.5f;  // Отступ по оси Y между рядами
    public Vector2 startPosition = new Vector2(-5f, 3f);  // Начальная позиция для размещения
    public Sprite[] letterSprites; // Массив спрайтов, который можно настроить в инспекторе

    private string[] cyrillicLetters = new string[]
    {
        "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й",
        "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф",
        "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я"
    };

    void Start()
    {
        GenerateLetterGrid();
    }

    void GenerateLetterGrid()
    {
        if (letterSprites == null || letterSprites.Length != 33)
        {
            Debug.LogError("Sprites count does not match 33 or letterSprites is null. Check your sprites.");
            return;
        }

        if (letterPrefab == null)
        {
            Debug.LogError("Letter Prefab is not assigned.");
            return;
        }

        int lettersPerRow = 11;
        Vector2 currentPosition = startPosition;

        for (int i = 0; i < 3; i++)
        {
            currentPosition.x = startPosition.x;
            for (int j = 0; j < lettersPerRow; j++)
            {
                int index = i * lettersPerRow + j;
                if (index >= letterSprites.Length) break;

                GameObject letter = Instantiate(letterPrefab, transform);
                if (letter == null)
                {
                    Debug.LogError("Failed to instantiate letterPrefab.");
                    continue;
                }

                letter.transform.position = currentPosition;

                SpriteRenderer spriteRenderer = letter.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    Debug.LogError("SpriteRenderer component not found on letterPrefab.");
                    continue;
                }

                spriteRenderer.sprite = letterSprites[index];

                // Add LetterClickHandler component
                LetterClickHandler clickHandler = letter.GetComponent<LetterClickHandler>();
                if (clickHandler == null)
                {
                    Debug.LogError("LetterClickHandler component not found on letterPrefab.");
                    continue;
                }

                clickHandler.Initialize(cyrillicLetters[index], OnLetterClicked);

                // Update currentPosition.x with fixed letter width and offset
                currentPosition.x += letterWidth + offset;
            }
            currentPosition.y -= yOffset;
        }
    }

    public void OnLetterClicked(string letter, GameObject clickedObject)
    {
        WordManager wordManager = FindObjectOfType<WordManager>();
        if (wordManager == null)
        {
            Debug.LogError("WordManager not found in the scene.");
            return;
        }

        bool isCorrect = wordManager.CheckLetter(letter);

        LetterClickHandler clickHandler = clickedObject.GetComponent<LetterClickHandler>();
        if (clickHandler == null)
        {
            Debug.LogError("LetterClickHandler component not found on clicked object.");
            return;
        }

        if (isCorrect)
        {
            clickHandler.ShowCorrectMark();
        }
        else
        {
            clickHandler.ShowIncorrectMark();
        }

        Debug.Log("Clicked letter: " + letter);
    }
}
