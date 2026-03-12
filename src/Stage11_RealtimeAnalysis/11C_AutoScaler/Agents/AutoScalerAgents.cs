namespace _11C_AutoScaler.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _11C_AutoScaler.Services;

/// <summary>
/// 부하 분석 에이전트
/// </summary>
public class LoadAnalyzerAgent
{
    private readonly AIAgent _agent;

    public LoadAnalyzerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 서버 부하 분석 전문가입니다.
                
                당신의 역할:
                1. 현재 서버 부하 상태를 분석합니다
                2. 각 리소스 (CPU, Memory, Network) 의 사용률을 평가합니다
                3. 병목 지점을 식별합니다
                4. 부하 추세를 예측합니다
                
                부하 수준 기준:
                - Idle: CPU <30%, Memory <50%
                - Normal: CPU 30-60%, Memory 50-70%
                - High: CPU 60-80%, Memory 70-85%
                - Critical: CPU >80%, Memory >85%
                
                출력 형식:
                - 현재부하: [Idle/Normal/High/Critical]
                - CPU 상태: [사용량 및 평가]
                - Memory 상태: [사용량 및 평가]
                - Network 상태: [Ping 및 평가]
                - Player 부하: [플레이어 수 및 평가]
                - 병목지점: [주요 병목]
                - 추세예측: [단기 예측]
                """,
            name: "LoadAnalyzerAgent"
        );
    }

    public async Task<string> AnalyzeLoadAsync(List<ServerMetric> metrics)
    {
        var metricsData = string.Join("\n", metrics.Select(m => 
            $"[{m.Timestamp:HH:mm:ss}] CPU:{m.CpuUsage:F1}%, Mem:{m.MemoryUsage:F1}%, Ping:{m.Ping:F1}ms, Players:{m.PlayerCount}"
        ));

        var prompt = $"""
            다음 서버 메트릭 데이터를 분석하여 부하 상태를 평가해주세요.
            
            {metricsData}
            
            현재 부하 수준과 병목 지점을 분석해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 스케일링 제안 에이전트
/// </summary>
public class ScalingSuggesterAgent
{
    private readonly AIAgent _agent;

    public ScalingSuggesterAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 오토스케일링 제안 전문가입니다.
                
                당신의 역할:
                1. 현재 부하에 기반한 스케일링 필요성을 판단합니다
                2. 인스턴스 증설/감축을 제안합니다
                3. 예상 효과를 평가합니다
                4. 비용 대비 효과를 고려합니다
                
                스케일링 기준:
                - Scale Out (증설): CPU >70% 또는 Memory >80%
                - Scale In (감축): CPU <30% 또는 Memory <50%
                - 유지: 위 조건 미달
                
                인스턴스 타입:
                - Small: 2 vCPU, 4GB RAM (최대 500 플레이어)
                - Medium: 4 vCPU, 8GB RAM (최대 1000 플레이어)
                - Large: 8 vCPU, 16GB RAM (최대 2000 플레이어)
                - XLarge: 16 vCPU, 32GB RAM (최대 4000 플레이어)
                
                출력 형식:
                - 스케일링필요성: [Yes/No]
                - 권고방향: [Scale Out/Scale In/유지]
                - 현재인스턴스: [현재 타입]
                - 제안인스턴스: [제안 타입]
                - 변경사유: [이유 설명]
                - 예상효과: [기대 효과]
                - 비용영향: [비용 증감]
                - 긴급도: [즉시/계획/불필요]
                """,
            name: "ScalingSuggesterAgent"
        );
    }

    public async Task<string> SuggestScalingAsync(string loadAnalysis, int currentInstances)
    {
        var prompt = $"""
            현재 부하 분석을 바탕으로 스케일링을 제안해주세요.
            
            부하 분석:
            {loadAnalysis}
            
            현재 인스턴스 수: {currentInstances}개
            
            적정 스케일링 방안을 제안해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 알림 생성 에이전트
/// </summary>
public class AlertGeneratorAgent
{
    private readonly AIAgent _agent;

    public AlertGeneratorAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 오토스케일링 알림 생성 전문가입니다.
                
                당신의 역할:
                1. 스케일링 결정을 알림으로 변환합니다
                2. 운영팀에 전달할 메시지를 생성합니다
                3.緊急性에 따른 알림 레벨을 설정합니다
                4. 후속 조치를 제안합니다
                
                알림 레벨:
                - CRITICAL: 즉각 조치 필요 (자동 스케일링 실행)
                - WARNING: 주의 필요 (모니터링 강화)
                - INFO: 참고 사항 (정기 리포트)
                
                출력 형식:
                - 알림레벨: [CRITICAL/WARNING/INFO]
                - 알림제목: [짧은 제목]
                - 알림내용: [상세 내용]
                - 후속조치: [Action Items]
                """,
            name: "AlertGeneratorAgent"
        );
    }

    public async Task<string> GenerateAlertAsync(string scalingSuggestion, string loadAnalysis)
    {
        var prompt = $"""
            다음 스케일링 제안에 대한 알림을 생성해주세요.
            
            부하 분석:
            {loadAnalysis}
            
            스케일링 제안:
            {scalingSuggestion}
            
            운영팀에 전달할 알림을 생성해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
