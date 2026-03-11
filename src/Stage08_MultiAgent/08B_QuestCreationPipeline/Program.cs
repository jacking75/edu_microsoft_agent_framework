// Copyright (c) Microsoft. All rights reserved.

using _08B_QuestCreationPipeline.Agents;
using _08B_QuestCreationPipeline.Workflows;

// ==========================================
// 8 단계 B: 퀘스트 생성 파이프라인
// ==========================================
// 학습 목표:
// 1. Sequential Workflow 구현 (2 단계 확장)
// 2. 다중 에이전트 간 데이터 전달
// 3. 스토리 → 보상 → 난이도 파이프라인
// ==========================================

Console.WriteLine("📜 퀘스트 생성 파이프라인에 오신 것을 환영합니다!");
Console.WriteLine("3 단계 워크플로우로 퀘스트를 생성합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 에이전트 생성
var storyAgent = new StoryAgent(apiKey, baseUrl);
var rewardAgent = new RewardAgent(apiKey, baseUrl);
var difficultyAgent = new DifficultyAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new QuestCreationWorkflow(storyAgent, rewardAgent, difficultyAgent);

Console.WriteLine("✅ 3 개의 에이전트가 초기화되었습니다.");
Console.WriteLine("   📖 StoryAgent → 💰 RewardAgent → ⚔️ DifficultyAgent\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"드래곤 슬레이어\"");
Console.WriteLine("  - \"잃어버린 유물 찾기\"");
Console.WriteLine("  - \"왕국의 비밀을 밝혀라\"\n");

Console.WriteLine("퀘스트 제목을 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var questTitle = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(questTitle) ||
        questTitle.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        questTitle.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // Sequential 워크플로우 실행
        var result = await workflow.ExecuteAsync(questTitle);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine($"📜 퀘스트: {result.QuestTitle}");
        Console.WriteLine(new string('=', 60));

        Console.WriteLine("\n📖 스토리:");
        Console.WriteLine(result.Story);

        Console.WriteLine("\n💰 보상:");
        Console.WriteLine(result.Reward);

        Console.WriteLine("\n⚔️ 난이도:");
        Console.WriteLine(result.Difficulty);

        Console.WriteLine($"\n📅 생성일: {result.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
