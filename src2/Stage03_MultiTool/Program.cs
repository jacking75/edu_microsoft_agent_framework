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

// Tool 1: 날씨 조회
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

// Tool 2: 현재 시간 조회
var timeTool = AIFunctionFactory.Create(
    (string timezone = "KST") =>
    {
        var now = DateTime.Now;
        var offset = timezone.ToUpper() switch
        {
            "KST" => TimeSpan.FromHours(9),
            "UTC" => TimeSpan.Zero,
            "EST" => TimeSpan.FromHours(-5),
            "PST" => TimeSpan.FromHours(-8),
            "JST" => TimeSpan.FromHours(9),
            _ => TimeSpan.FromHours(9)
        };
        
        var targetTime = new DateTimeOffset(now, offset);
        return $"현재 시간 ({timezone}) 은 {targetTime:yyyy-MM-dd HH:mm:ss}입니다.";
    },
    "GetTime",
    "지정된 시간대의 현재 시간을 조회합니다. (KST, UTC, EST, PST, JST)"
);

// Tool 3: 계산기
var calculatorTool = AIFunctionFactory.Create(
    (double a, double b, string operation) =>
    {
        return operation.ToUpper() switch
        {
            "ADD" or "+" => $"{a} + {b} = {a + b}",
            "SUB" or "-" => $"{a} - {b} = {a - b}",
            "MUL" or "*" => $"{a} * {b} = {a * b}",
            "DIV" or "/" => b != 0 ? $"{a} / {b} = {a / b}" : "0 으로 나눌 수 없습니다.",
            _ => "지원하지 않는 연산입니다. (+, -, *, /)"
        };
    },
    "Calculate",
    "두 숫자에 대한 사칙연산을 수행합니다. (operation: +, -, *, /)"
);

var tools = new[] { weatherTool, timeTool, calculatorTool };

var agent = new ChatClientAgent(
    name: "MultiToolAgent",
    instructions: "You are a helpful assistant with multiple tools. Use the appropriate tool based on the user's request: GetWeather for weather, GetTime for time, Calculate for math calculations.",
    chatClient: chatClient,
    tools: tools
);

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║     Stage 03 - Multi-Tool Agent            ║");
Console.WriteLine("║     Type 'quit' to exit                    ║");
Console.WriteLine("║     Tools: 날씨, 시간, 계산기                 ║");
Console.WriteLine("║     Try: '서울 날씨', '현재 시간', '10 + 20'  ║");
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
