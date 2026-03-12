# Stage 12: Multimodal Agent

> **학습 목표**: 비전 (Vision) 과 텍스트 통합 처리

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 핵심 기술 |
|----------|------|-----------|
| **12A_UIStyleChecker** | ✅ UI 스타일 검수기 | Vision Analysis, 스타일가이드 |
| **12B_CharacterModelChecker** | ⏸️ 캐릭터 모델 검수 (확장용) | 3D 모델 분석, 비율 검사 |
| **12C_IconConsistencyChecker** | ⏸️ 아이콘 일관성 (확장용) | 멀티모달 비교 |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 12A 실행 (완전 구현됨)
dotnet run --project Stage12_Multimodal/12A_UIStyleChecker
```

---

## 🎯 12A_UIStyleChecker 학습 내용

### 멀티모달 아키텍처

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  이미지 입력    │────▶│  VisionAnalysis │────▶│   StyleAgent    │
│  (스크린샷)     │     │     Tool        │     │  (텍스트 분석)   │
└─────────────────┘     └─────────────────┘     └─────────────────┘
                                                       │
                                                       ▼
                                              ┌─────────────────┐
                                              │  검수 결과      │
                                              │  (텍스트 리포트) │
                                              └─────────────────┘
```

### 1. Vision Tool 구현

```csharp
public class VisionAnalysisTool
{
    public string AnalyzeImage(string imagePath)
    {
        // 실제 구현: OpenAI Vision API 사용
        // 데모: 이미지 메타데이터 분석
        
        return $"""
            이미지 분석 결과:
            - 파일: {info.Name}
            - 크기: {info.Length} KB
            - 스타일 확인: [결과]
            """;
    }
}
```

### 2. Style Agent (멀티모달)

```csharp
public class StyleAgent
{
    public async Task<string> CheckStyleAsync(string imagePath, string styleGuide)
    {
        var prompt = $"""
            다음 UI 스크린샷을 스타일가이드와 비교하세요:
            
            이미지: {imagePath}
            스타일가이드: {styleGuide}
            """;
        
        // Vision capable model 사용
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
```

### 3. 스타일가이드 정의

```csharp
var styleGuide = """
    게임 UI 스타일가이드:
    
    1. 색상 팔레트:
       - Primary: #3498db
       - Secondary: #2ecc71
       - Accent: #e74c3c
    
    2. 폰트:
       - 제목: 24px, Bold
       - 본문: 16px, Regular
    
    3. 레이아웃:
       - 8px 그리드 정렬
       - 여백은 8px 배수
    """;
```

---

## 💡 실행 예시

```
🎨 UI 스타일 검수기에 오신 것을 환영합니다!

✅ OpenAI API 키가 설정되었습니다.
✅ UI 스타일 검수 도구가 초기화되었습니다.

📋 사용 예시:
  - "samples/main_menu.png 검수해줘"
  - "samples/inventory.png 스타일 확인해줘"

👤 사용자: samples/main_menu.png

🔍 이미지 분석 중...
이미지 분석 결과 (데모):

파일 정보:
- 이름: main_menu.png
- 크기: 256.3 KB
- 형식: .png

스타일 가이드 준수 사항:
- 색상 팔레트: 확인 필요
- 폰트 일관성: 확인 필요
- 레이아웃: 확인 필요

🎨 스타일 검수 중...

📊 검수 결과:
준수사항:
- 8px 그리드 정렬 준수
- Primary 색상 올바르게 사용

문제점:
- 본문 폰트 사이즈 14px (가이드: 16px)
- 버튼 여백 6px (가이드: 8px)

개선제안:
- 폰트 사이즈 16px 로 조정
- 버튼 여백 8px 로 통일
```

---

## 📊 멀티모달 처리 패턴

| 처리 단계 | 입력 | 출력 | 사용 API |
|-----------|------|------|----------|
| **이미지 분석** | 이미지 파일 | 메타데이터 | File I/O |
| **Vision 분석** | 이미지 + 프롬프트 | 이미지 설명 | OpenAI Vision |
| **텍스트 분석** | 이미지 설명 + 가이드 | 검수 결과 | LLM |

---

## ✅ 완료된 Stage

- [x] Stage 01: 텍스트 응답
- [x] Stage 02: 단일 Tool
- [x] Stage 03: 파일 시스템
- [x] Stage 04: 다중 Tool
- [x] Stage 05: 외부 API
- [x] Stage 06: 데이터베이스
- [x] Stage 07: RAG
- [x] Stage 08: Multi-Agent (Sequential)
- [x] Stage 09: Workflow QA (Graph)
- [x] Stage 10: Human-in-the-Loop
- [x] Stage 11: Real-time Analysis
- [x] **Stage 12: Multimodal**

총 **36 개 프로젝트** 완성! 🎉

---

## 🔗 관련 문서

- **[OpenAI Vision API](https://platform.openai.com/docs/guides/vision)**
- **[멀티모달 AI](https://learn.microsoft.com/azure/ai-services/multimodal)**
- **[GPT-4 Vision](https://learn.microsoft.com/azure/ai-services/openai/concepts/gpt-with-vision)**
