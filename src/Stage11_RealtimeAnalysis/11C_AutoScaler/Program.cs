// Copyright (c) Microsoft. All rights reserved.

using System.Reactive.Linq;
using _11C_AutoScaler.Agents;
using _11C_AutoScaler.Services;

// ==========================================
// 11 단계 C: 자동 스케일링 제안
// ==========================================
// 학습 목표:
// 1. 스트리밍 데이터 처리
// 2. 부하 분석과 스케일링 제안
// 3. 이벤트 기반 아키텍처
// 4. 실시간 알림
// ==========================================

Console.WriteLine("⚖️ 자동 스케일링 제안 시스템에 오신 것을 환영합니다!");
Console.WriteLine("실시간 부하 분석으로 스케일링을 제안합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var streamingService = new StreamingDataService();
var alertService = new AlertService();
var loadAnalyzerAgent = new LoadAnalyzerAgent(apiKey, baseUrl);
var scalingSuggesterAgent = new ScalingSuggesterAgent(apiKey, baseUrl);
var alertGeneratorAgent = new AlertGeneratorAgent(apiKey, baseUrl);

Console.WriteLine("✅ 모니터링 서비스가 초기화되었습니다.\n");

// 알림 구독
alertService.AlertRaised += (s, e) =>
{
    Console.WriteLine($"   📢 알림 수신: {e.Message}");
};

// 메트릭 스트림 처리 - 임계치 감지
Console.WriteLine("📡 메트릭 스트림 구독 중...");
streamingService.MetricStream
    .Where(m => m.CpuUsage > 70 || m.MemoryUsage > 80)
    .Subscribe(metric =>
    {
        if (metric.CpuUsage > 70)
        {
            alertService.RaiseAlert($"CPU 사용량 높음: {metric.CpuUsage:F1}% - 스케일아웃 고려", "WARNING");
        }
        if (metric.MemoryUsage > 80)
        {
            alertService.RaiseAlert($"Memory 사용량 높음: {metric.MemoryUsage:F1}% - 스케일아웃 필요", "HIGH");
        }
    });

Console.WriteLine("✅ 임계치 감지 구독 완료\n");

// 스트리밍 시작
streamingService.StartStreaming();

Console.WriteLine("\n💡 명령어:");
Console.WriteLine("  - 'analyze': 현재 부하 분석");
Console.WriteLine("  - 'suggest': 스케일링 제안");
Console.WriteLine("  - 'load <1-4>': 부하 레벨 설정 (1:Low, 2:Normal, 3:High, 4:Critical)");
Console.WriteLine("  - 'instances <n>': 현재 인스턴스 수 설정");
Console.WriteLine("  - 'quit': 종료\n");

int currentInstances = 3;
string? lastLoadAnalysis = null;

while (true)
{
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) ||
        input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n🛑 모니터링 중지");
        streamingService.StopStreaming();
        break;
    }

    try
    {
        if (input.Equals("analyze", StringComparison.OrdinalIgnoreCase))
        {
            // 부하 분석
            Console.WriteLine("\n📈 부하 분석 요청...");
            var recentMetrics = streamingService.GetRecentMetrics(10);
            
            if (recentMetrics.Count < 3)
            {
                Console.WriteLine("⚠️ 분석할 데이터가 부족합니다. 조금 기다려주세요.");
                continue;
            }
            
            lastLoadAnalysis = await loadAnalyzerAgent.AnalyzeLoadAsync(recentMetrics);
            
            Console.WriteLine($"\n📊 부하 분석 결과:\n{lastLoadAnalysis}\n");
        }
        else if (input.Equals("suggest", StringComparison.OrdinalIgnoreCase))
        {
            // 스케일링 제안
            Console.WriteLine("\n⚖️ 스케일링 제안 요청...");
            
            if (lastLoadAnalysis == null)
            {
                Console.WriteLine("⚠️ 먼저 'analyze' 명령으로 부하를 분석해주세요.");
                continue;
            }
            
            var scalingSuggestion = await scalingSuggesterAgent.SuggestScalingAsync(lastLoadAnalysis, currentInstances);
            
            Console.WriteLine($"\n📋 스케일링 제안:\n{scalingSuggestion}\n");
            
            // 스케일링이 필요하면 알림 생성
            if (scalingSuggestion.Contains("스케일링필요성: Yes", StringComparison.OrdinalIgnoreCase) ||
                scalingSuggestion.Contains("권고방향: Scale Out", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\n🔔 알림 생성 중...");
                var alert = await alertGeneratorAgent.GenerateAlertAsync(scalingSuggestion, lastLoadAnalysis);
                Console.WriteLine($"\n📢 생성된 알림:\n{alert}\n");
            }
        }
        else if (input.StartsWith("load", StringComparison.OrdinalIgnoreCase))
        {
            // 부하 레벨 설정
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && int.TryParse(parts[1], out var level))
            {
                streamingService.SetLoadLevel(level);
            }
            else
            {
                Console.WriteLine("💡 사용법: 'load <1-4>' (1:Low, 2:Normal, 3:High, 4:Critical)");
            }
        }
        else if (input.StartsWith("instances", StringComparison.OrdinalIgnoreCase))
        {
            // 인스턴스 수 설정
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && int.TryParse(parts[1], out var count))
            {
                currentInstances = count;
                Console.WriteLine($"✅ 현재 인스턴스 수: {currentInstances}개로 설정");
            }
            else
            {
                Console.WriteLine("💡 사용법: 'instances <숫자>'");
            }
        }
        else
        {
            Console.WriteLine("💡 사용 가능한 명령어: analyze, suggest, load <1-4>, instances <n>, quit");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
