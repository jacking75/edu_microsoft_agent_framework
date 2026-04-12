using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
var chatClientInner = openAiClient.GetChatClient(model);
IChatClient chatClient = chatClientInner.AsIChatClient();

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║     Stage 01 - Hello Agent                 ║");
Console.WriteLine("║     Type 'quit' to exit                    ║");
Console.WriteLine("╚════════════════════════════════════════════╝");
Console.WriteLine();

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "quit")
        break;
    
    Console.Write("Agent: ");
    
    var response = await chatClient.GetResponseAsync(
        new[] { new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, input) },
        new Microsoft.Extensions.AI.ChatOptions { MaxOutputTokens = 1000 }
    );
    
    Console.WriteLine(response.Text);
    Console.WriteLine();
}

Console.WriteLine("Goodbye!");
