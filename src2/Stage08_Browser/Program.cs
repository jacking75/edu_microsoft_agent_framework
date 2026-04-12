using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using Microsoft.Playwright;

// OpenAI 클라이언트 설정
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
var chatClientInner = openAiClient.GetChatClient(model);
IChatClient chatClient = chatClientInner.AsIChatClient();

// 브라우저 자동화 도구들
var browserTools = new List<AIFunction>
{
    AIFunctionFactory.Create(
        async (string url) =>
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(url);
            var title = await page.TitleAsync();
            
            var text = await page.EvaluateAsync<string>("() => document.body.innerText");
            var preview = text.Length > 500 ? text.Substring(0, 500) + "..." : text;
            
            return $"페이지 제목: {title}\n\n콘텐츠 미리보기:\n{preview.Trim()}";
        },
        "GetWebPage",
        "웹 페이지의 내용을 가져옵니다. URL 을 입력받습니다."
    ),
    
    AIFunctionFactory.Create(
        async (string url, string selector) =>
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(url);
            
            var element = await page.QuerySelectorAsync(selector);
            if (element == null)
                return $"요소를 찾을 수 없습니다: {selector}";
            
            var text = await element.TextContentAsync();
            return $"[{selector}] 의 텍스트: {text?.Trim()}";
        },
        "ExtractElement",
        "웹 페이지에서 특정 CSS 선택자의 요소 텍스트를 추출합니다."
    ),
    
    AIFunctionFactory.Create(
        async (string url, string query) =>
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(url);
            
            var escapedQuery = query.Replace("'", "\\'");
            var searchText = await page.EvaluateAsync<string>(
                $"() => {{ const text = document.body.innerText; const index = text.toLowerCase().indexOf('{escapedQuery.ToLower()}'); return index >= 0 ? '찾음' : '없음'; }}"
            );
            
            return $"'{query}' 검색 결과: {searchText}";
        },
        "SearchInPage",
        "웹 페이지에서 특정 텍스트를 검색합니다."
    )
};

var systemInstructions = """
    You are a web browsing assistant with access to browser automation tools.
    You can:
    - Get web page content (GetWebPage)
    - Extract specific elements (ExtractElement) 
    - Search for text in pages (SearchInPage)
    
    Always use the appropriate tool when the user asks about web content.
    """;

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║     Stage 08 - Browser Automation          ║");
Console.WriteLine("║     Type 'quit' to exit                    ║");
Console.WriteLine("║     Try: 'https://example.com 내용 보여줘'   ║");
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
    
    Console.Write("Agent: ");
    
    try
    {
        var chatOptions = new Microsoft.Extensions.AI.ChatOptions
        {
            Temperature = 0.7f,
            MaxOutputTokens = 2000,
            Tools = browserTools.Cast<AITool>().ToList()
        };
        
        var messages = new List<Microsoft.Extensions.AI.ChatMessage>
        {
            new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.System, systemInstructions),
            new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, input)
        };
        
        var response = await chatClient.GetResponseAsync(messages, chatOptions);
        Console.WriteLine(response.Text);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[Error: {ex.Message}]");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"[Inner: {ex.InnerException.Message}]");
        }
    }
    
    Console.WriteLine();
}

Console.WriteLine("Goodbye!");
