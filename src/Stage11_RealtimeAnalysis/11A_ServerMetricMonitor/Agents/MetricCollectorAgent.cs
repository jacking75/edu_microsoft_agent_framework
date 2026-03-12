namespace _11A_ServerMetricMonitor.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _11A_ServerMetricMonitor.Services;

/// <summary>
/// 메트릭 수집 에이전트
/// </summary>
public class MetricCollectorAgent
{
    private readonly AIAgent _agent;

    public MetricCollectorAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 서버 메트릭 수집 전문가입니다.
                
                당신의 역할:
                1. 실시간 메트릭 데이터를 수집합니다
                2.异常치 (이상치) 를 감지합니다
                3. 추세를 분석합니다
                4. 리포트를 생성합니다
                
                분석 항목:
                - FPS: 30 이하 주의, 20 이하 위험
                - Ping: 100ms 이상 주의, 200ms 이상 위험
                - CPU: 80% 이상 주의, 90% 이상 위험
                - Memory: 85% 이상 주의, 95% 이상 위험
                """,
            name: "MetricCollectorAgent"
        );
    }

    public async Task<string> AnalyzeMetricsAsync(List<ServerMetric> metrics)
    {
        var metricsJson = string.Join("\n", metrics.Select(m => 
            $"[{m.Timestamp:HH:mm:ss}] FPS:{m.Fps:F1}, Ping:{m.Ping:F1}ms, CPU:{m.CpuUsage:F1}%, Mem:{m.MemoryUsage:F1}%"
        ));

        var prompt = $"""
            다음 서버 메트릭 데이터를 분석해주세요:
            
            {metricsJson}
            
            이상치와 추세를 보고해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
