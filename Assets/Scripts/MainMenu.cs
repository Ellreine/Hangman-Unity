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

        string filePath = Path.Combine(Application.dataPath, "gameState.json");
        if (!File.Exists(filePath))
        {
            resumeButton.interactable = false;
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ResumeGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
