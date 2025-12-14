using UnityEngine;

public class DiceBattleManager : MonoBehaviour
{
    public PlayerStats player;
    public PlayerStats bot;

    public DiceRollScript dice;

    private PlayerStats currentPlayer;
    private PlayerStats targetPlayer;

    private bool waitingForDice = false;

    public enum ActionType { None, Attack, Heal }
    private ActionType selectedAction = ActionType.None;

    void Start()
    {
        currentPlayer = player;
        targetPlayer = bot;
        Debug.Log("Turn: PLAYER");
    }

    // ===== КНОПКИ ИГРОКА =====

    public void SelectAttack()
    {
        if (currentPlayer != player) return;

        selectedAction = ActionType.Attack;
        waitingForDice = true;
        dice.ResetDice();
    }

    public void SelectHeal()
    {
        if (currentPlayer != player) return;

        selectedAction = ActionType.Heal;
        waitingForDice = true;
        dice.ResetDice();
    }

    // ===== ХОД БОТА =====

    void BotTurn()
    {
        selectedAction = DecideBotAction();
        waitingForDice = true;
        dice.ResetDice();

        Debug.Log("BOT chose: " + selectedAction);
    }

    ActionType DecideBotAction()
    {
        float hpPercent = bot.HealthPercent();
        float rand = Random.value;

        // если мало хп — чаще хилится
        if (hpPercent < 0.35f && rand < bot.healBias)
            return ActionType.Heal;

        // стандартная агрессия
        if (rand < bot.aggression)
            return ActionType.Attack;

        return ActionType.Heal; // иногда хил даже при фулл хп
    }

    // ===== ОБРАБОТКА КУБИКА =====

    void Update()
    {
        if (!waitingForDice) return;

        if (dice.isLanded)
        {
            int value = int.Parse(dice.diceFaceNum);

            if (selectedAction == ActionType.Attack)
                targetPlayer.TakeDamage(value);
            else if (selectedAction == ActionType.Heal)
                currentPlayer.Heal(value);

            EndTurn();
        }
    }

    // ===== СМЕНА ХОДА =====

    void EndTurn()
    {
        waitingForDice = false;
        selectedAction = ActionType.None;

        if (player.IsDead())
        {
            Debug.Log("BOT WINS!");
            return;
        }

        if (bot.IsDead())
        {
            Debug.Log("PLAYER WINS!");
            return;
        }

        // меняем активного
        if (currentPlayer == player)
        {
            currentPlayer = bot;
            targetPlayer = player;
            BotTurn(); // ✅ бот ходит сам
        }
        else
        {
            currentPlayer = player;
            targetPlayer = bot;
            Debug.Log("Turn: PLAYER");
        }
    }
}
