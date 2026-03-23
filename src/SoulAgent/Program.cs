// ==========================================
// SoulAgent - 핵심 가치와 원칙을 가진 에이전트
// ==========================================
// 학습 목표:
// 1. 에이전트의 Soul(핵심 가치/원칙) 정의
// 2. JSON 기반 Soul 프로필 관리
// 3. Soul 에 기반한 일관된 에이전트 행동
// 4. 런타임에 Soul 업데이트
// ==========================================

using SoulAgent.Agents;
using SoulAgent.Soul;

Console.WriteLine("🧬 SoulAgent - 핵심 가치를 가진 에이전트");
Console.WriteLine("========================================\n");

// 1. 환경 변수에서 API 키와 베이스 URL 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. Soul 로드
var soulPath = Path.Combine(AppContext.BaseDirectory, "data", "soul_profile.json");

if (!File.Exists(soulPath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName
        ?? AppContext.BaseDirectory;
    soulPath = Path.Combine(projectDir, "data", "soul_profile.json");
}

var soulLoader = new SoulLoader(soulPath);
var agent = new SoulAgentClass(soulLoader, apiKey, baseUrl);

// 3. Soul 정보 표시
agent.ShowSoul();

Console.WriteLine("\n💬 Soul 과 대화하세요 (종료: 'quit', Soul 보기: 'soul', Soul 업데이트: 'update')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
    {
        continue;
    }

    var command = userInput.Trim().ToLower();

    if (command == "quit" || command == "exit" || command == "q")
    {
        Console.WriteLine("\n👋 안녕히 가세요!");
        break;
    }

    if (command == "soul")
    {
        agent.ShowSoul();
        continue;
    }

    if (command == "update")
    {
        await UpdateSoulAsync(agent);
        continue;
    }

    try
    {
        Console.Write("🤖 에이전트: ");
        var response = await agent.RunAsync(userInput);
        Console.WriteLine(response);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}

/// <summary>
/// Soul 업데이트
/// </summary>
async Task UpdateSoulAsync(SoulAgentClass agent)
{
    Console.WriteLine("\n📝 Soul 업데이트 모드 (각 항목을 입력하세요. 'skip' 입력 시 기존 유지)\n");

    Console.Write("이름: ");
    var nameInput = Console.ReadLine();

    Console.Write("정체성: ");
    var identityInput = Console.ReadLine();

    Console.Write("미션: ");
    var missionInput = Console.ReadLine();

    Console.Write("핵심 가치 (쉼표로 구분): ");
    var valuesInput = Console.ReadLine();

    Console.Write("행동 원칙 (쉼표로 구분): ");
    var principlesInput = Console.ReadLine();

    Console.Write("말투: ");
    var styleInput = Console.ReadLine();

    Console.Write("금지 사항 (쉼표로 구분): ");
    var taboosInput = Console.ReadLine();

    var newSoul = new SoulDefinition
    {
        Name = string.IsNullOrWhiteSpace(nameInput) ? "아리아" : nameInput,
        Identity = string.IsNullOrWhiteSpace(identityInput) ? "지식과 지혜를 전하는 철학자이자 멘토" : identityInput,
        Mission = string.IsNullOrWhiteSpace(missionInput) ? "사용자에게 깊은 통찰과 지혜를 제공하며, 긍정적인 성장을 돕는 것" : missionInput,
        SpeakingStyle = string.IsNullOrWhiteSpace(styleInput) ? "차분하고 깊이 있는 어조" : styleInput,
        CoreValues = string.IsNullOrWhiteSpace(valuesInput)
            ? new List<string> { "진실성", "지식 공유", "배려" }
            : valuesInput.Split(',').Select(v => v.Trim()).ToList(),
        Principles = string.IsNullOrWhiteSpace(principlesInput)
            ? new List<string> { "사실에 기반한 정보 제공", "윤리적 답변" }
            : principlesInput.Split(',').Select(p => p.Trim()).ToList(),
        Taboos = string.IsNullOrWhiteSpace(taboosInput)
            ? new List<string> { "허위 정보", "무례한 언어" }
            : taboosInput.Split(',').Select(t => t.Trim()).ToList()
    };

    agent.UpdateSoul(newSoul);
}
