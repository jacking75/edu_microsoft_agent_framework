// Copyright (c) Microsoft. All rights reserved.

using _14B_ParameterOptimizer.Agents;
using _14B_ParameterOptimizer.Services;

// ==========================================
// 14 단계 B: 파라미터 최적화
// ==========================================
// 학습 목표:
// 1. 그리드 서치 최적화
// 2. 다중 파라미터 분석
// 3. 밸런스 점수 계산
// 4. 결과 시각화
// ==========================================

Console.WriteLine("🎯 파라미터 최적화 시스템에 오신 것을 환영합니다!");
Console.WriteLine("그리드 서치로 최적 밸런스 파라미터를 찾습니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var optimizer = new GridSearchOptimizer();
var optimizationAgent = new ParameterOptimizationAgent(apiKey, baseUrl);

Console.WriteLine("✅ 최적화 엔진이 초기화되었습니다.\n");

// 기본 파라미터 설정
var baseParams = new BattleParams
{
    HP = 1000,
    Damage = 100,
    CritChance = 0.15,
    CritMultiplier = 2.0,
    DefenseReduction = 0.2,
    Name = "Base"
};

Console.WriteLine("📊 기본 파라미터:");
Console.WriteLine($"  HP: {baseParams.HP}, Damage: {baseParams.Damage}");
Console.WriteLine($"  Crit: {baseParams.CritChance:P0}, Multiplier: {baseParams.CritMultiplier}x");
Console.WriteLine();

// 그리드 서치 범위 설정
Console.WriteLine("🔍 그리드 서치 범위:");
var paramRanges = new Dictionary<string, List<double>>
{
    { "HP", new List<double> { 0.8, 0.9, 1.0, 1.1, 1.2 } },
    { "Damage", new List<double> { 0.8, 0.9, 1.0, 1.1, 1.2 } },
    { "CritChance", new List<double> { 0.8, 1.0, 1.2 } }
};

foreach (var range in paramRanges)
{
    Console.WriteLine($"  {range.Key}: {string.Join(", ", range.Value.Select(v => $"×{v:F1}"))}");
}
Console.WriteLine($"  총 조합: {paramRanges["HP"].Count * paramRanges["Damage"].Count * paramRanges["CritChance"].Count}개");
Console.WriteLine();

// 그리드 서치 실행
Console.WriteLine("🎯 그리드 서치 실행 중... (각 조합 1000 회 시뮬레이션)");
var results = optimizer.RunGridSearch(baseParams, paramRanges, iterations: 1000);
Console.WriteLine($"✅ {results.Count}개 조합 분석 완료\n");

// 결과 출력
Console.WriteLine("📊 Top 10 결과:");
Console.WriteLine(new string('-', 80));
foreach (var (result, index) in results.Take(10).Select((r, i) => (r, i)))
{
    var rating = BalanceScorer.GetBalanceRating(result.BalanceScore);
    Console.WriteLine($"{index + 1,2}. {result.Params.Name,-35} {rating,-15} {result.BalanceScore:F1}점");
    Console.WriteLine($"    승률: {result.Result.AttackerWinRate:F1}% vs {result.Result.DefenderWinRate:F1}%, " +
        $"평균라운드: {result.Result.AverageRounds:F1}");
}
Console.WriteLine();

// 밸런스 점수 분포
Console.WriteLine("📈 밸런스 점수 분포:");
var distribution = results.GroupBy(r => (int)(r.BalanceScore / 10) * 10)
    .OrderByDescending(g => g.Key)
    .ToDictionary(g => g.Key, g => g.Count());

foreach (var kvp in distribution)
{
    var bar = new string('█', (int)(kvp.Value / (double)results.Count * 40));
    Console.WriteLine($"  {kvp.Key,3}-{kvp.Key + 9,3}점: {bar} ({kvp.Value}개)");
}
Console.WriteLine();

// AI 분석
Console.WriteLine("🤖 AI 최적화 분석 중...");
var analysis = await optimizationAgent.AnalyzeOptimizationAsync(results, baseParams);
Console.WriteLine($"\n📋 최적화 분석 결과:\n{analysis}\n");

Console.WriteLine("시각화 결과를 보시겠습니까? (y/n)");
var showViz = Console.ReadLine();
if (showViz?.Equals("y", StringComparison.OrdinalIgnoreCase) == true)
{
    Console.WriteLine("\n📊 결과 시각화:");
    var visualization = await optimizationAgent.VisualizeResultsAsync(results);
    Console.WriteLine($"\n{visualization}\n");
}

Console.WriteLine("\n계속하려면 아무 키나 누르세요...");
Console.ReadKey();
