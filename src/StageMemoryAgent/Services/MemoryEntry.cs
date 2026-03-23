namespace StageMemoryAgent.Services;

/// <summary>
/// 기억 항목을 나타내는 데이터 모델
/// </summary>
public class MemoryEntry
{
    /// <summary>
    /// 고유 식별자 (GUID)
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 기억 제목
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 기억 내용
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 생성일시
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 수정일시
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 태그 목록 (# 제외하고 저장)
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// 중요도 (1-5)
    /// </summary>
    public int Importance { get; set; } = 3;

    /// <summary>
    /// 마크다운 형식으로 변환
    /// </summary>
    public string ToMarkdown()
    {
        var tags = string.Join(", ", Tags.Select(t => $"#{t}"));
        var stars = new string('⭐', Importance);

        return $"""
# 기억 항목: {Title}

- **ID**: {Id}
- **생성일**: {CreatedAt:yyyy-MM-dd HH:mm:ss}
- **수정일**: {UpdatedAt:yyyy-MM-dd HH:mm:ss}
- **중요도**: {stars} ({Importance}/5)
- **태그**: {tags}

## 내용
{Content}

---

""";
    }

    /// <summary>
    /// 검색용 텍스트 (태그 + 내용)
    /// </summary>
    public string GetSearchText()
    {
        return $"{Title} {string.Join(" ", Tags)} {Content}";
    }
}
