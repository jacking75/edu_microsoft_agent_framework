# Stage 04: MultiTool - 복수 도구 활용 학습

## 📋 개요

Stage 04 에서는 AI Agent 가 **여러 도구를 조합하여 사용하는 방법**을 학습합니다. 단일 도구를 넘어서서 복잡한 워크플로우를 구현하고, 도구 간 데이터를 공유하며, 상황에 따라 적절한 도구를 선택하는 능력을 기릅니다.

## 🎯 학습 목표

1. **여러 Tool 동시 등록**: Agent 에 여러 도구를 등록하고 활용
2. **Tool 조합 패턴**: 여러 도구를 순차적/병렬적으로 호출
3. **상황별 Tool 선택**: 컨텍스트에 맞는 적절한 도구 선택
4. **데이터 흐름 관리**: 도구 간 데이터 전달 및 공유

---

## 📁 프로젝트 구성

```
Stage04_MultiTool/
├── Stage04.sln
├── 04A_ResourceOptimizer/       # 리소스 최적화 어시스턴트
│   ├── Program.cs
│   └── Tools/
│       ├── TextureAnalyzerTool.cs    # 텍스처 분석 도구
│       └── MemoryEstimatorTool.cs    # 메모리 추정 도구
├── 04B_BuildAnalyzer/           # 빌드 분석기
│   ├── Program.cs
│   └── Tools/
│       ├── BuildLogParserTool.cs     # 빌드 로그 파싱 도구
│       └── WarningClassifierTool.cs  # 경고 분류 도구
└── 04C_PerformanceProfiler/     # 성능 프로파일러
    ├── Program.cs
    └── Tools/
        ├── FpsAnalyzerTool.cs        # FPS 분석 도구
        └── BottleneckDetectorTool.cs # 병목 감지 도구
```

---

## 🔧 각 프로젝트 설명

### 04A_ResourceOptimizer - 리소스 최적화 어시스턴트

**학습 내용:**
- 텍스처 분석 도구 + 메모리 추정 도구 조합
- 복수 파일 처리를 위한 `List<string>` 파라미터
- 도구 간 결과 통합 및 보고서 생성

**사용 가능한 도구:**
| 도구 | 설명 | 파라미터 |
|------|------|----------|
| `AnalyzeTexture` | 개별 텍스처 파일 분석 | `filePath: string` |
| `EstimateMemoryUsage` | 여러 텍스처 메모리 추정 | `texturePaths: List<string>` |
| `EstimateBuildMemory` | 프로젝트 빌드 메모리 추정 | `projectPath: string` |

**예시 요청:**
- "텍스처 메모리 사용량을 분석해줘"
- "빌드 메모리 추정이 필요해"
- "최적화 방안을 제안해줘"

---

### 04B_BuildAnalyzer - 빌드 분석기

**학습 내용:**
- 빌드 로그 파싱 + 경고 분류 도구 조합
- 에러/경우/정보 분류
- 우선순위 기반 보고서 생성

**사용 가능한 도구:**
| 도구 | 설명 | 반환 타입 |
|------|------|-----------|
| `ParseBuildLog` | 빌드 로그 전체 분석 | `string` |
| `ExtractErrors` | 에러 라인 추출 | `string` |
| `ClassifyWarnings` | 경고 카테고리 분류 | `Dictionary<string, int>` |
| `GetSummary` | 경고 요약 보고서 | `string` |

**우선순위 분류:**
1. **에러** - 빌드 실패, 즉시 수정 필요
2. **Null 참조 경고** - 런타임 에러 가능성
3. **사용하지 않는 코드** - 코드 정리 필요
4. **사용 중단된 API** - 호환성 문제

**예시 요청:**
- "빌드 로그를 분석해줘"
- "에러만 추출해줘"
- "경고를 분류하고 우선순위를 매겨줘"

---

### 04C_PerformanceProfiler - 성능 프로파일러

**학습 내용:**
- FPS 분석 + 병목 감지 도구 조합
- 수치 데이터 처리 (`List<int>`, `Dictionary<string, double>`)
- 상황별 최적화 제안 생성

**사용 가능한 도구:**
| 도구 | 설명 | 파라미터 |
|------|------|----------|
| `AnalyzeFpsData` | FPS 통계 분석 | `fpsValues: List<int>` |
| `DetectDrops` | FPS 드롭 감지 | `fpsValues: List<int>, threshold: int` |
| `DetectBottleneck` | 병목 현상 분석 | `metrics: Dictionary<string, double>` |
| `SuggestOptimizations` | 최적화 제안 | `bottleneckType: string` |

**성능 기준:**
- **60 FPS 이상**: 원활
- **30-60 FPS**: 일반
- **30 FPS 미만**: 개선 필요
- **1% Low < 30**: 스터터링 가능성

**예시 요청:**
- "FPS 55, 58, 60, 45, 52, 59 의 평균을 구해줘"
- "CPU 92%, GPU 85%, 메모리 78% 일 때 병목은?"
- "최적화 방안을 제안해줘"

---

## 🚀 실행 방법

### 환경 변수 설정

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"
```

### 프로젝트 실행

```bash
# 04A_ResourceOptimizer 실행
dotnet run --project 04A_ResourceOptimizer

# 04B_BuildAnalyzer 실행
dotnet run --project 04B_BuildAnalyzer

# 04C_PerformanceProfiler 실행
dotnet run --project 04C_PerformanceProfiler
```

---

## 💡 핵심 개념

### Tool 등록 패턴

```csharp
// 1. 여러 Tool 인스턴스 생성
var textureTool = new TextureAnalyzerTool();
var memoryTool = new MemoryEstimatorTool();

// 2. AIAgent 생성 시 tools 매개변수에 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: "...",
    name: "ResourceOptimizer",
    tools: [
        AIFunctionFactory.Create(textureTool.AnalyzeTexture),
        AIFunctionFactory.Create(textureTool.EstimateMemoryUsage),
        AIFunctionFactory.Create(memoryTool.EstimateBuildMemory)
    ]
);
```

### 복수 Tool 활용 전략

```
사용자 요청
    ↓
[Agent] - 요청 분석
    ↓
[Tool 선택] - 상황에 맞는 도구 선택
    ↓
[Tool 호출] - 단일 또는 복수 호출
    ↓
[결과 통합] - 여러 도구 결과 조합
    ↓
[응답 생성] - 최종 보고서 작성
```

### 데이터 타입 처리

| 타입 | 용도 | 예시 |
|------|------|------|
| `string` | 파일 경로, 텍스트 | `"path/to/file.txt"` |
| `List<string>` | 복수 파일 경로 | `["a.png", "b.png"]` |
| `List<int>` | 수치 데이터 시계열 | `[55, 58, 60, 45]` |
| `Dictionary<string, double>` | 키 - 값 메트릭 | `{"cpu": 92.5, "gpu": 85.0}` |

---

## 📝 연습 과제

1. **Tool 추가**: 각 프로젝트에 새로운 분석 도구 추가
2. **결과 캐싱**: 동일 입력에 대한 결과 재사용
3. **점진적 분석**: 대용량 데이터를 청크로 나누어 처리
4. **오류 복구**: 도구 실패 시 대체 전략 구현

---

## 🔗 다음 단계

- **Stage 05_ExternalAPI**: 외부 API 와 연동하는 Agent 학습
- **Stage 06_Memory**: Agent 에 메모리 기능 추가
- **Stage 07_Planning**: 복잡한 작업 계획 수립
