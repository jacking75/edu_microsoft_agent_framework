namespace _09C_QAReportGenerator.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 테스트 결과 분석 에이전트
/// </summary>
public class TestAnalyzerAgent
{
    private readonly AIAgent _agent;

    public TestAnalyzerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 QA 테스트 결과 분석 전문가입니다.
                
                당신의 역할:
                1. 테스트 결과를 분석합니다 (통과/실패/스킵)
                2. 실패한 테스트의 원인을 파악합니다
                3. 테스트 커버리지를 평가합니다
                4. 위험 요소를 식별합니다
                
                출력 형식:
                - 총테스트수: [숫자]
                - 통과: [숫자]
                - 실패: [숫자]
                - 스킵: [숫자]
                - 커버리지: [백분율]
                - 주요실패원인: [요약]
                - 위험요소: [목록]
                """,
            name: "TestAnalyzerAgent"
        );
    }

    public async Task<string> AnalyzeAsync(string testResults)
    {
        var prompt = $"다음 QA 테스트 결과를 분석해주세요:\n\n{testResults}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 리포트 요약 에이전트
/// </summary>
public class SummarizerAgent
{
    private readonly AIAgent _agent;

    public SummarizerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 QA 리포트 요약 전문가입니다.
                
                당신의 역할:
                1. 테스트 결과를 경영진용으로 요약합니다
                2. 주요 이슈를 강조합니다
                3. 개선 권고사항을 제시합니다
                4. 다음 단계 액션을 제안합니다
                
                출력 형식:
                - executive_summary: [한 줄 요약]
                - key_findings: [주요 발견사항 3-5 개]
                - critical_issues: [치명적 이슈]
                - recommendations: [권고사항]
                - next_actions: [다음 액션]
                """,
            name: "SummarizerAgent"
        );
    }

    public async Task<string> SummarizeAsync(string testResults, string analysis)
    {
        var prompt = $$"""
            다음 테스트 결과와 분석을 바탕으로 요약 리포트를 작성해주세요.
            
            테스트 결과:
            {{testResults}}
            
            분석 결과:
            {{analysis}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 배포 결정 에이전트
/// </summary>
public class DeploymentDeciderAgent
{
    private readonly AIAgent _agent;

    public DeploymentDeciderAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 배포 가능성 판단 전문가입니다.
                
                당신의 역할:
                1. 테스트 결과를 바탕으로 배포 가능 여부를 판단합니다
                2. 릴리스 리스크를 평가합니다
                3. 조건부 배포 권고사항을 제시합니다
                4. 핫픽스 필요 여부를 판단합니다
                
                배포 기준:
                - GO: 모든 중요 테스트 통과, 치명적 버그 없음
                - CONDITIONAL_GO: 경미한 실패만 존재, 핫픽스 준비 조건
                - NO_GO: 치명적 버그 존재, 중요 기능 실패
                
                출력 형식:
                - 배포판정: [GO/CONDITIONAL_GO/NO_GO]
                - 릴리스리스크: [상/중/하]
                - 핫픽스필요: [Yes/No]
                - 배포조건: [조건 또는 N/A]
                - 권장배포일: [YYYY-MM-DD 또는 보류]
                """,
            name: "DeploymentDeciderAgent"
        );
    }

    public async Task<string> DecideAsync(string analysis, string summary)
    {
        var prompt = $$"""
            다음 분석과 요약 리포트를 바탕으로 배포 가능 여부를 판단해주세요.
            
            분석 결과:
            {{analysis}}
            
            요약 리포트:
            {{summary}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
