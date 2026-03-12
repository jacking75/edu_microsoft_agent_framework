// Copyright (c) Microsoft. All rights reserved.

using _12A_UIStyleChecker.Agents;
using _12A_UIStyleChecker.Tools;

// ==========================================
// 12 단계 A: UI 스타일 검수기
// ==========================================
// 학습 목표:
// 1. 멀티모달 (이미지 + 텍스트) 처리
// 2. Vision API 활용
// 3. 스타일가이드 기반 검수
// ==========================================

Console.WriteLine("🎨 UI 스타일 검수기에 오신 것을 환영합니다!");
Console.WriteLine("UI 스크린샷의 스타일가이드 준수를 검수합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 도구 및 에이전트 생성
var visionTool = new VisionAnalysisTool();
var styleAgent = new StyleAgent(apiKey, baseUrl);

Console.WriteLine("✅ UI 스타일 검수 도구가 초기화되었습니다.\n");

// 데모용 스타일가이드
var styleGuide = """
    게임 UI 스타일가이드:
    
    1. 색상 팔레트:
       - Primary: #3498db (파란색)
       - Secondary: #2ecc71 (초록색)
       - Accent: #e74c3c (빨간색)
       - Background: #2c3e50 (어두운 회색)
       - Text: #ecf0f1 (밝은 회색)
    
    2. 폰트:
       - 제목: 24px, Bold
       - 본문: 16px, Regular
       - 작은 텍스트: 12px, Light
    
    3. 레이아웃:
       - 모든 요소는 8px 그리드에 정렬
       - 여백은 8px 배수 사용
       - 버튼 최소 크기: 120x40px
    
    4. 아이콘:
       - 플랫 스타일
       - 2px 스트로크
       - 통일된 색상 팔레트 사용
    """;

Console.WriteLine("📋 사용 예시:");
Console.WriteLine($"  - \"samples/main_menu.png 검수해줘\"");
Console.WriteLine($"  - \"samples/inventory.png 스타일 확인해줘\"\n");

Console.WriteLine("검수할 이미지 경로를 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var imagePath = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(imagePath) || 
        imagePath.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        imagePath.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // 1. 기본 이미지 분석
        Console.WriteLine("\n🔍 이미지 분석 중...");
        var basicAnalysis = visionTool.AnalyzeImage(imagePath);
        Console.WriteLine($"{basicAnalysis}\n");

        // 2. 스타일 검수
        Console.WriteLine("🎨 스타일 검수 중...");
        var styleCheck = await styleAgent.CheckStyleAsync(imagePath, styleGuide);
        
        Console.WriteLine($"\n📊 검수 결과:\n{styleCheck}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
