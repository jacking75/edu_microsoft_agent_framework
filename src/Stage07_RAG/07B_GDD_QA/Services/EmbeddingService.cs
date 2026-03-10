namespace _07B_GDD_QA.Services;

using System.Numerics.Tensors;

/// <summary>
/// 임베딩 서비스 (간단한 벡터 생성)
/// 실제 구현에서는 OpenAI Embedding API 사용
/// </summary>
public class EmbeddingService
{
    /// <summary>
    /// 텍스트를 임베딩 벡터로 변환합니다 (데모용 간단한 해시 기반)
    /// </summary>
    public float[] GenerateEmbedding(string text)
    {
        // 실제 구현: OpenAI Embedding API 호출
        // 데모: 텍스트 기반 해시 벡터 생성 (128 차원)
        const int dimensions = 128;
        var embedding = new float[dimensions];
        
        for (int i = 0; i < text.Length; i++)
        {
            var index = i % dimensions;
            embedding[index] += (float)Math.Sin(text[i] * 0.1);
        }
        
        // 정규화
        var norm = (float)Math.Sqrt(embedding.Sum(x => x * x));
        if (norm > 0)
        {
            for (int i = 0; i < dimensions; i++)
            {
                embedding[i] /= norm;
            }
        }
        
        return embedding;
    }
    
    /// <summary>
    /// 코사인 유사도 계산
    /// </summary>
    public double CosineSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
            return 0;
        
        // TensorPrimitives 사용 (System.Numerics.Tensors)
        return TensorPrimitives.CosineSimilarity(vector1, vector2);
    }
}

/// <summary>
/// 문서 인덱스 항목
/// </summary>
public class DocumentIndex
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";
    public string Source { get; set; } = "";
    public string Category { get; set; } = "";
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public DateTime IndexedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// In-Memory Vector Store (데모용)
/// </summary>
public class VectorStore
{
    private readonly List<DocumentIndex> _documents = new();
    private readonly EmbeddingService _embeddingService = new();
    
    /// <summary>
    /// 문서를 인덱싱합니다
    /// </summary>
    public void AddDocument(string id, string content, string source = "", string category = "")
    {
        var embedding = _embeddingService.GenerateEmbedding(content);
        
        _documents.Add(new DocumentIndex
        {
            Id = id,
            Content = content,
            Source = source,
            Category = category,
            Embedding = embedding
        });
    }
    
    /// <summary>
    /// 유사도 검색 (Top-K)
    /// </summary>
    public List<(DocumentIndex doc, double score)> Search(string query, int topK = 3, string? category = null)
    {
        var queryEmbedding = _embeddingService.GenerateEmbedding(query);
        
        var results = _documents
            .Where(d => category == null || d.Category == category)
            .Select(doc => (
                doc, 
                score: _embeddingService.CosineSimilarity(queryEmbedding, doc.Embedding)
            ))
            .OrderByDescending(r => r.score)
            .Take(topK)
            .ToList();
        
        return results;
    }
    
    /// <summary>
    /// 카테고리별 검색
    /// </summary>
    public List<string> GetCategories()
    {
        return _documents
            .Select(d => d.Category)
            .Distinct()
            .ToList();
    }
    
    /// <summary>
    /// 인덱스된 문서 수
    /// </summary>
    public int Count => _documents.Count;
}
