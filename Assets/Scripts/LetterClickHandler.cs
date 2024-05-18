// LetterClickHandler.cs
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
        if (onLetterClick != null)
        {
            onLetterClick.Invoke(letter, gameObject);
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
