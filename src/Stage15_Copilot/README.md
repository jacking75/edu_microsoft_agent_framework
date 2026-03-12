# Stage 15: Copilot

> **학습 목표**: 통합 개발 코파일럿 - AI 기반 개발 자동화

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 핵심 기술 |
|----------|------|-----------|
| **15A_CodeGenerator** | ✅ 코드 생성 어시스턴트 | 코드생성, 리뷰, DevUI |
| **15B_BugFixer** | ⏸️ 버그 수정 제안 (확장용) | 에러진단, 수정제안 |
| **15C_FullDevelopmentCopilot** | ⏸️ 통합 개발 (확장용) | 프로덕션배포, 자동화 |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 15A 실행 (완전 구현됨)
dotnet run --project Stage15_Copilot/15A_CodeGenerator
```

---

## 🎯 15A_CodeGenerator 학습 내용

### AI 코딩 워크플로우

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   Requirement   │────▶│   CodeAgent     │────▶│  ReviewAgent    │
│   (요구사항)     │     │  (코드생성)      │     │  (코드리뷰)      │
└─────────────────┘     └────────┬────────┘     └────────┬────────┘
                                 │                       │
                                 ▼                       ▼
                         ┌─────────────────┐     ┌─────────────────┐
                         │   DevUIService  │     │  Improvement    │
                         │  (코드표시)       │     │  (개선)         │
                         └─────────────────┘     └─────────────────┘
```

### 1. Code Agent (코드 생성)

```csharp
public class CodeAgent
{
    public async Task<string> GenerateCodeAsync(string requirement)
    {
        var prompt = $"""
            다음 요구사항을 구현하는 C# 코드를 생성하세요:
            
            요구사항: {requirement}
            
            .NET 10.0, C# 12.0 을 사용하세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
```

### 2. Review Agent (코드 리뷰)

```csharp
public class ReviewAgent
{
    public async Task<string> ReviewCodeAsync(string code)
    {
        var prompt = $"""
            다음 C# 코드를 리뷰하세요:
            
            ```csharp
            {code}
            ```
            
            컨벤션, 성능, 보안, 가독성을 평가하세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
```

### 3. DevUI Service (시각화)

```csharp
public class DevUIService
{
    public void DisplayCode(string code, string fileName)
    {
        Console.WriteLine($"📝 [{fileName}] 코드 미리보기:");
        // 줄번호付き 코드 표시
    }
    
    public void ApplyCode(string code, string filePath)
    {
        // 실제 파일에 저장
    }
}
```

---

## 💡 실행 예시

```
💻 코드 생성 어시스턴트에 오신 것을 환영합니다!

✅ OpenAI API 키가 설정되었습니다.
✅ 코드 생성 도구가 초기화되었습니다.

📋 사용 예시:
  - "사용자 인증 서비스 만들어줘"
  - "비동기 데이터 저장소 구현해줘"

👤 사용자: 사용자 인증 서비스 만들어줘

✍️ Step 1: 코드 생성 중...

📝 [GeneratedCode.cs] 코드 미리보기:
------------------------------------------------------------
  1: public interface IAuthService
  2: {
  3:     Task<AuthResult> LoginAsync(string username, string password);
  4:     Task<void> LogoutAsync(string userId);
  5:     Task<bool> ValidateTokenAsync(string token);
  6: }
... (15 줄 더 있음)
------------------------------------------------------------

🔍 Step 2: 코드 리뷰 중...

📋 리뷰 결과:
✅ 좋은점:
- 인터페이스 분리로 테스트 용이
- 비동기 메서드 적절히 사용

⚠️ 개선필요:
- 비밀번호 해싱 로직 추가 필요
- 토큰 만료 처리 구현 필요

💡 수정사항을 적용하시겠습니까? (y/n)
y

🔄 코드 개선 중...

✅ 개선된 코드가 준비되었습니다.
```

---

## 📊 Copilot 기능

### 코드 생성

| 유형 | 설명 | 예시 |
|------|------|------|
| **서비스** | 비즈니스 로직 | AuthService, PaymentService |
| **리포지토리** | 데이터 접근 | UserRepository, OrderRepository |
| **유틸리티** | 공통 함수 | JsonHelper, DateUtils |
| **모델** | 데이터 클래스 | User, Product, Order |

### 코드 리뷰

| 항목 | 확인사항 |
|------|---------|
| **컨벤션** | 네이밍, 포매팅, 주석 |
| **성능** | 비동기, 캐싱, LINQ |
| **보안** | 입력검증, 암호화, SQL 인젝션 |
| **가독성** | 메서드길이, 복잡도, 책임 |

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
- [x] Stage 12: Multimodal
- [x] Stage 13: PM Assistant
- [x] Stage 14: Balance Simulator
- [x] **Stage 15: Copilot**

총 **45 개 프로젝트** 완성! 🎉

---

## 🎓 학습 과정 완료

### 초급 (Stage 01-03)
- ✅ Agent 기본, Tool, 파일 시스템

### 중급 (Stage 04-07)
- ✅ Multi-Tool, API, DB, RAG

### 고급 (Stage 08-12)
- ✅ Workflow, Multi-Agent, Realtime, Multimodal

### 전문가 (Stage 13-15)
- ✅ PM Assistant, Balance Simulator, Copilot

---

## 🔗 관련 문서

- **[GitHub Copilot](https://github.com/features/copilot)**
- **[AI 기반 개발](https://learn.microsoft.com/azure/ai-services/ai-developer-tools)**
- **[코드 생성 모범 사례](https://learn.microsoft.com/dotnet/csharp/best-practices)**
