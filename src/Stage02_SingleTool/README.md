# Stage 2: 단일 Tool 사용 에이전트

> **학습 목표**: Microsoft Agent Framework 에서 Function Tool 사용하기

---

## 🎯 이 프로젝트에서 배우는 것

### 핵심 학습 목표

1. **Function Tool 정의**
   - `Description` Attribute 사용법
   - 파라미터 설명 작성
   - Tool 메서드 설계

2. **Tool 과 Agent 연동**
   - `AIFunctionFactory.Create()` 로 Tool 등록
   - `AsAIAgent()` 에 `tools` 파라미터로 전달
   - Agent 가 Tool 을 인식하는 방식

3. **다양한 Tool 유형**
   - 계산/변환 Tool (02A)
   - 시스템 명령어 실행 Tool (02B)
   - 파일 시스템 I/O Tool (02C)

4. **Agent 의 Tool 호출**
   - `RunAsync()` 가 Tool 호출을 자동으로 처리
   - Tool 호출과 결과 반환

---

## 📁 프로젝트 구성

| 프로젝트 | Tool 유형 | 설명 | 학습 내용 |
|----------|----------|------|-----------|
| **02A_DpsCalculator** | 계산 Tool | DPS 계산 | 순수 함수 호출, 계산 로직 |
| **02B_CmdExecutor** | 시스템 명령어 | CMD 명령 실행 | 외부 프로세스 실행, 보안 |
| **02C_FileManager** | 파일 시스템 | 파일读写/삭제/복사 | I/O 작업, 예외 처리 |

---

## 🚀 실행 방법

### 1. 환경 변수 설정

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"
```

### 2. NuGet 패키지 복원

```bash
cd src
dotnet restore Stage02.sln
```

### 3. 프로젝트 실행

```bash
cd src

# 02A: DPS 계산기 (계산 Tool)
dotnet run --project Stage02_SingleTool/02A_DpsCalculator

# 02B: 명령어 실행기 (시스템 명령어 Tool)
dotnet run --project Stage02_SingleTool/02B_CmdExecutor

# 02C: 파일 관리자 (파일 시스템 Tool)
dotnet run --project Stage02_SingleTool/02C_FileManager
```

---

## 🎮 실행해 볼 것 - 추천 미션

### 02A_DpsCalculator 에서 시도해보기

**기본 미션:**
- [ ] "공격력 100, 초당 공격 속도 1.5 인 무기의 DPS 를 계산해줘"
- [ ] "DPS 가 150 일 때 크리티컬 확률 20%, 배율 2 배면 기대 DPS 는?"
- [ ] "공격력 80, 공격 속도 2.0 계산해줘"

**심화 미션:**
- [ ] "DPS 200 무기가 크리티컬 30%, 배율 2.5 배일 때 최종 기대치는?"
- [ ] "공격력 150, 초당 1 회 vs 공격력 75, 초당 2 회 중 뭐가 더 세?"

**배우는 것:**
- ✅ `Description` Attribute 로 Tool 정의
- ✅ Tool 이 자동 호출되는 과정
- ✅ 계산 결과의 설명과 함께 제공

---

### 02B_CmdExecutor 에서 시도해보기

**기본 미션:**
- [ ] "현재 디렉토리 목록을 보여줘"
- [ ] "시스템 정보를 알려줘"
- [ ] "현재 날짜와 시간이 어떻게 돼?"

**심화 미션:**
- [ ] "실행 중인 프로세스 목록을 보여줘 (tasklist)"
- [ ] "컴퓨터 이름을 알려줘 (hostname)"
- [ ] "PATH 환경 변수 값을 보여줘"

**배우는 것:**
- ✅ 외부 프로세스 실행 (`Process.Start`)
- ✅ stdout/stderr 캡처
- ✅ 보안 고려사항 (위험한 명령어 차단)

---

### 02C_FileManager 에서 시도해보기

**기본 미션:**
- [ ] "현재 디렉토리 목록을 보여줘"
- [ ] "test.txt 파일에 'Hello World' 라고 써줘"
- [ ] "test.txt 파일 내용을 읽어줘"

**심화 미션:**
- [ ] "test.txt 파일을 backup.txt 로 복사해줘"
- [ ] "log.txt 파일에 새로운 로그를 추가해줘"
- [ ] "불필요한 파일을 삭제해줘"

**배우는 것:**
- ✅ 파일 읽기/쓰기 (`File.ReadAllText`, `File.WriteAllText`)
- ✅ 디렉토리 조작 (`Directory.CreateDirectory`)
- ✅ 예외 처리 (파일 없음, 접근 권한)

---

## 📖 학습 내용

### 1. Function Tool 정의

```csharp
using System.ComponentModel;

public class DpsCalculationTool
{
    [Description("무기의 초당 데미지 (DPS) 를 계산합니다.")]
    public int CalculateDps(
        [Description("무기의 기본 공격력")] int damage, 
        [Description("초당 공격 속도")] double attacksPerSecond)
    {
        return (int)(damage * attacksPerSecond);
    }
}
```

**중요 포인트:**
- `[Description]`: 메서드와 파라미터에 설명 추가 (Agent 가 이해하기 쉬움)
- 간단한 반환 타입 사용 (int, double, string 등)
- 부수 효과 (side effect) 가 없는 순수 함수가 좋음

---

### 2. Tool 과 함께 Agent 생성

```csharp
using Microsoft.Extensions.AI;

// Tool 인스턴스 생성
var dpsTool = new DpsCalculationTool();

