using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyAI))] // Added this requirement
public class EnemyEvolution : MonoBehaviour
{
    [Header("Leveling Settings")]
    public int currentLevel = 1;
    public int maxLevel = 20; // Cap at Level 20

    public float currentXp = 0;
    public float xpToNextLevel = 20f;
    public float passiveXpRate = 5f;

    [Header("Stat Buffs")]
    public float healthBuff = 10f; // Made float to match your new AI/Health
    public float damageBuff = 5f;

    private EnemyHealth _healthScript;
    private EnemyAI _aiScript; // Reference to the new AI brain

    // We can't tint these new 3D models with a simple Renderer color change easily
    // because they have complex materials. We'll stick to scaling for now!

    void Awake()
    {
        _healthScript = GetComponent<EnemyHealth>();
        _aiScript = GetComponent<EnemyAI>();
    }

    void Update()
    {
        // Stop gaining XP if we hit the max level
        if (currentLevel >= maxLevel) return;

        currentXp += passiveXpRate * Time.deltaTime;

        if (currentXp >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentXp = 0;
        xpToNextLevel *= 1.2f;

        // 1. Buff Max Health and Heal slightly
        if (_healthScript != null)
        {
            // Convert the float buff into a whole number
            int hpBuff = Mathf.RoundToInt(healthBuff);

            _healthScript.maxHealth += hpBuff;
            _healthScript.Heal(hpBuff); // Use your existing Heal function safely!
        }

        // 2. Buff Damage in the AI script
        if (_aiScript != null)
        {
            _aiScript.damage += damageBuff;
        }

        // 3. Size Growth Milestone
        int mileStoneLevel = 5;
        if (currentLevel % mileStoneLevel == 0)
        {
            transform.localScale *= 1.15f;
            if (_aiScript != null) _aiScript.moveSpeed *= 0.95f;
        }

        Debug.Log(gameObject.name + " Evolved to Level " + currentLevel + "!");
    }

    // Note: I removed OnCollisionEnter because the EnemyAI script now handles 
    // dealing damage during the Animation Event (PerformAttack).
}