# Stage 16: 에이전트 기억 기능

> 마크다운 파일을 사용하여 에이전트가 이전 세션의 정보를 기억하는 기능을 구현합니다.

## 🎯 학습 목표

- ✅ 에이전트에 지속적 기억 기능 추가하기
- ✅ 마크다운 파일을 통한 지식 저장 및 로드
- ✅ 자동 저장 메커니즘 구현하기
- ✅ 기억 검색 및 관리 기능 이해하기

---

## 📚 핵심 개념

### 1. 단기기억 vs 장기기억

| 구분 | 설명 | 파일 예시 |
|------|------|-----------|
| **단기기억** | 현재 세션에서의 대화 컨텍스트 | `session_001.md` |
| **장기기억** | 영구적으로 유지할 중요 정보 | `long_term.md` |

### 2. 컨텍스트 윈도우

LLM 은 한 번에 처리할 수 있는 토큰 수에 제한이 있습니다. 따라서 모든 기억을 매번 주입할 수 없습니다. 이 프로젝트에서는 다음과 같은 전략을 사용합니다:

- **중요도 기반 필터링**: 중요도가 높은 기억 (4-5) 을 우선적으로 주입
- **전체 기억 요약**: 모든 기억을 간단히 요약하여 컨텍스트에 포함
- **검색 기반 로드**: 사용자 질문과 관련된 기억만 동적으로 로드

### 3. 자동 저장 메커니즘

- 5 회 대화마다 LLM 이 중요 정보 추출
- 사용자 이름, 선호도, 결정 사항 등을 자동 저장
- 마크다운 파일에 실시간 추가

---

## 💻 코드 구조

### 프로젝트 구조

```
StageMemoryAgent/
├── StageMemoryAgent.sln          # 솔루션 파일
├── StageMemoryAgent.csproj       # 프로젝트 파일
├── Program.cs                    # 진입점 및 대화 루프
├── Agents/
│   └── MemoryAgent.cs            # 기억 관리 에이전트 클래스
├── Services/
│   ├── MemoryEntry.cs            # 기억 항목 데이터 모델
│   └── MemoryStore.cs            # 마크다운 파일 기반 저장소
├── memory/                       # 기억 파일 디렉토리
│   ├── session_001.md            # 세션별 기억 (단기)
│   └── long_term.md              # 통합 기억 (장기)
└── README.md                     # 이 문서
```

---

## 🔍 클래스 상세 설명

### 1. `MemoryEntry.cs` - 기억 항목 데이터 모델

**역할**: 하나의 기억 항목을 표현하는 데이터 클래스

**주요 속성**:

```csharp
public class MemoryEntry
{
    public string Id { get; set; }           // 고유 식별자 (GUID)
    public string Title { get; set; }        // 기억 제목
    public string Content { get; set; }      // 기억 내용
    public DateTime CreatedAt { get; set; }  // 생성일시
    public DateTime UpdatedAt { get; set; }  // 수정일시
    public List<string> Tags { get; set; }   // 태그 목록
    public int Importance { get; set; }      // 중요도 (1-5)
}
```

**핵심 메서드**:

```csharp
// 마크다운 형식으로 변환
public string ToMarkdown()
{
    var tags = string.Join(", ", Tags.Select(t => $"#{t}"));
    var stars = new string('⭐', Importance);
    
    return $"""
# 기억 항목: {Title}
- **ID**: {Id}
- **생성일**: {CreatedAt:yyyy-MM-dd HH:mm:ss}
- **중요도**: {stars} ({Importance}/5)
- **태그**: {tags}

## 내용
{Content}
---
""";
}

// 검색용 텍스트 생성 (태그 + 내용)
public string GetSearchText()
{
    return $"{Title} {string.Join(" ", Tags)} {Content}";
}
```

**포인트**:
- `ToMarkdown()` 메서드는 객체를 마크다운 형식으로 직렬화
- `GetSearchText()` 는 검색 성능을 위해 인덱스용 텍스트 생성

---

### 2. `MemoryStore.cs` - 마크다운 파일 기반 저장소

**역할**: 마크다운 파일의 읽기/쓰기 및 검색 담당

**주요 메서드**:

#### 2.1 기억 로드

```csharp
public void LoadAllMemories()
{
    lock (_lock)
    {
        _memories.Clear();

        // memory/ 디렉토리의 모든 .md 파일 읽기
        var mdFiles = Directory.GetFiles(_memoryDir, "*.md");
        foreach (var file in mdFiles)
        {
            var entries = ParseMarkdownFile(file);
            _memories.AddRange(entries);
        }

        Console.WriteLine($"📚 총 {_memories.Count}개의 기억을 로드했습니다.");
    }
}
```

