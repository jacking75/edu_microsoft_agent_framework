# Stage 13: PM Assistant

> **학습 목표**: 다중 데이터 소스 통합과 장기 메모리

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 핵심 기술 |
|----------|------|-----------|
| **13A_SprintPlanner** | ✅ 스프린트 계획 | Jira+Git 통합, 데이터 통합 |
| **13B_ResourceAllocator** | ⏸️ 리소스 배분 (확장용) | 팀원 역량 분석, 작업량 분배 |
| **13C_BottleneckAnalyzer** | ⏸️ 병목 분석 (확장용) | 생산성 메트릭, 장기 메모리 |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 13A 실행 (완전 구현됨)
dotnet run --project Stage13_PMAssistant/13A_SprintPlanner
```

---

## 🎯 13A_SprintPlanner 학습 내용

### 데이터 통합 아키텍처

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│     Jira API    │────▶│ DataIntegration │────▶│SprintPlanningAgent│
│  (백로그아이템)  │     │     Service     │     │  (계획수립)      │
└─────────────────┘     └────────┬────────┘     └─────────────────┘
                                 │
┌─────────────────┐              │
│     Git API     │──────────────┘
│   (커밋이력)     │
└─────────────────┘
```

### 1. 다중 데이터 소스 통합

```csharp
public class DataIntegrationService
{
    // Jira 에서 이슈 가져오기
    public List<JiraIssue> FetchJiraIssues(string projectId)
    {
        // 실제 구현: Jira REST API 호출
        return issues;
    }
    
    // Git 커밋 이력 가져오기
    public List<GitCommit> FetchGitCommits(string repo)
    {
        // 실제 구현: GitHub/GitLab API 호출
        return commits;
    }
}
```

### 2. Sprint Planning Agent

```csharp
public class SprintPlanningAgent
{
    public async Task<string> CreateSprintPlanAsync(
        List<JiraIssue> issues, 
        int sprintVelocity)
    {
        var prompt = $"""
            다음 백로그 아이템으로 스프린트 계획을 수립하세요:
            
            백로그: {issuesText}
            팀 벨로시티: {sprintVelocity} 점
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
```

### 3. 장기 메모리 (Long-term Memory)

```csharp
// 실제 구현 시:
// - 과거 스프린트 데이터 저장
// - 팀 벨로시티 추적
// - 작업 패턴 학습
// - 병목 이력 기록

public class LongTermMemory
{
    private readonly List<SprintHistory> _history = new();
    
    public void SaveSprint(SprintResult sprint)
    {
        _history.Add(sprint);
    }
    
    public SprintHistory GetSimilarSprint(List<JiraIssue> currentIssues)
    {
        // 유사한 과거 스프린트 검색
        return _history
            .OrderByDescending(h => CalculateSimilarity(h, currentIssues))
            .FirstOrDefault();
    }
}
```

---

## 💡 실행 예시

```
📋 스프린트 계획 어시스턴트에 오신 것을 환영합니다!

✅ OpenAI API 키가 설정되었습니다.
✅ 스프린트 계획 도구가 초기화되었습니다.

📊 백로그 아이템을 가져오는 중...
✅ 5 개 이슈를 가져왔습니다.

백로그:
  PROJ-001: 로그인 시스템 구현 (8 점)
  PROJ-002: 대시보드 UI 설계 (5 점)
  PROJ-003: API 엔드포인트 개발 (13 점)
  PROJ-004: 단위 테스트 작성 (5 점)
  PROJ-005: 성능 최적화 (8 점)

📋 스프린트 계획 수립 중...

📊 스프린트 계획:

스프린트 목표: 로그인 시스템 완료 및 대시보드 초안
포함작업: PROJ-001, PROJ-002, PROJ-004
일별계획:
  1-3 일일차: 로그인 구현
  4-6 일차: 대시보드 UI 설계
  7-8 일차: 단위 테스트
  9-10 일차: 버그수정 및 완료
리스크: API 개발 지연 시 로그인 테스트 불가
```

---

## 📊 PM 어시스턴트 패턴

### 데이터 소스 통합

| 소스 | 데이터 | 용도 |
|------|--------|------|
| **Jira** | 이슈, 스토리포인트 | 백로그 관리 |
| **Git** | 커밋, 브랜치 | 진행상황 추적 |
| **CI/CD** | 빌드결과, 테스트 | 품질 지표 |

### 메모리 계층

| 계층 | 저장내용 | 기간 |
|------|---------|------|
| **단기** | 현재 스프린트 | 2 주 |
| **장기** | 과거 스프린트 | 6 개월 + |
| **영구** | 팀 벨로시티 | 무제한 |

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
- [x] **Stage 13: PM Assistant**

총 **39 개 프로젝트** 완성! 🎉

---

## 🔗 관련 문서

- **[Jira API 문서](https://developer.atlassian.com/cloud/jira/platform/rest/v3/)**
- **[GitHub API 문서](https://docs.github.com/en/rest)**
- **[장기 메모리 패턴](https://learn.microsoft.com/agent-framework/concepts/long-term-memory)**