// OpenAI 클라이언트 생성
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// AIAgent 생성 - tools 파라미터로 전달
AIAgent agent = chatClient.AsAIAgent(
    instructions: "당신은 게임 DPS 계산 전문가입니다.",
    name: "DpsCalculator",
    tools: [
        AIFunctionFactory.Create(dpsTool.CalculateDps)
    ]
);
```

---

### 3. Agent 실행 (자동 Tool 호출)

```csharp
// RunAsync() 가 Tool 호출을 자동으로 처리합니다
var response = await agent.RunAsync("공격력 100, 공격 속도 1.5 인 무기의 DPS 를 계산해줘");

Console.WriteLine(response);
```

**동작 과정:**
1. 사용자 질문 분석
2. 필요한 Tool 이 있으면 호출
3. Tool 결과로 응답 생성
4. 응답 반환

---

### 4. Tool 유형별 특징

#### 계산 Tool (02A)
- 순수 함수 (같은 입력 → 같은 출력)
- 부수 효과 없음
- 빠른 실행

```csharp
[Description("DPS 를 계산합니다.")]
public int CalculateDps(int damage, double attacksPerSecond)
{
    return (int)(damage * attacksPerSecond);
}
```

#### 시스템 명령어 Tool (02B)
- 외부 프로세스 실행
- 보안 주의 필요
- stdout/stderr 캡처

```csharp
[Description("CMD 명령어를 실행합니다.")]
public string ExecuteCommand(string command)
{
    var startInfo = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = $"/c {command}",
        RedirectStandardOutput = true,
        UseShellExecute = false
    };
    // ... 실행 및 결과 반환
}
```

#### 파일 시스템 Tool (02C)
- I/O 작업
- 예외 처리 중요
- 경로 검증 필요

```csharp
[Description("파일 내용을 읽습니다.")]
public string ReadFile(string filePath)
{
    if (!File.Exists(filePath))
        return $"Error: File not found - {filePath}";
    return File.ReadAllText(filePath);
}
```

---

### 5. Tool 설계 모범 사례

**✅ 좋은 예:**
```csharp
[Description("무기의 DPS 를 계산합니다.")]
public int CalculateDps(
    [Description("무기의 공격력")] int damage,
    [Description("초당 공격 속도")] double attacksPerSecond)
{
    return (int)(damage * attacksPerSecond);
}
```

**❌ 나쁜 예:**
```csharp
// 설명이 없음
public int Calculate(int a, int b) { }

// 너무 많은 로직
[Description("복잡한 처리를 수행합니다.")]
public async Task<string> ProcessComplexLogic(...) { }

// 위험한 작업 (삭제, 포맷 등) 은 경고 필요
public string DeleteSystemFile(string path) { }
```

---

## 💡 예제 질문

### 02A_DpsCalculator

```
👤 사용자: 공격력 100, 초당 공격 속도 1.5 인 무기의 DPS 를 계산해줘
🤖 에이전트: 무기의 DPS 는 150 입니다. (100 × 1.5 = 150)

👤 사용자: DPS 가 150 이고 크리티컬 확률 20%, 배율 2 배일 때 기대 DPS 는?
🤖 에이전트: 기대 DPS 는 180 입니다. (150 × 1.2 = 180)
```

### 02B_CmdExecutor

```
👤 사용자: 현재 디렉토리 목록을 보여줘
🤖 에이전트: [cmd.exe 실행 → dir 결과 반환]
  Directory: C:\project
  Folders: bin, obj, Tools
  Files: Program.cs, appsettings.json

👤 사용자: 현재 시간이 어떻게 돼?
🤖 에이전트: 현재 시간은 2026-03-10 15:30:45 입니다.
```

### 02C_FileManager

```
👤 사용자: test.txt 파일에 'Hello World' 라고 써줘
🤖 에이전트: test.txt 파일에 내용을 작성했습니다. (11 bytes)

👤 사용자: test.txt 파일 내용을 읽어줘
🤖 에이전트: 파일 내용: Hello World
```

---

## 🔗 관련 문서

- **[Microsoft Agent Framework 공식 문서](https://learn.microsoft.com/agent-framework/)**
- **[Function Tools 가이드](https://learn.microsoft.com/agent-framework/agents/tools/function-tools)**
- **[GitHub Repository](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples)**

---

## ✅ 학습 체크리스트

### 기본 개념
- [ ] `Description` Attribute 이해
- [ ] `AIFunctionFactory.Create()` 사용법
- [ ] Tool 클래스 정의
- [ ] `AsAIAgent()` 에 `tools` 파라미터 전달

### 실습
- [ ] 02A_DpsCalculator 실행 및 테스트 (계산 Tool)
- [ ] 02B_CmdExecutor 실행 및 테스트 (시스템 명령어 Tool)
- [ ] 02C_FileManager 실행 및 테스트 (파일 시스템 Tool)
- [ ] 다양한 질문으로 Tool 호출 확인

### 심화 이해
- [ ] Tool 이 호출되는 과정 이해
- [ ] 파라미터 설명의 중요성 이해
- [ ] Tool 유형별 특징 이해 (계산 vs 시스템 vs 파일)
- [ ] 보안 고려사항 이해 (명령어 실행 시)

---

## 📌 NuGet 패키지

```xml
<PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-rc1" />
<PackageReference Include="Microsoft.Extensions.AI" Version="10.3.0" />
<PackageReference Include="Azure.Identity" Version="1.13.2" />
<PackageReference Include="System.ClientModel" Version="1.8.1" />
```

---

## 🎯 다음 단계 (Stage 3)

Stage 2 에서 배운 Function Tool 을 바탕으로, Stage 3 에서는 **파일 시스템 연동**을 심화 학습합니다:

- ✅ 파일 읽기/쓰기 Tool 구현
- ✅ 로그 파일 분석 에이전트
- ✅ 에러 패턴 발견

Stage 2 를 완료했다면, 이제 Stage 3 로 이동하세요!
