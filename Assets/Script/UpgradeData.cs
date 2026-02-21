using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Visuals")]
    public string upgradeName;
    public string description;
    public Sprite icon; // You can drag an image here later

    [Header("Stats")]
    public StatType statType;
    public float amount;
}

// This simple list helps us know what to upgrade
public enum StatType
{
    // Basic Stats
    MaxHealth,
    MoveSpeed,
    Damage,
    AttackSpeed,

    // NEW: Critical and Survival Modifiers
    CritChance,
    CritDamage,
    LifeSteal,

    // Weapon Modifiers
    Multishot,
    FrontArrow,
    DiagonalArrow,
    Ricochet,

    // Elements
    FireElement,
    IceElement
}