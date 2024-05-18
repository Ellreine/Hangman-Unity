// HangmanManager.cs
using UnityEngine;

public class HangmanManager : MonoBehaviour
{
    public GameObject[] hangmanParts; // ������ ������ ��������
    private int livesRemaining;

    void Start()
    {
        livesRemaining = hangmanParts.Length;
        HideAllParts();
    }

    // ������ ��� ����� ��������
    void HideAllParts()
    {
        foreach (GameObject part in hangmanParts)
        {
            part.SetActive(false);
        }
    }

    // ������ �����
    public void LoseLife()
    {
        int currentPart = hangmanParts.Length - livesRemaining;
        if (livesRemaining > 0)
        {
            hangmanParts[currentPart].SetActive(true);
            livesRemaining--;
        }
    }

    // ��������, ���� �� ���������� �����
    public bool HasLivesRemaining()
    {
        return livesRemaining > 0;
    }
}
