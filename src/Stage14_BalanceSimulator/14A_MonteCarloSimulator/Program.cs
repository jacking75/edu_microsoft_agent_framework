// Copyright (c) Microsoft. All rights reserved.

using _14A_MonteCarloSimulator.Agents;
using _14A_MonteCarloSimulator.Services;

// ==========================================
// 14 단계 A: 몬테카를로 시뮬레이터
// ==========================================
// 학습 목표:
// 1. 몬테카를로 시뮬레이션 구현
// 2. 대량 시뮬레이션 실행
// 3. 결과 분석 및 해석
// 4. 밸런스 조정 제안
// ==========================================

Console.WriteLine("🎲 몬테카를로 전투 시뮬레이터에 오신 것을 환영합니다!");
Console.WriteLine("수만 번의 시뮬레이션으로 밸런스를 분석합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var simulationEngine = new BattleSimulationEngine();
var analysisAgent = new MonteCarloAnalysisAgent(apiKey, baseUrl);

Console.WriteLine("✅ 시뮬레이션 엔진이 초기화되었습니다.\n");

// 데모용 전투 파라미터
Console.WriteLine("📊 기본 전투 파라미터:");
var attacker = new BattleParams 
{ 
    HP = 1000, 
    Damage = 100, 
    CritChance = 0.15, 
    CritMultiplier = 2.0,
    DefenseReduction = 0.2
};
var defender = new BattleParams 
{ 
    HP = 1200, 
    Damage = 80, 
    CritChance = 0.1, 
    CritMultiplier = 2.5,
    DefenseReduction = 0.25
};

Console.WriteLine($"공격자: HP {attacker.HP}, Damage {attacker.Damage}, Crit {attacker.CritChance * 100}%");
Console.WriteLine($"수비자: HP {defender.HP}, Damage {defender.Damage}, Crit {defender.CritChance * 100}%");
Console.WriteLine();

// 몬테카를로 시뮬레이션 실행
Console.WriteLine("🎲 몬테카를로 시뮬레이션 실행 중... (10,000 회)");
var result = simulationEngine.RunMonteCarlo(attacker, defender, iterations: 10000);

Console.WriteLine($"\n📊 시뮬레이션 결과:");
Console.WriteLine($"  공격자 승률: {result.AttackerWinRate:F2}%");
Console.WriteLine($"  수비자 승률: {result.DefenderWinRate:F2}%");
Console.WriteLine($"  무승부: {result.DrawRate:F2}%");
Console.WriteLine($"  평균 라운드: {result.AverageRounds:F2}");
Console.WriteLine();

// AI 분석
Console.WriteLine("🤖 AI 분석 중...");
var analysis = await analysisAgent.AnalyzeResultAsync(result);

Console.WriteLine($"\n📋 분석 결과:\n{analysis}\n");

Console.WriteLine("계속하려면 아무 키나 누르세요...");
Console.ReadKey();
