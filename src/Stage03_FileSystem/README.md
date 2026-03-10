# Stage 03: FileSystem - 파일 시스템 도구 학습

## 📋 개요

Stage 03 에서는 AI Agent 가 **파일 시스템과 상호작용**하는 방법을 학습합니다. 파일을 읽고, 분석하고, 검증하는 도구를 구현하여 Agent 가 실제 데이터를 처리할 수 있도록 합니다.

## 🎯 학습 목표

1. **파일 읽기/쓰기 Tool 구현**: Agent 가 파일을 조작할 수 있는 커스텀 도구 만들기
2. **파일 시스템 접근 패턴**: 안전한 파일 처리 방법 학습
3. **대용량 텍스트 처리**: 로그 파일 등 큰 데이터를 효율적으로 분석
4. **데이터 유효성 검사**: JSON 파일의 무결성 검증

---

## 📁 프로젝트 구성

```
Stage03_FileSystem/
├── Stage03.sln
├── 03A_LogSummarizer/          # 로그 파일 요약기
│   ├── Program.cs
│   └── Tools/
│       └── FileReadTool.cs     # 파일 읽기 전용 도구
├── 03B_ErrorPatternAnalyzer/   # 에러 패턴 분석기
│   ├── Program.cs
│   └── Tools/
│       └── FileAnalysisTool.cs # 에러 분석 도구
└── 03C_SaveFileValidator/      # 세이브 파일 검증기
    ├── Program.cs
    └── Tools/
        └── FileWriteTool.cs    # 파일 검증 도구
```

---

## 🔧 각 프로젝트 설명

### 03A_LogSummarizer - 로그 파일 요약기

**학습 내용:**
- 파일 읽기 도구 구현 (`ReadFile`, `ReadLastLines`, `SearchPattern`)
- 대용량 로그 파일 처리
- 로그 레벨별 분류 ([ERROR], [WARNING], [INFO])

**동작 방식:**
1. 게임 로그 파일을 읽습니다
2. 주요 이벤트와 에러 메시지를 추출합니다
3. AI 가 로그를 분석하여 요약보고서를 생성합니다
4. 문제 패턴을 발견하고 개선 사항을 제안합니다

**예시 명령:**
- "로그 파일을 요약해줘"
- "ERROR 메시지만 보여줘"
- "WARNING 패턴을 찾아줘"

---

### 03B_ErrorPatternAnalyzer - 에러 패턴 분석기

**학습 내용:**
- 에러 로그 분석 도구 구현
- 예외 타입 자동 분류
- 빈도수 기반 패턴 분석

**동작 방식:**
1. 크래시 리포트 파일에서 에러 라인을 추출합니다
2. 예외 타입 (NullReferenceException, TimeoutException 등) 을 분류합니다
3. 각 에러 유형의 발생 빈도를 계산합니다
4. AI 가 근본 원인을 추론하고 수정 우선순위를 제안합니다

**분석 대상 에러:**
- `NullReferenceException` - null 체크 누락
- `TimeoutException` - 네트워크/DB 지연
- `IOException` - 파일/디스크 문제
- `InvalidOperationException` - 컬렉션 수정 문제

---

### 03C_SaveFileValidator - 세이브 파일 검증기

**학습 내용:**
- JSON 파일 파싱 및 검증
- 필수 필드 존재 여부 확인
- 데이터 무결성 검사

**동작 방식:**
1. JSON 세이브 파일의 형식 유효성을 검사합니다
2. 필수 필드 (playerId, playerName, level 등) 가 있는지 확인합니다
3. 데이터 타입 일관성을 검증합니다
4. 손상된 파일에 대한 복구 방안을 제안합니다

**검사 항목:**
- JSON 형식 유효성
- 필수 필드 존재 여부
- 데이터 타입 일관성
- 값의 범위 검증

---

## 🚀 실행 방법

각 프로젝트는 독립적으로 실행할 수 있습니다.

```bash
# 03A_LogSummarizer 실행
dotnet run --project 03A_LogSummarizer

# 03B_ErrorPatternAnalyzer 실행
dotnet run --project 03B_ErrorPatternAnalyzer

# 03C_SaveFileValidator 실행
dotnet run --project 03C_SaveFileValidator
```

### 환경 변수 설정

실행 전에 다음 환경 변수를 설정해야 합니다:

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"
```

---

## 💡 핵심 개념

### Tool 구현 패턴

```csharp
public class FileReadTool
{
    // Agent 가 호출할 수 있는 메서드
    public string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return $"Error: File not found - {filePath}";
        }
        return File.ReadAllText(filePath);
    }
}
```

### AIAgent 와 Tool 연동

```csharp
// Tool 인스턴스 생성
var fileTool = new FileReadTool();

// AIAgent 생성 시 Tool 등록
AIAgent agent = chatClient.AsAIAgent(
    instructions: """
        당신은 게임 로그 분석 전문가입니다.
        사용 가능한 도구: ReadFile, ReadLastLines, SearchPattern
        """,
    name: "LogSummarizer"
);
```

---

## 📝 연습 과제

1. **파일 크기 제한 추가**: 대용량 파일을 처리할 때 메모리 초과 방지
2. **인코딩 지원 확장**: UTF-8, EUC-KR 등 다양한 문자 인코딩 지원
3. **비동기 처리**: `ReadAsync`, `WriteAsync`를 사용한 성능 개선
4. **패턴 확장**: 정규식을 사용한 고급 패턴 검색 구현

---

## 🔗 다음 단계

- **Stage 04_MultiTool**: 여러 도구를 동시에 사용하는 Agent 학습
- **Stage 05_ExternalAPI**: 외부 API 와 연동하는 Agent 학습
