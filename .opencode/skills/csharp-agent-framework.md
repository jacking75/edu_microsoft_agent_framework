# C# Microsoft Agent Framework 개발 스킬

## 📋 목적

Microsoft Agent Framework 공식 SDK 를 사용한 C# 교육용 코드 개발을 위한 전문 스킬

## 적용 범위

- README.md 의 15 단계 학습 경로에 따른 구현
- 교육용 예제 코드 작성
- Agent Framework 개념 시연
- **단위 테스트는 만들지 않는다** - 교육용 코드에 집중
- **Microsoft Agent Framework 공식 SDK 사용** (`Microsoft.Agents.AI.OpenAI`)

---

## 🎯 핵심 원칙

### 1. Microsoft Agent Framework 공식 API 사용

```csharp
// ✅ 좋음: 공식 API 사용
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;

AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "당신은 친절한 어시스턴트입니다.", name: "MyAgent");

// ❌ 나쁨: 구형 API 또는 직접 구현
var agent = new Agent { Name = "GameAssistant" }; // 구형
```

### 2. Azure OpenAI 우선 사용

```csharp
// 환경 변수
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4o-mini

// 인증: Azure CLI 기반 (개발 환경)
var credential = new DefaultAzureCredential();
```

### 3. 단계적 학습 경로 준수

```
Stage 1-3: 기본 (Agent, Tool, File)
Stage 4-7: 중급 (Multi-Tool, API, DB, RAG)
Stage 8-12: 고급 (Workflow, Multi-Agent, Realtime, Multimodal)
Stage 13-15: 전문가 (PM, Simulation, Copilot)
```

---

## 📦 NuGet 패키지 표준

### .NET 버전

- **반드시 .NET 10 사용** (`net10.0`)
- 모든 프로젝트는 최신 .NET 버전으로 통일

### 기본 패키지 (모든 프로젝트)

```xml
<PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-rc1" />
<PackageReference Include="Azure.Identity" Version="1.13.2" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.0" />
```

### ⚠️ 사용 금지 패키지

- **Azure.AI.OpenAI 사용 금지** - Microsoft.Agents.AI.OpenAI 만 사용
- Azure.AI.OpenAI 는 구형 SDK 이므로 사용하지 않음

### OpenAI API 키 방식 인증

Azure.AI.OpenAI 를 사용하지 않으므로 **API 키 방식**만 사용합니다:

```csharp
using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

// 환경 변수에서 API 키와 베이스 URL 읽기
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL") 
    ?? "https://api.openai.com/v1";

// OpenAI 클라이언트 생성 - ApiKeyCredential 과 OpenAIClientOptions 사용
var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
var chatClient = client.GetChatClient("gpt-4o-mini");

// AIAgent 생성
AIAgent agent = chatClient.AsAIAgent(
    instructions: "당신은 친절한 어시스턴트입니다.",
    name: "MyAgent"
);
```

### 환경 변수 설정

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

# 또는 OpenAI 호환 API 사용
$env:OPENAI_BASE_URL="https://your-custom-endpoint.com/v1"
```

### 추가 패키지 (필요시)

```xml
<!-- JSON 처리 -->
<PackageReference Include="System.Text.Json" Version="9.0.0" />

<!-- YAML 처리 -->
<PackageReference Include="YamlDotNet" Version="16.3.0" />

<!-- 설정 관리 -->
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.0" />
```

---

## 🏗️ 표준 프로젝트 구조

```
ProjectName/
├── Program.cs                  # 진입점
├── Agents/                     # Agent 클래스
│   ├── ConfigAgent.cs
│   └── CharacterAgent.cs
├── Tools/                      # Function Tools
│   └── CalculatorTool.cs
├── Data/                       # 데이터 파일
│   ├── config.json
│   └── characters.yaml
├── Models/                     # 데이터 모델
│   └── GameStats.cs
└── Workflows/                  # 워크플로우 (Stage 8+)
    └── QuestWorkflow.cs
