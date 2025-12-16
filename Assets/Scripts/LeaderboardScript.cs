using TMPro;
using UnityEngine;

public class LeaderboardScript : MonoBehaviour
{
    private const string PlayerWinsKey = "LB_PlayerWins";
    private const string BotWinsKey    = "LB_BotWins";
    private const string DrawsKey      = "LB_Draws";

    [Header("UI (assign in Inspector)")]
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text botText;
    [SerializeField] private TMP_Text drawsText;

    public static void AddPlayerWin()
    {
        PlayerPrefs.SetInt(PlayerWinsKey, GetPlayerWins() + 1);
        PlayerPrefs.Save();
        Debug.Log("[LB] Player win +1");
    }

    public static void AddBotWin()
    {
        PlayerPrefs.SetInt(BotWinsKey, GetBotWins() + 1);
        PlayerPrefs.Save();
        Debug.Log("[LB] Bot win +1");
    }

    public static void AddDraw()
    {
        PlayerPrefs.SetInt(DrawsKey, GetDraws() + 1);
        PlayerPrefs.Save();
        Debug.Log("[LB] Draw +1");
    }

    public static int GetPlayerWins() => PlayerPrefs.GetInt(PlayerWinsKey, 0);
    public static int GetBotWins()    => PlayerPrefs.GetInt(BotWinsKey, 0);
    public static int GetDraws()      => PlayerPrefs.GetInt(DrawsKey, 0);

    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey(PlayerWinsKey);
        PlayerPrefs.DeleteKey(BotWinsKey);
        PlayerPrefs.DeleteKey(DrawsKey);
        PlayerPrefs.Save();
        Debug.Log("[LB] Cleared");
    }

    public void RefreshUI()
    {
        if (playerText != null)
            playerText.text = $"{GetPlayerWins()}";

        if (botText != null)
            botText.text = $"{GetBotWins()}";

        if (drawsText != null)
            drawsText.text = $"{GetDraws()}";
    }

    private void OnEnable()
    {
        RefreshUI();
    }
}
