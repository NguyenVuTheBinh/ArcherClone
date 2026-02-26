using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelUpPanel;
    public Button[] optionButtons;
    public TMP_Text[] optionTexts;
    public Image[] optionIcons; // Drag the Image components of your 3 button icons here

    [Header("Data")]
    public UpgradeData[] allUpgrades; // Drag your ScriptableObjects here

    private PlayerStats playerStats;
    private MobilePlayer playerMovement;

    void Start()
    {
        levelUpPanel.SetActive(false);

        // Find the player scripts automatically
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerMovement = FindFirstObjectByType<MobilePlayer>();
    }

    public void ShowLevelUpOptions()
    {
        // 1. Pause the game
        Time.timeScale = 0;
        levelUpPanel.SetActive(true);

        // Create a temporary copy of your "Deck" so we can remove cards from it
        List<UpgradeData> deck = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            // Safety Check: Do we have enough upgrades left in the deck?
            if (deck.Count == 0)
            {
                optionButtons[i].gameObject.SetActive(false);
                continue;
            }

            optionButtons[i].gameObject.SetActive(true);

            // Pick a random index from the REMAINING cards
            int randomIndex = Random.Range(0, deck.Count);
            UpgradeData data = deck[randomIndex];

            // REMOVE it from the deck so it can't be picked again this round
            deck.RemoveAt(randomIndex);

            // Setup the text
            if (optionTexts[i] != null)
            {
                optionTexts[i].text = "<b>" + data.upgradeName + "</b>\n" + data.description;
            }

            // --- ICON LOGIC ---
            // Assign the sprite from your UpgradeData asset to the UI Image
            if (optionIcons[i] != null && data.icon != null)
            {
                optionIcons[i].sprite = data.icon;
                optionIcons[i].gameObject.SetActive(true);
            }

            // Click Event
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => SelectUpgrade(data));
        }
    }

    void SelectUpgrade(UpgradeData upgrade)
    {
        ApplyStats(upgrade);

        // Resume
        levelUpPanel.SetActive(false);
        Time.timeScale = 1;
    }

    void ApplyStats(UpgradeData data)
    {
        Debug.Log("Selected: " + data.upgradeName);

        if (playerStats == null) return;

        switch (data.statType)
        {
            // --- BASIC STATS ---
            case StatType.MaxHealth:
                playerStats.maxHealth += (int)data.amount;
                playerStats.Heal((int)data.amount);
                break;

            case StatType.Damage:
                playerStats.damage += (int)data.amount;
                break;

            case StatType.MoveSpeed:
                if (playerMovement != null)
                {
                    playerMovement.moveSpeed += data.amount;
                }
                break;

            // --- COMBAT PASSIVES ---
            case StatType.AttackSpeed:
                playerStats.attackSpeedModifier += data.amount;
                break;

            case StatType.CritChance:
                playerStats.critChance += data.amount;
                break;

            case StatType.LifeSteal:
                playerStats.hasLifeSteal = true;
                break;

            // --- PROJECTILE UPGRADES ---
            case StatType.Multishot:
                playerStats.multishotCount++;
                break;

            case StatType.FrontArrow:
                playerStats.frontArrowCount++;
                break;

            case StatType.Ricochet:
                playerStats.hasRicochet = true;
                break;
        }
    }
}