```

---

## 🔧 Agent 생성 표준 패턴

### 패턴 1: 기본 Agent

```csharp
using Azure.Identity;
using Microsoft.Agents.AI;

// 환경 변수에서 설정 읽기
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT 를 설정해주세요.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") 
    ?? "gpt-4o-mini";

// 1. Azure OpenAI 클라이언트 생성 (Microsoft.Agents.AI.OpenAI 사용)
var azureClient = new AzureOpenAIClient(
    new Uri(endpoint), 
    new DefaultAzureCredential()
);

// 2. Agent 생성 - AsAIAgent() 확장 메서드 사용
AIAgent agent = azureClient
    .GetChatClient(deploymentName)
    .AsAIAgent(
        instructions: "당신은 친절한 어시스턴트입니다.",
        name: "MyAgent"
    );

// 3. 실행
var response = await agent.RunAsync("안녕하세요?");
Console.WriteLine(response);
```

### 패턴 2: 커스텀 시스템 프롬프트

```csharp
public class GameAgent
{
    private readonly string _systemPrompt;
    
    public GameAgent()
    {
        _systemPrompt = """
            당신은 게임 정보 전문 어시스턴트입니다.
            
            역할:
            1. 게임 설정 파일에서 정보를 찾아 답변합니다
            2. 숫자 값은 정확하게 전달합니다
            3. 모르는 정보는 모른다고 답변합니다
            """;
    }
    
    public string GetSystemPrompt() => _systemPrompt;
}

// 사용
var gameAgent = new GameAgent();
AIAgent agent = azureClient
    .GetChatClient(deploymentName)
    .AsAIAgent(
        instructions: gameAgent.GetSystemPrompt(),
        name: "GameInfoBot"
    );
```

### 패턴 3: Function Tools 와 함께

```csharp
public class GameTools
{
    [Function]
    public int CalculateDps(int damage, int attacksPerSecond)
    {
        return damage * attacksPerSecond;
    }
    
    [Function]
    public double CalculateCritDamage(double baseDamage, double critChance, double critMultiplier)
    {
        return baseDamage * (1 + (critChance * critMultiplier));
    }
}

// Agent 생성 시 Tool 추가
var tools = new GameTools();
AIAgent agent = azureClient
    .GetChatClient(deploymentName)
    .AsAIAgent(
        instructions: "당신은 게임 밸런스 분석가입니다.",
        name: "BalanceBot",
        tools: tools.GetFunctionTools()
    );
```

---

## 💬 Agent 실행 패턴

### 1. 단일 응답 (RunAsync)

```csharp
// 간단한 질문
var response = await agent.RunAsync("최대 레벨이 얼마야?");
Console.WriteLine(response);

// 컨텍스트 포함
var prompt = $"""
    게임 설정에서 다음 질문에 답변해주세요:
    
    질문: {userInput}
    카테고리: {string.Join(", ", categories)}
    """;
var response = await agent.RunAsync(prompt);
```

### 2. 스트리밍 응답 (RunStreamingAsync)

```csharp
Console.Write("에이전트: ");
await foreach (var update in agent.RunStreamingAsync("이야기를 들려줘"))
{
    Console.Write(update);
}
Console.WriteLine();
```

### 3. 대화 루프

```csharp
Console.WriteLine("질문을 입력하세요 (종료: 'quit')");

while (true)
{
    Console.Write("사용자: ");
    var userInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userInput) || 
        userInput.Equals("quit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
    
    try
    {
        var response = await agent.RunAsync(userInput);
        Console.WriteLine($"\n에이전트: {response}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n오류: {ex.Message}\n");
    }
}
```

---

## 🔐 인증 표준

### 1. Azure CLI (개발 환경 - 권장)

```csharp
// 사전 준비: az login
var credential = new DefaultAzureCredential();
var client = new AzureOpenAIClient(new Uri(endpoint), credential);
```

### 2. API 키 (프로덕션)

```csharp
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
var credential = new AzureKeyCredential(apiKey);
var client = new AzureOpenAIClient(new Uri(endpoint), credential);
```

### 3. 환경 변수 설정 (PowerShell)

```powershell
# Azure OpenAI
$env:AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
$env:AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4o-mini"

