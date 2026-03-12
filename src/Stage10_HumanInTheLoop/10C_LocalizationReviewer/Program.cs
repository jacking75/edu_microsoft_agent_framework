// Copyright (c) Microsoft. All rights reserved.

using _10C_LocalizationReviewer.Agents;
using _10C_LocalizationReviewer.Services;
using _10C_LocalizationReviewer.Workflows;

// ==========================================
// 10 단계 C: 로컬라이제이션 검수 워크플로우
// ==========================================
// 학습 목표:
// 1. Human-in-the-Loop 패턴 구현 (로컬라이제이션)
// 2. Checkpointing (상태 저장)
// 3. Time-travel (상태 복원)
// 4. AI 번역 → 인간검토 → 수정반영
// ==========================================

Console.WriteLine("🌐 로컬라이제이션 검수 워크플로우에 오신 것을 환영합니다!");
Console.WriteLine("AI 번역을 인간이 검수하고 수정합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var checkpointService = new CheckpointService();
var translatorAgent = new TranslatorAgent(apiKey, baseUrl);
var humanReviewerAgent = new HumanReviewerAgent(apiKey, baseUrl);
var revisionAgent = new RevisionAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new LocalizationReviewWorkflow(
    translatorAgent, 
    humanReviewerAgent, 
    revisionAgent, 
    checkpointService
);

Console.WriteLine("✅ Human-in-the-Loop 시스템이 초기화되었습니다.");
Console.WriteLine("   🔤 Translator → 👤 HumanReviewer → ✏️ Revision\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"Welcome to the game! Start your adventure now.\" (영어→한국어)");
Console.WriteLine("  - \"Attack\", \"Defense\", \"Magic Power\" (스킬명 번역)");
Console.WriteLine("  - \"The dragon awakens from its thousand-year slumber.\" (스토리 텍스트)\n");

Console.WriteLine("번역할 원문 텍스트를 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var originalText = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(originalText) ||
        originalText.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        originalText.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    Console.WriteLine("\n목표 언어를 입력하세요 (예: 한국어, 일본어, 중국어, 스페인어 등):");
    Console.Write("🎯 목표언어: ");
    var targetLanguage = Console.ReadLine() ?? "한국어";

    try
    {
        // Human-in-the-Loop 워크플로우 실행
        var result = await workflow.ExecuteAsync(originalText, targetLanguage);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine($"🌐 로컬라이제이션 결과 ({targetLanguage})");
        Console.WriteLine(new string('=', 70));
        
        Console.WriteLine("\n🔤 원문:");
        Console.WriteLine(result.OriginalText);
        
        Console.WriteLine("\n🤖 AI 번역:");
        Console.WriteLine(result.Translation);
        
        Console.WriteLine("\n👤 인간 검수:");
        Console.WriteLine(result.HumanReview);
        
        Console.WriteLine("\n✏️ 수정된 번역:");
        Console.WriteLine(result.RevisedTranslation);
        
        Console.WriteLine("\n📊 최종 품질:");
        Console.WriteLine(result.FinalQuality);
        
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
                Console.Write("새로운 피드백/수정사항을 입력하세요: ");
                var newFeedback = Console.ReadLine() ?? "더 자연스러운 표현으로 수정해주세요.";
                
                var travelResult = await workflow.TravelAndReviseAsync(
                    targetId, 
                    newFeedback,
                    result.OriginalText,
                    result.TargetLanguage,
                    result.Translation);
                
                Console.WriteLine("\n🌐 시간 이동 후 새로운 번역:");
                Console.WriteLine(travelResult.RevisedTranslation);
                Console.WriteLine($"\n📊 새로운 품질 평가: {travelResult.FinalQuality}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
