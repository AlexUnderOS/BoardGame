using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    private const string SectionIndexKey = "MainMenuSectionIndex";

    [Header("UI")]
    public GameObject pauseMenuRoot;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
    }


    private void GoToMainMenuWithResume()
    {
        var board = FindFirstObjectByType<BoardManager>();
        if (board != null)
            BoardSaveData.Save(board.CreateSaveForExternal());

        GameResumeData.SetResumePoint(SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void GoToMainMenuNoResume()
    {
        GameResumeData.ClearResumePoint();
        BoardSaveData.Clear();

        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        PlayerPrefs.SetInt(SectionIndexKey, (int)MenuSection.Settings);
        PlayerPrefs.Save();
        GoToMainMenuWithResume();
    }

    public void OpenLeaderboard()
    {
        PlayerPrefs.SetInt(SectionIndexKey, (int)MenuSection.Leaderboard);
        PlayerPrefs.Save();
        GoToMainMenuWithResume();
    }

    public void OpenCharacters()
    {
        PlayerPrefs.SetInt(SectionIndexKey, (int)MenuSection.Characters);
        PlayerPrefs.Save();
        GoToMainMenuWithResume();
    }

    public void QuitAndClearContinue()
    {
        GoToMainMenuNoResume();
    }

    public void GoToMainMenu()
    {
        GoToMainMenuWithResume();
    }
}
