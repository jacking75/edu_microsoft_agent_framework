# Stage 05: ExternalAPI - 외부 API 연동 학습

## 📋 개요

Stage 05 에서는 AI Agent 가 **외부 REST API 와 연동하는 방법**을 학습합니다. Jira, GitHub, Slack 등의 실제 서비스 API 를 사용하여 인증 처리, HTTP 요청/응답, 자연어를 API 호출로 변환하는 패턴을 이해합니다.

> **주의**: 이 Stage 의 API 도구는 **모의 구현 **(Mock)입니다. 실제 API 호출 대신 예상 응답을 반환합니다. 실제 연동을 위해서는 각 서비스의 API 문서를 참조하여 구현을 수정해야 합니다.

## 🎯 학습 목표

1. **REST API 연동**: HTTP Client 를 사용한 외부 API 호출
2. **인증 처리**: API Key, Bearer Token, Webhook URL 기반 인증
3. **자연어 → API**: 사용자 요청을 API 파라미터로 변환
4. **설정 관리**: 환경 변수 및 설정 클래스를 통한 비밀 정보 관리
5. **에러 핸들링**: API 실패 시 대응 전략

---

## 📁 프로젝트 구성

```
Stage05_ExternalAPI/
├── Stage05.sln
├── 05A_JiraIssueBot/            # Jira 이슈 생성 봇
│   ├── Program.cs
│   ├── Config/
│   │   └── JiraSettings.cs      # Jira 설정 클래스
│   └── Tools/
│       └── JiraApiTool.cs       # Jira API 연동 도구
├── 05B_GitHubPRReviewer/        # GitHub PR 리뷰어
│   ├── Program.cs
│   ├── Config/
│   │   └── GitHubSettings.cs    # GitHub 설정 클래스
│   └── Tools/
│       └── GitHubApiTool.cs     # GitHub API 연동 도구
└── 05C_SlackNotifier/           # Slack 알림 에이전트
    ├── Program.cs
    ├── Config/
    │   └── SlackSettings.cs     # Slack 설정 클래스
    └── Tools/
        └── SlackWebhookTool.cs  # Slack Webhook 도구
```

---

## 🔧 각 프로젝트 설명

### 05A_JiraIssueBot - Jira 이슈 생성 봇

**학습 내용:**
- Jira REST API 연동 패턴
- 자연어 요구사항을 Jira 이슈로 변환
- JQL(Jira Query Language) 검색

**설정 정보 **(JiraSettings)
| 속성 | 설명 | 예시 |
|------|------|------|
| `BaseUrl` | Jira 인스턴스 URL | `https://your-company.atlassian.net` |
| `ApiKey` | Jira API Token | `your-jira-api-token` |
| `ProjectKey` | 프로젝트 키 | `PROJ`, `DEV`, `TEST` |

**사용 가능한 도구**:
| 도구 | 설명 | 파라미터 |
|------|------|----------|
| `CreateIssue` | 새 이슈 생성 | `summary, description, issueType` |
| `SearchIssues` | JQL 로 이슈 검색 | `query: string` |

**이슈 타입**:
- `Story`: 사용자 스토리
- `Task`: 일반 작업
- `Bug`: 버그 수정
- `Epic`: 대형 기능

**예시 요청:**
- "로그인 페이지 개선 이슈를 만들어줘"
- "진행 중인 이슈들을 검색해줘"
- "버그 타입으로 null 참조 에러 이슈 생성해줘"

**실제 API 호출**:
```http
POST /rest/api/3/issue
Host: your-company.atlassian.net
Authorization: Bearer {apiKey}
Content-Type: application/json

{
  "fields": {
    "project": { "key": "PROJ" },
    "summary": "제목",
    "description": "설명",
    "issuetype": { "name": "Task" }
  }
}
```

---

### 05B_GitHubPRReviewer - GitHub PR 리뷰어

**학습 내용:**
- GitHub REST API v3 연동
- Pull Request 정보 조회
- 변경 파일 분석 및 리뷰 코멘트 생성

**설정 정보 **(GitHubSettings)
| 속성 | 설명 | 예시 |
|------|------|------|
| `Token` | GitHub Personal Access Token | `ghp_xxxx...` |
| `Owner` | 저장소 소유자 | `microsoft`, `anomalyco` |
| `Repo` | 저장소 이름 | `agent-framework` |

