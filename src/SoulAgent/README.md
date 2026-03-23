# SoulAgent - 핵심 가치와 원칙을 가진 에이전트

## 📖 개요

SoulAgent 는 에이전트의 **핵심 가치 (Core Values)**와 **원칙 (Principles)**을 정의하고, 이를 기반으로 일관된 행동과 응답을 보여주는 학습용 프로젝트입니다.

> **Soul 이란?**  
> 에이전트의 정체성, 가치관, 행동 지침을 정의하는 것으로, 단순한 시스템 프롬프트를 넘어 에이전트의 "성격"과 "철학"을 형성합니다.

## 🎯 학습 목표

- ✅ 에이전트의 Soul(핵심 가치/원칙) 정의 방법
- ✅ JSON 기반 Soul 프로필 관리
- ✅ Soul 에 기반한 일관된 에이전트 행동 구현
- ✅ 런타임에 Soul 동적 업데이트
- ✅ Microsoft Agent Framework 의 `AsAIAgent()` 활용

## 📁 프로젝트 구조

```
src/SoulAgent/
├── SoulAgent.csproj           # 프로젝트 파일
├── Program.cs                 # 메인 프로그램
├── Agents/
│   └── SoulAgentClass.cs      # Soul 을 가진 에이전트 클래스
├── Soul/
│   ├── SoulDefinition.cs      # Soul 정의 클래스
│   └── SoulLoader.cs          # Soul 로드 및 관리 클래스
└── data/
    └── soul_profile.json      # Soul 프로필 (JSON)
```

## 🔑 핵심 개념

### SoulDefinition - 에이전트의 Soul 정의

| 속성 | 설명 |
|------|------|
| `Name` | 에이전트의 이름 |
| `Identity` | 에이전트의 정체성/페르소나 |
| `Mission` | 에이전트의 미션과 목적 |
| `CoreValues` | 핵심 가치 목록 |
| `Principles` | 행동 원칙 목록 |
| `SpeakingStyle` | 말투 및 언어 스타일 |
| `Taboos` | 금지 사항 목록 |

### SoulLoader - Soul 관리

- **Load()**: JSON 파일에서 Soul 정의 로드
- **BuildSystemPrompt()**: Soul 기반으로 시스템 프롬프트 생성
- **DisplaySoulInfo()**: Soul 정보 출력
- **GetSoul()**: Soul 정의 반환

## 🚀 사용법

### 1. 실행

```bash
cd src/SoulAgent
dotnet run
```

### 2. 환경 변수 설정

실행 전 다음 환경 변수를 설정하세요:

```bash
# Windows (PowerShell)
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

# Linux/Mac
export OPENAI_API_KEY="your-api-key"
export OPENAI_BASE_URL="https://api.openai.com/v1"
```

### 3. 대화 명령어

| 명령어 | 설명 |
|--------|------|
| `quit`, `exit`, `q` | 프로그램 종료 |
| `soul` | 현재 Soul 정보 표시 |
| `update` | Soul 업데이트 모드 |
| 그 외 입력 | 에이전트와 대화 |

### 4. Soul 업데이트

`update` 명령을 입력하면 다음 항목을 수정할 수 있습니다:

```
📝 Soul 업데이트 모드

이름: 
정체성: 
미션: 
핵심 가치 (쉼표로 구분): 
행동 원칙 (쉼표로 구분): 
말투: 
금지 사항 (쉼표로 구분): 
```

## 📝 예제: 기본 Soul 프로필

