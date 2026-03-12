// Copyright (c) Microsoft. All rights reserved.

using _12B_CharacterModelChecker.Agents;
using _12B_CharacterModelChecker.Tools;

// ==========================================
// 12 단계 B: 캐릭터 모델 검수기
// ==========================================
// 학습 목표:
// 1. 멀티모달 (이미지 + 텍스트) 처리
// 2. 캐릭터 비율 분석
// 3. 스타일가이드 기반 검수
// ==========================================

Console.WriteLine("🎭 캐릭터 모델 검수기에 오신 것을 환영합니다!");
Console.WriteLine("3D 캐릭터 모델의 비율과 스타일을 검수합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 도구 및 에이전트 생성
var visionTool = new CharacterVisionTool();
var characterAgent = new CharacterAgent(apiKey, baseUrl);

Console.WriteLine("✅ 캐릭터 검수 도구가 초기화되었습니다.\n");

// 데모용 캐릭터 스타일가이드
var styleGuide = """
    게임 캐릭터 스타일가이드:
    
    1. 신체 비율:
       - 머리:신체 = 1:7.5 (사실적)
       - 어깨너비:머리너비 = 2.5:1 (남성), 2:1 (여성)
       - 다리길이:신장 = 0.5:1
    
    2. 의상/장비:
       - 판타지 중세 스타일
       - 가죽/천/금속 재질 구분
       - 색상 팔레트: Earth Tone 위주
       - 장식은 기능성 우선
    
    3. 텍스처:
       - 4K PBR 텍스처
       - Normal map, Roughness map 사용
       - 일관된 라이트 방향
    
    4. 표정/포즈:
       - 자연스러운 표정
       - 전투 준비 포즈 기본
       - 과장되지 않은 동작
    """;

Console.WriteLine("📋 사용 예시:");
Console.WriteLine($"  - \"samples/warrior.png 검수해줘\"");
Console.WriteLine($"  - \"samples/mage.png 비율 확인해줘\"\n");

Console.WriteLine("검수할 캐릭터 이미지 경로를 입력하세요 (종료: 'quit' 또는 'exit')\n");

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
        Console.WriteLine("\n🔍 캐릭터 모델 분석 중...");
        var basicAnalysis = visionTool.AnalyzeCharacter(imagePath);
        Console.WriteLine($"{basicAnalysis}\n");

        // 2. 캐릭터 검수
        Console.WriteLine("🎭 캐릭터 검수 중...");
        var characterCheck = await characterAgent.CheckCharacterAsync(imagePath, styleGuide);
        
        Console.WriteLine($"\n📊 검수 결과:\n{characterCheck}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}