**사용 가능한 도구**:
| 도구 | 설명 | 파라미터 |
|------|------|----------|
| `GetPullRequest` | PR 정보 조회 | `repo: string, prNumber: int` |
| `GetChangedFiles` | 변경 파일 목록 | `repo: string, prNumber: int` |
| `AddComment` | PR 댓글 추가 | `repo, prNumber, comment` |

**리뷰 가이드라인**:
- 코드 스타일 일관성
- 잠재적 버그 및 예외 처리
- 성능 문제 (불필요한 루프, 메모리 할당)
- 테스트 커버리지
- 네이밍 컨벤션

**예시 요청:**
- "PR #42 리뷰해줘"
- "변경된 파일 목록을 보여줘"
- "코드 스타일 관점에서 리뷰 코멘트를 작성해줘"

**실제 API 호출**:
```http
# PR 정보 조회
GET /repos/{owner}/{repo}/pulls/{number}
Authorization: token {token}

# 변경 파일 목록
GET /repos/{owner}/{repo}/pulls/{number}/files

# 댓글 추가
POST /repos/{owner}/{repo}/issues/{number}/comments
Content-Type: application/json

{
  "body": "리뷰 코멘트"
}
```

---

### 05C_SlackNotifier - Slack 알림 에이전트

**학습 내용:**
- Slack Incoming Webhook 연동
- 알림 레벨에 따른 메시지 형식
- 채널별 타겟팅

**설정 정보 **(SlackSettings)
| 속성 | 설명 | 예시 |
|------|------|------|
| `WebhookUrl` | Slack Webhook URL | `https://hooks.slack.com/services/...` |
| `DefaultChannel` | 기본 채널 | `#dev-alerts`, `#general` |
| `BotUsername` | 봇 사용자 이름 | `Agent Bot`, `CI Bot` |

**사용 가능한 도구**:
| 도구 | 설명 | 파라미터 |
|------|------|----------|
| `SendMessage` | 일반 메시지 전송 | `channel, message, username` |
| `SendNotification` | 레벨별 알림 | `title, message, level` |

**알림 레벨**:
| 레벨 | 이모지 | 용도 |
|------|--------|------|
| `error` | 🔴 | 긴급 에러, 즉시 조치 필요 |
| `warning` | 🟡 | 주의 필요, 모니터링 대상 |
| `success` | 🟢 | 작업 완료, 성공 |
| `info` | 🔵 | 일반 정보 |

**예시 요청:**
- "배포 완료 알림을 #dev-alerts 채널로 보내줘"
- "에러 발생을 알리는 긴급 알림을 전송해줘"
- "빌드 성공 메시지를 #general 에 올려줘"

**실제 API 호출**:
```http
POST /services/YOUR/WEBHOOK/URL
Host: hooks.slack.com
Content-Type: application/json

{
  "channel": "#dev-alerts",
  "username": "Agent Bot",
  "text": "알림 내용",
  "attachments": [
    {
      "color": "good",
      "title": "배포 완료",
      "text": "상세 내용"
    }
  ]
}
```

---

## 🚀 실행 방법

### 1. 환경 변수 설정

```bash
# Windows PowerShell
$env:OPENAI_API_KEY="your-openai-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"
```

### 2. 각 프로젝트 실행

```bash
# 05A_JiraIssueBot 실행
dotnet run --project 05A_JiraIssueBot

# 05B_GitHubPRReviewer 실행
dotnet run --project 05B_GitHubPRReviewer

# 05C_SlackNotifier 실행
dotnet run --project 05C_SlackNotifier
```

---

## 🔐 인증 정보 관리

### 환경 변수 사용 (권장)

```bash
# .env 파일 예시 (실제 프로젝트에 포함하지 않음)
OPENAI_API_KEY=sk-...
JIRA_BASE_URL=https://your-company.atlassian.net
JIRA_API_KEY=your-jira-token
JIRA_PROJECT_KEY=PROJ

GITHUB_TOKEN=ghp_...
GITHUB_OWNER=your-org
GITHUB_REPO=your-repo

SLACK_WEBHOOK_URL=https://hooks.slack.com/services/...
SLACK_DEFAULT_CHANNEL=#dev-alerts
```

