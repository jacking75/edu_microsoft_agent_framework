// Copyright (c) Microsoft. All rights reserved.

using _09C_QAReportGenerator.Agents;
using _09C_QAReportGenerator.Workflows;

// ==========================================
// 9 단계 C: QA 리포트 생성기
// ==========================================
// 학습 목표:
// 1. Graph Workflow 구현 (확장 버전)
// 2. 테스트결과 → 요약 → 배포판결정
// 3. 자동화된 배포 가능성 판단
// ==========================================

Console.WriteLine("📊 QA 리포트 생성기에 오신 것을 환영합니다!");
Console.WriteLine("QA 테스트 결과를 분석하고 배포 가능 여부를 판단합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 에이전트 생성
var testAnalyzerAgent = new TestAnalyzerAgent(apiKey, baseUrl);
var summarizerAgent = new SummarizerAgent(apiKey, baseUrl);
var deploymentDeciderAgent = new DeploymentDeciderAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new QAReportWorkflow(testAnalyzerAgent, summarizerAgent, deploymentDeciderAgent);

Console.WriteLine("✅ 3 개의 에이전트가 초기화되었습니다.");
Console.WriteLine("   🔍 TestAnalyzer → 📝 Summarizer → 🚀 DeploymentDecider\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - 테스트 결과를 붙여넣으세요 (예: Passed: 50, Failed: 2, Skipped: 1...)");
Console.WriteLine("  - 단위 테스트, 통합 테스트, E2E 테스트 결과 등\n");

Console.WriteLine("QA 테스트 결과를 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var testResults = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(testResults) ||
        testResults.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        testResults.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // Graph 워크플로우 실행
        var result = await workflow.ExecuteAsync(testResults);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("📊 QA 리포트 생성 결과");
        Console.WriteLine(new string('=', 60));
        
        Console.WriteLine("\n🔍 테스트 분석:");
        Console.WriteLine(result.Analysis);
        
        Console.WriteLine("\n📝 요약 리포트:");
        Console.WriteLine(result.Summary);
        
        Console.WriteLine("\n🚀 배포 결정:");
        Console.WriteLine(result.DeploymentDecision);
        
        Console.WriteLine($"\n📅 처리일: {result.ProcessedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
