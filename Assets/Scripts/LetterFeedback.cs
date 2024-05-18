// LetterFeedback.cs
using UnityEngine;

public class LetterFeedback : MonoBehaviour
{
    public GameObject correctMark;  // ������ �� ������ Correct
    public GameObject incorrectMark;  // ������ �� ������ Uncorrect

    void Start()
    {
        correctMark.SetActive(false);  // ���������� �������� �������
        incorrectMark.SetActive(false);  // ���������� �������� �������
    }

    // ����� ��� ����������� �������
    public void ShowCorrect()
    {
        correctMark.SetActive(true);
    }

    // ����� ��� ����������� ��������
    public void ShowIncorrect()
    {
        incorrectMark.SetActive(true);
    }
}
