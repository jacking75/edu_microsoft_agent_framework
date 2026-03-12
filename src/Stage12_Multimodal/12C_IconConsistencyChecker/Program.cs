// Copyright (c) Microsoft. All rights reserved.

using _12C_IconConsistencyChecker.Agents;
using _12C_IconConsistencyChecker.Tools;

// ==========================================
// 12 단계 C: 아이콘 일관성 검증기
// ==========================================
// 학습 목표:
// 1. 멀티모달 (이미지 + 텍스트) 처리
// 2. 아이콘 세트 일관성 분석
// 3. 스타일가이드 기반 검수
// ==========================================

Console.WriteLine("🎨 아이콘 일관성 검증기에 오신 것을 환영합니다!");
Console.WriteLine("아이콘 세트의 스타일 일관성을 검수합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 도구 및 에이전트 생성
var visionTool = new IconVisionTool();
var iconAgent = new IconAgent(apiKey, baseUrl);

Console.WriteLine("✅ 아이콘 검수 도구가 초기화되었습니다.\n");

// 데모용 아이콘 스타일가이드
var styleGuide = """
    게임 아이콘 스타일가이드:
    
    1. 스타일:
       - 플랫 디자인 (Flat Design)
       - 2px 스트로크
       - Minimal & Clean
    
    2. 색상 팔레트:
       - 아이템: #3498db (파란색)
       - 스킬: #e74c3c (빨간색)
       - 퀘스트: #f1c40f (노란색)
       - 설정: #95a5a6 (회색)
    
    3. 크기:
       - 기본: 64x64px
       - 소형: 32x32px
       - 대형: 128x128px
       - 모두 정사각형 기준
    
    4. 효과:
       - 그림자: 우측 하단 45 도, 2px
       - 하이라이트: 좌측 상단, 투명도 30%
       - 외곽선: 1px, #2c3e50
    """;

Console.WriteLine("📋 사용 예시:");
Console.WriteLine($"  - \"samples/icons/*.png 일관성 확인해줘\"");
Console.WriteLine($"  - \"samples/attack.png,samples/defense.png 비교해줘\"\n");

Console.WriteLine("검수할 아이콘 경로를 쉼표로 구분하여 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) ||
        input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        // 경로 파싱 (쉼표 구분)
        var imagePaths = input.Split(',')
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .ToList();

        if (imagePaths.Count == 0)
        {
            Console.WriteLine("⚠️ 이미지 경로를 입력해주세요.");
            continue;
        }

        if (imagePaths.Count == 1)
        {
            // 단일 아이콘 분석
            Console.WriteLine("\n🔍 아이콘 분석 중...");
            var basicAnalysis = visionTool.AnalyzeIcon(imagePaths[0]);
            Console.WriteLine($"{basicAnalysis}\n");
        }
        else
        {
            // 여러 아이콘 일관성 분석
            Console.WriteLine("\n🔍 아이콘 일관성 분석 중...");
            var consistencyAnalysis = visionTool.AnalyzeConsistency(imagePaths);
            Console.WriteLine($"{consistencyAnalysis}\n");

            // 2 개 아이콘이면 상세 비교
            if (imagePaths.Count == 2)
            {
                Console.WriteLine("🎨 아이콘 비교 중...");
                var comparison = await iconAgent.CompareTwoIconsAsync(imagePaths[0], imagePaths[1]);
                Console.WriteLine($"\n📊 비교 결과:\n{comparison}\n");
            }

            // 아이콘 세트 일관성 검수
            Console.WriteLine("🎨 아이콘 일관성 검수 중...");
            var iconCheck = await iconAgent.CheckConsistencyAsync(imagePaths, styleGuide);
            
            Console.WriteLine($"\n📊 일관성 검수 결과:\n{iconCheck}\n");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
