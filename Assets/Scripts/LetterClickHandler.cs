// LetterClickHandler.cs
using UnityEngine;
using UnityEngine.Events;

public class LetterClickHandler : MonoBehaviour
{
    public string letter;
    public UnityEvent<string, GameObject> onLetterClick;
    private LetterFeedback letterFeedback;  // ������ �� ��������� LetterFeedback

    void Awake()
    {
        letterFeedback = GetComponent<LetterFeedback>();  // �������� ��������� LetterFeedback
    }

    public void Initialize(string letter, UnityAction<string, GameObject> callback)
    {
        this.letter = letter;
        if (onLetterClick == null)
            onLetterClick = new UnityEvent<string, GameObject>();

        onLetterClick.RemoveAllListeners();  // �������� ���������� ���������
        onLetterClick.AddListener(callback);
    }

    void OnMouseDown()
    {
        if (onLetterClick != null)
        {
            onLetterClick.Invoke(letter, gameObject);
        }
    }

    // ����� ��� ����������� �������
    public void ShowCorrectMark()
    {
        letterFeedback.ShowCorrect();
    }

    // ����� ��� ����������� ��������
    public void ShowIncorrectMark()
    {
        letterFeedback.ShowIncorrect();
    }
}
