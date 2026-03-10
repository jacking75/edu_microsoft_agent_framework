// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _05A_JiraIssueBot.Tools;
using _05A_JiraIssueBot.Config;

// ==========================================
// 5 단계 A: Jira 이슈 생성 봇
// ==========================================
// 학습 목표:
// 1. 외부 REST API 연동
// 2. 인증 처리 (API Key)
// 3. 자연어를 API 호출로 변환
// ==========================================

Console.WriteLine("🎫 Jira 이슈 생성 봇에 오신 것을 환영합니다!");
Console.WriteLine("자연어로 Jira 이슈를 생성합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// Jira 설정 (실제 사용 시 환경 변수에서 읽기)
var jiraSettings = new JiraSettings
{
    BaseUrl = "https://your-company.atlassian.net",
    ApiKey = "your-jira-api-token",
    ProjectKey = "PROJ"
};

var jiraTool = new JiraApiTool(jiraSettings.BaseUrl, jiraSettings.ApiKey);

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 Jira 이슈 관리 어시스턴트입니다.
        
        당신의 역할:
        1. 사용자의 자연어 요청을 Jira 이슈로 변환합니다
        2. 이슈를 생성하고 키를 반환합니다
        3. 이슈를 검색하고 상태를 보고합니다
        
        사용 가능한 도구:
        - CreateIssue(summary, description, issueType): 이슈 생성
        - SearchIssues(query): JQL 로 이슈 검색
        
        이슈 타입: Story, Task, Bug, Epic
        
        주의사항:
        - 실제 API 호출 전 사용자의 확인을 받습니다
        - 민감한 정보는 마스킹합니다
        """,
    name: "JiraIssueBot",
    tools: [
        AIFunctionFactory.Create(jiraTool.CreateIssue),
        AIFunctionFactory.Create(jiraTool.SearchIssues)
    ]
);

Console.WriteLine("✅ Jira 봇이 초기화되었습니다.\n");

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
