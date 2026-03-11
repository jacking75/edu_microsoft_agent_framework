// Copyright (c) Microsoft. All rights reserved.

using _08C_DialogueGenerator.Agents;
using _08C_DialogueGenerator.Workflows;

// ==========================================
// 8 단계 C: 대화 생성기
// ==========================================
// 학습 목표:
// 1. Sequential Workflow 구현 (3 단계)
// 2. 다중 에이전트 간 데이터 전달
// 3. 초안 → 톤 → 분기 파이프라인
// ==========================================

Console.WriteLine("💬 대화 생성기에 오신 것을 환영합니다!");
Console.WriteLine("3 단계 워크플로우로 NPC 대화문을 생성합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 에이전트 생성
var draftAgent = new DraftAgent(apiKey, baseUrl);
var toneAgent = new ToneAgent(apiKey, baseUrl);
var branchAgent = new BranchAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new DialogueGenerationWorkflow(draftAgent, toneAgent, branchAgent);

Console.WriteLine("✅ 3 개의 에이전트가 초기화되었습니다.");
Console.WriteLine("   ✏️ DraftAgent → 🎭 ToneAgent → 🌳 BranchAgent\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - 상인 / 마을 상점 주인 / 친절함");
Console.WriteLine("  - 경비병 / 성문 경비대장 / 무뚝뚝함");
Console.WriteLine("  - 마법사 / 길드 마스터 / 신비로움\n");

Console.WriteLine("NPC 정보를 입력하세요. 형식: 이름 / 역할 / 톤 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) ||
        input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    var parts = input.Split('/');
    if (parts.Length < 3)
    {
        Console.WriteLine("\n❌ 잘못된 형식입니다. '이름 / 역할 / 톤' 형식으로 입력해주세요.\n");
        continue;
    }

    var npcName = parts[0].Trim();
    var npcRole = parts[1].Trim();
    var desiredTone = parts[2].Trim();

    try
    {
        // Sequential 워크플로우 실행
        var result = await workflow.ExecuteAsync(npcName, npcRole, desiredTone);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine($"💬 NPC: {result.NpcName} ({result.NpcRole})");
        Console.WriteLine($"🎭 톤: {result.DesiredTone}");
        Console.WriteLine(new string('=', 60));

        Console.WriteLine("\n✏️ 초안:");
        Console.WriteLine(result.Draft);

        Console.WriteLine("\n🎭 톤 조정:");
        Console.WriteLine(result.Tone);

        Console.WriteLine("\n🌳 분기 구조:");
        Console.WriteLine(result.Branches);

        Console.WriteLine($"\n📅 생성일: {result.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
