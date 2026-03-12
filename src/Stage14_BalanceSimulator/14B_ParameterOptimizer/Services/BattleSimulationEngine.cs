namespace _14B_ParameterOptimizer.Services;

/// <summary>
/// 전투 시뮬레이션 엔진 (파라미터 최적화용)
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
            var isCrit = _random.NextDouble() < attacker.CritChance;
            var dmg = attacker.Damage * (isCrit ? attacker.CritMultiplier : 1);
            defenderHp -= (int)(dmg * (1 - defender.DefenseReduction));

            if (defenderHp <= 0) break;

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
    /// 몬테카를로 시뮬레이션
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
    public string Name { get; set; } = "";
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
    public double BalanceScore { get; set; }
}

public class GridSearchResult
{
    public BattleParams Params { get; set; } = new();
    public SimulationResult Result { get; set; } = new();
    public double BalanceScore { get; set; }
}

/// <summary>
/// 밸런스 점수 계산기
/// </summary>
public class BalanceScorer
{
    /// <summary>
    /// 밸런스 점수 계산 (0-100, 100 이 완벽한 밸런스)
    /// </summary>
    public static double CalculateBalance(SimulationResult result)
    {
        var winRateDiff = Math.Abs(result.AttackerWinRate - result.DefenderWinRate);
        var idealDiff = 0;
        var maxDiff = 50;
        
        var winRateScore = Math.Max(0, 100 - (winRateDiff / maxDiff * 100));
        
        return winRateScore;
    }
    
    public static string GetBalanceRating(double score)
    {
        return score switch
        {
            >= 90 => "🟢 Perfect",
            >= 80 => "🟢 Excellent",
            >= 70 => "🟡 Good",
            >= 60 => "🟡 Fair",
            >= 50 => "🟠 Poor",
            _ => "🔴 Bad"
        };
    }
}

/// <summary>
/// 그리드 서치 최적화 서비스
/// </summary>
public class GridSearchOptimizer
{
    private readonly BattleSimulationEngine _engine = new();
    
    public List<GridSearchResult> RunGridSearch(
        BattleParams baseParams,
        Dictionary<string, List<double>> paramRanges,
        int iterations = 1000)
    {
        var results = new List<GridSearchResult>();
        
        var hpValues = paramRanges.ContainsKey("HP") ? paramRanges["HP"] : new List<double> { 1.0 };
        var dmgValues = paramRanges.ContainsKey("Damage") ? paramRanges["Damage"] : new List<double> { 1.0 };
        var critValues = paramRanges.ContainsKey("CritChance") ? paramRanges["CritChance"] : new List<double> { 1.0 };
        
        foreach (var hpMult in hpValues)
        {
            foreach (var dmgMult in dmgValues)
            {
                foreach (var critMult in critValues)
                {
                    var testParams = new BattleParams
                    {
                        HP = (int)(baseParams.HP * hpMult),
                        Damage = (int)(baseParams.Damage * dmgMult),
                        CritChance = baseParams.CritChance * critMult,
                        CritMultiplier = baseParams.CritMultiplier,
                        DefenseReduction = baseParams.DefenseReduction,
                        Name = $"HP×{hpMult:F1}/DMG×{dmgMult:F1}/Crit×{critMult:F1}"
                    };
                    
                    var defender = new BattleParams
                    {
                        HP = baseParams.HP,
                        Damage = baseParams.Damage,
                        CritChance = baseParams.CritChance,
                        CritMultiplier = baseParams.CritMultiplier,
                        DefenseReduction = baseParams.DefenseReduction,
                        Name = "Base"
                    };
                    
                    var result = _engine.RunMonteCarlo(testParams, defender, iterations);
                    result.BalanceScore = BalanceScorer.CalculateBalance(result);
                    
                    results.Add(new GridSearchResult
                    {
                        Params = testParams,
                        Result = result,
                        BalanceScore = result.BalanceScore
                    });
                }
            }
        }
        
        return results.OrderByDescending(r => r.BalanceScore).ToList();
    }
}