**동작 과정**:
1. `memory/` 디렉토리에서 모든 `.md` 파일 찾기
2. 각 파일을 파싱하여 `MemoryEntry` 객체로 변환
3. 인메모리 리스트에 저장

#### 2.2 마크다운 파싱

```csharp
private List<MemoryEntry> ParseMarkdownFile(string filePath)
{
    var content = File.ReadAllText(filePath);
    
    // "# 기억 항목:"으로 섹션 분리
    var sections = Regex.Split(content, @"(?=# 기억 항목:)");
    
    foreach (var section in sections)
    {
        if (string.IsNullOrWhiteSpace(section)) continue;
        
        var entry = ParseMemoryEntry(section);
        if (entry != null)
        {
            entries.Add(entry);
        }
    }

    return entries;
}
```

**파싱 대상**:
- 제목: `# 기억 항목: [제목]`
- ID: `**ID**: [값]`
- 생성일: `**생성일**: [값]`
- 중요도: `**중요도**: ⭐⭐⭐ (3/5)`
- 태그: `**태그**: #태그 1, #태그 2`
- 내용: `## 내용` 섹션의 텍스트

#### 2.3 기억 저장

```csharp
public void SaveMemory(MemoryEntry entry)
{
    lock (_lock)
    {
        // 기존 기억 업데이트 또는 신규 추가
        var existing = _memories.FirstOrDefault(m => m.Id == entry.Id);
        if (existing != null)
        {
            _memories.Remove(existing);
        }
        
        entry.UpdatedAt = DateTime.Now;
        _memories.Add(entry);

        // long_term.md 에 추가
        AppendToFile(entry, "long_term.md");
    }
}
```

#### 2.4 기억 검색

```csharp
public List<MemoryEntry> FindMemories(string keyword)
{
    lock (_lock)
    {
        var lowerKeyword = keyword.ToLower();
        return _memories
            .Where(m => m.GetSearchText().ToLower().Contains(lowerKeyword))
            .OrderByDescending(m => m.Importance)  // 중요도 높은 순
            .ThenByDescending(m => m.UpdatedAt)    // 최근 수정 순
            .ToList();
    }
}
```

**검색 특징**:
- 제목, 태그, 내용 전체에서 검색
- 중요도가 높은 기억을 우선 반환
- 최신 정보가 앞에 오도록 정렬

---

### 3. `MemoryAgent.cs` - 기억 관리 에이전트

**역할**: LLM 과 연동하여 기억을 관리하고 대화에 활용

#### 3.1 초기화

```csharp
public void Initialize()
{
    _memoryStore.LoadAllMemories();  // 파일에서 기억 로드
    _systemPrompt = BuildSystemPrompt();  // 시스템 프롬프트 빌드
    _conversationHistory.Clear();
    _conversationCount = 0;
}
```

#### 3.2 시스템 프롬프트 빌드

```csharp
private string BuildSystemPrompt()
{
    var memories = _memoryStore.GetAllMemories();
    var importantMemories = _memoryStore.GetImportantMemories(4);

    var memoryContext = string.Join("\n\n", 
        memories.Select((m, i) => 
            $"[{i + 1}] {m.Title} (태그: {string.Join(", ", m.Tags)})\n    {m.Content}"));

    return $"""
        당신은 기억 기능을 가진 친절한 어시스턴트입니다.
        
        ## 당신의 기억 정보
        
        ### 전체 기억 ({memories.Count}개)
        {memoryContext}
        
        ### 중요 기억 (우선 참조)
        {string.Join("\n", importantMemories.Select(m => $"- {m.Title}: {m.Content}"))}
        
        ## 응답 가이드라인
        1. **기억 활용**: 이전 대화에서 언급된 내용을 기억하고 자연스럽게 참조하세요.
        2. **기억 언급**: "이전에 말씀해주셨는데...", "제 기억에..." 등으로 기억을 언급하세요.
        3. **정직한 답변**: 기억에 없는 정보는 모른다고 정직하게 답변하세요.
        """;
}
```

**포인트**:
- 전체 기억을 시스템 프롬프트에 주입
- 중요 기억은 별도로 강조하여 우선 참조
- 자연스러운 기억 활용을 위한 가이드라인 포함

#### 3.3 대화 처리

```csharp
public async Task<string> RunAsync(string userInput)
{
    _conversationCount++;

    // 메시지 구성: 시스템 프롬프트 + 대화 히스토리 + 사용자 입력
    var messages = new List<ChatMessage>
    {
        new SystemChatMessage(_systemPrompt)
    };
    messages.AddRange(_conversationHistory);
    messages.Add(new UserChatMessage(userInput));

    // LLM 응답 생성
    var response = await _chatClient.CompleteChatAsync(messages);
    var responseText = response.Value.Content[0].Text;

    // 대화 히스토리에 추가
    _conversationHistory.Add(new UserChatMessage(userInput));
    _conversationHistory.Add(new AssistantChatMessage(responseText));

    // 5 회 대화마다 자동 저장
    if (_conversationCount % AutoSaveInterval == 0)
    {
        await AutoSaveImportantInfoAsync();
    }

    return responseText;
}
```