### Program.cs 에서 읽기

```csharp
var jiraSettings = new JiraSettings
{
    BaseUrl = Environment.GetEnvironmentVariable("JIRA_BASE_URL") ?? "",
    ApiKey = Environment.GetEnvironmentVariable("JIRA_API_KEY") ?? "",
    ProjectKey = Environment.GetEnvironmentVariable("JIRA_PROJECT_KEY") ?? "PROJ"
};
```

---

## 💡 핵심 개념

### REST API 연동 패턴

```csharp
public class JiraApiTool
{
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public JiraApiTool(string baseUrl, string apiKey)
    {
        _baseUrl = baseUrl;
        _apiKey = apiKey;
    }

    public async Task<string> CreateIssueAsync(string summary, string description)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _apiKey);
        
        var payload = new { fields = new { summary, description } };
        var json = JsonSerializer.Serialize(payload);
        
        var response = await client.PostAsync(
            $"{_baseUrl}/rest/api/3/issue",
            new StringContent(json, Encoding.UTF8, "application/json")
        );
        
        return await response.Content.ReadAsStringAsync();
    }
}
```

### 자연어 → API 파라미터 변환

```
사용자: "로그인 페이지 버그 이슈 만들어줘"
    ↓
[Agent] 의도 분석: "이슈 생성"
    ↓
[파라미터 추출]
  - summary: "로그인 페이지 버그 수정"
  - issueType: "Bug"
  - description: "로그인 페이지에서 발생하는 버그 수정"
    ↓
[Tool 호출] CreateIssue(summary, description, "Bug")
    ↓
[API 응답] PROJ-123 이슈 생성됨
```

### 설정 클래스 패턴

```csharp
public class JiraSettings
{
    public string BaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string ProjectKey { get; set; } = "PROJ";
}

// 사용
var settings = new JiraSettings();
Configuration.Bind("Jira", settings);
var tool = new JiraApiTool(settings.BaseUrl, settings.ApiKey);
```

---

## ⚠️ 실제 구현 시 고려사항

### 1. 보안

- **API 키를 코드에 하드코딩하지 않기**
- 환경 변수 또는 비밀 저장소 (Azure Key Vault, AWS Secrets Manager) 사용
- `.gitignore` 에 `appsettings.local.json`, `.env` 추가

### 2. 에러 처리

```csharp
try
{
    var result = await apiTool.CreateIssueAsync(...);
}
catch (HttpRequestException ex)
{
    logger.LogError($"API 호출 실패: {ex.Message}");
    return "API 서버에 연결할 수 없습니다.";
}
catch (JsonException ex)
{
    logger.LogError($"JSON 파싱 오류: {ex.Message}");
    return "응답 처리 중 오류가 발생했습니다.";
}
```

### 3. 재시도 로직

```csharp
var policy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(retry * 2));

await policy.ExecuteAsync(async () => 
{
    return await apiTool.CreateIssueAsync(...);
});
```

### 4. 로깅

```csharp
logger.LogInformation("Jira 이슈 생성 요청: {Summary}", summary);
logger.LogDebug("API 응답: {Response}", response);
```

---

## 📝 연습 과제

1. **실제 API 연동**: 모의 구현을 실제 API 호출로 변경
2. **비동기 처리**: `async/await` 를 사용한 논블로킹 호출
3. **유효성 검사**: API 호출 전 파라미터 검증
4. **캐싱**: 동일 요청에 대한 응답 캐싱
5. **Rate Limiting**: API 호출 빈도 제한 구현

---

## 🔗 다음 단계

- **Stage 06_Memory**: Agent 에 메모리 기능 추가 (대화 기록 유지)
- **Stage 07_Planning**: 복잡한 작업 계획 수립 (ReAct 패턴)
- **Stage 08_MultiAgent**: 여러 Agent 간 협업

---

## 📚 참고 문서

- [Jira REST API 문서](https://developer.atlassian.com/cloud/jira/platform/rest/v3/)
- [GitHub REST API 문서](https://docs.github.com/en/rest)
- [Slack Incoming Webhooks](https://api.slack.com/messaging/webhooks)
- [Microsoft Agent Framework - Tool 구현](https://learn.microsoft.com/en-us/agent-framework/)
