namespace _08B_QuestCreationPipeline.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 퀘스트 스토리 작성 에이전트
/// </summary>
public class StoryAgent
{
    private readonly AIAgent _agent;

    public StoryAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 퀘스트 스토리 작가입니다.
                
                당신의 역할:
                1. 퀘스트 제목을 받아 흥미로운 스토리를 작성합니다
                2. 퀘스트의 배경, 동기, 목표를 포함합니다
                3. 판타지 세계관에 맞는 내러티브로 작성합니다
                4. 플레이어의 몰입감을 높이는 구성을 사용합니다
                
                출력 형식:
                - 제목: [퀘스트명]
                - 배경: [스토리 배경]
                - 목표: [퀘스트 목표]
                - 등장인물: [주요 NPC]
                """,
            name: "StoryAgent"
        );
    }

    public async Task<string> GenerateStoryAsync(string questTitle)
    {
        var prompt = $"다음 제목으로 퀘스트 스토리를 작성해주세요: {questTitle}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 퀘스트 보상 설계 에이전트
/// </summary>
public class RewardAgent
{
    private readonly AIAgent _agent;

    public RewardAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 이코노미 디자이너입니다.
                
                당신의 역할:
                1. 퀘스트 스토리를 분석하여 적절한 보상을 설계합니다
                2. 경험치, 골드를 포함한 기본 보상을 결정합니다
                3. 아이템 보상을 제안합니다
                4. 퀘스트 난이도와 노력에 상응하는 보상을 책정합니다
                
                출력 형식:
                - 경험치: [수치]
                - 골드: [수치]
                - 아이템: [보상 아이템]
                - 특수 보상: [칭호, 업적 등]
                - 설계 의도: [코멘트]
                """,
            name: "RewardAgent"
        );
    }

    public async Task<string> DesignRewardAsync(string questStory)
    {
        var prompt = $"""
            다음 퀘스트 스토리를 분석하여 보상을 설계해주세요:
            
            {questStory}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 퀘스트 난이도 조정 에이전트
/// </summary>
public class DifficultyAgent
{
    private readonly AIAgent _agent;

    public DifficultyAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 밸런스 디자이너입니다.
                
                당신의 역할:
                1. 퀘스트 내용과 보상을 분석하여 난이도를 설정합니다
                2. 권장 레벨을 제안합니다
                3. 예상 소요 시간을 산출합니다
                4. 난이도 곡선을 고려한 조정안을 제시합니다
                
                출력 형식:
                - 난이도: [Very Easy/Easy/Normal/Hard/Very Hard]
                - 권장 레벨: [Lv. X]
                - 예상 시간: [X 분]
                - 추천 파티 구성: [파티 구성 제안]
                - 밸런스 코멘트: [의견]
                """,
            name: "DifficultyAgent"
        );
    }

    public async Task<string> AdjustDifficultyAsync(string questStory, string questReward)
    {
        var prompt = $"""
            다음 퀘스트의 난이도를 조정해주세요:
            
            [스토리]
            {questStory}
            
            [보상]
            {questReward}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