# Azure CLI 로그인
az login
```

---

## 📊 데이터 처리 패턴

### JSON 파일 읽기

```csharp
using System.Text.Json;

public class ConfigAgent
{
    private readonly Dictionary<string, object> _config;
    
    public ConfigAgent(string jsonPath)
    {
        var json = File.ReadAllText(jsonPath);
        _config = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
            ?? new Dictionary<string, object>();
    }
    
    public object? GetValue(string key)
    {
        var keys = key.Split('.');
        object? current = _config;
        
        foreach (var k in keys)
        {
            if (current is Dictionary<string, object> dict && 
                dict.TryGetValue(k, out var value))
            {
                current = value;
            }
            else
            {
                return null;
            }
        }
        
        return current;
    }
}
```

### YAML 파일 읽기

```csharp
using YamlDotNet.RepresentationModel;

public class CharacterAgent
{
    public CharacterAgent(string yamlPath)
    {
        var yaml = new YamlStream();
        yaml.Load(new StringReader(File.ReadAllText(yamlPath)));
        
        var root = (YamlMappingNode)yaml.Documents[0].RootNode;
        // 파싱 로직 구현
    }
}
```

---

## 🧪 테스트 패턴

### xUnit 단위 테스트

```csharp
using Xunit;

public class GameAgentTests
{
    private readonly GameAgent _agent;
    
    public GameAgentTests()
    {
        _agent = new GameAgent("test_config.json");
    }
    
    [Fact]
    public void GetValue_ReturnsCorrectValue()
    {
        var value = _agent.GetValue("gameSettings.maxLevel");
        Assert.Equal(99, value);
    }
    
