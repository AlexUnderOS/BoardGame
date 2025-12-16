using System;
using UnityEngine;

[Serializable]
public class BoardSaveData
{
    public bool hasSave;

    // board state
    public int playerIndex;
    public int botIndex;

    public int[] blueCells;

    // turn
    public bool playerTurn;

    // series
    public int playerScore;
    public int botScore;
    public int matchesPlayed;

    // names
    public string playerName;
    public string botName;

    // optional: dice state (simple)
    public bool diceFirstThrow;
    public bool diceRolledThisTurn;
    public string diceFaceNum;

    private const string Key = "BoardSaveJson";

    public static void Save(BoardSaveData data)
    {
        data.hasSave = true;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
        Debug.Log("[Save] Board saved: " + json);
    }

    public static bool TryLoad(out BoardSaveData data)
    {
        data = null;
        if (!PlayerPrefs.HasKey(Key)) return false;

        string json = PlayerPrefs.GetString(Key, "");
        if (string.IsNullOrWhiteSpace(json)) return false;

        data = JsonUtility.FromJson<BoardSaveData>(json);
        bool ok = (data != null && data.hasSave);

        Debug.Log("[Save] Board load " + (ok ? "OK" : "FAILED") + ": " + json);
        return ok;
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
        Debug.Log("[Save] Board save cleared");
    }
}
