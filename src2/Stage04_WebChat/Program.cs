using System.ClientModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Agents.AI;
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

var agent = new ChatClientAgent(
    name: "WebChatAgent",
    instructions: "You are a friendly and helpful web chat assistant.",
    chatClient: chatClient
);

// WebSocket 엔드포인트
app.Map("/ws", async (HttpContext context, WebSocket webSocket) =>
{
    var buffer = new byte[1024 * 4];
    
    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
        if (result.MessageType == WebSocketMessageType.Close)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            break;
        }
        
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        
        // AI Agent 에게 메시지 전송하고 스트리밍 응답 받기
        var responseBuilder = new StringBuilder();
        await foreach (var update in agent.RunStreamingAsync(message))
        {
            var updateText = update.ToString();
            responseBuilder.Append(updateText);
            var responseBytes = Encoding.UTF8.GetBytes(updateText);
            await webSocket.SendAsync(
                new ArraySegment<byte>(responseBytes), 
                WebSocketMessageType.Text, 
                true, 
                CancellationToken.None
            );
        }
    }
});

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.Now }));

app.Run();
