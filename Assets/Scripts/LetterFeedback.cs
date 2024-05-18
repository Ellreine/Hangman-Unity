// LetterFeedback.cs
using UnityEngine;

public class LetterFeedback : MonoBehaviour
{
    public GameObject correctMark;  // Ссылка на объект Correct
    public GameObject incorrectMark;  // Ссылка на объект Uncorrect

    void Start()
    {
        correctMark.SetActive(false);  // Изначально скрываем галочку
        incorrectMark.SetActive(false);  // Изначально скрываем крестик
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
}
