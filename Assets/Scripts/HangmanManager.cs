// HangmanManager.cs
using UnityEngine;

public class HangmanManager : MonoBehaviour
{
    public GameObject[] hangmanParts; // Массив частей виселицы
    private int livesRemaining;

    void Start()
    {
        livesRemaining = hangmanParts.Length;
        HideAllParts();
    }

    // Скрыть все части виселицы
    void HideAllParts()
    {
        foreach (GameObject part in hangmanParts)
        {
            part.SetActive(false);
        }
    }

    // Потеря жизни
    public void LoseLife()
    {
        int currentPart = hangmanParts.Length - livesRemaining;
        if (livesRemaining > 0)
        {
            hangmanParts[currentPart].SetActive(true);
            livesRemaining--;
        }
    }

    // Проверка, есть ли оставшиеся жизни
    public bool HasLivesRemaining()
    {
        return livesRemaining > 0;
    }
}
