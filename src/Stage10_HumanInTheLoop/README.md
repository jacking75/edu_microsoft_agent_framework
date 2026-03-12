# Stage 10: Human-in-the-Loop

> **학습 목표**: Checkpointing 과 Time-travel 을 통한 인간-AI 협업

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 핵심 기능 |
|----------|------|-----------|
| **10A_StoryCollaborator** | ✅ 스토리 작성 협업 | Checkpointing, Time-travel |
| **10B_BalanceAdjuster** | ⏸️ 밸런스 조정 (확장용) | AI 제안 → 인간검토 → 시뮬레이션 |
| **10C_LocalizationReviewer** | ⏸️ 로컬라이제이션 (확장용) | AI 번역 → 인간검토 → 수정 |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 10A 실행 (완전 구현됨)
dotnet run --project Stage10_HumanInTheLoop/10A_StoryCollaborator
```

---

## 🎯 10A_StoryCollaborator 학습 내용

### Human-in-the-Loop 아키텍처

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  DraftAgent     │────▶│  Human Review   │────▶│ContinuationAgent│
│  (AI 초안작성)   │     │  (인간검토/수정)  │     │  (AI 계속작성)   │
└─────────────────┘     └─────────────────┘     └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────────────────────────────────────────────────────┐
│                    CheckpointService                            │
│  - 상태 저장 (Checkpoint)                                       │
│  - 피드백 관리 (Human Feedback)                                 │
│  - 시간 이동 (Time-travel)                                      │
└─────────────────────────────────────────────────────────────────┘
```

### 1. Checkpointing (상태 저장)

```csharp
public class CheckpointService
{
    private readonly List<Checkpoint> _checkpoints = new();
    
    // 체크포인트 생성
    public Checkpoint CreateCheckpoint(string storyContent)
    {
        var checkpoint = new Checkpoint
        {
            Id = ++_currentId,
            StoryContent = storyContent,
            CreatedAt = DateTime.Now
        };
        _checkpoints.Add(checkpoint);
        return checkpoint;
    }
    
    // 체크포인트로 시간 이동
    public Checkpoint? TravelToCheckpoint(int checkpointId)
    {
        return _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
    }
}
```

### 2. Time-travel (상태 복원)

```csharp
// 특정 체크포인트로 이동
var checkpoint = checkpointService.TravelToCheckpoint(targetId);

// 해당 시점에서 새로운 방향으로 계속 작성
var newStory = await continuationAgent.ContinueStoryAsync(
    checkpoint.StoryContent,
    newDirection
);
```

### 3. Human Feedback Loop

```
1. AI 가 초안 작성
        ↓
2. 체크포인트 저장
        ↓
3. 인간이 검토 및 피드백
        ↓
4. 피드백을 체크포인트에 추가
        ↓
5. AI 가 피드백 반영하여 계속 작성
        ↓
6. 승인 또는 다시 2 번으로
```

---

## 💡 실행 예시

```
📖 스토리 작성 협업 시스템에 오신 것을 환영합니다!

👤 사용자: 판타지 모험물

📖 스토리 협업 워크플로우 시작: 판타지 모험물
------------------------------------------------------------

✍️ Step 1: AI 초안 작성 중...
💾 체크포인트 1 저장됨
✅ 초안 작성 완료 (체크포인트 1)
   제목: 드래곤 슬레이어의 귀환
   등장인물: 에릭 (주인공), 리아 (마법사)...

👤 Step 2: 인간의 검토를 기다리는 중...
   피드백: 등장인물의 동기를 더 명확히 해주세요.

✍️ Step 3: AI 가 피드백을 반영하여 수정 중...
💾 체크포인트 2 저장됨
✅ 수정 완료 (체크포인트 2)
   리아는 과거에 드래곤에게 가족을 잃었다...

✅ Step 4: 최종 승인 대기 중...
✅ 체크포인트 2 승인됨

============================================================
📖 스토리: 판타지 모험물
============================================================
...

💾 저장된 체크포인트:
   ⏳ 체크포인트 1: 14:30:22
   ✅ 체크포인트 2: 14:32:15

⏰ 시간 이동 기능을 사용하시겠습니까? (y/n)
y
어느 체크포인트로 이동할까요? (번호 입력): 1
새로운 진행 방향을 입력하세요: 로맨스 요소 추가

⏰ 시간 이동: 체크포인트 1 으로
✍️ 새로운 방향으로 계속 작성: 로맨스 요소 추가
💾 체크포인트 3 저장됨
✅ 새로운 타임라인 생성 (체크포인트 3)
```

---

## 📊 Human-in-the-Loop 패턴

### 체크포인트 상태

| 상태 | 설명 | 아이콘 |
|------|------|--------|
| **Pending** | 인간 검토 대기 | ⏳ |
| **Approved** | 인간 승인 완료 | ✅ |
| **Rejected** | 인간 거부 (수정 필요) | ❌ |

### Time-travel 사용 사례

1. **다른 선택지 탐색**: "이 시점에서 다른 방향으로 가면?"
2. **실수 복원**: "실수로 삭제한 내용을 되돌리고 싶어"
3. **버전 비교**: "이전 버전과 현재 버전을 비교하고 싶어"

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
- [x] **Stage 10: Human-in-the-Loop**

총 **30 개 프로젝트** 완성! 🎉

---

## 🔗 관련 문서

- **[Human-in-the-Loop 가이드](https://learn.microsoft.com/agent-framework/concepts/human-in-the-loop)**
- **[Checkpointing 패턴](https://learn.microsoft.com/agent-framework/how-to/checkpointing)**
- **[Time-travel 구현](https://learn.microsoft.com/agent-framework/samples/time-travel)**
