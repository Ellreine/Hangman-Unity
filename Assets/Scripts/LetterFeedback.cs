using UnityEngine;

public class LetterFeedback : MonoBehaviour
{
    public GameObject correctMark;  // Ссылка на объект Correct
    public GameObject incorrectMark;  // Ссылка на объект Uncorrect

    void Start()
    {
        HideAllMarks();  // Изначально скрываем метки
    }

    // Метод для отображения галочки
    public void ShowCorrect()
    {
        correctMark.SetActive(true);
    }

    // Метод для отображения крестика
    public void ShowIncorrect()
    {
        incorrectMark.SetActive(true);
    }

    // Метод для скрытия всех меток
    public void HideAllMarks()
    {
        if (correctMark != null)
            correctMark.SetActive(false);
        if (incorrectMark != null)
            incorrectMark.SetActive(false);
    }

    // Метод для сброса состояния
    public void ResetFeedback()
    {
        Debug.Log("Resetting feedback for: " + gameObject.name); // Добавить отладочное сообщение
        HideAllMarks();
    }
}
