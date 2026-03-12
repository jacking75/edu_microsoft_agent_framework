// Copyright (c) Microsoft. All rights reserved.

using _09B_BugClassifier.Agents;
using _09B_BugClassifier.Workflows;

// ==========================================
// 9 단계 B: 자동 버그 분류기
// ==========================================
// 학습 목표:
// 1. Graph Workflow 구현 (확장 버전)
// 2. 로그 분석 → 중복체크 → 우선순위결정
// 3. 기존 버그와의 중복 판별
// ==========================================

Console.WriteLine("🐛 자동 버그 분류기에 오신 것을 환영합니다!");
Console.WriteLine("버그 리포트를 분석하고 우선순위를 자동 할당합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 에이전트 생성
var logAnalyzerAgent = new LogAnalyzerAgent(apiKey, baseUrl);
var duplicateCheckerAgent = new DuplicateCheckerAgent(apiKey, baseUrl);
var priorityAssignerAgent = new PriorityAssignerAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new BugClassificationWorkflow(logAnalyzerAgent, duplicateCheckerAgent, priorityAssignerAgent);

Console.WriteLine("✅ 3 개의 에이전트가 초기화되었습니다.");
Console.WriteLine("   🔍 LogAnalyzer → 🔄 DuplicateChecker → 📊 PriorityAssigner\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"캐릭터 이동중 갑자기 멈춰요. 로그: PathfindingException at Navigator.Move()\"");
Console.WriteLine("  - \"인벤토리에서 아이템이 두 개로 보여요. UI 렌더링 문제인 것 같습니다.\"\n");

Console.WriteLine("버그 리포트를 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var bugReport = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(bugReport) ||
        bugReport.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        bugReport.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // Graph 워크플로우 실행
        var result = await workflow.ExecuteAsync(bugReport);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("📊 버그 분류 처리 결과");
        Console.WriteLine(new string('=', 60));
        
        Console.WriteLine("\n🔍 로그 분석:");
        Console.WriteLine(result.LogAnalysis);
        
        Console.WriteLine("\n🔄 중복 체크:");
        Console.WriteLine(result.DuplicateCheck);
        
        Console.WriteLine("\n📊 우선순위 할당:");
        Console.WriteLine(result.PriorityAssignment);
        
        Console.WriteLine($"\n📅 처리일: {result.ProcessedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
