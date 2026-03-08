# Microsoft Agent Framework 학습 

## 01A_GameConfigBot

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

### 요약
`AIContextProvider`는 Microsoft Agent Framework에서 에이전트에게 **메모리, 상태, 동적 컨텍스트**를 부여하는 핵심 메커니즘입니다. 추상 클래스를 상속받아 두 가지 핵심 생명주기 훅(`invoking`/`invoked`)을 구현함으로써, 개발자는 LLM 호출 전후로 완전한 제어권을 가집니다. 단기 세션 메모리부터 장기 시맨틱 메모리, 사용자 프로파일, 동적 도구 제공까지 모든 컨텍스트 엔지니어링 요구사항을 **결정론적(deterministic)이고 모듈화된 방식**으로 처리할 수 있으며, 직렬화 지원을 통해 상태를 외부 저장소에 영속화할 수 있습니다.  

---  
  
이제 충분한 공식 문서 정보를 수집했습니다. 상세하게 설명해드리겠습니다.

---

### AIContextProvider의 동적 Function Tool 제공 기능

#### 핵심 개념: "정적 도구" vs "동적 도구"의 차이
일반적으로 에이전트에 도구(Function Tool)를 등록하는 방법은 **정적 등록**입니다. 에이전트를 생성하는 시점에 `tools=[...]` 파라미터로 도구 목록을 고정하면, 이후 모든 대화 턴에서 항상 동일한 도구 목록이 LLM에 노출됩니다. 이는 단순한 경우에 잘 작동하지만, 도구 수가 50~100개 이상으로 늘어나거나 사용자 역할·상황에 따라 다른 도구를 제공해야 할 때는 심각한 문제가 됩니다.

`AIContextProvider`의 **동적 도구 제공** 기능은 이 한계를 극복합니다. `ProvideAIContextAsync` (C#) 또는 `invoking` (Python) 메서드에서 반환하는 `AIContext` / `Context` 객체의 **`Tools` 속성**에 도구 목록을 담아 돌려주면, 해당 턴(turn)에만 한정적으로 그 도구들이 LLM에 공급됩니다. 즉, **매 대화 턴마다 다른 도구 목록을 에이전트에 주입**할 수 있습니다.

공식 문서의 `AIContext.Tools` 속성 설명은 이를 명확히 밝힙니다.

> *"These tools are transient and apply only to the current AI model invocation. Any existing tools are provided as input to the AIContextProvider instances, so context providers can choose to modify or replace the existing tools as needed based on the current context."*

---

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

---

#### 3. 동적 도구 제공이 필요한 핵심 시나리오
공식 문서와 GitHub 이슈에서 제시하는 실제 활용 시나리오들을 살펴보면 왜 이 기능이 중요한지 이해할 수 있습니다.

**시나리오 1: 사용자 역할 기반 도구 제어**  
관리자 사용자에게는 데이터 삭제 도구를, 일반 사용자에게는 조회 도구만 보여주는 경우입니다. 메모리에 저장된 사용자 역할 정보를 `InvokingAsync`에서 읽어 조건에 따라 다른 도구 셋을 반환합니다.

**시나리오 2: 대화 흐름에 따른 단계별 도구 활성화**  
결제 에이전트를 예로 들면, 상품 선택 단계에서는 검색 도구만, 결제 확인 단계에서는 결제 처리 도구를, 완료 후에는 영수증 발송 도구를 순차적으로 활성화할 수 있습니다. 이전 대화 내용과 상태를 분석하여 현재 단계에 적합한 도구만 노출합니다.

**시나리오 3: RAG 기반 동적 도구 선택 (ContextualFunctionProvider)**  
이는 GitHub Issue #2630에서 공식적으로 논의되고 있는 가장 고급 시나리오입니다. 도구가 수십~수백 개일 때 모든 도구를 LLM에 전달하면 토큰 비용이 급증하고 모델 성능이 저하됩니다. `ContextualFunctionProvider`는 사용자 메시지를 벡터화하여 현재 대화와 가장 관련성이 높은 상위 N개의 도구만 선별해 제공하는 방식입니다.  

---

#### C# (.NET) 구현 예시
  
**기본 구현: 역할 기반 동적 도구 주입**   

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

---

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

---


#### 6. RAG 기반 동적 도구 선택: ContextualFunctionProvider 패턴
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

---

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

---

#### 8. 정리: 동적 도구 제공의 핵심 가치
`AIContextProvider`를 통한 동적 Function Tool 제공은 단순한 도구 주입 이상의 의미를 갖습니다. 아래 표는 정적 등록과 동적 제공의 차이를 명확히 보여줍니다.

| 비교 항목 | 정적 도구 등록 | AIContextProvider 동적 도구 |
|---|---|---|
| 도구 결정 시점 | 에이전트 생성 시 고정 | 매 턴(turn)마다 결정 |
| 사용자 역할 반영 | 불가 | 가능 |
| 토큰 효율 | 항상 전체 도구 전달 | 필요한 도구만 선택 전달 |
| 대화 흐름 연동 | 불가 | 상태에 따라 단계별 활성화 |
| 보안 제어 | 런타임 제어 불가 | 권한에 따라 실시간 제어 |
| 도구 수 확장성 | 소규모에 적합 | 수백 개 이상도 대응 가능 |

결론적으로 `AIContextProvider`의 동적 도구 제공 기능은 에이전트를 **상황 인식(Context-Aware)**하게 만드는 핵심 메커니즘입니다. 단순히 "어떤 도구가 있는가"를 넘어 "지금 이 대화에서 어떤 도구가 필요한가"를 런타임에 결정할 수 있게 해주며, 이를 통해 토큰 효율성, 보안, 사용자 경험 모두를 동시에 개선할 수 있습니다.