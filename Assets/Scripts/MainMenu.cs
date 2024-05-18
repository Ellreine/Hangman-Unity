using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Button newGameButton;
    public Button resumeButton;

    void Start()
    {
        newGameButton.onClick.AddListener(StartNewGame);
        resumeButton.onClick.AddListener(ResumeGame);

        // Деактивируем кнопку Resume, если сохраненная игра не найдена
        string filePath = Path.Combine(Application.dataPath, "Resources", "gameState.json");
        if (!File.Exists(filePath))
        {
            resumeButton.interactable = false;
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("GameScene"); // Замените "GameScene" на название вашей игровой сцены
    }

    public void ResumeGame()
    {
        SceneManager.LoadScene("GameScene"); // Замените "GameScene" на название вашей игровой сцены
    }
}
