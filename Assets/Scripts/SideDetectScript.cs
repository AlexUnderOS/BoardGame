using UnityEngine;

public class SideDetectScript : MonoBehaviour
{
    DiceRollScript diceRollScript;

    void Awake()
    {
        diceRollScript = FindFirstObjectByType<DiceRollScript>();
    }

    void OnTriggerStay(Collider other)
    {
        if (diceRollScript == null) return;

        if (diceRollScript.GetComponent<Rigidbody>().linearVelocity == Vector3.zero)
        {
            diceRollScript.isLanded = true;

            string bottomSide = gameObject.name;

            diceRollScript.diceFaceNum = GetOppositeSide(bottomSide);
        }
        else
        {
            diceRollScript.isLanded = false;
        }
    }

    string GetOppositeSide(string side)
    {
        switch (side)
        {
            case "1": return "6";
            case "2": return "5";
            case "3": return "4";
            case "4": return "3";
            case "5": return "2";
            case "6": return "1";
            default: return "?";
        }
    }
}
