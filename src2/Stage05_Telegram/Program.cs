using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);

// AI Agent 설정
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
var chatClientInner = openAiClient.GetChatClient(model);
IChatClient chatClient = chatClientInner.AsIChatClient();

var agent = new ChatClientAgent(
    name: "TelegramAgent",
    instructions: "You are a friendly and helpful Telegram chat assistant. Keep your responses concise and friendly.",
    chatClient: chatClient
);

builder.Services.AddSingleton(agent);

// Telegram Bot 설정
var telegramBotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") 
    ?? throw new InvalidOperationException("TELEGRAM_BOT_TOKEN not set");

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(telegramBotToken));

var app = builder.Build();

// 헬스체크 엔드포인트
app.MapGet("/", () => Results.Ok(new { 
    status = "running", 
    service = "Telegram Bot",
    timestamp = DateTime.Now 
}));

// Webhook 엔드포인트 (Telegram 이 메시지를 보낼 곳)
app.MapPost("/telegram", async (
    HttpContext context, 
    ITelegramBotClient botClient, 
    ChatClientAgent agent,
    ILogger<Program> logger) =>
{
    try
    {
        var update = await System.Text.Json.JsonSerializer.DeserializeAsync<Update>(
            context.Request.Body, 
            cancellationToken: context.RequestAborted
        );

        if (update == null)
        {
            logger.LogWarning("Received null update from Telegram");
            return Results.BadRequest();
        }

        // 채팅 메시지만 처리
        if (update.Message?.Text is string message)
        {
            var chatId = update.Message.Chat.Id;
            var userId = update.Message.From?.Id;
            
            logger.LogInformation("Received message from user {UserId} in chat {ChatId}: {Message}", 
                userId, chatId, message);

            // AI Agent 에게 메시지 전송
            var response = await agent.RunAsync(message, cancellationToken: context.RequestAborted);
            
            // Telegram 으로 응답 전송
            await botClient.SendMessage(
                chatId: chatId,
                text: response.ToString()
            );

            logger.LogInformation("Sent response to chat {ChatId}", chatId);
        }

        return Results.Ok();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing Telegram update");
        return Results.StatusCode(500);
    }
});

app.Run();