```json
{
  "name": "아리아",
  "identity": "지식과 지혜를 전하는 철학자이자 멘토",
  "mission": "사용자에게 깊은 통찰과 지혜를 제공하며, 긍정적인 성장을 돕는 것",
  "coreValues": [
    "진실성과 정직함",
    "지식 공유와 교육",
    "배려와 공감",
    "지속적인 성장과 학습",
    "다양성 존중"
  ],
  "principles": [
    "사실에 기반한 정확한 정보를 제공한다",
    "모르는 것은 솔직하게 모른다고 말한다",
    "사용자의 질문 의도를 깊이 있게 파악한다",
    "단순한 정보 제공을 넘어 통찰을 제공한다",
    "윤리적이고 건전한 방향으로 답변한다",
    "사용자의 자율적 결정을 존중한다"
  ],
  "speakingStyle": "차분하고 깊이 있는 어조. 비유와 예시를 적절히 사용하며, 철학적 통찰을 담습니다.",
  "taboos": [
    "허위 정보나 추측성 내용 제공",
    "윤리적으로 문제 있는 조언",
    "사용자의 결정을 강요하거나 조작",
    "무례하거나 공격적인 언어 사용",
    "차별적 또는 편향된 발언"
  ]
}
```

## 💡 코드 예제

### SoulAgent 생성

```csharp
using SoulAgent.Agents;
using SoulAgent.Soul;

// Soul 로드
var soulLoader = new SoulLoader("data/soul_profile.json");
var agent = new SoulAgentClass(soulLoader, apiKey, baseUrl);

// 에이전트 실행
var response = await agent.RunAsync("인생의 목적이 무엇일까요?");
Console.WriteLine(response);
```

### 커스텀 Soul 정의

```csharp
var customSoul = new SoulDefinition
{
    Name = "가디언",
    Identity = "사용자를 보호하는 사이버 보안 전문가",
    Mission = "사용자의 디지털 자산을 보호하고 보안 위협으로부터 지키는 것",
    CoreValues = ["보안", "프라이버시", "투명성"],
    Principles = [
        "항상 사용자의 동의를 구한다",
        "보안 위협을 즉시 알린다",
        "개인정보 보호를 최우선으로 한다"
    ],
    SpeakingStyle = "전문적이고 간결한 어조. 보안 용어를 쉽게 설명합니다.",
    Taboos = ["위협 행위", "불법 해킹 방법", "개인정보 유출"]
};

agent.UpdateSoul(customSoul);
```

## 🎭 사용 시나리오 예시

### 1. 캐릭터 기반 에이전트

게임 NPC, 가상 캐릭터 등 일관된 페르소나 유지

### 2. 전문가 에이전트

의료, 법률, 금융 등 특정 분야의 전문가 역할 수행

### 3. 브랜드 에이전트

기업의 가치와 톤앤매너를 반영한 CS 에이전트

### 4. 교육용 에이전트

학습자의 수준과 가치관에 맞춘 맞춤형 튜터

## ⚠️ 주의사항

- **Soul 의 일관성**: Soul 에 정의된 가치와 원칙은 상충되지 않도록 설계하세요.
- **금지 사항 명확화**: 에이전트가 절대 하지 않아야 할 행동을 구체적으로 명시하세요.
- **런타임 업데이트**: Soul 을 업데이트하면 이전 에이전트의 성격이 변경됩니다.
- **JSON 형식**: `soul_profile.json` 파일의 JSON 형식을 올바르게 유지하세요.

## 🔗 관련 문서

- [Microsoft Agent Framework](https://learn.microsoft.com/en-us/agent-framework/)
- [AsAIAgent() 메서드](https://learn.microsoft.com/en-us/agent-framework/concepts/ai-agents)
- [Stage01_TextAgent](../Stage01_TextAgent/README.md) - 기본 에이전트 학습

## 📚 학습 경로

이 프로젝트는 Microsoft Agent Framework 학습 경로의 **확장 단계**에 해당합니다.

```
Stage01_TextAgent → StageMemoryAgent → SoulAgent(본 프로젝트)
     ↓                    ↓                    ↓
  기본 에이전트        기억 기능       핵심 가치/원칙
```

---

**다음 단계**: Soul 을 가진 에이전트를 여러 개 만들어 Multi-Agent 시스템을 구축해보세요!
