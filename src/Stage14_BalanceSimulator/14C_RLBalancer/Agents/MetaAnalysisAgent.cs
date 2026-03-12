namespace _14C_RLBalancer.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _14C_RLBalancer.Services;

/// <summary>
/// 메타 분석 에이전트
/// </summary>
public class MetaAnalysisAgent
{
    private readonly AIAgent _agent;

    public MetaAnalysisAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 메타 분석 전문가입니다.
                
                당신의 역할:
                1. 자기플레이 결과를 분석합니다
                2. 메타 빌드를 식별합니다
                3. 카운터 관계를 파악합니다
                4. 밸런스 조정 방안을 제안합니다
                
                분석 항목:
                - 상위 메타 빌드 (승률 기준)
                - 빌드 간 상성 관계
                - 메타 다양성
                - 지배적 빌드 존재 여부
                
                출력 형식:
                - 메타구성: [상위 빌드 목록]
                - 상성관계: [카운터/피카운터]
                - 메타건강도: [다양성 평가]
                - 조정방안: [밸런스 패치 제안]
                """,
            name: "MetaAnalysisAgent"
        );
    }

    public async Task<string> AnalyzeMetaAsync(List<SelfPlayMatch> matches)
    {
        var buildStats = matches
            .SelectMany(m => new[]
            {
                new { Build = m.Player1.BuildName, Win = m.Result.AttackerWinRate > 50 ? 1 : 0, Total = 1 },
                new { Build = m.Player2.BuildName, Win = m.Result.DefenderWinRate > 50 ? 1 : 0, Total = 1 }
            })
            .GroupBy(b => b.Build)
            .Select(g => new
            {
                BuildName = g.Key,
                WinRate = g.Sum(x => x.Win) / (double)g.Count() * 100,
                Matches = g.Count()
            })
            .OrderByDescending(b => b.WinRate)
            .ToList();

        var buildStatsText = string.Join("\n", buildStats.Select(b => 
            $"{b.BuildName}: 승률 {b.WinRate:F1}%, 경기수 {b.Matches}"
        ));

        var metaTypes = matches.GroupBy(m => m.MetaType)
            .Select(g => $"{g.Key}: {g.Count()}회")
            .ToList();
        
        var metaText = string.Join("\n", metaTypes);

        var prompt = $"""
            다음 자기플레이 결과를 분석하여 메타를 파악해주세요:
            
            빌드별 승률:
            {buildStatsText}
            
            메타 분포:
            {metaText}
            
            총 경기: {matches.Count}
            
            현재 메타를 분석하고 밸런스 조정 방안을 제안해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }

    public async Task<string> SuggestBalancePatchAsync(List<MetaAnalysis> metaBuilds)
    {
        var buildsText = string.Join("\n", metaBuilds.Select(b => 
            $"{b.MetaName}: 승률 {b.WinRate:F1}%, 경기 {b.MatchesPlayed}, 카운터: {b.Counters}, 약점: {b.WeakAgainst}"
        ));

        var prompt = $"""
            다음 메타 빌드 데이터를 바탕으로 밸런스 패치를 제안해주세요:
            
            메타 빌드:
            {buildsText}
            
            지배적 메타가 있다면 약화시키고,
            약한 빌드는 강화하는 방향을 제안해주세요.
            
            구체적인 수치 조정을 포함해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
