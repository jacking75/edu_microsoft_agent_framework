// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _05C_SlackNotifier.Tools;
using _05C_SlackNotifier.Config;

// ==========================================
// 5 단계 C: Slack 알림 에이전트
// ==========================================

Console.WriteLine("💬 Slack 알림 에이전트에 오신 것을 환영합니다!");
Console.WriteLine("중요 이벤트를 Slack 으로 전송합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

var slackSettings = new SlackSettings
{
    WebhookUrl = "https://hooks.slack.com/services/YOUR/WEBHOOK/URL",
    DefaultChannel = "#dev-alerts",
    BotUsername = "Agent Bot"
};

var slackTool = new SlackWebhookTool(slackSettings.WebhookUrl);

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 Slack 알림 관리 어시스턴트입니다.
        
        당신의 역할:
        1. 중요한 이벤트를 Slack 채널에 알림합니다
        2. 알림 레벨에 따라 적절한 형식을 사용합니다
        3. 메시지를 요약하고 핵심 정보를 전달합니다
        
        사용 가능한 도구:
        - SendMessage(channel, message, username): 일반 메시지
        - SendNotification(title, message, level): 레벨별 알림
        
        알림 레벨:
        - error: 🔴 긴급 에러
        - warning: 🟡 주의 필요
        - success: 🟢 작업 완료
        - info: 🔵 일반 정보
        
        주의사항:
        - 민감한 정보는 마스킹
        - 중요한 알림은 확인 요청
        """,
    name: "SlackNotifier",
    tools: [
        AIFunctionFactory.Create(slackTool.SendMessage),
        AIFunctionFactory.Create(slackTool.SendNotification)
    ]
);

Console.WriteLine("✅ Slack 에이전트가 초기화되었습니다.\n");

Console.WriteLine("명령을 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput) || 
        userInput.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        var response = await agent.RunAsync(userInput);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
