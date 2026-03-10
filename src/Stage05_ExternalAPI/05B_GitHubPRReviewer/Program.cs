// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _05B_GitHubPRReviewer.Tools;
using _05B_GitHubPRReviewer.Config;

// ==========================================
// 5 단계 B: GitHub PR 리뷰어
// ==========================================

Console.WriteLine("🐙 GitHub PR 리뷰어에 오신 것을 환영합니다!");
Console.WriteLine("Pull Request 를 자동으로 리뷰합니다.\n");

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

var githubSettings = new GitHubSettings
{
    Token = "ghp_your-token-here",
    Owner = "your-org",
    Repo = "your-repo"
};

var githubTool = new GitHubApiTool(githubSettings.Token);

var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 GitHub Pull Request 리뷰 어시스턴트입니다.
        
        당신의 역할:
        1. PR 정보를 가져와서 요약합니다
        2. 변경된 파일을 분석합니다
        3. 코드 리뷰 코멘트를 제안합니다
        
        사용 가능한 도구:
        - GetPullRequest(repo, prNumber): PR 정보 조회
        - GetChangedFiles(repo, prNumber): 변경 파일 목록
        - AddComment(repo, prNumber, comment): 댓글 추가
        
        리뷰 가이드라인:
        - 코드 스타일 일관성
        - 잠재적 버그
        - 성능 문제
        - 테스트 커버리지
        """,
    name: "GitHubPRReviewer",
    tools: [
        AIFunctionFactory.Create(githubTool.GetPullRequest),
        AIFunctionFactory.Create(githubTool.GetChangedFiles),
        AIFunctionFactory.Create(githubTool.AddComment)
    ]
);

Console.WriteLine("✅ GitHub 리뷰어가 초기화되었습니다.\n");

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
