namespace _10B_BalanceAdjuster.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 밸런스 제안 에이전트
/// </summary>
public class BalanceAgent
{
    private readonly AIAgent _agent;

    public BalanceAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 밸런스 디자이너입니다.
                
                당신의 역할:
                1. 캐릭터/아이템/스킬의 스탯을 분석합니다
                2. 현재 밸런스 문제점을 파악합니다
                3. 조정할 수치를 제안합니다
                4. 예상 영향을 평가합니다
                
                출력 형식:
                - 현재수치: [현 상태]
                - 문제점: [밸런스 이슈]
                - 제안수치: [조정안]
                - 예상영향: [게임에 미칠 영향]
                - 리스크: [잠재적 문제]
                """,
            name: "BalanceAgent"
        );
    }

    public async Task<string> ProposeBalanceAsync(string gameData)
    {
        var prompt = $$"""
            다음 게임 데이터의 밸런스를 분석하고 조정안을 제안해주세요.
            
            게임 데이터:
            {{gameData}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 인간 디자이너 검토 에이전트 (시뮬레이션)
/// </summary>
public class DesignerReviewAgent
{
    private readonly AIAgent _agent;

    public DesignerReviewAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 인간 게임 디자이너로서 AI 의 밸런스 조정안을 검토합니다.
                
                당신의 역할:
                1. AI 제안의 타당성을 평가합니다
                2. 디자인 의도와의 일치성을 확인합니다
                3. 수정 사항을 제안합니다
                4. 승인 여부를 결정합니다
                
                검토 기준:
                - 게임 디자인 철학과 일치하는가?
                - 플레이어 경험에 긍정적인가?
                - 기술적 구현이 가능한가?
                - 다른 시스템과 충돌하지 않는가?
                
                출력 형식:
                - 검토의견: [상세 의견]
                - 수정사항: [구체적 수정안]
                - 승인여부: [승인/수정필요/거부]
                - 우선순위: [높음/보통/낮음]
                """,
            name: "DesignerReviewAgent"
        );
    }

    public async Task<string> ReviewAsync(string balanceProposal, string designIntent)
    {
        var prompt = $$"""
            AI 의 밸런스 조정안을 검토해주세요.
            
            AI 제안:
            {{balanceProposal}}
            
            디자인 의도:
            {{designIntent}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 시뮬레이션 에이전트
/// </summary>
public class SimulationAgent
{
    private readonly AIAgent _agent;

    public SimulationAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 밸런스 시뮬레이터입니다.
                
                당신의 역할:
                1. 제안된 수치를 적용한 시뮬레이션을 수행합니다
                2. 다양한 시나리오에서 테스트합니다
                3. 예상 결과를 분석합니다
                4. 최종 권장사항을 제시합니다
                
                시뮬레이션 항목:
                - PVE 밸런스 (몬스터 전투)
                - PVP 밸런스 (플레이어 대전)
                - 성장 곡선 (레벨업 속도)
                - 경제 밸런스 (재화 수급)
                
                출력 형식:
                - 시뮬레이션결과: [각 항목별 결과]
                - 합격여부: [Pass/Fail]
                - 문제영역: [Issue 영역]
                - 최종권고: [최종 의견]
                """,
            name: "SimulationAgent"
        );
    }

    public async Task<string> SimulateAsync(string balanceProposal, string designerFeedback)
    {
        var prompt = $$"""
            다음 밸런스 조정안에 대한 시뮬레이션을 수행해주세요.
            
            조정안:
            {{balanceProposal}}
            
            디자이너 피드백:
            {{designerFeedback}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
