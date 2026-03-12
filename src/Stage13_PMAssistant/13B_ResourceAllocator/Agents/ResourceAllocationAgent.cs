namespace _13B_ResourceAllocator.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _13B_ResourceAllocator.Services;

/// <summary>
/// 리소스 배분 최적화 에이전트
/// </summary>
public class ResourceAllocationAgent
{
    private readonly AIAgent _agent;

    public ResourceAllocationAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 리소스 배분 최적화 전문가입니다.
                
                당신의 역할:
                1. 팀원의 역량과 현재 작업량을 분석합니다
                2. 작업의 필수 스킬과 우선순위를 고려합니다
                3. 최적의 작업 배정을 제안합니다
                4. 과부하와 리스크를 예방합니다
                
                배분 원칙:
                - 스킬 매칭: 작업에 필요한 스킬을 가진 팀원에게 배정
                - 작업량 균형: 팀원 간 작업량이 고르게 분배
                - 우선순위: High > Medium > Low 순으로 처리
                - 마감일 준수: 기한이 짧은 작업을 우선
                - 학습 기회: 주니어에게 적절한 난이도 배정
                
                출력 형식:
                - 배정결과: [팀원별 작업 목록]
                - 매칭근거: [스킬 매칭 설명]
                - 작업량분포: [팀원별 부하]
                - 리스크: [잠재적 문제]
                - 권장사항: [추가 조언]
                """,
            name: "ResourceAllocationAgent"
        );
    }

    public async Task<string> AllocateResourcesAsync(
        List<TeamMember> teamMembers, 
        List<WorkItem> workItems,
        List<AllocationHistory> history)
    {
        var teamText = string.Join("\n", teamMembers.Select(m => 
            $"{m.Id}: {m.Name} ({m.Role}) - 스킬: {string.Join(", ", m.Skills)}, 가용률: {m.AvailableCapacity:P0}"
        ));

        var workText = string.Join("\n", workItems.Select(w => 
            $"{w.Id}: {w.Title} - 스킬: {string.Join(", ", w.RequiredSkills)}, {w.EstimatedHours}h, 우선순위: {w.Priority}, 마감: {w.Deadline:MM/dd}"
        ));

        var historyText = string.Join("\n", history.Take(5).Select(h => 
            $"{h.MemberId} → {h.WorkItemId}: {h.EstimatedHours}h → {h.ActualHours}h (품질: {h.Quality}/5)"
        ));

        var prompt = $"""
            다음 데이터로 최적의 리소스 배정을 제안해주세요:
            
            팀원 정보:
            {teamText}
            
            작업 아이템:
            {workText}
            
            과거 배분 이력:
            {historyText}
            
            스킬 매칭, 작업량 균형, 우선순위를 고려하여 배정해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }

    public async Task<string> AnalyzeWorkloadAsync(List<TeamMember> teamMembers)
    {
        var teamText = string.Join("\n", teamMembers.Select(m => 
            $"{m.Name}: 현재부하 {m.CurrentWorkload:P0}, 가용률 {m.AvailableCapacity:P0}"
        ));

        var prompt = $"""
            팀원들의 현재 작업량을 분석해주세요:
            
            {teamText}
            
            과부하/저부하 팀원과 리밸런싱 방안을 제안해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
