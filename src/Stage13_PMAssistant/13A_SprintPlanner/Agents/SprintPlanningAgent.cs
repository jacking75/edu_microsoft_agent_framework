namespace _13A_SprintPlanner.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _13A_SprintPlanner.Services;

/// <summary>
/// 스프린트 계획 에이전트
/// </summary>
public class SprintPlanningAgent
{
    private readonly AIAgent _agent;

    public SprintPlanningAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 애자일 스프린트 계획 전문가입니다.
                
                당신의 역할:
                1. 백로그 아이템을 분석하여 우선순위를 매깁니다
                2. 팀 벨로시티를 기반으로 스프린트 목표를 설정합니다
                3. 작업을 일수에 맞게 분배합니다
                4. 리스크와 의존성을 식별합니다
                
                출력 형식:
                - 스프린트목표: [목표]
                - 포함작업: [목록]
                - 일별계획: [계획]
                - 리스크: [리스크]
                """,
            name: "SprintPlanningAgent"
        );
    }

    public async Task<string> CreateSprintPlanAsync(List<JiraIssue> issues, int sprintVelocity)
    {
        var issuesText = string.Join("\n", issues.Select(i => 
            $"{i.Id}: {i.Title} ({i.StoryPoints}점) - {i.Status}"
        ));

        var prompt = $"""
            다음 백로그 아이템으로 스프린트 계획을 수립해주세요:
            
            백로그:
            {issuesText}
            
            팀 벨로시티: {sprintVelocity} 스토리 포인트
            
            2 주 스프린트 (10 영업일) 계획을 만들어주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
