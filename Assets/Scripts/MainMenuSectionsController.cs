using UnityEngine;
public enum MenuSection
{
    Main = 0,
    Settings = 1,
    Leaderboard = 2,
    Characters = 3
}


public class MainMenuSectionsController : MonoBehaviour
{
    private const string SectionIndexKey = "MainMenuSectionIndex";

    [Header("Sections (ONLY ONE active)")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject charactersPanel;

    [Header("Default")]
    [SerializeField] private MenuSection defaultSection = MenuSection.Main;

    private void Start()
    {
        Time.timeScale = 1f;

        int raw = PlayerPrefs.GetInt(SectionIndexKey, (int)defaultSection);
        MenuSection requested = (MenuSection)raw;

        Debug.Log($"[Menu] Start. HasKey={PlayerPrefs.HasKey(SectionIndexKey)} raw={raw} requested={requested}");

        ShowSection(requested);

        // удаляем ПОСЛЕ успешного открытия
        if (PlayerPrefs.HasKey(SectionIndexKey))
            PlayerPrefs.DeleteKey(SectionIndexKey);
    }

    public void ShowSection(MenuSection section)
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (leaderboardPanel) leaderboardPanel.SetActive(false);
        if (charactersPanel) charactersPanel.SetActive(false);

        switch (section)
        {
            case MenuSection.Main:
                if (mainPanel) mainPanel.SetActive(true);
                else Debug.LogError("[Menu] mainPanel is NULL");
                break;

            case MenuSection.Settings:
                if (settingsPanel) settingsPanel.SetActive(true);
                else Debug.LogError("[Menu] settingsPanel is NULL");
                break;

            case MenuSection.Leaderboard:
                if (leaderboardPanel) leaderboardPanel.SetActive(true);
                else Debug.LogError("[Menu] leaderboardPanel is NULL");
                break;

            case MenuSection.Characters:
                if (charactersPanel) charactersPanel.SetActive(true);
                else Debug.LogError("[Menu] charactersPanel is NULL");
                break;

            default:
                if (mainPanel) mainPanel.SetActive(true);
                break;
        }

        Debug.Log($"[Menu] Open section: {section}");
    }

    public void OpenMain()        => ShowSection(MenuSection.Main);
    public void OpenSettings()    => ShowSection(MenuSection.Settings);
    public void OpenLeaderboard() => ShowSection(MenuSection.Leaderboard);
    public void OpenCharacters()  => ShowSection(MenuSection.Characters);
}
