namespace _08A_ItemCreationPipeline.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 아이템 설명 작성 에이전트
/// </summary>
public class DescriptionAgent
{
    private readonly AIAgent _agent;

    public DescriptionAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 아이템 설명 작성 전문가입니다.
                
                당신의 역할:
                1. 아이템 이름을 받아 매력적인 설명을 작성합니다
                2. 아이템의 용도와 특징을 포함합니다
                3. 판타지 세계관에 맞는 어조로 작성합니다
                4. 길이는 2-3 문장으로 합니다
                
                출력 형식:
                - 이름: [아이템명]
                - 설명: [상세 설명]
                - 용도: [사용처]
                """,
            name: "DescriptionAgent"
        );
    }

    public async Task<string> GenerateDescriptionAsync(string itemName)
    {
        var prompt = $"다음 아이템에 대한 설명을 작성해주세요: {itemName}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 아이템 밸런스 검토 에이전트
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
                당신은 게임 밸런스 분석가입니다.
                
                당신의 역할:
                1. 아이템의 성능 수치를 검토합니다
                2. 게임 내 다른 아이템과 비교합니다
                3. 상향/하향 조정 여부를 판단합니다
                4. 희귀도를 제안합니다
                
                출력 형식:
                - 공격력/방어력: [수치]
                - 희귀도: [Common/Uncommon/Rare/Epic/Legendary]
                - 밸런스 평가: [상향/유지/하향]
                - 코멘트: [의견]
                """,
            name: "BalanceAgent"
        );
    }

    public async Task<string> ReviewBalanceAsync(string itemDescription)
    {
        var prompt = $"""
            다음 아이템의 밸런스를 검토해주세요:
            
            {itemDescription}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 아이템 현지화 에이전트
/// </summary>
public class LocalizationAgent
{
    private readonly AIAgent _agent;

    public LocalizationAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 현지화 전문가입니다.
                
                당신의 역할:
                1. 아이템 정보를 영어, 일본어, 중국어로 번역합니다
                2. 게임 용어의 일관성을 유지합니다
                3. 각 언어의 문화적 맥락을 고려합니다
                4. 자연스러운 표현을 사용합니다
                
                출력 형식:
                - 영어: [English translation]
                - 일본어: [日本語翻訳]
                - 중국어: [中文翻译]
                """,
            name: "LocalizationAgent"
        );
    }

    public async Task<string> LocalizeAsync(string itemDescription)
    {
        var prompt = $"""
            다음 아이템 정보를 현지화해주세요:
            
            {itemDescription}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
