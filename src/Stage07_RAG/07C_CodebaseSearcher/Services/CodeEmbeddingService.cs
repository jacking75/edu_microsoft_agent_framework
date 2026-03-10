namespace _07C_CodebaseSearcher.Services;

using System.Numerics.Tensors;

/// <summary>
/// 임베딩 서비스 (코드 전용)
/// </summary>
public class CodeEmbeddingService
{
    /// <summary>
    /// 코드를 임베딩 벡터로 변환합니다
    /// </summary>
    public float[] GenerateEmbedding(string code)
    {
        const int dimensions = 256;
        var embedding = new float[dimensions];
        
        // 코드 특성 추출: 길이, 들여쓰기, 키워드 등
        var normalizedCode = code.ToLower().Trim();
        
        // 기본 해시 기반 임베딩
        for (int i = 0; i < code.Length; i++)
        {
            var index = i % dimensions;
            embedding[index] += (float)Math.Sin(code[i] * 0.1);
        }
        
        // C# 키워드 가중치 추가
        var keywords = new[] { "class", "method", "property", "async", "await", "linq", "interface", "namespace" };
        foreach (var keyword in keywords)
        {
            if (normalizedCode.Contains(keyword))
            {
                var keywordIndex = Array.IndexOf(keywords, keyword);
                embedding[keywordIndex + 128] += 1.0f;
            }
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
        
        return TensorPrimitives.CosineSimilarity(vector1, vector2);
    }
}

/// <summary>
/// 코드 스니펫 인덱스 항목
/// </summary>
public class CodeSnippet
{
    public string Id { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string MethodName { get; set; } = "";
    public string Content { get; set; } = "";
    public string Language { get; set; } = "csharp";
    public int LineNumber { get; set; }
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public DateTime IndexedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 코드용 Vector Store
/// </summary>
public class CodeVectorStore
{
    private readonly List<CodeSnippet> _snippets = new();
    private readonly CodeEmbeddingService _embeddingService = new();
    
    /// <summary>
    /// 코드 스니펫 인덱싱
    /// </summary>
    public void AddSnippet(CodeSnippet snippet)
    {
        var embedding = _embeddingService.GenerateEmbedding(snippet.Content);
        snippet.Embedding = embedding;
        _snippets.Add(snippet);
    }
    
    /// <summary>
    /// C# 파일에서 코드 스니펫 추출
    /// </summary>
    public void IndexCSharpFile(string filePath, string content)
    {
        var lines = content.Split('\n');
        var currentClass = "";
        var currentMethod = "";
        var inClass = false;
        var inMethod = false;
        var braceCount = 0;
        var methodStartLine = 0;
        var methodLines = new List<string>();
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            
            // 클래스 발견
            if (line.StartsWith("public class") || line.StartsWith("internal class"))
            {
                var parts = line.Split(' ');
                for (int j = 0; j < parts.Length; j++)
                {
                    if (parts[j] == "class" && j + 1 < parts.Length)
                    {
                        currentClass = parts[j + 1].Replace("{", "").Trim();
                        inClass = true;
                        break;
                    }
                }
            }
            
            // 메서드 발견
            if (inClass && (line.Contains("public ") || line.Contains("private ") || line.Contains("internal ")))
            {
                if (line.Contains("(") && line.Contains(")"))
                {
                    var methodMatch = System.Text.RegularExpressions.Regex.Match(line, @"(\w+)\s*\(");
                    if (methodMatch.Success)
                    {
                        currentMethod = methodMatch.Groups[1].Value;
                        methodStartLine = i + 1;
                        methodLines = new List<string> { line };
                        inMethod = true;
                        braceCount = line.Count(c => c == '{') - line.Count(c => c == '}');
                        continue;
                    }
                }
            }
            
            // 메서드 내용 추적
            if (inMethod)
            {
                methodLines.Add(lines[i]);
                braceCount += line.Count(ch => ch == '{') - line.Count(ch => ch == '}');
                
                if (braceCount <= 0)
                {
                    // 메서드 끝
                    var snippet = new CodeSnippet
                    {
                        Id = $"{Path.GetFileNameWithoutExtension(filePath)}.{currentMethod}_{methodStartLine}",
                        FilePath = filePath,
                        ClassName = currentClass,
                        MethodName = currentMethod,
                        Content = string.Join("\n", methodLines),
                        LineNumber = methodStartLine
                    };
                    AddSnippet(snippet);
                    
                    inMethod = false;
                    methodLines.Clear();
                }
            }
        }
    }
    
    /// <summary>
    /// 유사도 검색
    /// </summary>
    public List<(CodeSnippet snippet, double score)> Search(string query, int topK = 5)
    {
        var queryEmbedding = _embeddingService.GenerateEmbedding(query);
        
        var results = _snippets
            .Select(s => (
                s,
                score: _embeddingService.CosineSimilarity(queryEmbedding, s.Embedding)
            ))
            .OrderByDescending(r => r.score)
            .Take(topK)
            .ToList();
        
        return results;
    }
    
    /// <summary>
    /// 파일명으로 검색
    /// </summary>
    public List<CodeSnippet> SearchByFile(string fileName)
    {
        return _snippets
            .Where(s => s.FilePath.Contains(fileName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    /// <summary>
    /// 클래스명으로 검색
    /// </summary>
    public List<CodeSnippet> SearchByClass(string className)
    {
        return _snippets
            .Where(s => s.ClassName.Contains(className, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    /// <summary>
    /// 인덱스된 스니펫 수
    /// </summary>
    public int Count => _snippets.Count;
    
    /// <summary>
    /// 인덱스된 파일 목록
    /// </summary>
    public List<string> GetFiles()
    {
        return _snippets
            .Select(s => s.FilePath)
            .Distinct()
            .ToList();
    }
}
