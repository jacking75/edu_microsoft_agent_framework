# Microsoft Agent Framework 실습

C#/.NET 환경에서 **Microsoft Agent Framework(MAF)** 를 단계별로 학습하기 위한 통합 가이드입니다. 게임 개발 도메인의 실용 예제를 통해 초급(개념) → 중급(실무) → 고급(워크플로우) → 전문가(엔터프라이즈) 단계로 자연스럽게 발전하도록 설계되었습니다.

> **목표**: C#/.NET 을 사용한 Microsoft Agent Framework 15 단계 완전 마스터
> **방향성**: 각 단계별 2~3 개의 점진적 예제로 개념을 체계적으로 습득

---

## 🔗 공식 자료

- [GitHub - microsoft/agent-framework](https://github.com/microsoft/agent-framework)
- [Microsoft Agent Framework documentation](https://learn.microsoft.com/en-us/agent-framework/)

---

## 📑 목차

1. [프로젝트 디렉토리 구조](#-전체-프로젝트-디렉토리-구조)
2. [학습 커리큘럼 (15단계)](#-학습-커리큘럼-15단계)
   - [초급: 1~3단계](#-초급-단계-기본-개념-익히기)
   - [중급: 4~7단계](#-중급-단계-실무-활용-에이전트)
   - [고급: 8~12단계](#-고급-단계-복잡한-워크플로우)
   - [전문가: 13~15단계](#-전문가-단계-엔터프라이즈급-시스템)
3. [핵심 개념 심화 학습](#-핵심-개념-심화-학습)
   - [AIContextProvider 개요](#aicontextprovider란-무엇인가)
   - [2단계 생명주기](#핵심-개념-2단계-생명주기-two-phase-lifecycle)
   - [.NET(C#)에서의 구현](#netc에서의-구현)
   - [Python에서의 구현](#python에서의-구현)
   - [동적 Function Tool 제공](#aicontextprovider의-동적-function-tool-제공-기능)
4. [공통 구성 요소](#-공통-구성-요소)
5. [학습 진행 체크리스트](#-학습-진행-체크리스트)
6. [기술 스택](#-기술-스택)
7. [각 단계별 추가 학습 요소](#-각-단계별-추가-학습-요소)

---

## 📁 전체 프로젝트 디렉토리 구조

```
edu_microsoft_agent_framework/
├── src/
│   ├── Stage01_TextAgent/          # 1 단계: 기본 텍스트 응답
│   ├── Stage02_SingleTool/         # 2 단계: 단일 Tool
│   ├── Stage03_FileSystem/         # 3 단계: 파일 시스템
│   ├── Stage04_MultiTool/          # 4 단계: 다중 Tool
│   ├── Stage05_ExternalAPI/        # 5 단계: 외부 API
│   ├── Stage06_Database/           # 6 단계: 데이터베이스
│   ├── Stage07_RAG/                # 7 단계: RAG
│   ├── Stage08_MultiAgent/         # 8 단계: 멀티 에이전트
│   ├── Stage09_WorkflowQA/         # 9 단계: 워크플로우 QA
│   ├── Stage10_HumanInTheLoop/     # 10 단계: Human-in-the-Loop
│   ├── Stage11_RealtimeAnalysis/   # 11 단계: 실시간 분석
│   ├── Stage12_Multimodal/         # 12 단계: 멀티모달
│   ├── Stage13_PMAssistant/        # 13 단계: PM 어시스턴트
│   ├── Stage14_BalanceSimulator/   # 14 단계: 밸런싱 시뮬레이터
│   └── Stage15_Copilot/            # 15 단계: 통합 코파일럿
├── tests/
│   └── [각 단계별 테스트 프로젝트]
└── samples/
    └── [공통 샘플 데이터/설정 파일]
```

---

## 🎯 학습 커리큘럼 (15단계)

### 📘 초급 단계: 기본 개념 익히기

---

#### **1 단계: 텍스트 응답 에이전트**

게임 설정 파일(JSON/YAML)을 읽고 특정 정보를 반환하는 봇을 만듭니다. 예를 들어 "캐릭터의 HP는 얼마야?" 같은 질문에 답하는 에이전트입니다. 기본적인 프롬프트 작성과 에이전트 초기화 방법을 배우며, OpenAI를 사용해 간단한 대화형 인터페이스를 구축하고 게임 데이터베이스에서 정보를 조회하는 기초를 다집니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **1-A: 게임 설정 조회 봇** | JSON 게임 설정 파일에서 특정 값 조회 | Agent 초기화, 기본 프롬프트 |
| **1-B: 캐릭터 정보 문답 봇** | YAML 캐릭터 스탯 파일 질의응답 | 다양한 데이터 포맷 처리 |
| **1-C: 대화형 FAQ 봇** | 자주 묻는 질문 응답 에이전트 | 대화 컨텍스트 유지 |

```
Stage01_TextAgent/
├── 01A_GameConfigBot/
│   ├── Program.cs
│   ├── Agents/ConfigAgent.cs
│   └── data/game_config.json
├── 01B_CharacterInfoBot/
│   ├── Program.cs
│   ├── Agents/CharacterAgent.cs
│   └── data/characters.yaml
└── 01C_FAQBot/
    ├── Program.cs
    ├── Agents/FAQAgent.cs
    └── data/faq_database.json
```

---

#### **2 단계: 단일 Tool 사용 에이전트**

게임 밸런싱 계산기를 만듭니다. 데미지 공식 계산, DPS(초당 데미지) 계산 등의 함수를 Tool로 등록하고, 자연어 질문("이 무기의 DPS를 계산해줘")에 자동으로 함수를 호출하도록 합니다. Function Calling의 기본 개념을 익히고, 에이전트가 언제 어떤 도구를 사용해야 하는지 학습합니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **2-A: DPS 계산기** | 무기 공격력/속도로 DPS 계산 | Function Calling 기본 |
| **2-B: 크리티컬 데미지 계산기** | 크리틱 확률/데미지 기대값 계산 | Tool 파라미터 처리 |
| **2-C: 레벨업 필요 경험치 계산기** | 레벨별 경험치 곡선 계산 | Tool 등록 및 바인딩 |

```
Stage02_SingleTool/
├── 02A_DpsCalculator/
│   ├── Program.cs
│   ├── Tools/DpsCalculationTool.cs
│   └── Models/WeaponStats.cs
├── 02B_CritDamageCalculator/
│   ├── Program.cs
│   ├── Tools/CritCalculationTool.cs
│   └── Models/CritStats.cs
└── 02C_ExpCalculator/
    ├── Program.cs
    ├── Tools/ExpCalculationTool.cs
    └── Models/LevelCurve.cs
```

---

#### **3 단계: 파일 시스템 연동 에이전트**

로그 분석기를 개발합니다. 게임 플레이 로그나 크래시 리포트를 읽어서 요약하고 패턴을 찾는 에이전트입니다. 파일 읽기/쓰기 Tool을 추가하고, 대용량 텍스트를 처리하는 방법을 배웁니다. 게임 QA 과정에서 버그 리포트를 자동으로 분류하고 우선순위를 매기는 데 유용합니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **3-A: 로그 파일 요약기** | 게임 로그 읽어서 주요 이벤트 추출 | 파일 읽기 Tool 구현 |
| **3-B: 에러 패턴 분석기** | 크래시 리포트에서 공통 패턴 발견 | 텍스트 처리 및 분석 |
| **3-C: 세이브 파일 검증기** | 게임 세이브 파일 무결성 확인 | 파일 쓰기/수정 Tool |

```
Stage03_FileSystem/
├── 03A_LogSummarizer/
│   ├── Program.cs
│   ├── Tools/FileReadTool.cs
│   └── samples/game_log.txt
├── 03B_ErrorPatternAnalyzer/
│   ├── Program.cs
│   ├── Tools/FileAnalysisTool.cs
│   └── samples/crash_reports/
└── 03C_SaveFileValidator/
    ├── Program.cs
    ├── Tools/FileWriteTool.cs
    └── samples/save_data.json
```

---

### 📗 중급 단계: 실무 활용 에이전트

---

#### **4 단계: 다중 Tool 활용 에이전트**

게임 리소스 관리 어시스턴트를 만듭니다. 텍스처 파일 크기 확인, 메모리 사용량 추정, 최적화 제안 등 여러 기능을 통합합니다. 여러 Tool을 조합하여 복잡한 작업을 수행하는 방법을 익히고, Tool 간의 의존성을 관리하는 방법을 배웁니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **4-A: 리소스 최적화 어시스턴트** | 텍스처/오디오 파일 분석 + 메모리 추정 | 여러 Tool 조합 |
| **4-B: 빌드 분석기** | 빌드 로그 파싱 + 경고/에러 분류 | Tool 간 데이터 공유 |
| **4-C: 성능 프로파일러** | FPS 데이터 분석 + 병목 지점 제안 | Tool 우선순위 관리 |

```
Stage04_MultiTool/
├── 04A_ResourceOptimizer/
│   ├── Program.cs
│   ├── Tools/TextureAnalyzer.cs
│   ├── Tools/MemoryEstimator.cs
│   └── Tools/OptimizationSuggester.cs
├── 04B_BuildAnalyzer/
│   ├── Program.cs
│   ├── Tools/BuildLogParser.cs
│   ├── Tools/WarningClassifier.cs
│   └── Tools/ErrorReporter.cs
└── 04C_PerformanceProfiler/
    ├── Program.cs
    ├── Tools/FpsAnalyzer.cs
    ├── Tools/BottleneckDetector.cs
    └── Tools/RecommendationEngine.cs
```

---

#### **5 단계: 외부 API 연동 에이전트**

버그 트래킹 시스템(Jira, Azure DevOps 등) 연동 봇을 개발합니다. 버그 리포트를 자연어로 작성하면 자동으로 이슈를 생성하고, 유사한 기존 버그를 검색하여 중복을 방지합니다. REST API 호출을 Tool로 래핑하고, 인증 처리 및 에러 핸들링을 구현하는 방법을 학습합니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **5-A: Jira 이슈 생성 봇** | 자연어 → Jira 티켓 자동 생성 | REST API 호출 Tool |
| **5-B: GitHub PR 리뷰어** | PR 링크로 변경사항 요약 | API 인증 처리 |
| **5-C: Slack 알림 에이전트** | 중요 이벤트 Slack 전송 | Webhook 연동 |

```
Stage05_ExternalAPI/
├── 05A_JiraIssueBot/
│   ├── Program.cs
│   ├── Tools/JiraApiTool.cs
│   └── Config/JiraSettings.cs
├── 05B_GitHubPRReviewer/
│   ├── Program.cs
│   ├── Tools/GitHubApiTool.cs
│   └── Config/GitHubSettings.cs
└── 05C_SlackNotifier/
    ├── Program.cs
    ├── Tools/SlackWebhookTool.cs
    └── Config/SlackSettings.cs
```

---

#### **6 단계: 데이터베이스 연동 에이전트**

플레이어 행동 분석 에이전트를 구축합니다. SQL 데이터베이스에서 플레이어 통계를 조회하고, 리텐션 분석, 이탈 예측 등의 인사이트를 제공합니다. 안전한 데이터베이스 쿼리 실행(SQL 인젝션 방지), 결과 시각화, 자연어 쿼리를 SQL로 변환하는 방법을 배웁니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **6-A: 플레이어 통계 조회 봇** | SQL 쿼리로 플레이어 데이터 조회 | 안전한 DB 쿼리 |
| **6-B: 리텐션 분석 에이전트** | 일별/주별 리텐션 계산 | 집계 쿼리 처리 |
| **6-C: 자연어 SQL 변환기** | 자연어를 SQL 로 변환 실행 | NL-to-SQL 패턴 |

```
Stage06_Database/
├── 06A_PlayerStatsBot/
│   ├── Program.cs
│   ├── Tools/SqlQueryTool.cs
│   └── Data/PlayerDbContext.cs
├── 06B_RetentionAnalyzer/
│   ├── Program.cs
│   ├── Tools/RetentionCalcTool.cs
│   └── Data/AnalyticsContext.cs
└── 06C_NaturalLanguageSql/
    ├── Program.cs
    ├── Tools/SqlGeneratorTool.cs
    └── Services/QueryValidator.cs
```

---

#### **7 단계: RAG (Retrieval-Augmented Generation) 에이전트**

게임 디자인 문서 검색 봇을 만듭니다. 방대한 게임 디자인 문서, 기술 문서, 회의록을 임베딩하고, 질문에 대한 관련 섹션을 찾아 답변합니다. Vector Store(Chroma, Pinecone 등) 사용법, 임베딩 생성, 의미론적 검색 구현을 학습합니다. 신규 팀원 온보딩이나 프로젝트 지식 관리에 매우 유용합니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **7-A: 문서 검색 봇 (기본)** | 텍스트 문서를 벡터화하여 검색 | 임베딩 생성/저장 |
| **7-B: 디자인 문서 QA** | GDD 문서에서 관련 섹션 찾기 | Vector Store 연동 |
| **7-C: 코드베이스 검색기** | 소스 코드를 인덱싱하여 질의응답 | 코드 임베딩 |

```
Stage07_RAG/
├── 07A_DocumentSearchBot/
│   ├── Program.cs
│   ├── Tools/EmbeddingTool.cs
│   ├── Tools/VectorSearchTool.cs
│   └── Data/DocumentStore.cs
├── 07B_GDD_QA/
│   ├── Program.cs
│   ├── Tools/GddRetriever.cs
│   └── Data/GddVectorStore.cs
└── 07C_CodebaseSearcher/
    ├── Program.cs
    ├── Tools/CodeEmbeddingTool.cs
    └── Data/CodeIndex.cs
```

---

### 📙 고급 단계: 복잡한 워크플로우

---

#### **8 단계: 멀티 에이전트 협업 (기본)**

게임 콘텐츠 생성 파이프라인을 구축합니다. 하나의 에이전트는 아이템 설명을 작성하고, 다른 에이전트는 밸런싱을 검토하며, 세 번째 에이전트는 로컬라이제이션을 체크합니다. Sequential Workflow로 에이전트들을 연결하고, 각 단계의 출력을 다음 단계의 입력으로 전달하는 방법을 배웁니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **8-A: 아이템 생성 파이프라인** | 설명작성 → 밸런스검토 → 로컬라이제이션 | Sequential Workflow |
| **8-B: 퀘스트 생성 파이프라인** | 스토리 → 보상설정 → 난이도조정 | 에이전트 간 데이터 전달 |
| **8-C: 다이얼로그 생성기** | 초안작성 → 톤체크 → 분기지점추가 | 다단계 검토 워크플로우 |

```
Stage08_MultiAgent/
├── 08A_ItemCreationPipeline/
│   ├── Program.cs
│   ├── Agents/DescriptionAgent.cs
│   ├── Agents/BalanceAgent.cs
│   ├── Agents/LocalizationAgent.cs
│   └── Workflows/ItemWorkflow.cs
├── 08B_QuestCreationPipeline/
│   ├── Program.cs
│   ├── Agents/StoryAgent.cs
│   ├── Agents/RewardAgent.cs
│   ├── Agents/DifficultyAgent.cs
│   └── Workflows/QuestWorkflow.cs
└── 08C_DialogueGenerator/
    ├── Program.cs
    ├── Agents/DraftAgent.cs
    ├── Agents/ToneAgent.cs
    ├── Agents/BranchAgent.cs
    └── Workflows/DialogueWorkflow.cs
```

---

#### **9 단계: 워크플로우 기반 QA 자동화**

자동화된 QA 리포트 생성 시스템을 개발합니다. 크래시 로그 수집 → 패턴 분석 → 심각도 평가 → 개발자 할당 → 리포트 생성의 전체 워크플로우를 구현합니다. Graph-based Workflow를 사용하여 조건부 분기(심각한 버그는 즉시 알림)와 병렬 처리를 구현하는 방법을 익힙니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **9-A: 크래시 리포트 처리기** | 수집 → 분석 → 심각도평가 → 할당 | Graph Workflow 기본 |
| **9-B: 자동 버그 분류기** | 로그 분석 → 중복체크 → 우선순위결정 | 조건부 분기 |
| **9-C: QA 리포트 생성기** | 테스트결과 → 요약 → 배포판 결정 | 병렬 처리 |

```
Stage09_WorkflowQA/
├── 09A_CrashReportProcessor/
│   ├── Program.cs
│   ├── Agents/CollectorAgent.cs
│   ├── Agents/AnalyzerAgent.cs
│   ├── Agents/SeverityAgent.cs
│   └── Workflows/CrashReportGraph.cs
├── 09B_BugClassifier/
│   ├── Program.cs
│   ├── Agents/DuplicateChecker.cs
│   ├── Agents/PriorityAgent.cs
│   └── Workflows/BugClassificationGraph.cs
└── 09C_QAReportGenerator/
    ├── Program.cs
    ├── Agents/TestResultAnalyzer.cs
    ├── Agents/SummaryAgent.cs
    └── Workflows/QAReportGraph.cs
```

---

#### **10 단계: Human-in-the-Loop 에이전트**

게임 스토리 작성 어시스턴트를 만듭니다. AI가 대화문 초안을 생성하면, 작가가 검토하고 수정하며, 다시 AI가 다른 캐릭터의 반응을 생성하는 협업 시스템입니다. Checkpointing과 Time-travel 기능을 활용하여 작업을 저장하고 이전 버전으로 돌아가는 방법을 배웁니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **10-A: 스토리 협업 어시스턴트** | AI 초안 → 인간수정 → AI 계속작성 | Checkpointing |
| **10-B: 밸런스 조정 워크플로우** | AI 제안 → 디자이너검토 → 시뮬레이션 | Time-travel |
| **10-C: 로컬라이제이션 검수** | AI 번역 → 인간검토 → 수정반영 | 상태 저장/복원 |

```
Stage10_HumanInTheLoop/
├── 10A_StoryCollaborator/
│   ├── Program.cs
│   ├── Agents/DraftAgent.cs
│   ├── Agents/ContinuationAgent.cs
│   ├── Services/CheckpointService.cs
│   └── Workflows/StoryWorkflow.cs
├── 10B_BalanceAdjuster/
│   ├── Program.cs
│   ├── Agents/ProposalAgent.cs
│   ├── Agents/SimulationAgent.cs
│   ├── Services/TimeTravelService.cs
│   └── Workflows/BalanceWorkflow.cs
└── 10C_LocalizationReviewer/
    ├── Program.cs
    ├── Agents/TranslationAgent.cs
    ├── Agents/QualityAgent.cs
    ├── Services/ReviewCheckpoint.cs
    └── Workflows/LocalizationWorkflow.cs
```

---

#### **11 단계: 실시간 게임 플레이 분석 에이전트**

게임 서버 메트릭스를 실시간으로 모니터링하고 이상 징후를 탐지하는 에이전트를 구축합니다. 스트리밍 데이터 처리, 알림 발송, 자동 스케일링 제안 등을 구현합니다. 이벤트 기반 아키텍처와 실시간 데이터 처리 패턴을 학습합니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **11-A: 서버 메트릭 모니터링** | 실시간 FPS/핑치 데이터 수집 | 스트리밍 데이터 처리 |
| **11-B: 이상 징후 탐지기** | 평소 패턴과 비교하여 이상 감지 | 이벤트 기반 아키텍처 |
| **11-C: 자동 스케일링 제안** | 부하 분석 → 인스턴스 조정 제안 | 실시간 알림 |

```
Stage11_RealtimeAnalysis/
├── 11A_ServerMetricMonitor/
│   ├── Program.cs
│   ├── Agents/MetricCollector.cs
│   ├── Services/StreamingDataService.cs
│   └── Workflows/MonitoringWorkflow.cs
├── 11B_AnomalyDetector/
│   ├── Program.cs
│   ├── Agents/PatternAnalyzer.cs
│   ├── Agents/AnomalyAgent.cs
│   └── Services/EventBus.cs
└── 11C_AutoScaler/
    ├── Program.cs
    ├── Agents/LoadAnalyzer.cs
    ├── Agents/ScalingAgent.cs
    └── Services/AlertService.cs
```

---

#### **12 단계: 멀티모달 에이전트**

게임 에셋 검수 에이전트를 개발합니다. 캐릭터 모델 이미지를 받아 스타일 가이드 준수 여부를 체크하고, 피드백을 제공합니다. GPT-4V나 Gemini의 비전 기능을 활용하여 이미지 분석을 수행하고, 텍스트와 이미지를 함께 처리하는 방법을 배웁니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **12-A: UI 스타일 검수기** | 스크린샷 → 스타일가이드 준수여부 | 이미지 분석 Tool |
| **12-B: 캐릭터 모델 검수기** | 3D 모델 스크린샷 → 비율/스타일 체크 | 비전 + 텍스트 통합 |
| **12-C: 아이콘 일관성 검증기** | 아이콘 세트 → 일관성 분석 | 멀티모달 비교 |

```
Stage12_Multimodal/
├── 12A_UIStyleChecker/
│   ├── Program.cs
│   ├── Tools/VisionAnalysisTool.cs
│   ├── Agents/StyleAgent.cs
│   └── samples/ui_screenshots/
├── 12B_CharacterModelChecker/
│   ├── Program.cs
│   ├── Tools/ImageAnalyzer.cs
│   ├── Agents/ProportionAgent.cs
│   └── samples/character_models/
└── 12C_IconConsistencyChecker/
    ├── Program.cs
    ├── Tools/MultiImageTool.cs
    ├── Agents/ConsistencyAgent.cs
    └── samples/icon_sets/
```

---

### 📕 전문가 단계: 엔터프라이즈급 시스템

---

#### **13 단계: 게임 프로젝트 관리 AI 어시스턴트**

스프린트 계획, 작업 추정, 리소스 배분을 돕는 종합 PM 어시스턴트를 구축합니다. 여러 데이터 소스(Jira, Git, Slack)를 통합하고, 팀의 생산성을 분석하며, 병목 현상을 식별합니다. 복잡한 멀티 에이전트 시스템 설계, 상태 관리, 장기 메모리 구현 방법을 학습합니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **13-A: 스프린트 계획 어시스턴트** | Jira + Git 연동 → 작업 추정 | 다중 데이터 소스 통합 |
| **13-B: 리소스 배분 최적화** | 팀원 역량 + 작업량 분석 | 복잡한 상태 관리 |
| **13-C: 병목 분석 대시보드** | 생산성 메트릭 → 병목지점 식별 | 장기 메모리 구현 |

```
Stage13_PMAssistant/
├── 13A_SprintPlanner/
│   ├── Program.cs
│   ├── Agents/JiraAgent.cs
│   ├── Agents/GitAgent.cs
│   ├── Agents/EstimationAgent.cs
│   └── Services/DataIntegrationService.cs
├── 13B_ResourceAllocator/
│   ├── Program.cs
│   ├── Agents/CapacityAgent.cs
│   ├── Agents/WorkloadAgent.cs
│   └── Services/OptimizationService.cs
└── 13C_BottleneckAnalyzer/
    ├── Program.cs
    ├── Agents/MetricAgent.cs
    ├── Agents/InsightAgent.cs
    └── Services/LongTermMemory.cs
```

---

#### **14 단계: 게임 밸런싱 시뮬레이터**

다양한 밸런싱 시나리오를 시뮬레이션하고 최적의 설정을 제안하는 에이전트를 만듭니다. 몬테카를로 시뮬레이션을 실행하고, 결과를 분석하며, 강화학습 기법을 적용합니다. AF Labs의 실험적 기능(벤치마킹, 강화학습)을 활용하는 방법을 배웁니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **14-A: 몬테카를로 시뮬레이터** | 전투 시뮬레이션 → 승률 분석 | 대량 시뮬레이션 실행 |
| **14-B: 최적 파라미터 찾기** | 그리드서치 → 최적 밸런스 제안 | 결과 분석/시각화 |
| **14-C: 강화학습 기반 밸런서** | 자기플레이 → 메타 발견 | 실험적 기능 활용 |

```
Stage14_BalanceSimulator/
├── 14A_MonteCarloSimulator/
│   ├── Program.cs
│   ├── Agents/SimulationAgent.cs
│   ├── Agents/AnalysisAgent.cs
│   └── Services/MonteCarloEngine.cs
├── 14B_ParameterOptimizer/
│   ├── Program.cs
│   ├── Agents/GridSearchAgent.cs
│   ├── Agents/RecommendationAgent.cs
│   └── Services/OptimizationEngine.cs
└── 14C_RLBalancer/
    ├── Program.cs
    ├── Agents/SelfPlayAgent.cs
    ├── Agents/MetaAnalyzer.cs
    └── Services/RLEngine.cs
```

---

#### **15 단계: 통합 게임 개발 코파일럿**

코드 생성, 버그 수정 제안, 문서 작성, 리뷰 자동화를 모두 포함하는 종합 개발 어시스턴트를 구축합니다. DevUI를 활용하여 워크플로우를 시각화하고 디버깅하며, Observability 도구(OpenTelemetry)로 성능을 모니터링합니다. 프로덕션 환경 배포, 보안 고려사항, 스케일링 전략을 학습합니다.

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **15-A: 코드 생성 어시스턴트** | 요구사항 → C# 코드 생성 | DevUI 시각화 |
| **15-B: 버그 수정 제안기** | 에러로그 → 수정코드 제안 | Observability 통합 |
| **15-C: 종합 개발 워크플로우** | 코드/테스트/문서 자동화 | 프로덕션 배포 |

```
Stage15_Copilot/
├── 15A_CodeGenerator/
│   ├── Program.cs
│   ├── Agents/CodeAgent.cs
│   ├── Agents/ReviewAgent.cs
│   ├── Services/DevUIService.cs
│   └── Workflows/CodeGenerationWorkflow.cs
├── 15B_BugFixer/
│   ├── Program.cs
│   ├── Agents/DiagnosisAgent.cs
│   ├── Agents/FixAgent.cs
│   ├── Services/ObservabilityService.cs
│   └── Workflows/BugFixWorkflow.cs
└── 15C_FullDevelopmentCopilot/
    ├── Program.cs
    ├── Agents/CodeAgent.cs
    ├── Agents/TestAgent.cs
    ├── Agents/DocAgent.cs
    ├── Services/DeploymentService.cs
    └── Workflows/FullDevWorkflow.cs
```

---

## 🧠 핵심 개념 심화 학습

각 실습 단계에서 만나게 될 MAF의 핵심 추상화를 미리 정리해 둡니다. 단계별 구현에 들어가기 전 또는 막힐 때 참고하세요.

---

### AIContextProvider란 무엇인가?

`AIContextProvider`는 Microsoft Agent Framework의 핵심 추상 기반 클래스(abstract base class)로, **에이전트 호출(invocation) 생명주기(lifecycle)에 직접 참여하는 컴포넌트**입니다. 쉽게 말하면, AI 에이전트가 LLM(대형 언어 모델)을 호출하기 **직전**과 **직후**에 개발자가 원하는 로직을 삽입할 수 있는 플러그인 구조를 제공합니다.

Python SDK에서는 `ContextProvider`라는 이름으로 동일한 역할을 수행하며, .NET(C#) SDK에서는 `AIContextProvider`라는 이름을 사용합니다. 두 구현은 설계 철학이 동일합니다.

공식 문서의 정의를 요약하면, `AIContextProvider`는 다음 네 가지 역할을 수행합니다.

- **대화의 변화를 감지(Listen to changes in conversations):** 각 턴(turn)에서 대화 상태가 어떻게 변했는지 추적합니다.
- **추가 컨텍스트 제공(Providing additional context):** LLM 호출 시 기본 메시지 외에 추가적인 지시사항이나 메모리를 주입합니다.
- **추가 도구(Function Tool) 제공:** 에이전트가 사용할 수 있는 도구 목록을 동적으로 공급합니다.
- **호출 결과 처리(Processing invocation results):** LLM의 응답을 받은 뒤 상태를 업데이트하거나 메모리를 저장합니다.

---

### 핵심 개념: 2단계 생명주기 (Two-Phase Lifecycle)

`AIContextProvider`의 가장 중요한 개념은 **READ(읽기) → MODEL(모델 호출) → WRITE(쓰기)** 라는 명확한 3단계 흐름입니다. 개발자는 READ와 WRITE 단계에 훅(hook)을 삽입합니다.

```
[사용자 입력]
     ↓
[invoking / invoking_async]  ← READ Phase: 컨텍스트 조립 및 주입
     ↓
[LLM 호출 (모델)]
     ↓
[invoked / invoked_async]   ← WRITE Phase: 결과 처리 및 상태 업데이트
     ↓
[에이전트 응답 반환]
```

**READ Phase - `InvokingAsync` (C#) / `invoking` (Python)**
이 메서드는 LLM이 호출되기 직전에 실행됩니다. 여기서 개발자는 메모리 저장소, 벡터 DB, 사용자 프로파일 등에서 필요한 정보를 불러와 에이전트의 프롬프트에 **캡슐(capsule)** 형태로 주입할 수 있습니다. 반환값은 `AIContext`(C#) 또는 `Context`(Python) 객체로, `Instructions`, `Messages`, `Tools` 를 포함할 수 있습니다.

**WRITE Phase - `InvokedAsync` (C#) / `invoked` (Python)**
이 메서드는 LLM이 응답을 반환한 뒤 실행됩니다. 요청 메시지와 응답 메시지를 모두 검사할 수 있으며, 새로운 정보를 메모리에 저장하거나 상태를 갱신하는 데 활용됩니다. 예를 들어, 대화에서 사용자의 이름이 처음 언급되었다면 이 단계에서 추출하여 저장합니다.

**Thread Created - `thread_created` (Python)**
Python SDK에는 추가적으로 `thread_created` 메서드가 존재합니다. 새로운 대화 스레드(thread)가 생성될 때 호출되며, 장기 저장소에서 해당 세션에 관련된 데이터를 미리 불러오는 데 사용됩니다.

---

### .NET(C#)에서의 구현

.NET에서는 `AIContextProvider` 추상 클래스를 상속받아 커스텀 메모리 컴포넌트를 만듭니다. 아래는 퍼스널 트레이너 에이전트에 사용자 정보 메모리를 부여하는 실제 예시입니다.

```csharp
public class ClientDetailsMemory : AIContextProvider
{
    private readonly IChatClient _chatClient;
    public ClientDetailsModels UserInfo { get; set; }

    // 생성자: 직렬화된 상태(serializedState)로부터 메모리를 복원합니다
    public ClientDetailsMemory(IChatClient chatClient, JsonElement serializedState, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        this._chatClient = chatClient;
        this.UserInfo = serializedState.ValueKind == JsonValueKind.Object ?
            serializedState.Deserialize<ClientDetailsModels>(jsonSerializerOptions)! :
            new ClientDetailsModels();
    }

    // WRITE Phase: LLM 응답 후 사용자 정보를 추출하여 저장
    public override async ValueTask InvokedAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        if (this.UserInfo.CurrentWeight is null || this.UserInfo.Name is null)
        {
            var result = await this._chatClient.GetResponseAsync<ClientDetailsModels>(
                context.RequestMessages,
                new ChatOptions()
                {
                    Instructions = "Extract the user's name, current weight and desired weight from the conversation."
                },
                cancellationToken: cancellationToken);

            this.UserInfo.Name ??= result.Result.Name;
            this.UserInfo.CurrentWeight ??= result.Result.CurrentWeight;
            this.UserInfo.DesiredWeight ??= result.Result.DesiredWeight;
        }
    }

    // READ Phase: LLM 호출 전 현재 메모리 상태를 Instructions로 주입
    public override ValueTask<AIContext> InvokingAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        StringBuilder instructions = new();
        instructions
            .AppendLine(this.UserInfo.CurrentWeight is null ?
                        "Ask the user for their current weight." :
                        $"The user's current weight is {this.UserInfo.CurrentWeight}.")
            .AppendLine(this.UserInfo.Name is null ?
                        "Ask the user for their name." :
                        $"The user's name is {this.UserInfo.Name}.");

        return new ValueTask<AIContext>(new AIContext
        {
            Instructions = instructions.ToString()
        });
    }

    // 직렬화: 메모리 상태를 JSON으로 변환 (DB 저장 등에 활용)
    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return JsonSerializer.SerializeToElement(this.UserInfo, jsonSerializerOptions);
    }
}
```

에이전트에 등록할 때는 `AIContextProviderFactory`를 통해 스레드 생성 시마다 새로운 인스턴스가 만들어지도록 합니다.

```csharp
AIAgent agent = chatClient.CreateAIAgent(new ChatClientAgentOptions()
{
    Name = "IronMind AI",
    ChatOptions = new()
    {
        Instructions = "You are a personal trainer.",
        Tools = [AIFunctionFactory.Create(PersonalTrainerAgent.GetTimeToReachWeight)]
    },
    // AIContextProviderFactory: 스레드마다 독립적인 메모리 인스턴스를 생성
    AIContextProviderFactory = ctx => new ClientDetailsMemory(
        chatClient.AsIChatClient(),
        ctx.SerializedState,
        ctx.JsonSerializerOptions),
});
```

---

### Python에서의 구현

Python SDK에서는 `ContextProvider` 추상 클래스를 상속받아 `invoking` 메서드를 반드시 구현해야 합니다. `invoked`와 `thread_created`는 선택사항입니다.

```python
from agent_framework import ChatMessage
from agent_framework._memory import Context, ContextProvider

class ShortTermThreadMemoryProvider(ContextProvider):
    """
    AF 스레드 히스토리를 읽어 system message 캡슐로 주입하는 컨텍스트 프로바이더
    """

    def __init__(self, max_turns: int = 6) -> None:
        super().__init__()
        self._max_turns = max_turns

    # READ Phase: 최근 대화 히스토리를 system message로 압축하여 주입
    async def invoking(self, messages, **_) -> Context:
        recent = list(messages)[-self._max_turns:]

        turns = []
        for m in recent:
            role = getattr(m, "role", None)
            role_val = role.value if hasattr(role, "value") else role
            if role_val in ("user", "assistant") and getattr(m, "text", None):
                turns.append(f"{role_val.upper()}: {m.text}")

        if not turns:
            return Context(messages=None)

        capsule = "Short-term thread memory:\n" + "\n".join(turns)
        return Context(messages=[ChatMessage(role="system", text=capsule)])

    # WRITE Phase: 응답 후 메모리 저장소에 기록 (선택 구현)
    async def invoked(self, request_messages, response_messages=None, invoke_exception=None, **kwargs) -> None:
        # 예: Mem0, Azure AI Search 등에 대화 내용 저장
        pass

    # 새 스레드 생성 시 장기 메모리 불러오기 (선택 구현)
    async def thread_created(self, thread_id: str | None) -> None:
        # 예: DB에서 사용자 프로파일 로드
        pass
```

에이전트 생성 시 `context_providers` 파라미터로 전달합니다.

```python
from agent_framework.azure import AzureOpenAIResponsesClient

provider = ShortTermThreadMemoryProvider(max_turns=6)

agent = client.create_agent(
    name="threaded-agent",
    instructions="Be concise.",
    context_providers=[provider],  # ← 여기서 등록
)

# 동일 스레드를 공유하여 대화 히스토리가 누적됨
thread = agent.get_new_thread()
await agent.run("My favourite black hole is Sagittarius A*.", thread=thread)
resp = await agent.run("What did I say my favourite black hole is?", thread=thread)
print(resp.text)
```

---

### Context 객체 구조

`invoking` / `InvokingAsync` 메서드는 **Context (Python) / AIContext (C#)** 객체를 반환합니다. 이 객체는 세 가지 요소를 포함할 수 있습니다.

- **Instructions:** 에이전트의 시스템 프롬프트에 추가되는 지시사항 문자열입니다. 메모리 내용, 사용자 프로파일, 정책 규칙 등을 텍스트 형태로 주입합니다.
- **Messages:** 프롬프트에 삽입될 추가 메시지 목록(`ChatMessage`)입니다. 시스템 메시지 형태로 캡슐화된 메모리 내용을 주입하는 데 자주 사용됩니다.
- **Tools:** 이번 턴에 에이전트가 사용할 수 있는 추가 함수 도구 목록입니다. 컨텍스트에 따라 동적으로 도구를 활성화/비활성화하는 데 활용됩니다.

---

### 직렬화(Serialization)와 상태 영속성

`AIContextProvider`의 강력한 특징 중 하나는 **직렬화 지원**입니다. `Serialize()` 메서드를 오버라이드하면 메모리 상태를 JSON으로 변환하여 데이터베이스나 파일에 저장할 수 있습니다. 이를 통해 사용자가 며칠 뒤에 다시 접속하더라도 이전 대화 맥락을 그대로 복원할 수 있습니다.

- `agentThread.Serialize()` 를 호출하면 스레드 전체 상태(메모리 포함)가 JSON으로 직렬화됩니다.
- 나중에 이 JSON을 사용해 스레드를 역직렬화하면 메모리 상태가 그대로 복원됩니다.
- `AIContextProviderFactory`의 생성자는 `ctx.SerializedState`를 매개변수로 받아 이전 상태를 자동으로 복원합니다.

---

### 다중 컨텍스트 프로바이더 파이프라인

단일 에이전트에 **여러 개의 ContextProvider를 파이프라인으로 연결**하는 것이 가능합니다. 이를 통해 관심사를 명확히 분리할 수 있습니다.

| Provider 유형 | 담당 역할 |
|---|---|
| `ShortTermMemoryProvider` | 현재 세션의 최근 대화 히스토리 관리 |
| `LongTermSemanticMemoryProvider` | 벡터 DB(Azure AI Search + Mem0)를 통한 장기 에피소드 메모리 |
| `UserProfileProvider` | 사용자 선호도, 프로파일 정보 주입 |
| `SafetyPolicyProvider` | 안전 정책, 콘텐츠 필터 규칙 주입 |
| `DynamicToolProvider` | 상황에 따라 동적으로 도구 활성화 |

각 Provider는 **자체 토큰 버짓(token budget)** 을 가지므로 전체 컨텍스트 윈도우를 효율적으로 관리할 수 있습니다.

---

### AIContextProvider vs 단순 RAG의 차이점

Microsoft의 공식 설명에 따르면, `AIContextProvider`를 활용한 컨텍스트 관리는 단순 RAG(Retrieval-Augmented Generation) 방식과 본질적으로 다릅니다. RAG는 "주어진 쿼리와 유사한 것이 무엇인가?"라는 질문에만 답하지만, `AIContextProvider`는 메모리를 **상태(state) + 정책(policy)** 으로 취급합니다. 즉, 무엇이 메모리가 될지(선택), 어떻게 표현될지(스키마/압축), 언제 검색될지(게이팅), 언제 업데이트/소멸될지(자기 유지), 언제 사용하지 말아야 할지(노이즈 회피) 등을 모두 제어하는 **컨트롤 시스템(control system)** 역할을 합니다.

---

### AIContextProvider의 동적 Function Tool 제공 기능

#### 핵심 개념: "정적 도구" vs "동적 도구"의 차이

일반적으로 에이전트에 도구(Function Tool)를 등록하는 방법은 **정적 등록**입니다. 에이전트를 생성하는 시점에 `tools=[...]` 파라미터로 도구 목록을 고정하면, 이후 모든 대화 턴에서 항상 동일한 도구 목록이 LLM에 노출됩니다. 이는 단순한 경우에 잘 작동하지만, 도구 수가 50~100개 이상으로 늘어나거나 사용자 역할·상황에 따라 다른 도구를 제공해야 할 때는 심각한 문제가 됩니다.

`AIContextProvider`의 **동적 도구 제공** 기능은 이 한계를 극복합니다. `ProvideAIContextAsync` (C#) 또는 `invoking` (Python) 메서드에서 반환하는 `AIContext` / `Context` 객체의 **`Tools` 속성**에 도구 목록을 담아 돌려주면, 해당 턴(turn)에만 한정적으로 그 도구들이 LLM에 공급됩니다. 즉, **매 대화 턴마다 다른 도구 목록을 에이전트에 주입**할 수 있습니다.

공식 문서의 `AIContext.Tools` 속성 설명은 이를 명확히 밝힙니다.

> *"These tools are transient and apply only to the current AI model invocation. Any existing tools are provided as input to the AIContextProvider instances, so context providers can choose to modify or replace the existing tools as needed based on the current context."*

#### AIContext.Tools 속성의 구조

`AIContext.Tools`는 `IEnumerable<Microsoft.Extensions.AI.AITool>?` 타입입니다. `AITool`은 `Microsoft.Extensions.AI` 라이브러리의 기본 도구 추상화이며, `AIFunctionFactory.Create()`로 생성한 함수 도구도 이 타입으로 다뤄집니다.

```csharp
// AIContext 클래스의 Tools 속성 정의 (공식 API)
public IEnumerable<AITool>? Tools { get; set; }
```

이 속성의 동작 방식에는 중요한 특징이 있습니다.

- **일시성(Transient):** 반환된 도구 목록은 현재 단일 LLM 호출에만 유효합니다. 다음 턴에는 다시 `ProvideAIContextAsync`가 호출되어 새로운 도구 목록이 결정됩니다.
- **기존 도구 접근:** `InvokingContext`를 통해 에이전트에 기본 등록된 도구 목록을 참조할 수 있습니다. 이를 바탕으로 기존 도구를 **유지, 추가, 교체, 제거** 중 원하는 방식을 선택할 수 있습니다.
- **병합(Merging):** 기본 구현(`InvokingCoreAsync`)은 `ProvideAIContextAsync`가 반환한 도구를 기존 도구 목록에 **추가(append)** 합니다. 완전히 교체하려면 `InvokingCoreAsync`를 직접 오버라이드해야 합니다.

#### 동적 도구 제공이 필요한 핵심 시나리오

공식 문서와 GitHub 이슈에서 제시하는 실제 활용 시나리오들을 살펴보면 왜 이 기능이 중요한지 이해할 수 있습니다.

**시나리오 1: 사용자 역할 기반 도구 제어**
관리자 사용자에게는 데이터 삭제 도구를, 일반 사용자에게는 조회 도구만 보여주는 경우입니다. 메모리에 저장된 사용자 역할 정보를 `InvokingAsync`에서 읽어 조건에 따라 다른 도구 셋을 반환합니다.

**시나리오 2: 대화 흐름에 따른 단계별 도구 활성화**
결제 에이전트를 예로 들면, 상품 선택 단계에서는 검색 도구만, 결제 확인 단계에서는 결제 처리 도구를, 완료 후에는 영수증 발송 도구를 순차적으로 활성화할 수 있습니다. 이전 대화 내용과 상태를 분석하여 현재 단계에 적합한 도구만 노출합니다.

**시나리오 3: RAG 기반 동적 도구 선택 (ContextualFunctionProvider)**
이는 GitHub Issue #2630에서 공식적으로 논의되고 있는 가장 고급 시나리오입니다. 도구가 수십~수백 개일 때 모든 도구를 LLM에 전달하면 토큰 비용이 급증하고 모델 성능이 저하됩니다. `ContextualFunctionProvider`는 사용자 메시지를 벡터화하여 현재 대화와 가장 관련성이 높은 상위 N개의 도구만 선별해 제공하는 방식입니다.

#### C# (.NET) 구현 예시 — 역할 기반 동적 도구 주입

```csharp
using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// 도구 함수 정의
public static class AdminTools
{
    [Description("모든 사용자 데이터를 삭제합니다.")]
    public static string DeleteAllUserData() => "모든 데이터가 삭제되었습니다.";

    [Description("시스템 로그를 조회합니다.")]
    public static string GetSystemLogs() => "시스템 로그: [2026-03-08] 정상 운영 중";
}

public static class UserTools
{
    [Description("사용자 본인의 프로필을 조회합니다.")]
    public static string GetMyProfile() => "이름: 홍길동, 등급: 일반회원";

    [Description("상품 목록을 검색합니다.")]
    public static string SearchProducts(
        [Description("검색할 상품 키워드")] string keyword)
        => $"'{keyword}' 검색 결과: 상품A, 상품B, 상품C";
}

// AIContextProvider 구현: 사용자 역할에 따라 동적으로 도구를 공급
public class RoleBasedToolProvider : AIContextProvider
{
    private readonly ProviderSessionState<RoleState> _sessionState;

    public RoleBasedToolProvider() : base(null, null)
    {
        _sessionState = new ProviderSessionState<RoleState>(
            _ => new RoleState { Role = "user" }, // 기본 역할: 일반 사용자
            nameof(RoleBasedToolProvider));
    }

    public override string StateKey => _sessionState.StateKey;

    // READ Phase: 사용자 역할에 따른 도구 목록을 동적으로 반환
    protected override ValueTask<AIContext> ProvideAIContextAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        var state = _sessionState.GetOrInitializeState(context.Session);

        // 역할에 따라 완전히 다른 도구 셋을 구성
        IEnumerable<AITool> tools = state.Role switch
        {
            "admin" => new[]
            {
                AIFunctionFactory.Create(AdminTools.DeleteAllUserData),
                AIFunctionFactory.Create(AdminTools.GetSystemLogs),
                AIFunctionFactory.Create(UserTools.GetMyProfile),
                AIFunctionFactory.Create(UserTools.SearchProducts),
            },
            "user" => new[]
            {
                AIFunctionFactory.Create(UserTools.GetMyProfile),
                AIFunctionFactory.Create(UserTools.SearchProducts),
            },
            _ => Array.Empty<AITool>()
        };

        return new ValueTask<AIContext>(new AIContext
        {
            Instructions = $"현재 사용자 역할: {state.Role}",
            Tools = tools // ← 핵심: 동적 도구 목록 주입
        });
    }

    // WRITE Phase: 대화에서 역할 변경 감지
    protected override async ValueTask StoreAIContextAsync(
        InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        var state = _sessionState.GetOrInitializeState(context.Session);

        // 대화 중 역할 변경 메시지를 감지 (예시)
        foreach (var msg in context.RequestMessages)
        {
            if (msg.Text?.Contains("관리자 모드") == true)
            {
                state.Role = "admin";
                _sessionState.SaveState(context.Session, state);
            }
        }
    }

    public class RoleState
    {
        public string Role { get; set; } = "user";
    }
}
```

에이전트에 등록할 때는 다음과 같이 합니다.

```csharp
AIAgent agent = new OpenAIClient(apiKey)
    .GetChatClient("gpt-4o")
    .AsAIAgent(new ChatClientAgentOptions()
    {
        ChatOptions = new()
        {
            Instructions = "You are a helpful assistant.",
            // 기본 정적 도구는 없음 - 모두 AIContextProvider가 동적으로 공급
        },
        AIContextProviders = [new RoleBasedToolProvider()]
    });
```

#### 고급 구현: 도구 교체(Replace) 패턴

기본 `InvokingCoreAsync`는 도구를 **추가(append)** 합니다. 기존 도구를 완전히 **교체**하려면 `InvokingCoreAsync`를 직접 오버라이드해야 합니다.

```csharp
public class ContextAwareToolProvider : AIContextProvider
{
    private readonly ProviderSessionState<ConversationState> _sessionState;

    public ContextAwareToolProvider() : base(null, null)
    {
        _sessionState = new ProviderSessionState<ConversationState>(
            _ => new ConversationState { Stage = "initial" },
            nameof(ContextAwareToolProvider));
    }

    public override string StateKey => _sessionState.StateKey;

    // InvokingCoreAsync를 직접 오버라이드하여 도구를 교체(replace) 처리
    protected override async ValueTask<AIContext> InvokingCoreAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        var state = _sessionState.GetOrInitializeState(context.Session);

        // 대화 단계별 도구 셋 정의
        var stageTools = state.Stage switch
        {
            "initial"  => GetInitialStageTools(),
            "payment"  => GetPaymentStageTools(),
            "complete" => GetCompletionStageTools(),
            _          => Enumerable.Empty<AITool>()
        };

        // 기존 context.AIContext를 교체(replace)하여 완전히 새로운 AIContext 반환
        return new AIContext
        {
            Instructions = context.AIContext.Instructions
                           + $"\n\n현재 단계: {state.Stage}",
            Messages = context.AIContext.Messages, // 메시지는 유지
            Tools = stageTools                     // 도구는 완전히 교체
        };
    }

    private IEnumerable<AITool> GetInitialStageTools()
    {
        // 초기 단계: 검색 도구만 활성화
        return [AIFunctionFactory.Create(SearchProducts)];
    }

    private IEnumerable<AITool> GetPaymentStageTools()
    {
        // 결제 단계: 결제 처리 도구 활성화
        return
        [
            AIFunctionFactory.Create(ProcessPayment),
            AIFunctionFactory.Create(ApplyCoupon)
        ];
    }

    private IEnumerable<AITool> GetCompletionStageTools()
    {
        // 완료 단계: 영수증/리뷰 도구 활성화
        return
        [
            AIFunctionFactory.Create(SendReceipt),
            AIFunctionFactory.Create(LeaveReview)
        ];
    }

    [Description("상품을 검색합니다.")]
    private static string SearchProducts(string keyword) => $"검색 결과: {keyword}";

    [Description("결제를 처리합니다.")]
    private static string ProcessPayment(string orderId) => $"주문 {orderId} 결제 완료";

    [Description("쿠폰을 적용합니다.")]
    private static string ApplyCoupon(string couponCode) => $"쿠폰 {couponCode} 적용됨";

    [Description("영수증을 발송합니다.")]
    private static string SendReceipt(string email) => $"{email}로 영수증 발송 완료";

    [Description("리뷰를 작성합니다.")]
    private static string LeaveReview(string content) => $"리뷰 등록 완료: {content}";

    public class ConversationState
    {
        public string Stage { get; set; } = "initial";
    }
}
```

#### RAG 기반 동적 도구 선택: ContextualFunctionProvider 패턴

가장 고급화된 패턴은 **벡터 검색으로 관련 도구를 자동 선택**하는 것입니다. GitHub Issue #2630에서 공식 포팅이 논의 중인 `ContextualFunctionProvider`의 설계 구조는 다음과 같습니다.

```
전체 등록 도구: 100개
          ↓
   InvokingAsync 실행
          ↓
  사용자 메시지 임베딩(벡터화)
          ↓
  벡터 유사도 검색 (InMemoryVectorStore / Azure AI Search)
          ↓
  상위 5개 관련 도구만 선택
          ↓
  AIContext.Tools = [5개만 반환]
          ↓
  LLM에는 5개만 전달 → 토큰 절약 + 정확도 향상
```

```csharp
// ContextualFunctionProvider 사용 개념 코드 (구현 예정 API)
var vectorStore = new InMemoryVectorStore();
var allFunctions = new List<AIFunction> { /* 100개 이상의 도구 */ };

var contextProvider = new ContextualFunctionProvider(
    vectorStore: vectorStore,
    vectorDimensions: 1536,
    functions: allFunctions,
    maxNumberOfFunctions: 5, // 현재 턴에 상위 5개만 제공
    options: new ContextualFunctionProviderOptions
    {
        NumberOfRecentMessagesInContext = 3 // 최근 3턴을 문맥으로 사용
    }
);

agent.AddContextProvider(contextProvider);
// → 매 턴마다 대화 내용 기반으로 가장 관련된 5개 도구만 자동 선택
```

#### 내장(Built-in) ContextProvider: TextSearchProvider의 도구 노출 방식

MAF의 공식 내장 Provider인 `TextSearchProvider`는 동적 도구 제공 기능을 잘 보여주는 실제 사례입니다. 이 Provider는 두 가지 동작 모드를 지원합니다.

- **자동 주입 모드(Auto Inject):** 매 턴마다 사용자 입력으로 검색을 자동 수행하고 결과를 메시지로 주입합니다.
- **도구 노출 모드(Expose as Tool):** 검색 기능 자체를 LLM이 호출할 수 있는 Function Tool로 `AIContext.Tools`에 담아 제공합니다. LLM이 필요하다고 판단할 때만 검색이 호출됩니다.

```csharp
// TextSearchProvider가 내부적으로 Tools에 검색 도구를 주입하는 방식
var provider = new TextSearchProvider(
    searchAdapter: mySearchAdapter,
    options: new TextSearchProviderOptions
    {
        // 검색 기능을 도구로 노출할지, 자동 주입할지 설정
        TextSearchBehavior = TextSearchProviderOptions.TextSearchBehavior.ExposeTool
    }
);
```

#### 정리: 동적 도구 제공의 핵심 가치

`AIContextProvider`를 통한 동적 Function Tool 제공은 단순한 도구 주입 이상의 의미를 갖습니다. 아래 표는 정적 등록과 동적 제공의 차이를 명확히 보여줍니다.

| 비교 항목 | 정적 도구 등록 | AIContextProvider 동적 도구 |
|---|---|---|
| 도구 결정 시점 | 에이전트 생성 시 고정 | 매 턴(turn)마다 결정 |
| 사용자 역할 반영 | 불가 | 가능 |
| 토큰 효율 | 항상 전체 도구 전달 | 필요한 도구만 선택 전달 |
| 대화 흐름 연동 | 불가 | 상태에 따라 단계별 활성화 |
| 보안 제어 | 런타임 제어 불가 | 권한에 따라 실시간 제어 |
| 도구 수 확장성 | 소규모에 적합 | 수백 개 이상도 대응 가능 |

결론적으로 `AIContextProvider`의 동적 도구 제공 기능은 에이전트를 **상황 인식(Context-Aware)** 하게 만드는 핵심 메커니즘입니다. 단순히 "어떤 도구가 있는가"를 넘어 "지금 이 대화에서 어떤 도구가 필요한가"를 런타임에 결정할 수 있게 해주며, 이를 통해 토큰 효율성, 보안, 사용자 경험 모두를 동시에 개선할 수 있습니다.

#### AIContextProvider 요약

`AIContextProvider`는 Microsoft Agent Framework에서 에이전트에게 **메모리, 상태, 동적 컨텍스트**를 부여하는 핵심 메커니즘입니다. 추상 클래스를 상속받아 두 가지 핵심 생명주기 훅(`invoking`/`invoked`)을 구현함으로써, 개발자는 LLM 호출 전후로 완전한 제어권을 가집니다. 단기 세션 메모리부터 장기 시맨틱 메모리, 사용자 프로파일, 동적 도구 제공까지 모든 컨텍스트 엔지니어링 요구사항을 **결정론적(deterministic)이고 모듈화된 방식**으로 처리할 수 있으며, 직렬화 지원을 통해 상태를 외부 저장소에 영속화할 수 있습니다.

---

## 📋 공통 구성 요소

### 1. 환경 설정 (공통)

```json
// appsettings.json
{
  "LLM": {
    "Provider": "AzureOpenAI",
    "Endpoint": "your-endpoint",
    "ApiKey": "your-key",
    "Deployment": "gpt-4",
    "Model": "gpt-5.4"
  }
}
```

### 2. 테스트 프로젝트 구조

```
tests/
├── Stage01_TextAgent.Tests/
│   ├── ConfigAgentTests.cs
│   ├── CharacterAgentTests.cs
│   └── FAQAgentTests.cs
├── Stage02_SingleTool.Tests/
│   ├── DpsCalculatorTests.cs
│   └── ...
...
```

### 3. 샘플 데이터 디렉토리

```
samples/
├── game_configs/        # 게임 설정 파일들
├── logs/                # 로그 파일 샘플
├── documents/           # GDD 문서 샘플
├── images/              # 테스트 이미지
└── databases/           # SQLite 샘플 DB
```

### 4. 각 프로젝트 필수 포함 항목

1. **완전한 실행 가능 코드**
2. **인라인 주석** (왜/어떻게 설명)
3. **단위 테스트** (최소 3 개 이상)
4. **샘플 데이터/설정 파일**
5. **실행 방법 문서** (README)
6. **예상 입출력 예시**

---

## 📅 학습 진행 체크리스트

### 초급 완료 조건

- [ ] 1 단계: Agent 초기화 및 기본 응답 이해
- [ ] 2 단계: Tool 등록 및 Function Calling 이해
- [ ] 3 단계: 파일 시스템 연동 이해

### 중급 완료 조건

- [ ] 4 단계: 다중 Tool 조합 및 우선순위 관리
- [ ] 5 단계: 외부 API 연동 및 인증 처리
- [ ] 6 단계: 데이터베이스 안전한 쿼리
- [ ] 7 단계: RAG 및 Vector Search 구현

### 고급 완료 조건

- [ ] 8 단계: Sequential Workflow 구현
- [ ] 9 단계: Graph Workflow 및 조건부 분기
- [ ] 10 단계: Checkpointing 및 Time-travel
- [ ] 11 단계: 실시간 데이터 처리
- [ ] 12 단계: 멀티모달 (이미지 + 텍스트)

### 전문가 완료 조건

- [ ] 13 단계: 다중 데이터 소스 통합
- [ ] 14 단계: 시뮬레이션 및 최적화
- [ ] 15 단계: 프로덕션 배포 및 모니터링

---

## 🔧 기술 스택

| 구성 요소 | 기술 |
|-----------|------|
| 언어 | C# 12.0 / .NET 8 |
| Agent Framework | Microsoft Agent Framework (최신) |
| LLM 제공자 | Azure OpenAI, OpenAI, Anthropic |
| Vector Store | Azure AI Search, Chroma |
| 데이터베이스 | SQLite, PostgreSQL |
| 테스트 | xUnit, Moq |
| CI/CD | GitHub Actions |

---

## 🌱 각 단계별 추가 학습 요소

각 실습 단계에서 다음 요소들을 점진적으로 추가하면 학습 효과가 극대화됩니다.

- **Middleware 사용**: 로깅, 인증, 에러 처리를 위한 미들웨어 구현
- **다양한 LLM 제공자 테스트**: OpenAI, Claude, Gemini를 번갈아 사용하며 각 모델의 특성 파악
- **프롬프트 엔지니어링**: 각 게임 도메인에 맞는 효과적인 시스템 프롬프트 작성
- **비용 최적화**: 캐싱, 스트리밍, 적절한 모델 선택으로 API 비용 절감
- **테스트 및 벤치마킹**: 에이전트 성능 측정 및 개선

---

## 🚀 다음 단계

1. Stage01 부터 순차적으로 코드 생성
2. 각 프로젝트 완료 후 테스트 실행
3. 학습자 피드백 반영하여 개선

> **시작 명령**: `/generate-code 1` - 1 단계 코드 스캐폴딩 생성

---

이 커리큘럼은 단순한 개념 이해부터 실제 게임 개발 업무에 즉시 적용 가능한 실용 시스템까지 자연스럽게 발전하도록 설계되었습니다. Windows 11 환경에서 C#/.NET 으로 시작하시면 됩니다.
