using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardManager : MonoBehaviour
{
    [Header("Board")]
    public Transform[] cells;

    [Header("Cell Materials")]
    [SerializeField] private Material defaultCellMaterial;
    [SerializeField] private Material blueCellMaterial;

    [Header("Blue Cells Settings (4–5)")]
    [SerializeField] private int minBlueCells = 4;
    [SerializeField] private int maxBlueCells = 5;

    [Header("Dice")]
    public DiceRollScript dice;

    [Header("Spawns")]
    public Transform playerSpawn;
    public Transform botSpawn;

    [Header("Character Prefabs (same order as menu)")]
    public GameObject[] characterPrefabs;

    [Header("UI")]
    [SerializeField] private Button resetButton;
    [SerializeField] private TMP_Text scoreText;

    [Header("Win Panel UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winPanelWinnerText;
    [SerializeField] private TMP_Text winPanelScoreText;

    [Header("Match Series")]
    [SerializeField] private int maxMatches = 3;
    [SerializeField] private float roundRestartDelay = 1.2f;

    // runtime
    private TokenMover player;
    private TokenMover bot;

    private bool playerTurn;
    private bool decidingFirstTurn = true;
    private bool turnLocked = false;

    private int[] blueCells;

    private int botStartRoll;
    private int playerStartRoll;

    private List<string> namePool;

    // score
    private string playerName;
    private string botName;

    private int playerScore = 0;
    private int botScore = 0;
    private int matchesPlayed = 0;

    private bool seriesFinished = false;
    private bool leaderboardRecorded = false;


    private bool roundEndedThisMove = false;
    private bool lastMoveWasPlayer = false;

    private Coroutine restartRoutine;
    public static BoardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[Board] Duplicate BoardManager detected -> destroying this one");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void CleanupExistingTokens()
    {
        TokenMover[] movers = FindObjectsByType<TokenMover>(FindObjectsSortMode.None);

        int count = 0;
        foreach (var m in movers)
        {
            Destroy(m.gameObject);
            count++;
        }

        if (count > 0)
            Debug.LogWarning($"[Board] CleanupExistingTokens: destroyed {count} TokenMover objects");
    }


    void Start()
    {
        Time.timeScale = 1f;

        SpawnCharacters_FromPlayerPrefs();

        if (BoardSaveData.TryLoad(out var save))
        {
            Debug.Log("[Board] Save found -> applying");
            ApplySave(save);

            dice.ForceReadyForNextRoll();

            decidingFirstTurn = false;
            turnLocked = false;
            StartTurn();
        }
        else
        {
            Debug.Log("[Board] No save -> new game");
            SetupRandomBlueCells();
            UpdateScoreUI();
            StartCoroutine(FirstTurnRoutine_FirstMatchOnly());
        }

        Debug.Log("[Board] Game start");

        if (winPanel != null) winPanel.SetActive(false);

        namePool = LoadNamesFromResources("PlayerNames");

        SpawnCharacters_FromPlayerPrefs();
        SetupRandomBlueCells();
        UpdateScoreUI();

        StartCoroutine(FirstTurnRoutine_FirstMatchOnly());
    }

    void SpawnCharacters_FromPlayerPrefs()
    {
        CleanupExistingTokens();

        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        selectedIndex = Mathf.Clamp(selectedIndex, 0, characterPrefabs.Length - 1);

        playerName = PlayerPrefs.GetString("PlayerName", "Player");
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Player";

        Debug.Log($"[Board] PlayerPrefs: index={selectedIndex}, name='{playerName}'");

        GameObject p = Instantiate(characterPrefabs[selectedIndex], playerSpawn.position, Quaternion.identity);
        player = p.GetComponent<TokenMover>();
        p.GetComponent<NameScript>()?.SetName(playerName);

        int botIndex = Random.Range(0, characterPrefabs.Length);
        if (characterPrefabs.Length > 1)
            while (botIndex == selectedIndex)
                botIndex = Random.Range(0, characterPrefabs.Length);

        GameObject b = Instantiate(characterPrefabs[botIndex], botSpawn.position, Quaternion.identity);
        bot = b.GetComponent<TokenMover>();

        botName = GetRandomName();
        b.GetComponent<NameScript>()?.SetName(botName);

        player.SetOffset(new Vector3(-1f, 1.3f, 0f));
        bot.SetOffset(new Vector3(1f, 1.3f, 0f));

        player.SetPosition(cells[0], 0);
        bot.SetPosition(cells[0], 0);

        Debug.Log("[Board] Characters spawned");
    }


    void SetupRandomBlueCells()
    {
        for (int i = 1; i < cells.Length - 1; i++)
            SetCellMaterial(cells[i], defaultCellMaterial);

        int blueCount = Random.Range(minBlueCells, maxBlueCells + 1);

        HashSet<int> chosen = new HashSet<int>();
        while (chosen.Count < blueCount)
        {
            int idx = Random.Range(1, cells.Length - 1);
            chosen.Add(idx);
        }

        blueCells = new int[chosen.Count];
        chosen.CopyTo(blueCells);
        System.Array.Sort(blueCells);

        foreach (int idx in blueCells)
            SetCellMaterial(cells[idx], blueCellMaterial);

        Debug.Log("[Board] Blue cells: " + string.Join(", ", blueCells));
    }

    void SetCellMaterial(Transform cell, Material mat)
    {
        if (cell == null || mat == null) return;
        Renderer r = cell.GetComponent<Renderer>();
        if (r != null) r.material = mat;
    }

    IEnumerator FirstTurnRoutine_FirstMatchOnly()
    {
        if (seriesFinished) yield break;

        roundEndedThisMove = false;

        decidingFirstTurn = true;
        turnLocked = true;
        dice.inputEnabled = false;
        SetResetButton(false);

        Debug.Log("[Board] First match: decide who starts by dice");

        while (true)
        {
            PrepareDice();
            yield return new WaitForSeconds(0.5f);

            dice.RollFromManager();
            yield return new WaitUntil(() => dice.isLanded);
            botStartRoll = ParseDice();
            Debug.Log($"[Board] BOT start roll = {botStartRoll}");

            PrepareDice();
            dice.inputEnabled = true;
            SetResetButton(true);

            yield return new WaitUntil(() => dice.rolledThisTurn && dice.isLanded);

            dice.inputEnabled = false;
            SetResetButton(false);

            playerStartRoll = ParseDice();
            Debug.Log($"[Board] PLAYER start roll = {playerStartRoll}");

            if (playerStartRoll != botStartRoll)
            {
                playerTurn = playerStartRoll > botStartRoll;
                break;
            }

            Debug.Log("[Board] Tie → reroll");
        }

        int firstSteps = playerTurn ? playerStartRoll : botStartRoll;
        Debug.Log($"[Board] First mover: {(playerTurn ? "PLAYER" : "BOT")} steps={firstSteps}");

        decidingFirstTurn = false;
        yield return HandleMove(playerTurn ? player : bot, firstSteps);

        if (seriesFinished || roundEndedThisMove) yield break;

        turnLocked = false;
        playerTurn = !playerTurn;
        StartTurn();
    }

    void StartTurn()
    {
        dice.ForceReadyForNextRoll();

        if (seriesFinished) return;

        turnLocked = false;
        decidingFirstTurn = false;

        PrepareDice();

        if (playerTurn)
        {
            dice.inputEnabled = true;
            SetResetButton(true);
            Debug.Log("[Board] Player turn");
        }
        else
        {
            dice.inputEnabled = false;
            SetResetButton(false);
            StartCoroutine(BotTurn());
        }
    }

    void Update()
    {
        if (seriesFinished || turnLocked || decidingFirstTurn) return;

        if (playerTurn && dice.rolledThisTurn && dice.isLanded)
        {
            turnLocked = true;
            dice.inputEnabled = false;
            SetResetButton(false);

            int roll = ParseDice();
            Debug.Log($"[Board] Player rolled {roll}");

            StartCoroutine(PlayerMoveThenNext(roll));
        }
    }

    IEnumerator PlayerMoveThenNext(int steps)
    {
        roundEndedThisMove = false;

        yield return HandleMove(player, steps);

        if (seriesFinished || roundEndedThisMove) yield break;

        playerTurn = false;
        turnLocked = false;
        StartTurn();
    }

    IEnumerator BotTurn()
    {
        roundEndedThisMove = false;

        Debug.Log("[Board] Bot turn: rolling...");
        yield return new WaitForSeconds(0.8f);

        dice.rolledThisTurn = false;
        dice.RollFromManager();
        yield return new WaitUntil(() => dice.isLanded);

        turnLocked = true;

        int roll = ParseDice();
        Debug.Log($"[Board] Bot rolled {roll}");

        yield return HandleMove(bot, roll);

        if (seriesFinished || roundEndedThisMove) yield break;

        playerTurn = true;
        turnLocked = false;
        StartTurn();
    }

    IEnumerator HandleMove(TokenMover mover, int steps)
    {
        lastMoveWasPlayer = (mover == player);

        yield return mover.MoveSteps(cells, steps);

        int idx = mover.CurrentIndex;

        if (IsBlue(idx))
        {
            int target = GetBlueTarget(idx);
            Debug.Log($"[Board] BLUE {idx} → {target}");
            mover.SetPosition(cells[target], target);
            idx = target;
        }

        if (idx >= cells.Length - 1)
        {
            bool playerWon = (mover == player);
            Debug.Log($"[Board] ROUND WINNER: {(playerWon ? "PLAYER" : "BOT")}");
            OnRoundFinished(playerWon);
        }
    }

    void OnRoundFinished(bool playerWon)
    {
        if (seriesFinished) return;

        roundEndedThisMove = true;

        turnLocked = true;
        decidingFirstTurn = true;
        dice.inputEnabled = false;
        SetResetButton(false);

        matchesPlayed++;

        if (playerWon) playerScore++;
        else botScore++;

        UpdateScoreUI();

        Debug.Log($"[Board] Score: {botName}={botScore}, {playerName}={playerScore} | matches={matchesPlayed}/{maxMatches}");


        if (playerScore >= maxMatches || botScore >= maxMatches || matchesPlayed >= maxMatches)
        {
            seriesFinished = true;

            Debug.Log("[Board] SERIES FINISHED");

            ShowWinPanel();
            return;
        }

        if (restartRoutine != null) StopCoroutine(restartRoutine);
        restartRoutine = StartCoroutine(RestartRoundCoroutine());
    }

    IEnumerator RestartRoundCoroutine()
    {
        Debug.Log("[Board] Restarting round...");
        yield return new WaitForSeconds(roundRestartDelay);

        player.SetPosition(cells[0], 0);
        bot.SetPosition(cells[0], 0);

        dice.ResetDice();

        SetupRandomBlueCells();

        playerTurn = !lastMoveWasPlayer;

        decidingFirstTurn = false;
        turnLocked = false;
        roundEndedThisMove = false;

        Debug.Log($"[Board] New round start. Next turn: {(playerTurn ? "PLAYER" : "BOT")}");

        StartTurn();
    }

    void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        string resultText;

        if (playerScore > botScore) resultText = "You Win!";
        else if (playerScore < botScore) resultText = "You Lost!";
        else resultText = "Draw!";

        if (winPanelWinnerText != null)
            winPanelWinnerText.text = resultText;

        if (winPanelScoreText != null)
            winPanelScoreText.text = $"{playerName} - {playerScore}\n{botName} - {botScore}";

        if (playerScore > botScore) LeaderboardScript.AddPlayerWin();
        else if (playerScore < botScore) LeaderboardScript.AddBotWin();
        else LeaderboardScript.AddDraw();

        Debug.Log($"[Board] WinPanel shown: {resultText}");
    }



    private BoardSaveData CreateSave()
    {
        return new BoardSaveData
        {
            playerIndex = player != null ? player.CurrentIndex : 0,
            botIndex = bot != null ? bot.CurrentIndex : 0,

            blueCells = blueCells,

            playerTurn = playerTurn,

            playerScore = playerScore,
            botScore = botScore,
            matchesPlayed = matchesPlayed,

            playerName = playerName,
            botName = botName,

            diceFirstThrow = dice != null && dice.firstThrow,
            diceRolledThisTurn = dice != null && dice.rolledThisTurn,
            diceFaceNum = dice != null ? dice.diceFaceNum : "?"
        };
    }

    private void ApplySave(BoardSaveData s)
    {
        if (s == null) return;

        // restore names (если уже заспавнены)
        playerName = string.IsNullOrWhiteSpace(s.playerName) ? playerName : s.playerName;
        botName = string.IsNullOrWhiteSpace(s.botName) ? botName : s.botName;

        // restore series
        playerScore = s.playerScore;
        botScore = s.botScore;
        matchesPlayed = s.matchesPlayed;

        // restore turn
        playerTurn = s.playerTurn;

        // restore blue cells
        if (s.blueCells != null && s.blueCells.Length > 0)
        {
            blueCells = s.blueCells;
            RefreshBlueCellMaterials();
        }

        int pIdx = Mathf.Clamp(s.playerIndex, 0, cells.Length - 1);
        int bIdx = Mathf.Clamp(s.botIndex, 0, cells.Length - 1);

        player.SetPosition(cells[pIdx], pIdx);
        bot.SetPosition(cells[bIdx], bIdx);

        if (dice != null)
        {
            dice.firstThrow = s.diceFirstThrow;
            dice.rolledThisTurn = s.diceRolledThisTurn;
            dice.diceFaceNum = string.IsNullOrEmpty(s.diceFaceNum) ? "?" : s.diceFaceNum;
            dice.isLanded = false;
        }

        UpdateScoreUI();

        Debug.Log($"[Save] Applied. pIdx={pIdx}, bIdx={bIdx}, turn={(playerTurn ? "PLAYER" : "BOT")}, score P={playerScore} B={botScore}");
    }

    private void RefreshBlueCellMaterials()
    {
        for (int i = 1; i < cells.Length - 1; i++)
            SetCellMaterial(cells[i], defaultCellMaterial);

        if (blueCells == null) return;

        foreach (int idx in blueCells)
        {
            if (idx <= 0 || idx >= cells.Length - 1) continue;
            SetCellMaterial(cells[idx], blueCellMaterial);
        }

        Debug.Log("[Board] Blue materials refreshed from save");
    }

    public BoardSaveData CreateSaveForExternal()
    {
        return CreateSave();
    }




    void UpdateScoreUI()
    {
        if (scoreText == null) return;
        scoreText.text = $"{botName} - {botScore}\n{playerName} - {playerScore}";
    }

    void SetResetButton(bool enabled)
    {
        if (resetButton != null)
            resetButton.interactable = enabled;
    }

    public void ResetDiceFromUI()
    {
        if (!playerTurn || !dice.inputEnabled || seriesFinished) return;
        Debug.Log("[UI] ResetDice clicked");

        dice.ResetDice();
    }

    void PrepareDice()
    {
        dice.isLanded = false;
        dice.diceFaceNum = "?";
        dice.rolledThisTurn = false;
    }

    int ParseDice()
    {
        return int.TryParse(dice.diceFaceNum, out int v) ? Mathf.Clamp(v, 1, 6) : 1;
    }

    bool IsBlue(int index)
    {
        if (blueCells == null) return false;
        foreach (int b in blueCells)
            if (b == index) return true;
        return false;
    }

    int GetBlueTarget(int index)
    {
        int lastBlue = -1;
        foreach (int b in blueCells)
            if (b < index) lastBlue = Mathf.Max(lastBlue, b);
        return lastBlue == -1 ? 0 : lastBlue;
    }

    List<string> LoadNamesFromResources(string file)
    {
        List<string> list = new List<string>();
        TextAsset ta = Resources.Load<TextAsset>(file);
        if (ta == null) return list;

        foreach (string s in ta.text.Split('\n'))
            if (!string.IsNullOrWhiteSpace(s))
                list.Add(s.Trim());

        return list;
    }

    string GetRandomName()
    {
        return (namePool != null && namePool.Count > 0)
            ? namePool[Random.Range(0, namePool.Count)]
            : "Bot_" + Random.Range(100, 999);
    }
}
