using UnityEngine;

public static class GameResumeData
{
    private const string HasResumeKey = "HasResumeGame";
    private const string ResumeSceneKey = "ResumeSceneBuildIndex";
    private const string GameFinishedKey = "GameFinished";

    public static void SetResumePoint(int sceneBuildIndex)
    {
        PlayerPrefs.SetInt(HasResumeKey, 1);
        PlayerPrefs.SetInt(ResumeSceneKey, sceneBuildIndex);
        PlayerPrefs.SetInt(GameFinishedKey, 0);
        PlayerPrefs.Save();

        Debug.Log($"[Resume] SetResumePoint scene={sceneBuildIndex}");
    }

    public static void MarkGameFinished()
    {
        PlayerPrefs.SetInt(GameFinishedKey, 1);
        PlayerPrefs.DeleteKey(HasResumeKey); 
        PlayerPrefs.Save();

        Debug.Log("[Resume] Game marked as FINISHED");
    }

    public static void ClearResumePoint()
    {
        PlayerPrefs.DeleteKey(HasResumeKey);
        PlayerPrefs.DeleteKey(ResumeSceneKey);
        PlayerPrefs.DeleteKey(GameFinishedKey);
        PlayerPrefs.Save();

        Debug.Log("[Resume] ClearResumePoint");
    }

    public static bool HasResume()
    {
        bool hasResume = PlayerPrefs.GetInt(HasResumeKey, 0) == 1;
        bool finished = PlayerPrefs.GetInt(GameFinishedKey, 0) == 1;

        return hasResume && !finished;
    }

    public static int GetResumeSceneIndex(int fallback = 1)
    {
        return PlayerPrefs.GetInt(ResumeSceneKey, fallback);
    }
}
