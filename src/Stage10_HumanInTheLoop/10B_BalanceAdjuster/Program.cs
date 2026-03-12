// Copyright (c) Microsoft. All rights reserved.

using _10B_BalanceAdjuster.Agents;
using _10B_BalanceAdjuster.Services;
using _10B_BalanceAdjuster.Workflows;

// ==========================================
// 10 단계 B: 밸런스 조정 워크플로우
// ==========================================
// 학습 목표:
// 1. Human-in-the-Loop 패턴 구현 (밸런스 조정)
// 2. Checkpointing (상태 저장)
// 3. Time-travel (상태 복원)
// 4. AI 제안 → 인간검토 → 시뮬레이션
// ==========================================

Console.WriteLine("⚖️ 밸런스 조정 워크플로우에 오신 것을 환영합니다!");
Console.WriteLine("AI 와 인간 디자이너가 함께 게임 밸런스를 조정합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var checkpointService = new CheckpointService();
var balanceAgent = new BalanceAgent(apiKey, baseUrl);
var designerReviewAgent = new DesignerReviewAgent(apiKey, baseUrl);
var simulationAgent = new SimulationAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new BalanceAdjustmentWorkflow(
    balanceAgent, 
    designerReviewAgent, 
    simulationAgent, 
    checkpointService
);

Console.WriteLine("✅ Human-in-the-Loop 시스템이 초기화되었습니다.");
Console.WriteLine("   💡 BalanceAgent → 👤 DesignerReview → 🔬 Simulation\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"블레이드 클래스: 공격력 120, 방어력 80, HP 1000\"");
Console.WriteLine("  - \"파이어볼 스킬: 데미지 250, 마나소모 50, 쿨타임 3 초\"");
Console.WriteLine("  - \"전설 무기: 확률 0.1%, 공격력 500, 특수효과: 흡혈 10%\"\n");

Console.WriteLine("밸런스 조정이 필요한 게임 데이터를 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var gameData = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(gameData) ||
        gameData.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        gameData.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    Console.WriteLine("\n디자인 의도를 입력하세요 (예: 초반 약함, 후반 강함, 공평한 PVP 등):");
    Console.Write("🎯 디자인의도: ");
    var designIntent = Console.ReadLine() ?? "밸런스 있고 공정한 게임플레이";

    try
    {
        // Human-in-the-Loop 워크플로우 실행
        var result = await workflow.ExecuteAsync(gameData, designIntent);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("⚖️ 밸런스 조정 결과");
        Console.WriteLine(new string('=', 70));
        
        Console.WriteLine("\n💡 AI 제안:");
        Console.WriteLine(result.BalanceProposal);
        
        Console.WriteLine("\n👤 디자이너 검토:");
        Console.WriteLine(result.DesignerReview);
        
        Console.WriteLine("\n🔬 시뮬레이션 결과:");
        Console.WriteLine(result.SimulationResult);
        
        Console.WriteLine("\n📊 최종 권고:");
        Console.WriteLine(result.FinalRecommendation);
        
        Console.WriteLine($"\n📅 처리일: {result.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();

        // 체크포인트 목록 출력
        var checkpoints = checkpointService.GetAllCheckpoints();
        Console.WriteLine("💾 저장된 체크포인트:");
        foreach (var cp in checkpoints)
        {
            var status = cp.IsApproved ? "✅" : "⏳";
            Console.WriteLine($"   {status} 체크포인트 {cp.Id} ({cp.ContentType}): {cp.CreatedAt:HH:mm:ss}");
        }
        Console.WriteLine();

        // 시간 이동 데모 (옵션)
        Console.WriteLine("⏰ 시간 이동 기능을 사용하시겠습니까? (y/n)");
        var travelChoice = Console.ReadLine();
        
        if (travelChoice?.Equals("y", StringComparison.OrdinalIgnoreCase) == true && checkpoints.Count > 1)
        {
            Console.Write("어느 체크포인트로 이동할까요? (번호 입력): ");
            if (int.TryParse(Console.ReadLine(), out var targetId))
            {
                Console.Write("새로운 피드백/의견을 입력하세요: ");
                var newFeedback = Console.ReadLine() ?? "다른 관점에서 검토해주세요.";
                
                var travelResult = await workflow.TravelAndReReviewAsync(targetId, newFeedback);
                
                Console.WriteLine("\n⚖️ 시간 이동 후 새로운 결과:");
                Console.WriteLine(travelResult.SimulationResult);
                Console.WriteLine($"\n📊 새로운 최종 권고: {travelResult.FinalRecommendation}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
