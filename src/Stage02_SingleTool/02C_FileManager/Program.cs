// Copyright (c) Microsoft. All rights reserved.

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using _02C_FileManager.Tools;

// ==========================================
// 2 단계 C: 파일 관리자
// ==========================================
// 학습 목표:
// 1. 파일 읽기/쓰기 Tool 구현
// 2. 디렉토리 조작 Tool 구현
// 3. 파일 시스템 I/O 와 예외 처리
// ==========================================

Console.WriteLine("📁 파일 관리자에 오신 것을 환영합니다!");
Console.WriteLine("파일과 디렉토리를 관리해드립니다.\n");

// 1. 환경 변수에서 API 키와 베이스 URL 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 2. 파일 관리 Tool 생성
var fileTool = new FileManagerTool();

// 3. OpenAI 클라이언트 생성
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// 4. AIAgent 생성 - Tool 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 파일 관리 전문가입니다.
        
        당신의 역할:
        1. 사용자의 요청에 맞는 파일 작업을 수행합니다
        2. 파일 읽기/쓰기/삭제/복사 작업을 안전하게 수행합니다
        3. 디렉토리 목록을 조회합니다
        4. 위험한 작업 (시스템 파일 삭제 등) 은 경고합니다
        
        사용 가능한 도구:
        - ReadFile(path): 파일 내용 읽기
        - WriteFile(path, content): 파일에 내용 쓰기
        - AppendToFile(path, content): 파일에 내용 추가
        - ListDirectory(path): 디렉토리 목록 조회
        - DeleteFile(path): 파일 삭제
        - CopyFile(source, dest): 파일 복사
        
        주의사항:
        - 파일 삭제 전 반드시 확인합니다
        - 존재하지 않는 경로에 대한 작업은 오류를 반환합니다
        """,
    name: "FileManager",
    tools: [
        AIFunctionFactory.Create(fileTool.ReadFile),
        AIFunctionFactory.Create(fileTool.WriteFile),
        AIFunctionFactory.Create(fileTool.AppendToFile),
        AIFunctionFactory.Create(fileTool.ListDirectory),
        AIFunctionFactory.Create(fileTool.DeleteFile),
        AIFunctionFactory.Create(fileTool.CopyFile)
    ]
);

Console.WriteLine("✅ 파일 관리자가 초기화되었습니다.\n");

// 5. 사용 예시 안내
Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"현재 디렉토리 목록을 보여줘\"");
Console.WriteLine("  - \"test.txt 파일에 'Hello World' 라고 써줘\"");
Console.WriteLine("  - \"test.txt 파일 내용을 읽어줘\"");
Console.WriteLine("  - \"test.txt 파일을 backup.txt 로 복사해줘\"\n");

// 6. 작업 디렉토리 안내
Console.WriteLine($"📂 현재 작업 디렉토리: {Directory.GetCurrentDirectory()}\n");

// 7. 대화 루프 시작
Console.WriteLine("파일 작업을 입력하세요 (종료: 'quit' 또는 'exit')\n");

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
