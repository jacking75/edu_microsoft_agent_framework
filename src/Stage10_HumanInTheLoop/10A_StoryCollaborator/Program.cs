// Copyright (c) Microsoft. All rights reserved.

using _10A_StoryCollaborator.Agents;
using _10A_StoryCollaborator.Services;
using _10A_StoryCollaborator.Workflows;

// ==========================================
// 10 단계 A: 스토리 작성 협업 시스템
// ==========================================
// 학습 목표:
// 1. Human-in-the-Loop 패턴 구현
// 2. Checkpointing (상태 저장)
// 3. Time-travel (상태 복원)
// 4. AI-인간 협업 워크플로우
// ==========================================

Console.WriteLine("📖 스토리 작성 협업 시스템에 오신 것을 환영합니다!");
Console.WriteLine("AI 와 인간이 함께 스토리를 작성합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var checkpointService = new CheckpointService();
var draftAgent = new DraftAgent(apiKey, baseUrl);
var continuationAgent = new ContinuationAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new StoryCollaborationWorkflow(
    draftAgent, 
    continuationAgent, 
    checkpointService
);

Console.WriteLine("✅ Human-in-the-Loop 시스템이 초기화되었습니다.");
Console.WriteLine("   ✍️ DraftAgent → 👤 Human Review → ✍️ ContinuationAgent\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"판타지 모험물\"");
Console.WriteLine("  - \"SF 스릴러\"");
Console.WriteLine("  - \"로맨스 드라마\"\n");

Console.WriteLine("스토리의 테마를 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var theme = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(theme) || 
        theme.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        theme.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // Human-in-the-Loop 워크플로우 실행
        var result = await workflow.ExecuteAsync(theme);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine($"📖 스토리: {result.Theme}");
        Console.WriteLine(new string('=', 70));
        
        Console.WriteLine("\n✍️ 초안:");
        Console.WriteLine(result.InitialDraft);
        
        if (result.Revisions.Any())
        {
            Console.WriteLine("\n✏️ 수정본:");
            Console.WriteLine(result.Revisions.Last());
        }
        
        Console.WriteLine("\n📊 통계:");
        Console.WriteLine($"   총 체크포인트: {result.TotalCheckpoints}개");
        Console.WriteLine($"   생성일: {result.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();

        // 체크포인트 목록 출력
        var checkpoints = checkpointService.GetAllCheckpoints();
        Console.WriteLine("💾 저장된 체크포인트:");
        foreach (var cp in checkpoints)
        {
            var status = cp.IsApproved ? "✅" : "⏳";
            Console.WriteLine($"   {status} 체크포인트 {cp.Id}: {cp.CreatedAt:HH:mm:ss}");
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
                Console.Write("새로운 진행 방향을 입력하세요: ");
                var newDirection = Console.ReadLine() ?? "새로운 전개를 만들어주세요.";
                
                var travelResult = await workflow.TravelAndContinueAsync(targetId, newDirection);
                
                Console.WriteLine("\n📖 시간 이동 후 새로운 스토리:");
                Console.WriteLine(travelResult.FinalStory);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
