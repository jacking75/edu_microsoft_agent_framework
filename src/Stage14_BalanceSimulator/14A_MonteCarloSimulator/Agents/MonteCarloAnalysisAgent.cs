namespace _14A_MonteCarloSimulator.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _14A_MonteCarloSimulator.Services;

/// <summary>
/// 몬테카를로 시뮬레이션 분석 에이전트
/// </summary>
public class MonteCarloAnalysisAgent
{
    private readonly AIAgent _agent;

    public MonteCarloAnalysisAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 밸런스 시뮬레이션 분석 전문가입니다.
                
                당신의 역할:
                1. 몬테카를로 시뮬레이션 결과를 분석합니다
                2. 승률과 평균 라운드를 해석합니다
                3. 밸런스 조정 방안을 제안합니다
                4. 최적의 파라미터를 추천합니다
                
                분석 기준:
                - 승률 45-55%: 양호한 밸런스
                - 승률 40-60%: 허용 가능한 밸런스
                - 승률 40% 미만/60% 초과: 조정 필요
                
                출력 형식:
                - 시뮬레이션결과요약: [요약]
                - 밸런스평가: [평가]
                - 조정방안: [제안]
                - 권장파라미터: [권장]
                """,
            name: "MonteCarloAnalysisAgent"
        );
    }

    public async Task<string> AnalyzeResultAsync(SimulationResult result)
    {
        var prompt = $"""
            다음 몬테카를로 시뮬레이션 결과를 분석해주세요:
            
            - 시뮬레이션 횟수: {result.Iterations}회
            - 공격자 승률: {result.AttackerWinRate:F2}%
            - 수비자 승률: {result.DefenderWinRate:F2}%
            - 무승부: {result.DrawRate:F2}%
            - 평균 라운드: {result.AverageRounds:F2}
            
            밸런스를 평가하고 조정 방안을 제안해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
