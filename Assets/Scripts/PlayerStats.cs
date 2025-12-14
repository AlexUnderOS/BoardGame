using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public string playerName;
    public int maxHP = 100;
    public int currentHP;

    [Header("AI Settings (only for bot)")]
    [Range(0f, 1f)] public float aggression = 0.6f;  // шанс атаковать
    [Range(0f, 1f)] public float healBias = 0.4f;    // склонность к хилу

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int value)
    {
        currentHP = Mathf.Clamp(currentHP - value, 0, maxHP);
    }

    public void Heal(int value)
    {
        currentHP = Mathf.Clamp(currentHP + value, 0, maxHP);
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }

    public float HealthPercent()
    {
        return (float)currentHP / maxHP;
    }
}
