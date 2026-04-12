using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
var chatClientInner = openAiClient.GetChatClient(model);
IChatClient chatClient = chatClientInner.AsIChatClient();

// 대화 메모리 관리
var conversationHistory = new List<Microsoft.Extensions.AI.ChatMessage>();
const int maxHistoryLength = 20; // 최대 메시지 수 (짝수로 유지)

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║     Stage 06 - Memory Agent                ║");
Console.WriteLine("║     Type 'quit' to exit                    ║");
Console.WriteLine("║     Type 'clear' to clear memory           ║");
Console.WriteLine("║     Type 'history' to show conversation    ║");
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
    
    if (input.ToLower() == "clear")
    {
        conversationHistory.Clear();
        Console.WriteLine("[Memory cleared]");
        Console.WriteLine();
        continue;
    }
    
    if (input.ToLower() == "history")
    {
        Console.WriteLine();
        Console.WriteLine("=== Conversation History ===");
        for (int i = 0; i < conversationHistory.Count; i++)
        {
            var msg = conversationHistory[i];
            Console.WriteLine($"{msg.Role}: {msg.Text}");
        }
        Console.WriteLine($"[Total: {conversationHistory.Count} messages]");
        Console.WriteLine();
        continue;
    }
    
    // 사용자 메시지를 히스토리에 추가
    conversationHistory.Add(new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, input));
    
    Console.Write("Agent: ");
    
    // AI 와 대화 (히스토리 사용)
    var chatOptions = new Microsoft.Extensions.AI.ChatOptions
    {
        Temperature = 0.7f,
        MaxOutputTokens = 1000
    };
    
    var response = await chatClient.GetResponseAsync(
        new[] { new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, input) },
        chatOptions
    );
    
    // 응답 출력
    Console.WriteLine(response.Text);
    Console.WriteLine();
    
    // 응답을 히스토리에 추가
    conversationHistory.Add(new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.Assistant, response.Text));
    
    // 히스토리 길이 제한 (최대 maxHistoryLength 개 유지)
    while (conversationHistory.Count > maxHistoryLength)
    {
        // 가장 오래된 메시지부터 제거 (User 와 Assistant 쌍으로)
        conversationHistory.RemoveAt(0);
        conversationHistory.RemoveAt(0);
    }
}

Console.WriteLine("Goodbye!");
