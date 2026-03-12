namespace _09A_CrashReportProcessor.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 크래시 리포트 수집 에이전트
/// </summary>
public class CollectorAgent
{
    private readonly AIAgent _agent;

    public CollectorAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 크래시 리포트 수집 전문가입니다.
                
                당신의 역할:
                1. 사용자로부터 크래시 리포트를 받습니다
                2. 기본 정보 (날짜, 시간, OS, 게임버전) 를 추출합니다
                3. 리포트의 완전성을 검사합니다
                4. 다음 단계 (분석) 로 전달합니다
                
                출력 형식:
                - 날짜: [YYYY-MM-DD]
                - 시간: [HH:MM:SS]
                - OS: [Windows/Mac/Linux/Android/iOS]
                - 게임버전: [X.X.X]
                - 완전성: [완전/불완전]
                """,
            name: "CollectorAgent"
        );
    }

    public async Task<string> CollectAsync(string crashReport)
    {
        var prompt = $"다음 크래시 리포트를 분석하고 기본 정보를 추출해주세요:\n\n{crashReport}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 크래시 분석 에이전트
/// </summary>
public class AnalyzerAgent
{
    private readonly AIAgent _agent;

    public AnalyzerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 크래시 분석 전문가입니다.
                
                당신의 역할:
                1. 크래시 원인을 분석합니다
                2. 에러 타입을 분류합니다 (NullReference, Exception, Crash 등)
                3. 발생 위치를 파악합니다
                4. 재현 단계를 추정합니다
                
                출력 형식:
                - 에러타입: [분류]
                - 원인: [상세 설명]
                - 발생위치: [모듈/함수]
                - 재현단계: [추정]
                """,
            name: "AnalyzerAgent"
        );
    }

    public async Task<string> AnalyzeAsync(string crashReport)
    {
        var prompt = $"다음 크래시 리포트를 심층 분석해주세요:\n\n{crashReport}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 심각도 평가 에이전트
/// </summary>
public class SeverityAgent
{
    private readonly AIAgent _agent;

    public SeverityAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 버그 심각도 평가 전문가입니다.
                
                당신의 역할:
                1. 크래시의 영향을 평가합니다
                2. 사용자 영향도를 계산합니다
                3. 우선순위를 결정합니다
                4. 담당 팀을 제안합니다
                
                심각도 기준:
                - Critical: 게임 실행 불가, 데이터 손실
                - High: 주요 기능 사용 불가
                - Medium: 일부 기능 제한
                - Low: 경미한 문제
                
                출력 형식:
                - 심각도: [Critical/High/Medium/Low]
                - 우선순위: [1-5]
                - 사용자영향: [상/중/하]
                - 담당팀: [서버/클라이언트/인프라]
                """,
            name: "SeverityAgent"
        );
    }

    public async Task<string> EvaluateSeverityAsync(string analysisReport)
    {
        var prompt = $"다음 분석 리포트를 바탕으로 심각도를 평가해주세요:\n\n{analysisReport}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
