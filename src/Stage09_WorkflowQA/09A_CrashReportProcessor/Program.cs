// Copyright (c) Microsoft. All rights reserved.

using _09A_CrashReportProcessor.Agents;
using _09A_CrashReportProcessor.Workflows;

// ==========================================
// 9 단계 A: 크래시 리포트 처리기
// ==========================================
// 학습 목표:
// 1. Graph Workflow 구현
// 2. 다중 에이전트 간 데이터 전달
// 3. 병렬 처리 가능한 워크플로우
// ==========================================

Console.WriteLine("💥 크래시 리포트 처리기에 오신 것을 환영합니다!");
Console.WriteLine("크래시 리포트를 분석하고 심각도를 평가합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 에이전트 생성
var collectorAgent = new CollectorAgent(apiKey, baseUrl);
var analyzerAgent = new AnalyzerAgent(apiKey, baseUrl);
var severityAgent = new SeverityAgent(apiKey, baseUrl);

// 워크플로우 생성
var workflow = new CrashReportWorkflow(collectorAgent, analyzerAgent, severityAgent);

Console.WriteLine("✅ 3 개의 에이전트가 초기화되었습니다.");
Console.WriteLine("   📥 Collector → 🔍 Analyzer → ⚠️ Severity\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"게임이 시작하자마자 꺼져요. 에러 로그: NullReferenceException...\"");
Console.WriteLine("  - \"던전 입장시 크래시 발생. Error Code: 500\"\n");

Console.WriteLine("크래시 리포트를 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var crashReport = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(crashReport) || 
        crashReport.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        crashReport.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // Graph 워크플로우 실행
        var result = await workflow.ExecuteAsync(crashReport);

        // 최종 결과 출력
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("📊 크래시 리포트 처리 결과");
        Console.WriteLine(new string('=', 60));
        
        Console.WriteLine("\n📥 수집 정보:");
        Console.WriteLine(result.CollectedInfo);
        
        Console.WriteLine("\n🔍 분석 결과:");
        Console.WriteLine(result.Analysis);
        
        Console.WriteLine("\n⚠️ 심각도 평가:");
        Console.WriteLine(result.Severity);
        
        Console.WriteLine($"\n📅 처리일: {result.ProcessedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
