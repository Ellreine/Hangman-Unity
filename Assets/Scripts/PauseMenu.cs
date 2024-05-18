using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Button pauseButton;
    private HangmanManager hangmanManager;

    void Start()
    {
        hangmanManager = FindObjectOfType<HangmanManager>();
        pauseButton.onClick.AddListener(PauseAndSave);
    }

    public void PauseAndSave()
    {
        hangmanManager.SaveGameState();
        Time.timeScale = 1f; // Возобновляем время перед переходом в главное меню
        SceneManager.LoadScene("MainMenu"); // Замените "MainMenu" на название вашей главной сцены
    }
}
