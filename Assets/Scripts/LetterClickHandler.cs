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
        HangmanManager hangmanManager = FindObjectOfType<HangmanManager>();
        if (hangmanManager != null && hangmanManager.IsGameOver())
        {
            return; // ���� ���� ��������, ������ �� ������
        }

        if (onLetterClick != null && enabled)
        {
            onLetterClick.Invoke(letter, gameObject);
            enabled = false;  // ��������� ���������� ������ ����� ������� �������
            Debug.Log("Click handler disabled for letter: " + gameObject.name);
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
