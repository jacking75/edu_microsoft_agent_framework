// Copyright (c) Microsoft. All rights reserved.

using _14C_RLBalancer.Agents;
using _14C_RLBalancer.Services;

// ==========================================
// 14 단계 C: 강화학습 밸런서
// ==========================================
// 학습 목표:
// 1. 자기플레이 (Self-Play) 구현
// 2. 메타 분석
// 3. 빌드 간 상성 관계 파악
// 4. 밸런스 패치 제안
// ==========================================

Console.WriteLine("🎮 강화학습 밸런서에 오신 것을 환영합니다!");
Console.WriteLine("자기플레이로 메타를 분석하고 밸런스를 조정합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var selfPlayRunner = new SelfPlayRunner();
var metaAnalysisAgent = new MetaAnalysisAgent(apiKey, baseUrl);

Console.WriteLine("✅ 자기플레이 엔진이 초기화되었습니다.\n");

// 사전 정의된 빌드 소개
Console.WriteLine("📊 사전 정의된 빌드:");
var builds = new[]
{
    ("Balanced", "균형형", "HP 1000, Damage 100, Crit 15%"),
    ("Tank", "탱커형", "HP 1400, Damage 80, 방어도 높음"),
    ("Glass Cannon", "유리대포형", "HP 800, Damage 150, Crit 25%"),
    ("Crit Master", "치명타특화", "Crit 35%, Multiplier 3.0"),
    ("Fortress", "요새형", "HP 1200, 방어도 40%")
};

foreach (var (name, kr, stats) in builds)
{
    Console.WriteLine($"  {name,-15} ({kr,-6}): {stats}");
}
Console.WriteLine();

// 자기플레이 실행
Console.WriteLine("🎮 자기플레이 시작...");
Console.WriteLine("   5 세대 × 20 경기 = 100 경기 시뮬레이션\n");

var matches = selfPlayRunner.RunSelfPlay(generations: 5, matchesPerGen: 20);

Console.WriteLine($"\n✅ 자기플레이 완료: {matches.Count}경기");

// 메타 분포 출력
Console.WriteLine("\n📊 메타 분포:");
var metaDistribution = matches.GroupBy(m => m.MetaType)
    .OrderByDescending(g => g.Count())
    .ToList();

foreach (var meta in metaDistribution)
{
    var bar = new string('█', (int)(meta.Count() / (double)matches.Count * 40));
    Console.WriteLine($"  {meta.Key,-15}: {bar} ({meta.Count()}경기, {meta.Count() / (double)matches.Count * 100:F1}%)");
}
Console.WriteLine();

// 빌드별 승률
Console.WriteLine("📈 빌드별 승률:");
var buildStats = matches
    .SelectMany(m => new[]
    {
        new { Build = m.Player1.BuildName, Won = m.Result.AttackerWinRate > 50 },
        new { Build = m.Player2.BuildName, Won = m.Result.DefenderWinRate > 50 }
    })
    .GroupBy(b => b.Build)
    .Select(g => new
    {
        BuildName = g.Key,
        WinRate = g.Count(b => b.Won) / (double)g.Count() * 100,
        Matches = g.Count()
    })
    .OrderByDescending(b => b.WinRate)
    .ToList();

foreach (var stat in buildStats)
{
    var indicator = stat.WinRate >= 55 ? "🔴 (OP)" : stat.WinRate <= 45 ? "🔵 (Under)" : "🟢";
    Console.WriteLine($"  {indicator} {stat.BuildName,-15}: 승률 {stat.WinRate:F1}% ({stat.Matches}경기)");
}
Console.WriteLine();

// AI 메타 분석
Console.WriteLine("🤖 AI 메타 분석 중...");
var metaAnalysis = await metaAnalysisAgent.AnalyzeMetaAsync(matches);
Console.WriteLine($"\n📋 메타 분석 결과:\n{metaAnalysis}\n");

// 밸런스 패치 제안
Console.WriteLine("🔧 밸런스 패치 제안 생성 중...");
var metaBuilds = buildStats.Select(b => new MetaAnalysis
{
    MetaName = b.BuildName,
    WinRate = b.WinRate,
    MatchesPlayed = b.Matches,
    Counters = b.WinRate >= 55 ? "N/A" : "TBD",
    WeakAgainst = b.WinRate <= 45 ? "N/A" : "TBD"
}).ToList();

var patchSuggestions = await metaAnalysisAgent.SuggestBalancePatchAsync(metaBuilds);
Console.WriteLine($"\n📝 밸런스 패치 제안:\n{patchSuggestions}\n");

Console.WriteLine("\n계속하려면 아무 키나 누르세요...");
Console.ReadKey();
