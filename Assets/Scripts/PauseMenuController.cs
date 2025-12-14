using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [Header("UI")]
    public GameObject pauseMenuRoot;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        pauseMenuRoot.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        pauseMenuRoot.SetActive(false);
    }

    // ===== BUTTON ACTIONS =====

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenLeaderboard()
    {
        PlayerPrefs.SetString("MainMenuSection", "Leaderboard");
        GoToMainMenu();
    }

    public void OpenSettings()
    {
        PlayerPrefs.SetString("MainMenuSection", "Settings");
        GoToMainMenu();
    }
}
