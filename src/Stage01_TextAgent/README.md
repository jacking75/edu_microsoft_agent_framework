# Stage 1: 텍스트 응답 에이전트

> **학습 목표**: Microsoft Agent Framework 의 기본 개념 익히기

---

## 🎯 이 프로젝트에서 배우는 것

### 핵심 학습 목표

1. **Microsoft Agent Framework 기본 구조 이해**
   - AIAgent 인터페이스와 AsAIAgent() 확장 메서드
   - OpenAI 클라이언트 생성 및 설정
   - 환경 변수 관리 (OPENAI_API_KEY, OPENAI_BASE_URL)

2. **에이전트와의 대화 구현**
   - RunAsync() 를 이용한 단일 응답 처리
   - 대화 루프 패턴 구현
   - 사용자 입력 처리 및 예외 처리

3. **데이터 기반 에이전트 구축**
   - JSON 파일 파싱 및 활용 (01A)
   - YAML 파일 파싱 및 활용 (01B)
   - FAQ 데이터베이스 구축 및 검색 (01C)

4. **시스템 프롬프트 설계**
   - 에이전트의 역할과 응답 방식 정의
   - 명확한 지침 작성 방법
   - 컨텍스트 유지 및 참조

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 주요 기술 |
|----------|------|-----------|
| **01A_GameConfigBot** | JSON 게임 설정 파일 조회 | JSON 파싱, AIAgent |
| **01B_CharacterInfoBot** | YAML 캐릭터 정보 문의 | YAML 파싱, 한글/영문 매핑 |
| **01C_FAQBot** | FAQ 검색 및 답변 | 키워드 매칭, 유사도 점수 |

---

## 🚀 실행 방법

### 1. 환경 변수 설정

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

# 또는 OpenAI 호환 API 사용 (예: Azure OpenAI, 로컬 LLM)
$env:OPENAI_BASE_URL="https://your-custom-endpoint.com/v1"
```

### 2. NuGet 패키지 복원

```bash
cd src
dotnet restore Stage01.sln
```

### 3. 프로젝트 실행

```bash
# src 디렉토리에서 실행
cd src

# 01A: 게임 설정 조회 봇
dotnet run --project Stage01_TextAgent/01A_GameConfigBot

# 01B: 캐릭터 정보 봇
dotnet run --project Stage01_TextAgent/01B_CharacterInfoBot

# 01C: FAQ 봇
dotnet run --project Stage01_TextAgent/01C_FAQBot
```

---

## 🎮 실행해 볼 것 - 추천 미션

### 01A_GameConfigBot 에서 시도해보기

**기본 미션:**
- [ ] "최대 레벨이 얼마야?" - 기본 정보 조회
- [ ] "크리티컬 확률은 얼마야?" - 중첩된 설정 조회
- [ ] "플레이어의 기본 HP 와 레벨당 증가량을 알려줘" - 여러 값 조합
- [ ] "얼마나 많은 던전이 있어?" - 숫자 정보 조회

**심화 미션:**
- [ ] "전체 게임 설정을 요약해줘" - 종합 정보 요청
- [ ] "战斗 시스템을 설명해줘" - 관련 설정 기반 설명
- [ ] "골드 드롭률이 0.8 이면 1000 번 사냥했을 때 기대 골드량은?" - 계산 요청

**배우는 것:**
- ✅ JSON 데이터 구조 이해
- ✅ 중첩된 키 조회 (점 표기법)
- ✅ 에이전트가 데이터를 기반으로 답변하는 방식

---

### 01B_CharacterInfoBot 에서 시도해보기

**기본 미션:**
- [ ] "전사의 HP 가 얼마나 돼?" - 한글 클래스명 조회
- [ ] "mage 의 스킬이 뭐가 있어?" - 영문 클래스명 조회
- [ ] "마법사와 전사의 HP 를 비교해줘" - 클래스 간 비교
- [ ] "가장 크리티컬 확률이 높은 클래스는?" - 최대값 찾기

**심화 미션:**
- [ ] "암살자의 성장률을 모두 알려줘" - 복잡한 데이터 조회
- [ ] "전사가 마법사에게 약해? 상성 관계를 알려줘" - 관계형 데이터 조회
- [ ] "초보자한테 궁수를 추천해줄래? 이유도 말해줘" - 추천 및 근거 제시
- [ ] "모든 클래스의 HP 를 높은 순으로 정렬해줘" - 정렬 요청

**배우는 것:**
- ✅ YAML 데이터 구조 이해
- ✅ 한글/영문 매핑 처리
- ✅ 복잡한 데이터 구조 조회
- ✅ 클래스 상성 관계와 같은 관계형 데이터 처리

---

### 01C_FAQBot 에서 시도해보기

**기본 미션:**
- [ ] "비밀번호를 잃어버렸는데 어떻게 해?" - 정확한 매칭
- [ ] "최대 레벨이 어떻게 되지?" - FAQ 에 없는 질문 (게임플레이 FAQ 유도)
- [ ] "아이템 거래 어떻게 해?" - 관련 FAQ 찾기
- [ ] "게임이 자꾸 끊겨" - 기술 지원 FAQ

**심화 미션:**
- [ ] "계정을 없애고 싶어" - 탈퇴 관련 FAQ
- [ ] "PVP 언제 되는데?" - PVP unlock 조건 조회
- [ ] "골드 빨리 버는 방법 알려줘" - 경제 시스템 FAQ
- [ ] "완전 다른 질문 abc xyz" - 매칭 실패 케이스 확인

**배우는 것:**
- ✅ 키워드 매칭 기반 검색
- ✅ 유사도 점수 계산
- ✅ FAQ 에 없는 질문에 대한 처리
- ✅ 자연어 질의응답 시스템

---

## 🔍 비교 분석 미션

### 3 개 봇 모두 실행해보기

1. **동일한 질문 던지기:**
   ```
   "최대 레벨이 얼마야?"
   ```
   - 01A: 게임 설정에서 정확한 값 반환
   - 01B: 캐릭터 정보와 무관함을 설명
   - 01C: FAQ 에서 관련 항목 검색

2. **각 봇의 전문 분야 확인:**
   - 01A: 숫자, 설정 값 → **정확한 데이터 조회**
   - 01B: 캐릭터, 클래스, 스탯 → **도메인 지식 기반 답변**
   - 01C: 사용자 질문, 문제 해결 → **FAQ 기반 안내**

3. **배우는 것:**
   - ✅ 에이전트의 역할 (system prompt) 의 중요성
   - ✅ 데이터 소스에 따른 응답 차이
   - ✅ 컨텍스트 이해의 한계와 가능성

---

## 📖 학습 내용

### 1. Microsoft Agent Framework - Agent 초기화

**현재 사용하는 방식 (OPENAI_BASE_URL):**
```csharp
using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

