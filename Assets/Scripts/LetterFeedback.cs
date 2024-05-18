using UnityEngine;

public class LetterFeedback : MonoBehaviour
{
    public GameObject correctMark;  // ������ �� ������ Correct
    public GameObject incorrectMark;  // ������ �� ������ Uncorrect

    void Start()
    {
        HideAllMarks();  // ���������� �������� �����
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

    // ����� ��� ������� ���� �����
    public void HideAllMarks()
    {
        if (correctMark != null)
            correctMark.SetActive(false);
        if (incorrectMark != null)
            incorrectMark.SetActive(false);
    }

    // ����� ��� ������ ���������
    public void ResetFeedback()
    {
        Debug.Log("Resetting feedback for: " + gameObject.name); // �������� ���������� ���������
        HideAllMarks();
    }
}
