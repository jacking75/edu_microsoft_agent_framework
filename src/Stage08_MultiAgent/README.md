# Stage 08: Multi-Agent Workflow (Sequential)

> **학습 목표**: Sequential Workflow 를 통한 다중 에이전트 협업

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 워크플로우 |
|----------|------|-----------|
| **08A_ItemCreationPipeline** | ✅ 아이템 생성 | Description → Balance → Localization |
| **08B_QuestCreationPipeline** | ✅ 퀘스트 생성 | Story → Reward → Difficulty |
| **08C_DialogueGenerator** | ✅ 대화 생성 | Draft → Tone → Branch |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 08A 실행 (아이템 생성)
dotnet run --project Stage08_MultiAgent/08A_ItemCreationPipeline

# 08B 실행 (퀘스트 생성)
dotnet run --project Stage08_MultiAgent/08B_QuestCreationPipeline

# 08C 실행 (대화 생성)
dotnet run --project Stage08_MultiAgent/08C_DialogueGenerator
```

---

## 🎯 08A_ItemCreationPipeline 학습 내용

### Sequential Workflow 아키텍처

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│ DescriptionAgent│────▶│  BalanceAgent   │────▶│LocalizationAgent│
│  (아이템 설명)   │     │  (밸런스 검토)   │     │   (현지화)      │
└─────────────────┘     └─────────────────┘     └─────────────────┘
        │                       │                       │
        └───────────────────────┴───────────────────────┘
                                │
                        ┌───────▼───────┐
                        │ItemCreationResult│
                        └───────────────┘
```

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│ DescriptionAgent│────▶│  BalanceAgent   │────▶│LocalizationAgent│
│  (아이템 설명)   │     │  (밸런스 검토)   │     │   (현지화)      │
└─────────────────┘     └─────────────────┘     └─────────────────┘
        │                       │                       │
        └───────────────────────┴───────────────────────┘
                                │
                        ┌───────▼───────┐
                        │ItemCreationResult│
                        └───────────────┘
```

### 1. 에이전트 정의

```csharp
public class DescriptionAgent
{
    private readonly AIAgent _agent;

