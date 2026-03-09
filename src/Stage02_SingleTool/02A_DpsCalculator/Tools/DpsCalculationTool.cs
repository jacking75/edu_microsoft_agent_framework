using System.ComponentModel;

namespace _02A_DpsCalculator.Tools;

/// <summary>
/// DPS(Damage Per Second) 계산을 위한 Tool
/// </summary>
public class DpsCalculationTool
{
    /// <summary>
    /// 무기의 DPS 를 계산합니다
    /// </summary>
    [Description("무기의 초당 데미지 (DPS) 를 계산합니다. 공격력과 초당 공격 속도를 입력받습니다.")]
    public int CalculateDps(
        [Description("무기의 기본 공격력")] int damage, 
        [Description("초당 공격 속도 (예: 1.5 는 초당 1.5 회 공격")] double attacksPerSecond)
    {
        return (int)(damage * attacksPerSecond);
    }

    /// <summary>
    /// 크리티컬을 고려한 기대 DPS 를 계산합니다
    /// </summary>
    [Description("크리티컬 확률과 배율을 고려한 기대 DPS 를 계산합니다.")]
    public double CalculateExpectedDps(
        [Description("기본 DPS (공격력 × 공격속도)")] int baseDps, 
        [Description("크리티컬 확률 (0.0~1.0, 예: 0.2 는 20%)")] double critChance, 
        [Description("크리티컬 데미지 배율 (예: 2.0 은 2 배)")] double critMultiplier)
    {
        // 기대값 = 기본 DPS * (1 + 크리티컬 확률 * (배율 - 1))
        return baseDps * (1 + (critChance * (critMultiplier - 1)));
    }
}
