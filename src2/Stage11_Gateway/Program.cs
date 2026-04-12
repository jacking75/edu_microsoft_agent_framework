using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenAI;
using Serilog;

// Serilog 설정
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/gateway-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Gateway");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog 적용
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/gateway-.log", rollingInterval: RollingInterval.Day));

    // CORS 설정
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });

    // AI Agent 설정
    var apiKey = builder.Configuration["OPENAI_API_KEY"] 
        ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
    var model = builder.Configuration["OPENAI_MODEL"] ?? "gpt-4o-mini";

    var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
    var chatClientInner = openAiClient.GetChatClient(model);
    IChatClient chatClient = chatClientInner.AsIChatClient();

    builder.Services.AddSingleton(chatClient);

    // Health Checks 설정
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy())
        .AddAsyncCheck("openai", async () =>
        {
            try
            {
                var response = await chatClient.GetResponseAsync("Hello");
                return HealthCheckResult.Healthy("OpenAI responsive");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("OpenAI error", ex);
            }
        });

    var app = builder.Build();

    app.UseCors("AllowAll");
    app.UseSerilogRequestLogging();

    // Health Check 엔드포인트
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready");

    // AI Chat 엔드포인트
    app.MapPost("/api/chat", async (
        ChatRequest request,
        IChatClient chatClient,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Received chat request from {User}", request.User ?? "anonymous");
        
        try
        {
            var response = await chatClient.GetResponseAsync(
                request.Message,
                new ChatOptions { MaxOutputTokens = request.MaxTokens ?? 1000 }
            );

            return Results.Ok(new ChatResponse(
                Message: response.Text,
                Timestamp: DateTime.Now,
                Model: model
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing chat request");
            return Results.Problem(ex.Message);
        }
    });

    // Streaming Chat 엔드포인트
    app.MapPost("/api/chat/stream", async (
        ChatRequest request,
        IChatClient chatClient,
        ILogger<Program> logger,
        HttpContext httpContext) =>
    {
        logger.LogInformation("Received streaming chat request");

        httpContext.Response.ContentType = "text/event-stream";
        await foreach (var chunk in chatClient.GetStreamingResponseAsync(request.Message))
        {
            await httpContext.Response.WriteAsync(chunk.ToString());
            await httpContext.Response.Body.FlushAsync();
        }
    });

    // Info 엔드포인트
    app.MapGet("/api/info", () => Results.Ok(new
    {
        Service = "Stage 11 Gateway",
        Version = "1.0.0",
        Model = model,
        Timestamp = DateTime.Now
    }));

    // Root 엔드포인트
    app.MapGet("/", () => Results.Ok(new
    {
        Service = "Stage 11 Gateway",
        Status = "running",
        Endpoints = new[]
        {
            "GET  /                  - Service info",
            "GET  /health            - Health check (all checks)",
            "GET  /health/live       - Liveness probe",
            "GET  /health/ready      - Readiness probe",
            "POST /api/chat          - Chat with AI",
            "POST /api/chat/stream   - Streaming chat"
        }
    }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// DTO classes
record ChatRequest(string Message, string? User = null, int? MaxTokens = null);
record ChatResponse(string Message, DateTime Timestamp, string Model);
