using System;

public enum StatsType
{
    MaxHp,
    Atk,
    Def,
    Gold
    
}
[Serializable]
public class LegacyPoint
{
    public StatsType statsType;
    public int Value;
    public int payCost;
}
