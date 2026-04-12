using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

// OpenAI 클라이언트 설정
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
var chatClientInner = openAiClient.GetChatClient(model);
IChatClient chatClient = chatClientInner.AsIChatClient();

// 특화된 에이전트들 정의
var codingAgent = new CodingSpecialistAgent(chatClient);
var writingAgent = new WritingSpecialistAgent(chatClient);
var mathAgent = new MathSpecialistAgent(chatClient);

// 오케스트레이터 - 작업 라우팅 담당
var orchestrator = new AgentOrchestrator(
    chatClient,
    new ISpecialistAgent[] { codingAgent, writingAgent, mathAgent }
);

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║     Stage 07 - Multi-Agent System          ║");
Console.WriteLine("║     Type 'quit' to exit                    ║");
Console.WriteLine("║     Agents: Coding, Writing, Math          ║");
Console.WriteLine("╚════════════════════════════════════════════╝");
Console.WriteLine();

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input))
        continue;
    
    if (input.ToLower() == "quit")
        break;
    
    // 오케스트레이터가 적절한 에이전트에게 라우팅
    var (selectedAgent, response) = await orchestrator.RouteAndExecuteAsync(input);
    
    Console.WriteLine($"[{selectedAgent}]: {response}");
    Console.WriteLine();
}

Console.WriteLine("Goodbye!");

// ============================================
// 에이전트 오케스트레이터
// ============================================
class AgentOrchestrator
{
    private readonly IChatClient _chatClient;
    private readonly ISpecialistAgent[] _agents;

    public AgentOrchestrator(IChatClient chatClient, ISpecialistAgent[] agents)
    {
        _chatClient = chatClient;
        _agents = agents;
    }

    public async Task<(string AgentName, string Response)> RouteAndExecuteAsync(string userMessage)
    {
        // 어떤 에이전트가 적합한지 판단
        var agentDescriptions = string.Join("\n", 
            _agents.Select(a => $"- {a.Name}: {a.Description}"));

        var routingPrompt = $"""
            You are a router that selects the most appropriate specialist agent.
            
            Available agents:
            {agentDescriptions}
            
            User message: {userMessage}
            
            Select ONLY the agent name (CodingSpecialist, WritingSpecialist, or MathSpecialist) that is best suited for this task.
            Respond with only the agent name, nothing else.
            """;

        var routingResponse = await _chatClient.GetResponseAsync(routingPrompt);
        var selectedAgentName = routingResponse.Text.Trim();

        // 적합한 에이전트 찾기
        var selectedAgent = _agents.FirstOrDefault(a => 
            a.Name.Contains(selectedAgentName, StringComparison.OrdinalIgnoreCase))
            ?? _agents[0]; // 기본값으로 첫 번째 에이전트 사용

        // 선택된 에이전트에게 작업 수행
        var response = await selectedAgent.HandleAsync(userMessage);

        return (selectedAgent.Name, response);
    }
}

// ============================================
// 에이전트 인터페이스
// ============================================
interface ISpecialistAgent
{
    string Name { get; }
    string Description { get; }
    Task<string> HandleAsync(string message);
}

// ============================================
// 코딩 전문 에이전트
// ============================================
class CodingSpecialistAgent : ISpecialistAgent
{
    private readonly IChatClient _chatClient;

    public string Name => "CodingSpecialist";
    public string Description => "Expert in programming, code review, debugging, and software development.";

    public CodingSpecialistAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> HandleAsync(string message)
    {
        var prompt = $"""
            You are an expert programmer. Help with:
            - Writing clean, efficient code
            - Code review and optimization
            - Debugging and troubleshooting
            - Explaining programming concepts
            
            User request: {message}
            
            Provide a clear, helpful response with code examples when appropriate.
            """;

        var response = await _chatClient.GetResponseAsync(prompt);
        return response.Text;
    }
}

// ============================================
// 작문 전문 에이전트
// ============================================
class WritingSpecialistAgent : ISpecialistAgent
{
    private readonly IChatClient _chatClient;

    public string Name => "WritingSpecialist";
    public string Description => "Expert in writing, editing, translation, and content creation.";

    public WritingSpecialistAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> HandleAsync(string message)
    {
        var prompt = $"""
            You are an expert writer and editor. Help with:
            - Writing and editing documents
            - Translation (Korean ↔ English)
            - Content creation
            - Grammar and style improvement
            
            User request: {message}
            
            Provide a clear, well-written response.
            """;

        var response = await _chatClient.GetResponseAsync(prompt);
        return response.Text;
    }
}

// ============================================
// 수학 전문 에이전트
// ============================================
class MathSpecialistAgent : ISpecialistAgent
{
    private readonly IChatClient _chatClient;

    public string Name => "MathSpecialist";
    public string Description => "Expert in mathematics, calculations, and problem solving.";

    public MathSpecialistAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> HandleAsync(string message)
    {
        var prompt = $"""
            You are a mathematics expert. Help with:
            - Mathematical calculations
            - Problem solving
            - Explaining mathematical concepts
            - Step-by-step solutions
            
            User request: {message}
            
            Provide accurate calculations and clear explanations.
            """;

        var response = await _chatClient.GetResponseAsync(prompt);
        return response.Text;
    }
}
