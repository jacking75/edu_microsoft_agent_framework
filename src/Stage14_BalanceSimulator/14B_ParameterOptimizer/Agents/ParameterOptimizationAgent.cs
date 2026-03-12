namespace _14B_ParameterOptimizer.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _14B_ParameterOptimizer.Services;

/// <summary>
/// 파라미터 최적화 에이전트
/// </summary>
public class ParameterOptimizationAgent
{
    private readonly AIAgent _agent;

    public ParameterOptimizationAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 밸런스 파라미터 최적화 전문가입니다.
                
                당신의 역할:
                1. 그리드 서치 결과를 분석합니다
                2. 최적의 파라미터 조합을 찾습니다
                3. 밸런스 점수를 해석합니다
                4. 권장 설정을 제안합니다
                
                분석 기준:
                - 밸런스 점수 90+: 완벽한 밸런스
                - 밸런스 점수 80-89: 우수한 밸런스
                - 밸런스 점수 70-79: 양호한 밸런스
                - 밸런스 점수 60-69: 개선필요
                - 밸런스 점수 <60: 불량
                
                출력 형식:
                - 최적파라미터: [권장 조합]
                - 밸런스분석: [상세 분석]
                - 민감도분석: [파라미터 영향도]
                - 권장사항: [조정 방안]
                """,
            name: "ParameterOptimizationAgent"
        );
    }

    public async Task<string> AnalyzeOptimizationAsync(
        List<GridSearchResult> results, 
        BattleParams baseParams)
    {
        var topResults = results.Take(5).ToList();
        
        var resultsText = string.Join("\n", topResults.Select((r, i) => 
            $"{i + 1}. {r.Params.Name}: 밸런스점수 {r.BalanceScore:F1} " +
            $"(승률 {r.Result.AttackerWinRate:F1}% vs {r.Result.DefenderWinRate:F1}%)"
        ));

        var prompt = $"""
            다음 그리드 서치 결과를 분석하여 최적 파라미터를 제안해주세요:
            
            기본 파라미터:
            HP {baseParams.HP}, Damage {baseParams.Damage}, CritChance {baseParams.CritChance:P0}
            
            Top 5 결과:
            {resultsText}
            
            총 탐색 조합: {results.Count}개
            
            최적의 밸런스 파라미터를 제안해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }

    public async Task<string> VisualizeResultsAsync(List<GridSearchResult> results)
    {
        var summary = results.GroupBy(r => (int)(r.BalanceScore / 10) * 10)
            .OrderBy(g => g.Key)
            .Select(g => $"밸런스 {g.Key}-{g.Key + 9}: {g.Count()}개")
            .ToList();
        
        var summaryText = string.Join("\n", summary);
        
        var prompt = $"""
            그리드 서치 결과를 시각화해주세요:
            
            분포:
            {summaryText}
            
            총 결과: {results.Count}개
            최고점수: {results.Max(r => r.BalanceScore):F1}
            평균점수: {results.Average(r => r.BalanceScore):F1}
            
            결과를 해석하고 시각적으로 표현해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
