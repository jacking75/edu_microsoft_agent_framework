// Copyright (c) Microsoft. All rights reserved.

using _15A_CodeGenerator.Agents;
using _15A_CodeGenerator.Services;

// ==========================================
// 15 단계 A: 코드 생성 어시스턴트
// ==========================================
// 학습 목표:
// 1. AI 기반 코드 생성
// 2. 코드 리뷰 자동화
// 3. DevUI 시각화
// 4. 반복적 개선
// ==========================================

Console.WriteLine("💻 코드 생성 어시스턴트에 오신 것을 환영합니다!");
Console.WriteLine("AI 와 함께 코드를 작성하고 리뷰합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var devUIService = new DevUIService();
var codeAgent = new CodeAgent(apiKey, baseUrl);
var reviewAgent = new ReviewAgent(apiKey, baseUrl);

Console.WriteLine("✅ 코드 생성 도구가 초기화되었습니다.\n");

Console.WriteLine("📋 사용 예시:");
Console.WriteLine("  - \"사용자 인증 서비스 만들어줘\"");
Console.WriteLine("  - \"비동기 데이터 저장소 구현해줘\"");
Console.WriteLine("  - \"JSON 파싱 유틸리티 만들어줘\"\n");

Console.WriteLine("코드 생성 요구사항을 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var requirement = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(requirement) || 
        requirement.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        requirement.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // Step 1: 코드 생성
        Console.WriteLine("\n✍️ Step 1: 코드 생성 중...");
        var generatedCode = await codeAgent.GenerateCodeAsync(requirement);
        
        // DevUI 에 표시
        devUIService.DisplayCode(generatedCode);
        
        // Step 2: 코드 리뷰
        Console.WriteLine("\n🔍 Step 2: 코드 리뷰 중...");
        var reviewResult = await reviewAgent.ReviewCodeAsync(generatedCode);
        
        Console.WriteLine($"\n📋 리뷰 결과:\n{reviewResult}\n");
        
        // Step 3: 개선 여부 확인
        Console.WriteLine("💡 수정사항을 적용하시겠습니까? (y/n)");
        var applyChoice = Console.ReadLine();
        
        if (applyChoice?.Equals("y", StringComparison.OrdinalIgnoreCase) == true)
        {
            Console.WriteLine("\n🔄 코드 개선 중...");
            var improvementPrompt = $"""
                다음 코드를 리뷰 결과를 반영하여 개선해주세요:
                
                원본코드:
                {generatedCode}
                
                리뷰결과:
                {reviewResult}
                
                개선된 코드를 완성해주세요.
                """;
            
            var improvedCode = await codeAgent.GenerateCodeAsync(improvementPrompt);
            devUIService.DisplayCode(improvedCode);
            
            Console.WriteLine("\n✅ 개선된 코드가 준비되었습니다.");
        }
        
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
