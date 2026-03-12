# Stage 09: Workflow QA Automation

> **학습 목표**: Graph Workflow 를 통한 QA 자동화

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 워크플로우 |
|----------|------|-----------|
| **09A_CrashReportProcessor** | ✅ 크래시 리포트 처리 | Collector → Analyzer → Severity |
| **09B_BugClassifier** | ⏸️ 자동 버그 분류 (확장용) | 로그분석 → 중복체크 → 우선순위 |
| **09C_QAReportGenerator** | ⏸️ QA 리포트 생성 (확장용) | 테스트결과 → 요약 → 배포판결정 |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 09A 실행 (완전 구현됨)
dotnet run --project Stage09_WorkflowQA/09A_CrashReportProcessor
```

---

## 🎯 09A_CrashReportProcessor 학습 내용

### Graph Workflow 아키텍처

```
                    ┌─────────────────┐
                    │  CollectorAgent │
                    │  (정보 수집)     │
                    └────────┬────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │  AnalyzerAgent  │
                    │  (심층 분석)     │
                    └────────┬────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │  SeverityAgent  │
                    │  (심각도 평가)   │
                    └─────────────────┘
```

### Sequential vs Graph Workflow

| 구분 | Sequential (Stage08) | Graph (Stage09) |
|------|---------------------|-----------------|
| **구조** | 직선형 | 단계적 심화 |
| **데이터흐름** | 단순 전달 | 분석결과 누적 |
| **특징** | 병렬 가능 | 의존성 명확 |
| **용도** | 콘텐츠 생성 | 분석/평가 |

### 1. 에이전트 역할 분리

```csharp
// CollectorAgent: 기본 정보 추출
public async Task<string> CollectAsync(string crashReport)
{
    // 날짜, 시간, OS, 게임버전 추출
}

// AnalyzerAgent: 원인 분석
public async Task<string> AnalyzeAsync(string crashReport)
{
    // 에러타입, 원인, 발생위치 분석
}

// SeverityAgent: 심각도 평가
public async Task<string> EvaluateSeverityAsync(string analysisReport)
{
    // Critical/High/Medium/Low 평가
}
```

### 2. 워크플로우 구현

```csharp
public class CrashReportWorkflow
{
    public async Task<CrashReportResult> ExecuteAsync(string crashReport)
    {
        // Node 1: 수집
        var collectedInfo = await _collectorAgent.CollectAsync(crashReport);
        
        // Node 2: 분석 (수집 결과 기반)
        var analysis = await _analyzerAgent.AnalyzeAsync(crashReport);
        
        // Node 3: 심각도 평가 (분석 결과 기반)
        var severity = await _severityAgent.EvaluateSeverityAsync(analysis);
        
        return new CrashReportResult
        {
            CollectedInfo = collectedInfo,
            Analysis = analysis,
            Severity = severity
        };
    }
}
```

### 3. 그래프 패턴

```
단순 Graph (09A):
Collector → Analyzer → Severity (직렬)

복잡한 Graph (확장시):
                    ┌─→ DuplicateCheck ─┐
Collector → Analyzer ┤                   ├→ Assign
                    └─→ SeverityEval ───┘
```

---

## 💡 실행 예시

```
💥 크래시 리포트 처리기에 오신 것을 환영합니다!

👤 사용자: 게임이 시작하자마자 꺼져요.
          Error: NullReferenceException at GameLauncher.Launch()

💥 크래시 리포트 처리 시작
--------------------------------------------------

📥 Node 1: 정보 수집 중...
✅ 수집 완료
   날짜: 2025-01-15, 시간: 14:30:22, OS: Windows 11...

🔍 Node 2: 심층 분석 중...
✅ 분석 완료
   에러타입: NullReferenceException
   원인: 초기화되지 않은 객체 접근...

⚠️ Node 3: 심각도 평가 중...
✅ 평가 완료
   심각도: Critical
   우선순위: 1
   담당팀: 클라이언트...

============================================================
📊 크래시 리포트 처리 결과
============================================================

📥 수집 정보:
...
============================================================
```

---

## 📊 QA 워크플로우 패턴

### 처리 단계

| 단계 | 에이전트 | 입력 | 출력 |
|------|---------|------|------|
| 1 | Collector | 크래시리포트 | 기본정보 |
| 2 | Analyzer | 크래시리포트 | 분석결과 |
| 3 | Severity | 분석결과 | 심각도평가 |

### 확장 패턴

```
09B_BugClassifier:
BugReport → LogAnalyzer → DuplicateChecker → PriorityAssigner → AutoAssign

09C_QAReportGenerator:
TestResults → TestAnalyzer → Summarizer → DeploymentDecision → ReportGenerator
```

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
- [x] **Stage 09: Workflow QA (Graph)**

총 **27 개 프로젝트** 완성! 🎉

---

## 🔗 관련 문서

- **[Workflow Types](https://learn.microsoft.com/agent-framework/concepts/workflow-types)**
- **[Graph Workflow](https://learn.microsoft.com/agent-framework/how-to/graph-workflow)**
- **[QA Automation](https://learn.microsoft.com/agent-framework/samples/qa-automation)**