#### 3.4 자동 저장 (핵심 기능)

```csharp
private async Task AutoSaveImportantInfoAsync()
{
    // 최근 20 개 대화 추출
    var historyText = string.Join("\n", 
        _conversationHistory.TakeLast(20).Select(m => 
            $"{(m is UserChatMessage ? "사용자" : "어시스턴트")}: {m.Content[0].Text}"));

    // LLM 에게 중요 정보 추출 요청
    var extractionPrompt = $"""
        다음 대화에서 기억할 만한 중요 정보를 추출해주세요.
        
        [대화 내역]
        {historyText}
        
        [추출할 정보]
        1. 사용자의 이름이나 개인 정보
        2. 선호도 (색음식, 취향 등)
        3. 중요한 결정 사항
        4. 새로 알게 된 사실
        
        [출력 형식]
        - 제목: [짧은 제목]
        - 내용: [내용]
        - 태그: [태그 1, 태그 2]
        - 중요도: [1-5]
        """;

    var extractionResponse = await _chatClient.CompleteChatAsync(
        new SystemChatMessage("당신은 정보 추출 전문가입니다."),
        new UserChatMessage(extractionPrompt));

    // 추출된 텍스트를 MemoryEntry 로 파싱
    var extractedText = extractionResponse.Value.Content[0].Text;
    var entries = ParseExtractedMemories(extractedText);
    
    // 저장소 저장
    foreach (var entry in entries)
    {
        _memoryStore.SaveMemory(entry);
    }
}
```

**자동 저장 프로세스**:
1. 최근 대화 히스토리 수집
2. LLM 에게 중요 정보 추출 요청
3. 파싱하여 `MemoryEntry` 객체 생성
4. `MemoryStore` 를 통해 파일에 저장

**추출 대상 정보**:
- 사용자 이름, 직업, 개인 정보
- 선호도 (음식, 색, 취향)
- 프로젝트 관련 결정 사항
- 새로 학습한 정보

---

### 4. `Program.cs` - 메인 실행 코드

#### 4.1 초기화

```csharp
static async Task Main(string[] args)
{
    var memoryDir = Path.Combine(AppContext.BaseDirectory, "memory");
    var memoryStore = new MemoryStore(memoryDir);
    var memoryAgent = new MemoryAgent(memoryStore);

    memoryAgent.Initialize();  // 기억 로드 및 초기화

    PrintHelp();  // 도움말 표시

    // 대화 루프
    while (true)
    {
        var userInput = Console.ReadLine();
        var command = userInput.Trim().ToLower();

        // 명령어 처리
        if (command == "quit") break;
        if (command == "help") { PrintHelp(); continue; }
        if (command.StartsWith("search ")) { SearchMemories(...); continue; }
        if (command == "memories") { ShowAllMemories(...); continue; }

        // 일반 대화
        var response = await memoryAgent.RunAsync(userInput);
        Console.WriteLine(response);
    }
}
```

#### 4.2 제공 명령어

| 명령어 | 설명 |
|--------|------|
| `quit`, `exit`, `q` | 프로그램 종료 |
| `help`, `h` | 도움말 표시 |
| `search [키워드]` | 기억 검색 |
| `save` | 수동으로 기억 저장 |
| `memories` | 모든 기억 표시 |
| `clear` | 대화 히스토리 초기화 |

---

## 📝 마크다운 파일 형식

### 예시: `long_term.md`

```markdown
# 기억 항목: 사용자 이름

- **ID**: mem_001
- **생성일**: 2026-03-19 10:00:00
- **수정일**: 2026-03-19 10:00:00
- **중요도**: ⭐⭐⭐⭐⭐ (5/5)
- **태그**: #사용자 #이름 #개인정보

## 내용
사용자의 이름은 "김민수"입니다. 개발자이며 게임 개발에 관심이 많습니다.

---

# 기억 항목: 선호하는 프로그래밍 언어

- **ID**: mem_002
- **생성일**: 2026-03-19 10:05:00
- **중요도**: ⭐⭐⭐⭐ (4/5)
- **태그**: #선호도 #프로그래밍 #언어

## 내용
사용자는 C# 을 선호합니다. .NET 기술 스택을 주로 사용하며, Microsoft Agent Framework 를 학습 중입니다.

---
```

**형식 규칙**:
- 각 기억은 `# 기억 항목: [제목]`으로 시작
- 메타데이터는 `- **키**: 값` 형식
- 내용은 `## 내용` 섹션에 작성
- 구분선은 `---` 사용

