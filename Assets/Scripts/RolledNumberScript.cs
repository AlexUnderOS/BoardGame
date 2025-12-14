using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RolledNumberScript : MonoBehaviour
{
    DiceRollScript diceRollScript;
    [SerializeField] TextMeshProUGUI rolledNumberText;

    void Awake()
    {
        diceRollScript = FindFirstObjectByType<DiceRollScript>();
    }

    void Update()
    {
        if (diceRollScript != null)
        {
            if (diceRollScript.isLanded)
                rolledNumberText.text = diceRollScript.diceFaceNum;
            else
                rolledNumberText.text = "?";
        }
        else
        {
            Debug.Log("DiceRollScript not found!");
        }
    }
}
