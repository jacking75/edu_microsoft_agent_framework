namespace _08C_DialogueGenerator.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 대화 초안 작성 에이전트
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
                당신은 게임 NPC 대화문 작가입니다.
                
                당신의 역할:
                1. NPC 이름과 역할을 받아 대화문 초안을 작성합니다
                2. 자연스러운 대화 흐름을 만듭니다
                3. 캐릭터의 개성과 배경을 반영합니다
                4.玩家의 선택지를 포함합니다
                
                출력 형식:
                - NPC: [이름/역할]
                - 상황: [대화 상황]
                - 대화내용: [대화 내용]
                - 선택지: [玩家 응답 옵션 1/2/3]
                """,
            name: "DraftAgent"
        );
    }

    public async Task<string> GenerateDraftAsync(string npcName, string npcRole)
    {
        var prompt = $"다음 NPC 에 대한 대화문 초안을 작성해주세요 - 이름: {npcName}, 역할: {npcRole}";
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 대화 톤 조정 에이전트
/// </summary>
public class ToneAgent
{
    private readonly AIAgent _agent;

    public ToneAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 대화문 편집자입니다.
                
                당신의 역할:
                1. 대화문 초안을 받아 톤앤매너를 조정합니다
                2. 캐릭터 성격 (친절함, 무뚝뚝함, 유머러스함 등) 을 반영합니다
                3. 게임 세계관에 맞는 어조로 수정합니다
                4. 자연스러운 말투로 다듬습니다
                
                출력 형식:
                - 캐릭터 톤: [성격 설명]
                - 수정된 대화: [수정된 대화 내용]
                - 어조: [존댓말/반말/고풍적 등]
                - 수정 사항: [주요 변경점]
                """,
            name: "ToneAgent"
        );
    }

    public async Task<string> AdjustToneAsync(string dialogueDraft, string desiredTone)
    {
        var prompt = $"""
            다음 대화문의 톤을 조정해주세요:
            
            {dialogueDraft}
            
            원하는 톤: {desiredTone}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 대화 분기 생성 에이전트
/// </summary>
public class BranchAgent
{
    private readonly AIAgent _agent;

    public BranchAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 스토리 디자이너입니다.
                
                당신의 역할:
                1. 대화문을 받아 분기 구조를 설계합니다
                2.玩家의 선택에 따른 다른 대화 경로를 만듭니다
                3. 각 분기의 결과를 설계합니다
                4. 일관된 스토리 흐름을 유지합니다
                
                출력 형식:
                - 분기 수: [X 개]
                - 분기 1: [선택지 → 결과]
                - 분기 2: [선택지 → 결과]
                - 분기 3: [선택지 → 결과]
                - 엔딩: [각 분기의 최종 결과]
                """,
            name: "BranchAgent"
        );
    }

    public async Task<string> CreateBranchesAsync(string dialogueToned)
    {
        var prompt = $"""
            다음 대화문에 대한 분기 구조를 생성해주세요:
            
            {dialogueToned}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
