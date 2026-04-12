using System.ClientModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseWebSockets();

// AI Agent 설정
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
var chatClientInner = openAiClient.GetChatClient(model);
IChatClient chatClient = chatClientInner.AsIChatClient();

// Canvas 상태 관리
var canvasState = new CanvasState();

// WebSocket 엔드포인트
app.Map("/ws", async (HttpContext context, WebSocket webSocket) =>
{
    var buffer = new byte[1024 * 4];
    
    // 연결 시 초기 상태 전송
    await SendCanvasUpdate(webSocket, canvasState);
    
    while (webSocket.State == WebSocketState.Open)
    {
        try
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                break;
            }
            
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            
            // AI Agent 에게 메시지 전송
            var response = await chatClient.GetResponseAsync(
                new[] { new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, message) },
                new Microsoft.Extensions.AI.ChatOptions { MaxOutputTokens = 500 }
            );
            
            // Canvas 업데이트
            canvasState.AddNote(response.Text);
            await SendCanvasUpdate(webSocket, canvasState);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
});

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.Now }));

// Canvas 상태 초기화
app.MapPost("/reset", () =>
{
    canvasState.Reset();
    return Results.Ok(new { status = "reset" });
});

app.Run();

static async Task SendCanvasUpdate(WebSocket webSocket, CanvasState state)
{
    var json = JsonSerializer.Serialize(state);
    var bytes = Encoding.UTF8.GetBytes(json);
    await webSocket.SendAsync(
        new ArraySegment<byte>(bytes),
        WebSocketMessageType.Text,
        true,
        CancellationToken.None
    );
}

// Canvas 상태 클래스
class CanvasState
{
    public List<CanvasNote> Notes { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public void AddNote(string text)
    {
        Notes.Add(new CanvasNote
        {
            Id = Notes.Count + 1,
            Content = text,
            Timestamp = DateTime.Now,
            Color = GetRandomColor()
        });
        LastUpdated = DateTime.Now;
    }

    public void Reset()
    {
        Notes.Clear();
        LastUpdated = DateTime.Now;
    }

    private static string GetRandomColor()
    {
        var colors = new[] { "#FFB3BA", "#BAFFC9", "#BAE1FF", "#FFFFBA", "#FFDFBA" };
        return colors[Random.Shared.Next(colors.Length)];
    }
}

class CanvasNote
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string Color { get; set; } = "#FFFFFF";
}
