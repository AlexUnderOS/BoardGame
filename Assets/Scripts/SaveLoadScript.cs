using UnityEngine;
using System;
using System.IO;

public class SaveLoadScript : MonoBehaviour
{
    public string saveFileName = "mansFails.json";

    [Serializable]
    public class GameData
    {
        public int character;
        public String characterName;
    }

    private GameData gameData = new GameData();

    public void SaveGame(int character, String characterName)
    {
        gameData.character = character;
        gameData.characterName = characterName;
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
        Debug.Log("Game Saved to: " + Application.persistentDataPath + "/" + saveFileName);
    }

    public void LoadGame()
    {
        string filePath = Application.persistentDataPath + "/" + saveFileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + filePath);
        }

    }

    public int GetLoadedCharacterIndex() => gameData.character;
    public string GetLoadedCharacterName() => gameData.characterName;

}
