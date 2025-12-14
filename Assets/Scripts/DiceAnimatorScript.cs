using UnityEngine;

public class DiceAnimatorScript : MonoBehaviour
{

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void RollDice()
    {
        animator.SetBool("isRoaling", true);
    }
    public void StoplDice()
    {
        animator.SetBool("isRoaling", false);
    }
}
