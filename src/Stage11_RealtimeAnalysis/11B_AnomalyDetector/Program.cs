// Copyright (c) Microsoft. All rights reserved.

using System.Reactive.Linq;
using _11B_AnomalyDetector.Agents;
using _11B_AnomalyDetector.Services;

// ==========================================
// 11 단계 B: 이상 징후 탐지기
// ==========================================
// 학습 목표:
// 1. 스트리밍 데이터 처리
// 2. 패턴 분석과 이상 탐지
// 3. 이벤트 기반 아키텍처
// 4. 실시간 알림
// ==========================================

Console.WriteLine("🔍 이상 징후 탐지기에 오신 것을 환영합니다!");
Console.WriteLine("실시간 패턴 분석으로 이상을 감지합니다.\n");

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
var patternAnalyzerAgent = new PatternAnalyzerAgent(apiKey, baseUrl);
var anomalyDetectorAgent = new AnomalyDetectorAgent(apiKey, baseUrl);
var diagnosticReportAgent = new DiagnosticReportAgent(apiKey, baseUrl);

Console.WriteLine("✅ 모니터링 서비스가 초기화되었습니다.\n");

// 알림 구독
alertService.AlertRaised += (s, e) =>
{
    Console.WriteLine($"   📢 알림 수신: {e.Message}");
};

// 메트릭 스트림 처리 - 이상치 감지
Console.WriteLine("📡 메트릭 스트림 구독 중...");
streamingService.MetricStream
    .Where(m => m.Fps < 35 || m.CpuUsage > 75 || m.Ping > 100)
    .Subscribe(metric =>
    {
        if (metric.Fps < 35)
        {
            alertService.RaiseAlert($"FPS 이상: {metric.Fps:F1} (정상: 50-60)", "HIGH");
        }
        if (metric.CpuUsage > 75)
        {
            alertService.RaiseAlert($"CPU 사용량 이상: {metric.CpuUsage:F1}% (정상: <60%)", "HIGH");
        }
        if (metric.Ping > 100)
        {
            alertService.RaiseAlert($"Ping 지연 이상: {metric.Ping:F1}ms (정상: <50ms)", "MEDIUM");
        }
    });

Console.WriteLine("✅ 이상치 감지 구독 완료\n");

// 스트리밍 시작
streamingService.StartStreaming();

Console.WriteLine("\n💡 명령어:");
Console.WriteLine("  - 'analyze': 최근 메트릭 패턴 분석");
Console.WriteLine("  - 'detect': 이상 탐지 실행");
Console.WriteLine("  - 'anomaly on/off': 이상 모드 토글");
Console.WriteLine("  - 'quit': 종료\n");

string? baselinePattern = null;
var analysisCount = 0;

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
            // 패턴 분석
            Console.WriteLine("\n📈 패턴 분석 요청...");
            var allMetrics = streamingService.GetAllHistory();
            
            if (allMetrics.Count < 5)
            {
                Console.WriteLine("⚠️ 분석할 데이터가 부족합니다. 조금 기다려주세요.");
                continue;
            }
            
            baselinePattern = await patternAnalyzerAgent.AnalyzePatternAsync(allMetrics);
            
            Console.WriteLine($"\n📊 기본 패턴 분석 결과:\n{baselinePattern}\n");
            analysisCount++;
        }
        else if (input.Equals("detect", StringComparison.OrdinalIgnoreCase))
        {
            // 이상 탐지
            Console.WriteLine("\n🔍 이상 탐지 실행...");
            
            if (baselinePattern == null)
            {
                Console.WriteLine("⚠️ 먼저 'analyze' 명령으로 기본 패턴을 분석해주세요.");
                continue;
            }
            
            var recentMetrics = streamingService.GetRecentMetrics(10);
            var anomalyResult = await anomalyDetectorAgent.DetectAnomalyAsync(recentMetrics, baselinePattern);
            
            Console.WriteLine($"\n🚨 이상 탐지 결과:\n{anomalyResult}\n");
            
            // 이상이 감지되면 진단 리포트 생성
            if (anomalyResult.Contains("이상여부: Yes", StringComparison.OrdinalIgnoreCase) ||
                anomalyResult.Contains("심각도: CRITICAL", StringComparison.OrdinalIgnoreCase) ||
                anomalyResult.Contains("심각도: HIGH", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\n📋 진단 리포트 생성 중...");
                var report = await diagnosticReportAgent.GenerateReportAsync(anomalyResult);
                Console.WriteLine($"\n📄 진단 리포트:\n{report}\n");
            }
        }
        else if (input.StartsWith("anomaly", StringComparison.OrdinalIgnoreCase))
        {
            // 이상 모드 토글
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                var enabled = parts[1].Equals("on", StringComparison.OrdinalIgnoreCase);
                streamingService.SetAnomalyMode(enabled);
            }
            else
            {
                Console.WriteLine("💡 사용법: 'anomaly on' 또는 'anomaly off'");
            }
        }
        else
        {
            Console.WriteLine("💡 사용 가능한 명령어: analyze, detect, anomaly on/off, quit");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
