using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
var chatClientInner = openAiClient.GetChatClient(model);
IChatClient chatClient = chatClientInner.AsIChatClient();

var weatherTool = AIFunctionFactory.Create(
    (string city) =>
    {
        var temperatures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["seoul"] = "25°C, 맑음",
            ["busan"] = "23°C, 구름 조금",
            ["incheon"] = "24°C, 맑음",
            ["daegu"] = "27°C, 흐림",
            ["jeju"] = "22°C, 비",
        };

        return temperatures.TryGetValue(city, out var temp) 
            ? $"{city}의 현재 날씨는 {temp}입니다." 
            : $"{city}의 날씨 정보를 찾을 수 없습니다. (서울, 부산, 인천, 대구, 제주 중 하나를 입력하세요)";
    },
    "GetWeather",
    "지정된 도시의 현재 날씨를 조회합니다."
);

var agent = new ChatClientAgent(
    name: "WeatherAgent",
    instructions: "You are a helpful weather assistant. Use the GetWeather tool to get weather information for cities.",
    chatClient: chatClient,
    tools: [weatherTool]
);

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║     Stage 02 - Single Tool Agent           ║");
Console.WriteLine("║     Type 'quit' to exit                    ║");
Console.WriteLine("║     Try: '서울 날씨 어때?'                   ║");
Console.WriteLine("╚════════════════════════════════════════════╝");
Console.WriteLine();

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "quit")
        break;
    
    Console.Write("Agent: ");
    
    await foreach (var update in agent.RunStreamingAsync(input))
    {
        Console.Write(update);
    }
    
    Console.WriteLine();
    Console.WriteLine();
}

Console.WriteLine("Goodbye!");
