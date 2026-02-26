using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Required for reloading the scene

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
    public int multishotCount = 0;
    public int frontArrowCount = 0;
    public bool hasRicochet = false;
    public bool hasFire = false;
    public bool hasIce = false;

    [Header("Combat Stats")]
    public int damage = 1;
    public float attackSpeedModifier = 1f;
    public float critChance = 0.05f;
    public float critMultiplier = 2f;
    public bool hasLifeSteal = false;

    [Header("UI References")]
    public Slider xpSlider;
    public LevelUpManager levelManager;

    // ==========================================
    //               2. HEALTH SYSTEM
    // ==========================================
    [Header("Health System")]
    public int maxHealth = 100;
    public int currentHealth;
    public FloatingHealthBar healthBar;

    [Header("Game Over UI")]
    public GameObject gameOverCanvas; // Drag your GameOverCanvas here

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

        // Ensure Game Over screen is hidden at start
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        AddExp(passiveXpRate * Time.deltaTime);
    }

    // --- XP FUNCTIONS ---
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

    // --- HEALTH FUNCTIONS ---
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
        Time.timeScale = 0; // Pause the game

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true); // Show Game Over Screen
        }
    }

    // --- RESTART FUNCTION ---
    public void RestartGame()
    {
        Time.timeScale = 1; // UNPAUSE the game before reloading

        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // --- RETURN TO TITLE FUNCTION ---
    public void ReturnToTitle()
    {
        Time.timeScale = 1; // UNPAUSE the game so the menu works correctly

        // Loads the scene at index 0 (UI-Start)
        SceneManager.LoadScene(0);
    }
}