using UnityEngine;

public class GameSpawnScript : MonoBehaviour
{
    [Header("Save/Load")]
    [SerializeField] private SaveLoadScript saveLoad;

    [Header("Prefabs (same order as menu)")]
    [SerializeField] private GameObject[] characterPrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform botSpawn;

    [Header("Bot Names")]
    [SerializeField] private string[] botNames = { "Robo", "KubaMeistars", "Randoms", "Zigis", "Bots" };

    private void Start()
    {
        // 1) Загружаем выбранного игрока из JSON
        saveLoad.LoadGame();

        int playerIndex = Mathf.Clamp(saveLoad.GetLoadedCharacterIndex(), 0, characterPrefabs.Length - 1);
        string playerName = saveLoad.GetLoadedCharacterName();
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Player";

        // 2) Спавним игрока
        var playerObj = Instantiate(characterPrefabs[playerIndex], playerSpawn.position, playerSpawn.rotation);
        var playerNameScript = playerObj.GetComponent<NameScript>();
        if (playerNameScript != null) playerNameScript.SetName(playerName);

        // 3) Выбираем бота (желательно не тем же персом)
        int botIndex = Random.Range(0, characterPrefabs.Length);
        if (characterPrefabs.Length > 1)
        {
            while (botIndex == playerIndex)
                botIndex = Random.Range(0, characterPrefabs.Length);
        }

        string botName = botNames.Length > 0
            ? botNames[Random.Range(0, botNames.Length)]
            : $"Bot_{Random.Range(100, 999)}";

        // 4) Спавним бота
        var botObj = Instantiate(characterPrefabs[botIndex], botSpawn.position, botSpawn.rotation);
        var botNameScript = botObj.GetComponent<NameScript>();
        if (botNameScript != null) botNameScript.SetName(botName);
    }
}
