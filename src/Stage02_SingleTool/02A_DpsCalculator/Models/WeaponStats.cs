namespace _02A_DpsCalculator.Models;

/// <summary>
/// 무기 스탯 모델
/// </summary>
public class WeaponStats
{
    public string Name { get; set; } = "";
    public int Damage { get; set; }
    public double AttacksPerSecond { get; set; }
    public double CritChance { get; set; }
    public double CritMultiplier { get; set; }
}
