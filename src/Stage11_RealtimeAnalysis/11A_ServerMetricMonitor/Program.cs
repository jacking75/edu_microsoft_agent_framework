// Copyright (c) Microsoft. All rights reserved.

using System.Reactive.Linq;
using _11A_ServerMetricMonitor.Agents;
using _11A_ServerMetricMonitor.Services;

// ==========================================
// 11 단계 A: 서버 메트릭 모니터링
// ==========================================
// 학습 목표:
// 1. 스트리밍 데이터 처리
// 2. 실시간 메트릭 수집
// 3. 이벤트 기반 아키텍처
// ==========================================

Console.WriteLine("📊 서버 메트릭 모니터링에 오신 것을 환영합니다!");
Console.WriteLine("실시간 서버 상태를 모니터링합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 생성
var streamingService = new StreamingDataService();
var alertService = new AlertService();
var collectorAgent = new MetricCollectorAgent(apiKey, baseUrl);

Console.WriteLine("✅ 모니터링 서비스가 초기화되었습니다.\n");

// 알림 구독
alertService.AlertRaised += (s, e) =>
{
    Console.WriteLine($"   📢 알림 수신: {e.Message}");
};

// 메트릭 스트림 처리 (이벤트 기반)
Console.WriteLine("📡 메트릭 스트림 구독 중...");
streamingService.MetricStream
    .Where(m => m.Fps < 30 || m.CpuUsage > 80)
    .Subscribe(metric =>
    {
        if (metric.Fps < 30)
        {
            alertService.RaiseAlert($"FPS 저하 감지: {metric.Fps:F1}", "WARNING");
        }
        if (metric.CpuUsage > 80)
        {
            alertService.RaiseAlert($"CPU 사용량 높음: {metric.CpuUsage:F1}%", "WARNING");
        }
    });

Console.WriteLine("✅ 이상치 감지 구독 완료\n");

// 스트리밍 시작
streamingService.StartStreaming();

Console.WriteLine("\n모니터링 중... (중료: 'stop' 또는 'quit')\n");

while (true)
{
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || 
        input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("stop", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n🛑 모니터링 중지");
        streamingService.StopStreaming();
        break;
    }

    try
    {
        // 최근 메트릭 분석
        var recentMetrics = streamingService.GetRecentMetrics(10);
        
        Console.WriteLine("\n📈 최근 메트릭 분석 요청...");
        var analysis = await collectorAgent.AnalyzeMetricsAsync(recentMetrics);
        
        Console.WriteLine($"\n📊 분석 결과:\n{analysis}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
