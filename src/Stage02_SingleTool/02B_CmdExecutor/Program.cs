// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _02B_CmdExecutor.Tools;

// ==========================================
// 2 단계 B: 명령어 실행기
// ==========================================
// 학습 목표:
// 1. 시스템 명령어 실행 Tool 구현
// 2. 외부 프로세스 실행 및 출력 캡처
// 3. 보안 고려사항 이해
// ==========================================

Console.WriteLine("💻 명령어 실행기에 오신 것을 환영합니다!");
Console.WriteLine("CMD 명령어를 실행해드립니다.\n");

// 1. 환경 변수에서 API 키와 베이스 URL 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. 명령어 실행 Tool 생성
var cmdTool = new CommandExecutionTool();

// 3. OpenAI 클라이언트 생성
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 4. AIAgent 생성 - Tool 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 시스템 명령어 실행 전문가입니다.
        
        당신의 역할:
        1. 사용자가 요청한 작업을 수행할 적절한 CMD 명령어를 선택합니다
        2. 명령어를 실행하고 결과를 해석합니다
        3. 보안을 위해 위험한 명령어 (rm -rf, del /s, format 등) 는 실행하지 않습니다
        4. 결과를 사용자가 이해하기 쉽게 설명합니다
        
        사용 가능한 도구:
        - ExecuteCommand(cmd): CMD 명령어 실행
        - GetEnvironmentVariable(name): 환경 변수 조회
        - GetCurrentDateTime(): 현재 날짜/시간 반환
        
        자주 사용하는 명령어 매핑:
        - "파일 목록 보여줘" → dir
        - "시스템 정보" → systeminfo
        - "프로세스 목록" → tasklist
        - "컴퓨터 이름" → hostname
        - "현재 폴더" → cd
        - "네트워크 정보" → ipconfig
        """,
    name: "CmdExecutor",
    tools: [
        AIFunctionFactory.Create(cmdTool.ExecuteCommand),
        AIFunctionFactory.Create(cmdTool.GetEnvironmentVariable),
        AIFunctionFactory.Create(cmdTool.GetCurrentDateTime)
    ]
);

Console.WriteLine("✅ 명령어 실행기가 초기화되었습니다.\n");

// 5. 사용 예시 안내
Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"현재 디렉토리 목록을 보여줘\"");
Console.WriteLine("  - \"시스템 정보를 알려줘\"");
Console.WriteLine("  - \"실행 중인 프로세스 목록을 보여줘\"");
Console.WriteLine("  - \"현재 날짜와 시간이 어떻게 돼?\"\n");

// 6. 대화 루프 시작
Console.WriteLine("실행할 명령어를 입력하세요 (종료: 'quit' 또는 'exit')\n");

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
