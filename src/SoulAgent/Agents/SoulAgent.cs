namespace SoulAgent.Agents;

using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using Microsoft.Agents.AI;
using SoulAgent.Soul;

/// <summary>
/// Soul(핵심 가치/원칙) 을 가진 에이전트
/// </summary>
public class SoulAgentClass
{
    private readonly AIAgent _agent;
    private readonly SoulLoader _soulLoader;

    public SoulAgentClass(SoulLoader soulLoader, string apiKey, string baseUrl)
    {
        _soulLoader = soulLoader;
        _soulLoader.Load();

        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(new ChatClientAgentOptions
        {
            ChatOptions = new()
            {
                Instructions = _soulLoader.BuildSystemPrompt()
            }
        });
    }

    /// <summary>
    /// 에이전트 실행
    /// </summary>
    public async Task<string> RunAsync(string userInput)
    {
        var response = await _agent.RunAsync(userInput);
        return response.ToString() ?? "";
    }

    /// <summary>
    /// Soul 정보 표시
    /// </summary>
    public void ShowSoul()
    {
        _soulLoader.DisplaySoulInfo();
    }

    /// <summary>
    /// Soul 업데이트 (런타임에 가치/원칙 변경)
    /// </summary>
    public void UpdateSoul(SoulDefinition newSoul)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(newSoul, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText("data/soul_profile.json", json);
        _soulLoader.Load();
        Console.WriteLine("✅ Soul 이 업데이트되었습니다.");
    }
}
