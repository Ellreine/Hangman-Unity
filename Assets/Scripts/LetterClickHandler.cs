using UnityEngine;
using UnityEngine.Events;

public class LetterClickHandler : MonoBehaviour
{
    public string letter;
    public UnityEvent<string, GameObject> onLetterClick;
    private LetterFeedback letterFeedback;  // Ссылка на компонент LetterFeedback

    void Awake()
    {
        letterFeedback = GetComponent<LetterFeedback>();  // Получаем компонент LetterFeedback
    }

    public void Initialize(string letter, UnityAction<string, GameObject> callback)
    {
        this.letter = letter;
        if (onLetterClick == null)
            onLetterClick = new UnityEvent<string, GameObject>();

        onLetterClick.RemoveAllListeners();  // Очистить предыдущие слушатели
        onLetterClick.AddListener(callback);
    }

    void OnMouseDown()
    {
        HangmanManager hangmanManager = FindObjectOfType<HangmanManager>();
        if (hangmanManager != null && hangmanManager.IsGameOver())
        {
            return; // Если игра окончена, ничего не делаем
        }

        if (onLetterClick != null && enabled)
        {
            onLetterClick.Invoke(letter, gameObject);
            enabled = false;  // Отключаем обработчик кликов после первого нажатия
            Debug.Log("Click handler disabled for letter: " + gameObject.name);
        }
    }

    // Метод для отображения галочки
    public void ShowCorrectMark()
    {
        letterFeedback.ShowCorrect();
    }

    // Метод для отображения крестика
    public void ShowIncorrectMark()
    {
        letterFeedback.ShowIncorrect();
    }
}
