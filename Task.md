# Microsoft Agent Framework 학습 프로젝트 계획서

> **목표**: C#/.NET 을 사용한 Microsoft Agent Framework 15 단계 완전 마스터
> **방향성**: 각 단계별 2~3 개의 점진적 예제로 개념을 체계적으로 습득

---

## 📁 전체 프로젝트 구조

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

## 🎯 단계별 상세 구현 계획

### 📘 초급 단계: 기본 개념 익히기

---

#### **1 단계: 텍스트 응답 에이전트**

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **1-A: 게임 설정 조회 봇** | JSON 게임 설정 파일에서 특정 값 조회 | Agent 초기화, 기본 프롬프트 |
| **1-B: 캐릭터 정보问答 봇** | YAML 캐릭터 스탯 파일 질의응답 | 다양한 데이터 포맷 처리 |
| **1-C: 대화형 FAQ 봇** | 자주 묻는 질문 응답 에이전트 | 대화 컨텍스트 유지 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **2-A: DPS 계산기** | 무기 공격력/속도로 DPS 계산 | Function Calling 기본 |
| **2-B: 크리티컬 데미지 계산기** | 크리틱 확률/데미지 기대값 계산 | Tool 파라미터 처리 |
| **2-C: 레벨업 필요 경험치 계산기** | 레벨별 경험치 곡선 계산 | Tool 등록 및 바인딩 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **3-A: 로그 파일 요약기** | 게임 로그 읽어서 주요 이벤트 추출 | 파일 읽기 Tool 구현 |
| **3-B: 에러 패턴 분석기** | 크래시 리포트에서 공통 패턴 발견 | 텍스트 처리 및 분석 |
| **3-C: 세이브 파일 검증기** | 게임 세이브 파일 무결성 확인 | 파일 쓰기/수정 Tool |

**구형 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **4-A: 리소스 최적화 어시스턴트** | 텍스처/오디오 파일 분석 + 메모리 추정 | 여러 Tool 조합 |
| **4-B: 빌드 분석기** | 빌드 로그 파싱 + 경고/에러 분류 | Tool 간 데이터 공유 |
| **4-C: 성능 프로파일러** | FPS 데이터 분석 + 병목 지점 제안 | Tool 우선순위 관리 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **5-A: Jira 이슈 생성 봇** | 자연어 → Jira 티켓 자동 생성 | REST API 호출 Tool |
| **5-B: GitHub PR 리뷰어** | PR 링크로 변경사항 요약 | API 인증 처리 |
| **5-C: Slack 알림 에이전트** | 중요 이벤트 Slack 전송 | Webhook 연동 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **6-A: 플레이어 통계 조회 봇** | SQL 쿼리로 플레이어 데이터 조회 | 안전한 DB 쿼리 |
| **6-B: 리텐션 분석 에이전트** | 일별/주별 리텐션 계산 | 집계 쿼리 처리 |
| **6-C: 자연어 SQL 변환기** | 자연어를 SQL 로 변환 실행 | NL-to-SQL 패턴 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **7-A: 문서 검색 봇 (기본)** | 텍스트 문서를 벡터화하여 검색 | 임베딩 생성/저장 |
| **7-B: 디자인 문서 QA** | GDD 문서에서 관련 섹션 찾기 | Vector Store 연동 |
| **7-C: 코드베이스 검색기** | 소스 코드를 인덱싱하여 질의응답 | 코드 임베딩 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **8-A: 아이템 생성 파이프라인** | 설명작성 → 밸런스검토 → 로컬라이제이션 | Sequential Workflow |
| **8-B: 퀘스트 생성 파이프라인** | 스토리 → 보상설정 → 난이도조정 | 에이전트 간 데이터 전달 |
| **8-C: 다이얼로그 생성기** | 초안작성 → 톤체크 → 분기지점추가 | 다단계 검토 워크플로우 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **9-A: 크래시 리포트 처리기** | 수집 → 분석 → 심각도평가 → 할당 | Graph Workflow 기본 |
| **9-B: 자동 버그 분류기** | 로그 분석 → 중복체크 → 우선순위결정 | 조건부 분기 |
| **9-C: QA 리포트 생성기** | 테스트결과 → 요약 → 배포판 결정 | 병렬 처리 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **10-A: 스토리 협업 어시스턴트** | AI 초안 → 인간수정 → AI 계속작성 | Checkpointing |
| **10-B: 밸런스 조정 워크플로우** | AI 제안 → 디자이너검토 → 시뮬레이션 | Time-travel |
| **10-C: 로컬라이제이션 검수** | AI 번역 → 인간검토 → 수정반영 | 상태 저장/복원 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **11-A: 서버 메트릭 모니터링** | 실시간 FPS/핑치 데이터 수집 | 스트리밍 데이터 처리 |
| **11-B: 이상 징후 탐지기** | 평소 패턴과 비교하여 이상 감지 | 이벤트 기반 아키텍처 |
| **11-C: 자동 스케일링 제안** | 부하 분석 → 인스턴스 조정 제안 | 실시간 알림 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **12-A: UI 스타일 검수기** | 스크린샷 → 스타일가이드 준수여부 | 이미지 분석 Tool |
| **12-B: 캐릭터 모델 검수기** | 3D 모델 스크린샷 → 비율/스타일 체크 | 비전 + 텍스트 통합 |
| **12-C: 아이콘 일관성 검증기** | 아이콘 세트 → 일관성 분석 | 멀티모달 비교 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **13-A: 스프린트 계획 어시스턴트** | Jira + Git 연동 → 작업 추정 | 다중 데이터 소스 통합 |
| **13-B: 리소스 배분 최적화** | 팀원 역량 + 작업량 분석 | 복잡한 상태 관리 |
| **13-C: 병목 분석 대시보드** | 생산성 메트릭 → 병목지점 식별 | 장기 메모리 구현 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **14-A: 몬테카를로 시뮬레이터** | 전투 시뮬레이션 → 승률 분석 | 대량 시뮬레이션 실행 |
| **14-B: 최적 파라미터 찾기** | 그리드서치 → 최적 밸런스 제안 | 결과 분석/시각화 |
| **14-C: 강화학습 기반 밸런서** | 자기플레이 → 메타 발견 | 실험적 기능 활용 |

**구현 세부사항:**
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

| 프로젝트 | 설명 | 학습 목표 |
|----------|------|-----------|
| **15-A: 코드 생성 어시스턴트** | 요구사항 → C# 코드 생성 | DevUI 시각화 |
| **15-B: 버그 수정 제안기** | 에러로그 → 수정코드 제안 | Observability 통합 |
| **15-C: 종합 개발 워크플로우** | 코드/테스트/문서 자동화 | 프로덕션 배포 |

**구현 세부사항:**
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

## 📝 각 프로젝트 필수 포함 항목

1. **완전한 실행 가능 코드**
2. **인라인 주석** (왜/어떻게 설명)
3. **단위 테스트** (최소 3 개 이상)
4. **샘플 데이터/설정 파일**
5. **실행 방법 문서** (README)
6. **예상 입출력 예시**

---

## 🚀 다음 단계

1. Stage01 부터 순차적으로 코드 생성
2. 각 프로젝트 완료 후 테스트 실행
3. 학습자 피드백 반영하여 개선

> **시작 명령**: `/generate-code 1` - 1 단계 코드 스캐폴딩 생성