    public DescriptionAgent(string apiKey, string baseUrl)
    {
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), 
                                       new Uri(baseUrl));
        _agent = client.GetChatClient("gpt-4o-mini")
                       .AsAIAgent(instructions: "...", name: "DescriptionAgent");
    }

    public async Task<string> GenerateDescriptionAsync(string itemName)
    {
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
```

### 2. Workflow 구현

```csharp
public class ItemCreationWorkflow
{
    private readonly DescriptionAgent _descriptionAgent;
    private readonly BalanceAgent _balanceAgent;
    private readonly LocalizationAgent _localizationAgent;

    public async Task<ItemCreationResult> ExecuteAsync(string itemName)
    {
        // Stage 1: 설명 작성
        var description = await _descriptionAgent.GenerateDescriptionAsync(itemName);
        
        // Stage 2: 밸런스 검토 (Stage 1 결과 사용)
        var balance = await _balanceAgent.ReviewBalanceAsync(description);
        
        // Stage 3: 현지화 (Stage 2 결과 사용)
        var localization = await _localizationAgent.LocalizeAsync(description);
        
        return new ItemCreationResult 
        { 
            ItemName = itemName,
            Description = description,
            BalanceReview = balance,
            Localization = localization
        };
    }
}
```

### 3. 데이터 전달 패턴

```csharp
// 각 Stage 는 이전 Stage 의 출력을 입력으로 받음
Stage1.Output → Stage2.Input
Stage2.Output → Stage3.Input

// 최종 결과는 모든 Stage 의 출력을 포함
Result = { Stage1.Output, Stage2.Output, Stage3.Output }
```

---

## 💡 실행 예시

```
🎒 아이템 생성 파이프라인에 오신 것을 환영합니다!

👤 사용자: 플레임 소드

🎒 아이템 생성 워크플로우 시작: 플레임 소드
--------------------------------------------------

📝 Stage 1: 설명 작성 중...
✅ 설명 작성 완료
   이름: 플레임 소드
   설명: 고대의 불꽃이 서린 검으로, 적을 베면 화염이 ...

⚖️ Stage 2: 밸런스 검토 중...
✅ 밸런스 검토 완료
   공격력: 85
   희귀도: Epic
   밸런스 평가: 유지
   코멘트: 강력한 불속성 추가데미지가 있지만...

🌐 Stage 3: 현지화 중...
✅ 현지화 완료
   영어: Flame Sword - A blade infused with ancient fire...
   일본어: フレイムソード - 古代の炎が宿る剣...
   중국어: 火焰之剑 - 一把蕴含古代火焰的宝剑...

============================================================
📦 아이템: 플레임 소드
============================================================
...
```

---

## 📊 Multi-Agent 패턴

### Sequential Workflow 특징

| 특징 | 설명 |
|------|------|
| **순차적 실행** | Stage 1 → 2 → 3 순서로 실행 |
| **데이터 전달** | 이전 Stage 출력이 다음 Stage 입력 |
| **상태 관리** | 각 Stage 는 독립적이나 결과를 공유 |
| **에러 처리** | 한 Stage 실패 시 전체 워크플로우 중단 |

### 사용 사례

- ✅ 콘텐츠 생성 파이프라인
- ✅ 문서 검토 워크플로우
- ✅ 코드 리뷰 프로세스
- ✅ 다단계 번역 작업

---

## ✅ 완료된 Stage

- [x] Stage 01: 텍스트 응답
- [x] Stage 02: 단일 Tool
- [x] Stage 03: 파일 시스템
- [x] Stage 04: 다중 Tool
- [x] Stage 05: 외부 API
- [x] Stage 06: 데이터베이스
- [x] Stage 07: RAG
- [x] **Stage 08: Multi-Agent (Sequential)**

총 **24 개 프로젝트** 완성! 🎉

---

## 🎯 08B_QuestCreationPipeline 학습 내용

### 퀘스트 생성 워크플로우

```
┌─────────────────┐     ┌─────────────────┐     ┌───────────────────┐
│   StoryAgent    │────▶│   RewardAgent   │────▶│ DifficultyAgent   │
│  (퀘스트 스토리) │     │  (보상 설계)     │     │  (난이도 조정)     │
└─────────────────┘     └─────────────────┘     └───────────────────┘
```

### 각 Stage 역할

| Stage | 에이전트 | 입력 | 출력 |
|-------|----------|------|------|
| 1 | StoryAgent | 퀘스트 제목 | 스토리/배경/목표 |
| 2 | RewardAgent | 스토리 | 경험치/골드/아이템 |
| 3 | DifficultyAgent | 스토리 + 보상 | 난이도/권장레벨/소요시간 |

---

## 🎯 08C_DialogueGenerator 학습 내용

### 대화 생성 워크플로우

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   DraftAgent    │────▶│    ToneAgent    │────▶│   BranchAgent   │
│  (대화 초안)     │     │  (톤 조정)       │     │  (분기 생성)     │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

### 각 Stage 역할

| Stage | 에이전트 | 입력 | 출력 |
|-------|----------|------|------|
| 1 | DraftAgent | NPC 이름/역할 | 대화 초안/선택지 |
| 2 | ToneAgent | 초안/원하는 톤 | 톤 조정된 대화 |
| 3 | BranchAgent | 톤 조정된 대화 | 분기 구조/엔딩 |

### 톤 예시

- **친절함**: 친절한 상인, 마을 주민
- **무뚝뚝함**: 경비병, 용병
- **신비로움**: 마법사, 예언자
- **유머러스함**: 떠돌이 상인, 술집 주인

---

## 🔗 관련 문서

- **[Multi-Agent Patterns](https://learn.microsoft.com/agent-framework/concepts/multi-agent)**
- **[Workflow 가이드](https://learn.microsoft.com/agent-framework/how-to/workflows)**
- **[Sequential vs Graph](https://learn.microsoft.com/agent-framework/concepts/workflow-types)**
