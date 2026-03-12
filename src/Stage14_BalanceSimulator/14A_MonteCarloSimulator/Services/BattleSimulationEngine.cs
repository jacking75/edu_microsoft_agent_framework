namespace _14A_MonteCarloSimulator.Services;

/// <summary>
/// 전투 시뮬레이션 엔진 (몬테카를로)
/// </summary>
public class BattleSimulationEngine
{
    private readonly Random _random = new();

    /// <summary>
    ///单次 전투 시뮬레이션
    /// </summary>
    public BattleResult SimulateBattle(BattleParams attacker, BattleParams defender)
    {
        var attackerHp = attacker.HP;
        var defenderHp = defender.HP;
        int rounds = 0;

        while (attackerHp > 0 && defenderHp > 0 && rounds < 100)
        {
            // Attacker attack
            var isCrit = _random.NextDouble() < attacker.CritChance;
            var dmg = attacker.Damage * (isCrit ? attacker.CritMultiplier : 1);
            defenderHp -= (int)(dmg * (1 - defender.DefenseReduction));

            if (defenderHp <= 0) break;

            // Defender counter-attack
            isCrit = _random.NextDouble() < defender.CritChance;
            dmg = defender.Damage * (isCrit ? defender.CritMultiplier : 1);
            attackerHp -= (int)(dmg * (1 - attacker.DefenseReduction));

            rounds++;
        }

        return new BattleResult
        {
            Winner = attackerHp > 0 ? "Attacker" : defenderHp > 0 ? "Defender" : "Draw",
            Rounds = rounds,
            AttackerRemainingHP = Math.Max(0, attackerHp),
            DefenderRemainingHP = Math.Max(0, defenderHp)
        };
    }

    /// <summary>
    /// 몬테카를로 시뮬레이션 (N 회 반복)
    /// </summary>
    public SimulationResult RunMonteCarlo(BattleParams attacker, BattleParams defender, int iterations = 10000)
    {
        var attackerWins = 0;
        var defenderWins = 0;
        var draws = 0;
        var totalRounds = 0;

        for (int i = 0; i < iterations; i++)
        {
            var result = SimulateBattle(attacker, defender);
            
            if (result.Winner == "Attacker") attackerWins++;
            else if (result.Winner == "Defender") defenderWins++;
            else draws++;
            
            totalRounds += result.Rounds;
        }

        return new SimulationResult
        {
            Iterations = iterations,
            AttackerWinRate = (double)attackerWins / iterations * 100,
            DefenderWinRate = (double)defenderWins / iterations * 100,
            DrawRate = (double)draws / iterations * 100,
            AverageRounds = (double)totalRounds / iterations
        };
    }
}

public class BattleParams
{
    public int HP { get; set; } = 1000;
    public int Damage { get; set; } = 100;
    public double CritChance { get; set; } = 0.1;
    public double CritMultiplier { get; set; } = 2.0;
    public double DefenseReduction { get; set; } = 0.2;
}

public class BattleResult
{
    public string Winner { get; set; } = "";
    public int Rounds { get; set; }
    public int AttackerRemainingHP { get; set; }
    public int DefenderRemainingHP { get; set; }
}

public class SimulationResult
{
    public int Iterations { get; set; }
    public double AttackerWinRate { get; set; }
    public double DefenderWinRate { get; set; }
    public double DrawRate { get; set; }
    public double AverageRounds { get; set; }
}
