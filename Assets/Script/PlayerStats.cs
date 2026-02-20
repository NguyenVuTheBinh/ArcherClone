using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    // ==========================================
    //               1. LEVEL / XP SYSTEM
    // ==========================================
    [Header("XP System")]
    public int currentLevel = 1;
    public float currentExp = 0;
    public float expToNextLevel = 100;
    public float passiveXpRate = 5f;

    [Header("Combat Upgrades")]
    public int multishotCount = 0; // 0 = normal
    public int frontArrowCount = 0; // <--- NEW: For straight-line arrows
    public bool hasRicochet = false;
    public bool hasFire = false;
    public bool hasIce = false;

    [Header("Combat Stats")]
    public int damage = 1;
    public float attackSpeedModifier = 1f; // <--- NEW: 1.0 is base speed

    [Header("UI References")]
    public Slider xpSlider;
    public LevelUpManager levelManager; //

    // ==========================================
    //               2. HEALTH SYSTEM
    // ==========================================
    [Header("Health System")]
    public int maxHealth = 100;
    public int currentHealth;
    public FloatingHealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    void Update()
    {
        AddExp(passiveXpRate * Time.deltaTime);
    }

    public void AddExp(float amount)
    {
        currentExp += amount;
        if (xpSlider != null) xpSlider.value = currentExp / expToNextLevel;
        if (currentExp >= expToNextLevel) LevelUp();
    }

    void LevelUp()
    {
        currentLevel++;
        currentExp = 0;
        expToNextLevel *= 1.2f;
        if (levelManager != null) levelManager.ShowLevelUpOptions();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    void Die()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0;
    }
}