// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _03A_LogSummarizer.Tools;

// ==========================================
// 3 단계 A: 로그 파일 요약기
// ==========================================
// 학습 목표:
// 1. 파일 읽기 Tool 구현
// 2. 파일 시스템 접근 패턴
// 3. 대용량 텍스트 처리
// 4. 로그 분석 및 요약
// ==========================================

Console.WriteLine("📋 로그 파일 요약기에 오신 것을 환영합니다!");
Console.WriteLine("게임 로그를 분석하고 요약해드립니다.\n");

// 1. 환경 변수 설정
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. 파일 읽기 Tool 생성
var fileTool = new FileReadTool();

// 3. OpenAI 클라이언트 생성
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 4. AIAgent 생성 - Tool 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 로그 분석 전문가입니다.
        
        당신의 역할:
        1. 로그 파일을 읽고 주요 이벤트를 추출합니다
        2. 에러, 경고, 정보 메시지를 분류합니다
        3. 문제 패턴을 발견하고 보고합니다
        4. 요약을 작성하고 개선 사항을 제안합니다
        
        사용 가능한 도구:
        - ReadFile(filePath): 전체 파일 읽기
        - ReadLastLines(filePath, lineCount): 마지막 N 줄 읽기
        - SearchPattern(filePath, pattern): 패턴 검색
        
        로그 분석 시 다음에 주의하세요:
        - [ERROR]: 치명적 오류 - 즉시 조치 필요
        - [WARNING]: 경고 - 모니터링 필요
        - [INFO]: 일반 정보
        """,
    name: "LogSummarizer",
    tools: [
        AIFunctionFactory.Create(fileTool.ReadFile),
        AIFunctionFactory.Create(fileTool.ReadLastLines),
        AIFunctionFactory.Create(fileTool.SearchPattern)
    ]
);

Console.WriteLine("✅ 로그 분석기가 초기화되었습니다.\n");

// 5. 기본 로그 파일 경로
var defaultLogPath = Path.Combine(AppContext.BaseDirectory, "data", "game_log.txt");

if (!File.Exists(defaultLogPath))
{
    var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.FullName 
        ?? AppContext.BaseDirectory;
    defaultLogPath = Path.Combine(projectDir, "data", "game_log.txt");
}

Console.WriteLine($"📁 기본 로그 파일: {defaultLogPath}\n");

// 6. 사용 예시 안내
Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"로그 파일을 요약해줘\"");
Console.WriteLine("  - \"ERROR 메시지만 보여줘\"");
Console.WriteLine("  - \"최근 10 줄만 보여줘\"");
Console.WriteLine("  - \"WARNING 패턴을 찾아줘\"\n");

// 7. 대화 루프 시작
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
        // 8. 파일 경로를 포함한 프롬프트 생성
        var enhancedPrompt = $"""
            다음 요청을 처리해주세요. 기본 로그 파일 경로는 아래와 같습니다.
            
            로그 파일 경로: {defaultLogPath}
            
            사용자 요청: {userInput}
            
            파일을 읽어서 분석한 후 결과를 제공하세요.
            """;

        var response = await agent.RunAsync(enhancedPrompt);
        
        Console.WriteLine($"\n🤖 에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
