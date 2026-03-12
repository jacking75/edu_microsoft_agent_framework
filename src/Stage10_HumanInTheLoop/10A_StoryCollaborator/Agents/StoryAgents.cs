namespace _10A_StoryCollaborator.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 스토리 초안 작성 에이전트
/// </summary>
public class DraftAgent
{
    private readonly AIAgent _agent;

    public DraftAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 스토리 작가입니다.
                
                당신의 역할:
                1. 사용자의 요청에 따라 스토리 초안을 작성합니다
                2. 도입 - 전개 - 절정 - 결말 구조를 따릅니다
                3. 등장인물과 배경을 상세히 묘사합니다
                4. 분량은 A4 1 장 정도 (500 자) 로 합니다
                
                출력 형식:
                - 제목: [스토리 제목]
                - 등장인물: [캐릭터 설명]
                - 배경: [배경 설명]
                - 줄거리: [상세 줄거리]
                """,
            name: "DraftAgent"
        );
    }

    public async Task<string> WriteDraftAsync(string theme)
    {
        var prompt = $"다음 테마로 게임 스토리 초안을 작성해주세요: {theme}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 스토리 계속 작성 에이전트
/// </summary>
public class ContinuationAgent
{
    private readonly AIAgent _agent;

    public ContinuationAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 스토리 속편 작가입니다.
                
                당신의 역할:
                1. 이전 스토리의 맥락을 유지하면서 계속 작성합니다
                2. 인간의 수정 사항을 반영합니다
                3. 일관된 톤과 스타일을 유지합니다
                4. 새로운 전개와 긴장감을 추가합니다
                
                출력 형식:
                - 이전줄거리요약: [요약]
                - 계속되는스토리: [본문]
                - 다음전개예고: [예고]
                """,
            name: "ContinuationAgent"
        );
    }

    public async Task<string> ContinueStoryAsync(string previousStory, string humanFeedback)
    {
        var prompt = $"""
            이전 스토리:
            {previousStory}
            
            인간의 수정 사항:
            {humanFeedback}
            
            위 내용을 반영하여 스토리를 계속 작성해주세요.
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
