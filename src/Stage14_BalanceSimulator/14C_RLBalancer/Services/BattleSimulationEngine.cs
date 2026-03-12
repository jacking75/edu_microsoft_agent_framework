namespace _14C_RLBalancer.Services;

/// <summary>
/// 전투 시뮬레이션 엔진 (RL 용)
/// </summary>
public class BattleSimulationEngine
{
    private readonly Random _random = new();

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
            Rounds = rounds
        };
    }

    public SimulationResult RunMonteCarlo(BattleParams attacker, BattleParams defender, int iterations = 1000)
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
    public string BuildName { get; set; } = "Default";
}

public class BattleResult
{
    public string Winner { get; set; } = "";
    public int Rounds { get; set; }
}

public class SimulationResult
{
    public int Iterations { get; set; }
    public double AttackerWinRate { get; set; }
    public double DefenderWinRate { get; set; }
    public double DrawRate { get; set; }
    public double AverageRounds { get; set; }
}

/// <summary>
/// 자기플레이 경기 기록
/// </summary>
public class SelfPlayMatch
{
    public BattleParams Player1 { get; set; } = new();
    public BattleParams Player2 { get; set; } = new();
    public SimulationResult Result { get; set; } = new();
    public string MetaType { get; set; } = "";
}

/// <summary>
/// 메타 분석 결과
/// </summary>
public class MetaAnalysis
{
    public string MetaName { get; set; } = "";
    public BattleParams Params { get; set; } = new();
    public double WinRate { get; set; }
    public int MatchesPlayed { get; set; }
    public string Counters { get; set; } = "";
    public string WeakAgainst { get; set; } = "";
}

/// <summary>
/// 자기플레이 러너
/// </summary>
public class SelfPlayRunner
{
    private readonly BattleSimulationEngine _engine = new();
    private readonly Random _random = new();

    public List<SelfPlayMatch> RunSelfPlay(int generations = 5, int matchesPerGen = 20)
    {
        var matches = new List<SelfPlayMatch>();
        var builds = GenerateBuilds();

        for (int gen = 0; gen < generations; gen++)
        {
            Console.WriteLine($"\n🎮 Generation {gen + 1}/{generations}");
            
            for (int m = 0; m < matchesPerGen; m++)
            {
                var p1 = builds[_random.Next(builds.Count)];
                var p2 = builds[_random.Next(builds.Count)];
                
                var result = _engine.RunMonteCarlo(p1, p2, iterations: 500);
                
                matches.Add(new SelfPlayMatch
                {
                    Player1 = p1,
                    Player2 = p2,
                    Result = result,
                    MetaType = IdentifyMeta(p1, p2, result)
                });
            }

            EvolveBuilds(builds, matches);
        }

        return matches;
    }

    private List<BattleParams> GenerateBuilds()
    {
        return new List<BattleParams>
        {
            new() { HP = 1000, Damage = 100, CritChance = 0.15, CritMultiplier = 2.0, DefenseReduction = 0.2, BuildName = "Balanced" },
            new() { HP = 1400, Damage = 80, CritChance = 0.1, CritMultiplier = 1.8, DefenseReduction = 0.3, BuildName = "Tank" },
            new() { HP = 800, Damage = 150, CritChance = 0.25, CritMultiplier = 2.5, DefenseReduction = 0.1, BuildName = "Glass Cannon" },
            new() { HP = 1100, Damage = 90, CritChance = 0.35, CritMultiplier = 3.0, DefenseReduction = 0.15, BuildName = "Crit Master" },
            new() { HP = 1200, Damage = 70, CritChance = 0.1, CritMultiplier = 2.0, DefenseReduction = 0.4, BuildName = "Fortress" }
        };
    }

    private string IdentifyMeta(BattleParams p1, BattleParams p2, SimulationResult result)
    {
        if (result.AttackerWinRate > 55) return p1.BuildName;
        if (result.DefenderWinRate > 55) return p2.BuildName;
        return "Balanced";
    }

    private void EvolveBuilds(List<BattleParams> builds, List<SelfPlayMatch> matches)
    {
        var winRates = builds.Select(b => new
        {
            Build = b,
            WinRate = matches.Where(m => m.Player1.BuildName == b.BuildName && m.Result.AttackerWinRate > 50 ||
                                         m.Player2.BuildName == b.BuildName && m.Result.DefenderWinRate > 50)
                             .Count() / (double)matches.Count(m => m.Player1.BuildName == b.BuildName || m.Player2.BuildName == b.BuildName) * 100
        }).ToList();
    }
}
