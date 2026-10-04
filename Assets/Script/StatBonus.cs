using System;

public enum StatType
{
    MaxHp,
    MaxMp,
    MaxStamina,
    Attack,
    SpellPower,
    Defense
}

public enum ModifierType
{
    Flat,      // +10
    Percent    // +0.1 = +10%
}

[Serializable]
public struct StatBonus
{
    public StatType stat;
    public ModifierType modifierType;
    public float value;
}