// 환경 변수에서 API 키와 베이스 URL 읽기
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL") 
    ?? "https://api.openai.com/v1";

// OpenAI 클라이언트 생성
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// AIAgent 생성
AIAgent agent = chatClient.AsAIAgent(
    instructions: "당신은 친절한 어시스턴트입니다.",
    name: "MyAgent"
);
```

### 2. 시스템 프롬프트 작성

```csharp
var systemPrompt = """
    당신은 게임 설정 정보 어시스턴트입니다.
    
    당신의 역할:
    1. 게임 설정 파일에서 정보를 찾아 정확하게 답변합니다
    2. 숫자 값은 그대로 전달하고, 필요한 경우 계산합니다
    3. 모르는 정보는 "해당 정보를 찾을 수 없습니다"라고 답변합니다
    """;

AIAgent agent = chatClient.AsAIAgent(
    instructions: systemPrompt,
    name: "GameConfigBot"
);
```

### 3. Agent 실행 - 단일 응답

```csharp
// 단일 응답 받기
var response = await agent.RunAsync("최대 레벨이 얼마야?");
Console.WriteLine(response);
```

### 4. Agent 실행 - 스트리밍

```csharp
// 스트리밍 응답 받기
await foreach (var update in agent.RunStreamingAsync("재미있는 이야기를 들려줘"))
{
    Console.Write(update);
}
```

### 5. 데이터 파일 처리

- **JSON**: `System.Text.Json` 사용
- **YAML**: `YamlDotNet` 라이브러리 사용
- **경로 처리**: `AppContext.BaseDirectory` 와 상대 경로 조합

---

## 💡 예제 질문

### 01A_GameConfigBot

```
👤 사용자: 최대 레벨이 얼마야?
🤖 에이전트: 최대 레벨은 99 입니다.

👤 사용자: 크리티컬 확률은 얼마인가요?
🤖 에이전트: 기본 크리티컬 확률은 15% 입니다.
```

### 01B_CharacterInfoBot

```
👤 사용자: 전사의 HP 가 얼마나 돼?
🤖 에이전트: 전사의 기본 HP 는 800 입니다.

👤 사용자: 마법사 스킬이 뭐가 있어?
🤖 에이전트: 마법사의 스킬: Fireball, Ice Storm, Lightning Bolt, Arcane Shield
```

### 01C_FAQBot

```
👤 사용자: 비밀번호 잃어버렸는데 어떻게 해?
🤖 에이전트: 로그인 화면에서 '비밀번호 찾기'를 클릭하고...
```

---

## 🔗 관련 문서

- **[Microsoft Agent Framework 공식 문서](https://learn.microsoft.com/agent-framework/)**
- **[C# Quick Start](https://learn.microsoft.com/agent-framework/get-started/your-first-agent)**
- **[GitHub Repository](https://github.com/microsoft/agent-framework/tree/main/dotnet)**
- [System.Text.Json 문서](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/)
- [YamlDotNet GitHub](https://github.com/aaubry/YamlDotNet)

---

## ✅ 학습 체크리스트

### 기본 개념
- [ ] Microsoft Agent Framework 이해
- [ ] `AIAgent` 생성 방법
- [ ] `AsAIAgent()` 메서드 사용
- [ ] `RunAsync()`로 단일 응답
- [ ] OPENAI_API_KEY, OPENAI_BASE_URL 환경 변수 설정

### 데이터 처리
- [ ] JSON 데이터 파싱 (01A)
- [ ] YAML 데이터 파싱 (01B)
- [ ] 시스템 프롬프트 작성
- [ ] 데이터 기반 응답 생성

### 심화 이해
- [ ] 키워드 매칭 기반 검색 (01C)
- [ ] 한글/영문 클래스명 매핑 (01B)
- [ ] 중첩된 데이터 조회 (01A)
- [ ] 예외 처리 및 오류 메시지

---

## 📌 NuGet 패키지

```xml
<PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-rc1" />
<PackageReference Include="Azure.Identity" Version="1.13.2" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.0" />
<PackageReference Include="YamlDotNet" Version="16.3.0" />
<PackageReference Include="System.ClientModel" Version="1.8.1" />
```

---

## 🎯 다음 단계 (Stage 2)

Stage 1 에서 배운 기본기를 바탕으로, Stage 2 에서는 **Function Tool**을 배웁니다:

- ✅ Agent 가 외부 함수를 호출하도록 설정
- ✅ 게임 밸런스 계산기 구현 (DPS, 크리티컬 데미지 등)
- ✅ Tool 파라미터 처리 및 바인딩
- ✅ 여러 Tool 간 선택과 사용

Stage 1 을 완료했다면, 이제 Stage 2 로 이동하세요!
