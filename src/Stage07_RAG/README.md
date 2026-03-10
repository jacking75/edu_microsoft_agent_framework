# Stage 07: RAG (Retrieval-Augmented Generation)

> **학습 목표**: Vector Search 와 RAG 패턴 이해

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 핵심 기술 | 상태 |
|----------|------|-----------|------|
| **07A_DocumentSearchBot** | 기본 문서 검색 | 임베딩, Vector Store, 유사도 검색 | ✅ 완료 |
| **07B_GDD_QA** | 게임 디자인 문서 QA | JSON 문서 인덱싱, 카테고리별 검색 | ✅ 완료 |
| **07C_CodebaseSearcher** | 코드 검색 | 코드 파싱, 메서드 단위 인덱싱 | ✅ 완료 |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 07A 실행 (기본 RAG)
dotnet run --project Stage07_RAG/07A_DocumentSearchBot

# 07B 실행 (GDD QA)
dotnet run --project Stage07_RAG/07B_GDD_QA

# 07C 실행 (코드 검색)
dotnet run --project Stage07_RAG/07C_CodebaseSearcher
```

---

## 🎯 프로젝트별 학습 내용

### 07A_DocumentSearchBot - 기본 문서 검색

임베딩과 Vector Search 의 기본 패턴을 학습합니다.

**핵심 컴포넌트:**
- `EmbeddingService`: 텍스트 → 벡터 변환 (해시 기반 데모 구현)
- `VectorStore`: In-Memory 벡터 인덱스
- `DocumentIndex`: 문서 메타데이터 저장

**RAG 워크플로우:**
1. 사용자 질문 임베딩 생성
2. Vector Store 에서 코사인 유사도 검색 (Top-K)
3. 검색된 문서를 컨텍스트로 LLM 에 전달
4. 출처 기반 답변 생성

---

### 07B_GDD_QA - 게임 디자인 문서 QA

JSON 구조화된 문서를 인덱싱하고 카테고리별 검색을 수행합니다.

**학습 내용:**
- JSON 문서 파싱 및 인덱싱
- 카테고리 메타데이터 활용
- 출처 (섹션 ID) 명시적 표시

**GDD 문서 구조:**
```json
{
  "gameTitle": "Legend of Eternia",
  "sections": [
    {
      "id": "GDD001",
      "category": "Classes",
      "title": "클래스 시스템",
      "content": "총 6 개의 클래스가 있습니다..."
    }
  ]
}
```

**사용 예시:**
```
👤 사용자: 클래스 시스템이 어떻게 돼?
🤖 에이전트: [GDD003] [Classes] 총 6 개의 클래스가 있습니다...
```

---

### 07C_CodebaseSearcher - 코드베이스 검색

소스 코드를 파싱하여 메서드/클래스 단위로 인덱싱합니다.

**학습 내용:**
- C# 코드 파싱 (클래스/메서드 추출)
- 코드 키워드 기반 임베딩
- 파일 위치 및 줄번호 추적

**코드 스니펫 구조:**
```csharp
public class CodeSnippet
{
    public string Id { get; set; }
    public string FilePath { get; set; }
    public string ClassName { get; set; }
    public string MethodName { get; set; }
    public string Content { get; set; }
    public int LineNumber { get; set; }
}
```

**사용 예시:**
```
👤 사용자: 인벤토리에 아이템 추가하는 코드가 어디 있어?
🤖 에이전트: 
📍 [InventoryService.cs:15]
클래스: InventoryService.AddItem

public bool AddItem(Item item) { ... }
```

---

## 💡 RAG 아키텍처

```csharp
public class EmbeddingService
{
    // 텍스트를 벡터로 변환
    public float[] GenerateEmbedding(string text)
    {
        // 실제 구현: OpenAI Embedding API
        // 데모: 해시 기반 벡터 생성
    }
    
    // 코사인 유사도 계산
    public double CosineSimilarity(float[] v1, float[] v2)
    {
        return TensorPrimitives.CosineSimilarity(v1, v2);
    }
}
```

### 2. Vector Store 구현

```csharp
public class VectorStore
{
    private readonly List<DocumentIndex> _documents = new();
    
    // 문서 인덱싱
    public void AddDocument(string id, string content, string source)
    {
        var embedding = _embeddingService.GenerateEmbedding(content);
        _documents.Add(new DocumentIndex { Id = id, Content = content, Embedding = embedding });
    }
    
    // 유사도 검색 (Top-K)
    public List<(DocumentIndex doc, double score)> Search(string query, int topK = 3)
    {
        var queryEmbedding = _embeddingService.GenerateEmbedding(query);
        return _documents
            .Select(doc => (doc, score: CosineSimilarity(queryEmbedding, doc.Embedding)))
            .OrderByDescending(r => r.score)
            .Take(topK)
            .ToList();
    }
}
```

### 3. RAG 워크플로우

```
1. 사용자 질문 입력
        ↓
2. 질문 임베딩 생성
        ↓
3. Vector Store 에서 유사 문서 검색
        ↓
4. 검색된 문서를 컨텍스트로 추가
        ↓
5. LLM 이 컨텍스트 기반 답변 생성
        ↓
6. 응답 반환 (출처 포함)
```

---

## 📊 RAG 아키텍처

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐
│   사용자    │────▶│  임베딩 생성  │────▶│ Vector Store│
│   질문      │     │  (Query)     │     │   검색      │
└─────────────┘     └──────────────┘     └─────────────┘
                                              │
                                              ▼
┌─────────────┐     ┌──────────────┐     ┌─────────────┐
│   응답      │◀────│  LLM 생성    │◀────│   검색된    │
│   반환      │     │  (RAG)       │     │   문서      │
└─────────────┘     └──────────────┘     └─────────────┘
```

---

## 💡 실행 예시

```
👤 사용자: 최대 레벨이 얼마야?

🔍 검색된 문서 (3 개):
  - [게임 가이드] 게임의 최대 레벨은 99 입니다. 99 레벨에 도달하... (유사도: 0.892)
  - [게임 가이드] 파티는 최대 4 명까지 구성할 수 있습니다... (유사도: 0.654)
  - [상점 가이드] 유료 아이템은 의상, 탈것, 펫... (유사도: 0.432)

🤖 에이전트: 게임의 최대 레벨은 99 입니다. 99 레벨에 도달하면 
             추가 경험치는 명성 포인트로 전환됩니다.
```

---

## ✅ 완료된 Stage

- [x] Stage 01: 텍스트 응답 (3 프로젝트)
- [x] Stage 02: 단일 Tool (3 프로젝트)
- [x] Stage 03: 파일 시스템 (3 프로젝트)
- [x] Stage 04: 다중 Tool (3 프로젝트)
- [x] Stage 05: 외부 API (3 프로젝트)
- [x] Stage 06: 데이터베이스 (3 프로젝트)
- [x] **Stage 07: RAG (3 프로젝트)**

총 **21 개 프로젝트** 완성! 🎉

---

## 🔗 관련 문서

- **[RAG 패턴 설명](https://learn.microsoft.com/agent-framework/concepts/rag)**
- **[임베딩 가이드](https://learn.microsoft.com/azure/ai-services/openai/how-to/embeddings)**
- **[Vector Search](https://learn.microsoft.com/azure/search/vector-search-overview)**