    [Fact]
    public void GetTopLevelKeys_ReturnsAllSections()
    {
        var keys = _agent.GetTopLevelKeys().ToList();
        Assert.Contains("combat", keys);
    }
}
```

---

## 🚀 배포 체크리스트

### 환경 설정

- [ ] `AZURE_OPENAI_ENDPOINT` 설정
- [ ] `AZURE_OPENAI_DEPLOYMENT_NAME` 설정
- [ ] Azure CLI 로그인 확인 (`az login`)
- [ ] `Cognitive Services OpenAI Contributor` 권한 확인

### 코드 검증

- [ ] `AIAgent` 인터페이스 사용
- [ ] `AsAIAgent()` 메서드로 생성
- [ ] `RunAsync()` 또는 `RunStreamingAsync()` 실행
- [ ] 예외 처리 구현
- [ ] 시스템 프롬프트 명확성 검증

### 테스트

- [ ] 단위 테스트 실행
- [ ] 통합 테스트 실행
- [ ] 실제 Azure OpenAI 연결 테스트

---

## 📚 학습 로드맵

### Stage 1-3: 기본

| Stage | 주제 | 핵심 개념 |
|-------|------|-----------|
| 1 | 텍스트 응답 | Agent 초기화, RunAsync, 데이터 로드 |
| 2 | 단일 Tool | Function Tools, 파라미터 처리 |
| 3 | 파일 시스템 | 파일 읽기/쓰기, 로그 분석 |

### Stage 4-7: 중급

| Stage | 주제 | 핵심 개념 |
|-------|------|-----------|
| 4 | 다중 Tool | Tool 조합, 우선순위 |
| 5 | 외부 API | REST API, 인증 |
| 6 | 데이터베이스 | SQL 쿼리, NL-to-SQL |
| 7 | RAG | Vector Search, 임베딩 |

### Stage 8-12: 고급

| Stage | 주제 | 핵심 개념 |
|-------|------|-----------|
| 8 | 멀티 에이전트 | Sequential Workflow |
| 9 | 워크플로우 QA | Graph Workflow |
| 10 | Human-in-the-Loop | Checkpointing, Time-travel |
| 11 | 실시간 분석 | 스트리밍, 이벤트 |
| 12 | 멀티모달 | 이미지 처리 |

### Stage 13-15: 전문가

| Stage | 주제 | 핵심 개념 |
|-------|------|-----------|
| 13 | PM 어시스턴트 | 다중 데이터 소스 |
| 14 | 밸런싱 시뮬레이터 | 몬테카를로, 최적화 |
| 15 | 코파일럿 | DevUI, Observability |

---

## 🔗 참조 리소스

### 공식 문서

- **[Microsoft Agent Framework 문서](https://learn.microsoft.com/agent-framework/)**
- **[C# Quick Start](https://learn.microsoft.com/agent-framework/get-started/your-first-agent)**
- **[GitHub Repository](https://github.com/microsoft/agent-framework/tree/main/dotnet)**
- **[C# Samples](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples)**

### 마이그레이션

- [Semantic Kernel 에서 마이그레이션](https://learn.microsoft.com/agent-framework/migration-guide/from-semantic-kernel)
- [AutoGen 에서 마이그레이션](https://learn.microsoft.com/agent-framework/migration-guide/from-autogen)

---

## 💡 프롬프트 사용법

### 이 skill 을 활성화하는 방법

다음 중 하나를 프롬프트에 포함하세요:

1. **"Microsoft Agent Framework 로 만들어줘"**
2. **"Agent Framework 패턴으로 구현해줘"**
3. **"C# Agent Framework skill 사용해서"**
4. **"Stage X 의 프로젝트를 만들어줘"** (X = 1~15)
5. **"AIAgent 를 사용해서..."**

### 예시 프롬프트

```
✅ "Microsoft Agent Framework 를 사용해서 게임 로그 분석 Agent 를 만들어줘"
✅ "Stage 2 학습용으로 Function Tool 을 사용하는 예제를 보여줘"
✅ "AIAgent 인터페이스로 RAG 에이전트를 구현해줘"
✅ "AsAIAgent() 와 RunAsync() 를 사용하는 기본 예제를 만들어줘"
✅ "Azure OpenAI 로 인증하는 Agent 코드를 작성해줘"
```

### 컨텍스트 제공

```
"현재 Stage 1 을 개발 중이야. Microsoft Agent Framework skill 을 사용해서
JSON 파일을 읽고 질문에 답변하는 Agent 를 만들어줘. 
RunAsync() 와 스트리밍 두 가지 방식 모두 보여줘."
```

---

## 🎯 모범 사례

### 1. 시스템 프롬프트 작성

```
❌ 나쁨: "도와주세요"
✅ 좋음: """
    당신은 게임 정보 전문 어시스턴트입니다.
    
    응답 가이드라인:
    1. 질문의 의도를 먼저 파악합니다
    2. 관련 데이터를 정확히 인용합니다
    3. 숫자와 통계는 그대로 전달합니다
    4. 모르는 정보는 추측하지 않습니다
    """
```

### 2. Tool 정의 (단일 책임)

```csharp
// ❌ 나쁨: 너무 많은 로직
[Function]
public async Task<string> ProcessComplexGameLogic(...) { }

// ✅ 좋음: 단일 책임
[Function]
public int CalculateDamage(int attack, int defense) 
    => attack - defense;

[Function]
public double ApplyCrit(double damage, bool isCrit) 
    => isCrit ? damage * 2.0 : damage;
```

### 3. 오류 처리

```csharp
try
{
    var response = await agent.RunAsync(userInput);
    Console.WriteLine(response);
}
catch (Exception ex)
{
    // 구체적 오류 메시지
    Console.WriteLine($"죄송합니다. 오류가 발생했습니다: {ex.Message}");
    
    // 로그 기록 (프로덕션)
    logger.LogError(ex, "Agent execution failed");
}
```

---

## 📝 체크리스트

코드 작성 전 확인:

- [ ] README.md 의 해당 단계 번호 확인
- [ ] Microsoft Agent Framework 공식 SDK 사용
- [ ] 최소한의 종속성만 포함
- [ ] 교육용 주석 충분히 포함
- [ ] 예외 처리 구현
- [ ] 환경 변수 설정 명시
