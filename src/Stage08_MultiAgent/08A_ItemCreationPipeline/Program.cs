// Copyright (c) Microsoft. All rights reserved.

using _08A_ItemCreationPipeline.Agents;
using _08A_ItemCreationPipeline.Workflows;

// ==========================================
// 8 단계 A: 아이템 생성 파이프라인
// ==========================================
// 학습 목표:
// 1. Sequential Workflow 구현
// 2. 다중 에이전트 간 데이터 전달
// 3. 워크플로우 단계별 진행
// ==========================================

Console.WriteLine("🎒 아이템 생성 파이프라인에 오신 것을 환영합니다!");
Console.WriteLine("3 단계 워크플로우로 아이템을 생성합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 에이전트 생성
var descriptionAgent = new DescriptionAgent(apiKey, baseUrl);
var balanceAgent = new BalanceAgent(apiKey, baseUrl);
var localizationAgent = new LocalizationAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new ItemCreationWorkflow(descriptionAgent, balanceAgent, localizationAgent);

Console.WriteLine("✅ 3 개의 에이전트가 초기화되었습니다.");
Console.WriteLine("   📝 DescriptionAgent → ⚖️ BalanceAgent → 🌐 LocalizationAgent\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"플레임 소드\"");
Console.WriteLine("  - \"아이언 실드\"");
Console.WriteLine("  - \"엘븐 보우\"\n");

Console.WriteLine("아이템 이름을 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var itemName = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(itemName) ||
        itemName.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        itemName.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // Sequential 워크플로우 실행
        var result = await workflow.ExecuteAsync(itemName);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine($"📦 아이템: {result.ItemName}");
        Console.WriteLine(new string('=', 60));

        Console.WriteLine("\n📝 설명:");
        Console.WriteLine(result.Description);

        Console.WriteLine("\n⚖️ 밸런스 리뷰:");
        Console.WriteLine(result.BalanceReview);

        Console.WriteLine("\n🌐 현지화:");
        Console.WriteLine(result.Localization);

        Console.WriteLine($"\n📅 생성일: {result.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