---

## 🚀 실행 방법

### 1. 환경 변수 설정

```powershell
# Windows PowerShell
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"
```

### 2. 빌드

```bash
cd src/StageMemoryAgent
dotnet build
```

### 3. 실행

```bash
dotnet run
```

### 4. 대화 예시

```
🧠 기억 기능을 가진 에이전트
=============================

📚 총 5 개의 기억을 로드했습니다.
✅ MemoryAgent 가 초기화되었습니다.

👤 사용자: 내 이름이 뭐였지?

🤖 에이전트: 네 이름은 김민수 님입니다. 개발자시라고 알려주셨어요.

👤 사용자: 내가 좋아하는 프로그래밍 언어는?

🤖 에이전트: C# 을 좋아하신다고 말씀해주셨어요. .NET 기술 스택을 주로 사용하신다고 했죠.

👤 사용자: search 게임

🔍 '게임' 검색 결과 (2 개):
  📌 프로젝트 목표
     태그: 프로젝트, 목표, 게임개발
     중요도: ⭐⭐⭐⭐⭐
     내용: Microsoft Agent Framework 를 활용한 게임 개발 어시스턴트를 만들 계획입니다.

👤 사용자: quit

👋 안녕히 가세요!
```

---

## 🔧 확장 아이디어

### 1. 태그 기반 검색 강화

```csharp
// 태그로 필터링
public List<MemoryEntry> GetMemoriesByTags(List<string> tags)
{
    return _memories
        .Where(m => tags.Any(t => m.Tags.Contains(t.ToLower())))
        .OrderByDescending(m => m.Importance)
        .ToList();
}
```

### 2. 기억 삭제/수정 기능

```csharp
// 기억 삭제
public bool DeleteMemory(string id)
{
    var entry = _memories.FirstOrDefault(m => m.Id == id);
    if (entry != null)
    {
        _memories.Remove(entry);
        return true;
    }
    return false;
}
```

### 3. 백업 시스템

```csharp
// 자동 백업
public void BackupMemories(string backupDir)
{
    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    var backupPath = Path.Combine(backupDir, $"memory_backup_{timestamp}");
    Directory.CreateDirectory(backupPath);
    
    foreach (var file in Directory.GetFiles(_memoryDir, "*.md"))
    {
        File.Copy(file, Path.Combine(backupPath, Path.GetFileName(file)));
    }
}
```

### 4. 중요도 자동 조정

- 오래된 기억은 중요도 감소
- 자주 참조되는 기억은 중요도 증가
- 사용자가 명시한 중요도는 우선

---

## 💡 활용 시나리오

### 1. 개인 비서 에이전트

- 사용자의 일정, 선호도, 습관 기억
- 맞춤형 추천 제공

### 2. 고객 지원 봇

- 이전 문의 내역 기억
- 연속적인 지원 가능

### 3. 학습 도우미

- 학습进度, 약점, 관심 분야 기억
- 맞춤형 학습 계획 제안

### 4. 게임 NPC

- 플레이어와의 상호작용 기억
- 관계도 시스템 구현

---

## 📊 성능 최적화 팁

1. **기억 파일 분리**
   - 세션별 파일 + 통합 파일 사용
   - 파일 크기 제한 (100 개 항목 이하)

2. **지연 로드**
   - 초기화 시 메타데이터만 로드
   - 검색 시 상세 내용 로드

3. **인덱싱**
   - 검색용 인덱스 파일 생성
   - 태그별 그룹핑

---

## ⚠️ 주의사항

1. **컨텍스트 윈도우 제한**
   - 너무 많은 기억을 한 번에 주입하지 않기
   - 중요도 기반 필터링 필수

2. **파일 동시 접근**
   - `lock` 을 사용한 스레드 안전성 확보
   - 파일 쓰기 시 충돌 주의

3. **개인정보 보호**
   - 민감한 정보는 암호화 고려
   .gitignore 에 memory/ 디렉토리 추가 권장

---

## 📚 더 학습하기

- **Stage 07 RAG**: 벡터 기반 기억 검색
- **Stage 10 Human-in-the-Loop**: 기억 수동 수정
- **Stage 13 PM 어시스턴트**: 장기 기억 활용 사례

---

## ✅ 체크리스트

- [x] `MemoryEntry` 데이터 모델 구현
- [x] `MemoryStore` 파일 입출력 구현
- [x] `MemoryAgent` LLM 연동 구현
- [x] 자동 저장 메커니즘 구현
- [x] 검색 기능 구현
- [x] 샘플 기억 파일 제공
- [x] 상세 설명 문서 작성

---

**완성!** 🎉

이제 에이전트가 이전 세션의 정보를 기억하고 활용할 수 있습니다.
