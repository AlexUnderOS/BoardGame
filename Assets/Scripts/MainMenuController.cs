using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private const string MainMenuSectionKey = "MainMenuSection";

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject leaderboardPanel;

    [Header("Default")]
    public string defaultSection = "Main";

    void Start()
    {
        // Если вдруг пришли из паузы — можно подстраховаться
        Time.timeScale = 1f;

        string section = PlayerPrefs.GetString(MainMenuSectionKey, defaultSection);

        // Чтобы не “залипало” при следующем заходе в меню
        PlayerPrefs.DeleteKey(MainMenuSectionKey);

        OpenSection(section);
    }

    public void OpenSection(string section)
    {
        // выключаем всё
        if (mainPanel) mainPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (leaderboardPanel) leaderboardPanel.SetActive(false);

        switch (section)
        {
            case "Settings":
                if (settingsPanel) settingsPanel.SetActive(true);
                break;

            case "Leaderboard":
                if (leaderboardPanel) leaderboardPanel.SetActive(true);
                break;

            case "Main":
            default:
                if (mainPanel) mainPanel.SetActive(true);
                break;
        }

        Debug.Log($"[MainMenu] OpenSection: {section}");
    }

    public void OpenMain() => OpenSection("Main");
    public void OpenSettings() => OpenSection("Settings");
    public void OpenLeaderboard() => OpenSection("Leaderboard");
}
