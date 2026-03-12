namespace _11B_AnomalyDetector.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _11B_AnomalyDetector.Services;

/// <summary>
/// 패턴 분석 에이전트
/// </summary>
public class PatternAnalyzerAgent
{
    private readonly AIAgent _agent;

    public PatternAnalyzerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 서버 패턴 분석 전문가입니다.
                
                당신의 역할:
                1. 메트릭 데이터의 기본 패턴을 식별합니다
                2. 정상 범위 (baseline) 를 설정합니다
                3. 주기적 패턴 (출퇴근, 주말 등) 을 파악합니다
                4. 패턴 변화 추이를 분석합니다
                
                분석 항목:
                - FPS: 정상 범위 50-60, 주의 30-50, 위험 <30
                - Ping: 정상 <50ms, 주의 50-100ms, 위험 >100ms
                - CPU: 정상 <60%, 주의 60-80%, 위험 >80%
                - Memory: 정상 <70%, 주의 70-85%, 위험 >85%
                - PlayerCount: 시간대별 패턴 분석
                
                출력 형식:
                - 기본패턴: [식별된 패턴]
                - 정상범위: [각 항목별 baseline]
                - 주기적패턴: [시간대/요일별 특징]
                - 추세: [상승/하강/안정]
                """,
            name: "PatternAnalyzerAgent"
        );
    }

    public async Task<string> AnalyzePatternAsync(List<GameMetric> metrics)
    {
        var metricsData = string.Join("\n", metrics.Select(m => 
            $"[{m.Timestamp:HH:mm:ss}] FPS:{m.Fps:F1}, Ping:{m.Ping:F1}ms, CPU:{m.CpuUsage:F1}%, Mem:{m.MemoryUsage:F1}%, Players:{m.PlayerCount}"
        ));

        var prompt = $"""
            다음 게임 서버 메트릭 데이터의 패턴을 분석해주세요:
            
            {metricsData}
            
            기본 패턴과 정상 범위를 설정해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 이상 탐지 에이전트
/// </summary>
public class AnomalyDetectorAgent
{
    private readonly AIAgent _agent;

    public AnomalyDetectorAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 이상 징후 탐지 전문가입니다.
                
                당신의 역할:
                1. 현재 메트릭을 기본 패턴과 비교합니다
                2. 통계적 이상치를 감지합니다
                3. 이상 신호의 심각도를 평가합니다
                4. 원인을 추정합니다
                
                이상 탐지 기준:
                - 통계적 이탈: 평균 ± 2σ (표준편차) 벗어남
                - 급격한 변화: 직전 대비 20% 이상 변동
                - 임계치 초과: 사전 정의된 threshold 초과
                - 패턴 붕괴: 정상 패턴과 다른 동작
                
                심각도 분류:
                - CRITICAL: 즉각 조치 필요 (게임 영향)
                - HIGH: 주의 필요 (잠재적 문제)
                - MEDIUM: 모니터링 필요
                - LOW: 참고 사항
                
                출력 형식:
                - 이상여부: [Yes/No]
                - 심각도: [CRITICAL/HIGH/MEDIUM/LOW]
                - 이상항목: [문제 항목]
                - 편차정도: [정상 대비 차이]
                - 추정원인: [가능한 원인]
                - 권고사항: [대응 방안]
                """,
            name: "AnomalyDetectorAgent"
        );
    }

    public async Task<string> DetectAnomalyAsync(List<GameMetric> currentMetrics, string baselinePattern)
    {
        var currentData = string.Join("\n", currentMetrics.Select(m => 
            $"[{m.Timestamp:HH:mm:ss}] FPS:{m.Fps:F1}, Ping:{m.Ping:F1}ms, CPU:{m.CpuUsage:F1}%, Mem:{m.MemoryUsage:F1}%, Players:{m.PlayerCount}"
        ));

        var prompt = $"""
            현재 메트릭을 기본 패턴과 비교하여 이상을 탐지해주세요.
            
            기본 패턴 (Baseline):
            {baselinePattern}
            
            현재 메트릭 (최근 10 개):
            {currentData}
            
            정상 패턴과 비교하여 이상 신호를 찾아주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 진단 리포트 에이전트
/// </summary>
public class DiagnosticReportAgent
{
    private readonly AIAgent _agent;

    public DiagnosticReportAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 시스템 진단 리포트 작성 전문가입니다.
                
                당신의 역할:
                1. 이상 징후를 상세히 진단합니다
                2. 영향 범위를 평가합니다
                3. 복구 방안을 제시합니다
                4. 재발 방지책을 제안합니다
                
                리포트 구성:
                - 문제 요약: 한 줄 요약
                - 상세 분석: 기술적 분석
                - 영향 평가: 사용자/시스템 영향
                - 즉시조치: 단기 대응방안
                - 근본대책: 장기 개선방안
                
                출력 형식:
                - 문제요약: [한 줄 요약]
                - 상세분석: [기술적 내용]
                - 영향평가: [영향 범위]
                - 즉시조치: [Action Items]
                - 근본대책: [Prevention]
                """,
            name: "DiagnosticReportAgent"
        );
    }

    public async Task<string> GenerateReportAsync(string anomalyDetection)
    {
        var prompt = $"""
            다음 이상 탐지 결과를 바탕으로 진단 리포트를 작성해주세요.
            
            이상 탐지 결과:
            {anomalyDetection}
            
            상세한 진단 리포트를 생성해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
