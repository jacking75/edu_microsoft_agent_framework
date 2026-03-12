namespace _09B_BugClassifier.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 로그 분석 에이전트
/// </summary>
public class LogAnalyzerAgent
{
    private readonly AIAgent _agent;

    public LogAnalyzerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 버그 로그 분석 전문가입니다.
                
                당신의 역할:
                1. 버그 리포트의 로그를 분석합니다
                2. 에러 패턴을 식별합니다
                3. 관련 모듈을 파악합니다
                4. 재현 가능성을 평가합니다
                
                출력 형식:
                - 에러패턴: [분류]
                - 관련모듈: [모듈명]
                - 로그수준: [ERROR/WARN/INFO]
                - 재현가능성: [높음/보통/낮음]
                """,
            name: "LogAnalyzerAgent"
        );
    }

    public async Task<string> AnalyzeAsync(string bugReport)
    {
        var prompt = $"다음 버그 리포트의 로그를 분석해주세요:\n\n{bugReport}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 중복 체크 에이전트
/// </summary>
public class DuplicateCheckerAgent
{
    private readonly AIAgent _agent;

    public DuplicateCheckerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 버그 중복 체크 전문가입니다.
                
                당신의 역할:
                1. 새로운 버그와 기존 버그의 유사성을 평가합니다
                2. 중복 여부를 판단합니다
                3. 유사한 기존 버그 ID 를 제안합니다
                4. 병합 필요성을 평가합니다
                
                출력 형식:
                - 중복여부: [Yes/No]
                - 유사버그 ID: [BUG-XXXX 또는 N/A]
                - 유사도: [높음/보통/낮음]
                - 병합권고: [권장/검토/불필요]
                """,
            name: "DuplicateCheckerAgent"
        );
    }

    public async Task<string> CheckDuplicateAsync(string bugReport)
    {
        var prompt = $$"""
            다음 버그 리포트가 기존 버그와 중복인지 확인해주세요.
            기존에 알려진 버그들:
            - BUG-001: 게임 시작시 NullReferenceException 크래시
            - BUG-002: 던전 입장시 연결 끊김
            - BUG-003: 인벤토리 아이템 중복 표시
            - BUG-004: 길찾기 AI 작동 불량
            
            분석할 버그 리포트:
            {{bugReport}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 우선순위 결정 에이전트
/// </summary>
public class PriorityAssignerAgent
{
    private readonly AIAgent _agent;

    public PriorityAssignerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 버그 우선순위 결정 전문가입니다.
                
                당신의 역할:
                1. 버그의 영향도를 평가합니다
                2. 사용자 수를 고려합니다
                3. 비즈니스 영향을 평가합니다
                4. 최종 우선순위를 할당합니다
                
                우선순위 기준:
                - P0 (Critical): 게임 실행 불가, 데이터 손실, 전체 서비스 장애
                - P1 (High): 주요 기능 사용 불가, 다수 사용자 영향
                - P2 (Medium): 일부 기능 제한, 제한적 사용자 영향
                - P3 (Low): 경미한 문제, 일부 사용자 영향
                - P4 (Trivial): 미미한 문제, 차기 버전 처리 가능
                
                출력 형식:
                - 우선순위: [P0/P1/P2/P3/P4]
                - 영향도: [상/중/하]
                -대상사용자: [전체/다수/일부/제한적]
                - 비즈니스영향: [심각함/보통/경미함]
                -권장마일스톤: [마일스톤명]
                """,
            name: "PriorityAssignerAgent"
        );
    }

    public async Task<string> AssignPriorityAsync(string bugReport, string analysisResult)
    {
        var prompt = $$"""
            다음 버그 리포트와 분석 결과를 바탕으로 우선순위를 할당해주세요.
            
            버그 리포트:
            {{bugReport}}
            
            분석 결과:
            {{analysisResult}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
