using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuContinueController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button continueButton;
    public Button startButton;

    [Header("Fallback game scene index (if not stored)")]
    [SerializeField] private int defaultGameSceneBuildIndex = 1;

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool canContinue = GameResumeData.HasResume();

        if (continueButton != null)
            continueButton.gameObject.SetActive(canContinue);

        if (startButton != null)
            startButton.interactable = !canContinue;

        Debug.Log($"[MainMenu] Continue available: {canContinue}");
    }


    public void ContinueGame()
    {
        if (!GameResumeData.HasResume())
        {
            Debug.LogWarning("[MainMenu] Continue pressed but no resume data!");
            Refresh();
            return;
        }

        int sceneIndex = GameResumeData.GetResumeSceneIndex(defaultGameSceneBuildIndex);

        Debug.Log($"[MainMenu] Continue -> load scene index {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);

    }
}
