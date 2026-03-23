using System.Text.RegularExpressions;

namespace StageMemoryAgent.Services;

/// <summary>
/// 마크다운 파일 기반 기억 저장소
/// </summary>
public class MemoryStore
{
    private readonly string _memoryDir;
    private readonly List<MemoryEntry> _memories = new();
    private readonly object _lock = new();

    /// <summary>
    /// MemoryStore 생성자
    /// </summary>
    /// <param name="memoryDir">기억 파일 디렉토리</param>
    public MemoryStore(string memoryDir)
    {
        _memoryDir = memoryDir;
        if (!Directory.Exists(_memoryDir))
        {
            Directory.CreateDirectory(_memoryDir);
        }
    }

    /// <summary>
    /// 모든 기억 파일 로드
    /// </summary>
    public void LoadAllMemories()
    {
        lock (_lock)
        {
            _memories.Clear();

            var mdFiles = Directory.GetFiles(_memoryDir, "*.md");
            foreach (var file in mdFiles)
            {
                var entries = ParseMarkdownFile(file);
                _memories.AddRange(entries);
            }

            Console.WriteLine($"📚 총 {_memories.Count}개의 기억을 로드했습니다.");
        }
    }

    /// <summary>
    /// 마크다운 파일 파싱
    /// </summary>
    private List<MemoryEntry> ParseMarkdownFile(string filePath)
    {
        var entries = new List<MemoryEntry>();
        var content = File.ReadAllText(filePath);

        var sections = Regex.Split(content, @"(?=# 기억 항목:)");

        foreach (var section in sections)
        {
            if (string.IsNullOrWhiteSpace(section) || !section.Contains("# 기억 항목:"))
                continue;

            var entry = ParseMemoryEntry(section);
            if (entry != null)
            {
                entries.Add(entry);
            }
        }

        return entries;
    }

    /// <summary>
    /// 마크다운 섹션에서 MemoryEntry 파싱
    /// </summary>
    private MemoryEntry? ParseMemoryEntry(string section)
    {
        try
        {
            var titleMatch = Regex.Match(section, @"# 기억 항목:\s*(.+)");
            var idMatch = Regex.Match(section, @"\*\*ID\*\*:\s*(.+)");
            var createdAtMatch = Regex.Match(section, @"\*\*생성일\*\*:\s*(.+)");
            var updatedAtMatch = Regex.Match(section, @"\*\*수정일\*\*:\s*(.+)");
            var importanceMatch = Regex.Match(section, @"\*\*중요도\*\*:\s*⭐+\s*\((\d+)/5\)");
            var tagsMatch = Regex.Match(section, @"\*\*태그\*\*:\s*(.+)");
            var contentMatch = Regex.Match(section, @"## 내용\s+(.+?)(?=---|$)", RegexOptions.Singleline);

            if (!titleMatch.Success) return null;

            var entry = new MemoryEntry
            {
                Title = titleMatch.Groups[1].Value.Trim(),
                Id = idMatch.Success ? idMatch.Groups[1].Value.Trim() : Guid.NewGuid().ToString(),
                CreatedAt = ParseDateTime(createdAtMatch),
                UpdatedAt = ParseDateTime(updatedAtMatch),
                Importance = importanceMatch.Success ? int.Parse(importanceMatch.Groups[1].Value) : 3,
                Tags = ParseTags(tagsMatch),
                Content = contentMatch.Success ? contentMatch.Groups[1].Value.Trim() : string.Empty
            };

            return entry;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  기억 파싱 오류: {ex.Message}");
            return null;
        }
    }

    private DateTime ParseDateTime(Match match)
    {
        if (match.Success && DateTime.TryParse(match.Groups[1].Value.Trim(), out var result))
        {
            return result;
        }
        return DateTime.Now;
    }

    private List<string> ParseTags(Match match)
    {
        if (!match.Success) return new List<string>();

        var tagsText = match.Groups[1].Value.Trim();
        return tagsText
            .Split(',')
            .Select(t => t.Trim().TrimStart('#'))
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList();
    }

    /// <summary>
    /// 기억 항목 저장
    /// </summary>
    public void SaveMemory(MemoryEntry entry)
    {
        lock (_lock)
        {
            var existing = _memories.FirstOrDefault(m => m.Id == entry.Id);
            if (existing != null)
            {
                _memories.Remove(existing);
            }

            entry.UpdatedAt = DateTime.Now;
            _memories.Add(entry);

            AppendToFile(entry, "long_term.md");
            Console.WriteLine($"💾 기억을 저장했습니다: {entry.Title}");
        }
    }

    /// <summary>
    /// 파일에 기억 추가
    /// </summary>
    private void AppendToFile(MemoryEntry entry, string filename)
    {
        var filePath = Path.Combine(_memoryDir, filename);
        var markdown = entry.ToMarkdown();
        File.AppendAllText(filePath, markdown);
    }

    /// <summary>
    /// 키워드로 기억 검색
    /// </summary>
    public List<MemoryEntry> FindMemories(string keyword)
    {
        lock (_lock)
        {
            var lowerKeyword = keyword.ToLower();
            return _memories
                .Where(m => m.GetSearchText().ToLower().Contains(lowerKeyword))
                .OrderByDescending(m => m.Importance)
                .ThenByDescending(m => m.UpdatedAt)
                .ToList();
        }
    }

    /// <summary>
    /// 태그로 기억 필터링
    /// </summary>
    public List<MemoryEntry> GetMemoriesByTags(List<string> tags)
    {
        lock (_lock)
        {
            return _memories
                .Where(m => tags.Any(t => m.Tags.Contains(t.ToLower())))
                .OrderByDescending(m => m.Importance)
                .ToList();
        }
    }

    /// <summary>
    /// 모든 기억 반환
    /// </summary>
    public List<MemoryEntry> GetAllMemories()
    {
        lock (_lock)
        {
            return _memories.ToList();
        }
    }

    /// <summary>
    /// 중요도 기반 기억 조회
    /// </summary>
    public List<MemoryEntry> GetImportantMemories(int minImportance = 4)
    {
        lock (_lock)
        {
            return _memories
                .Where(m => m.Importance >= minImportance)
                .OrderByDescending(m => m.Importance)
                .ToList();
        }
    }

    /// <summary>
    /// 세션 파일로 저장
    /// </summary>
    public void SaveToSessionFile(string sessionName, List<MemoryEntry> entries)
    {
        lock (_lock)
        {
            var filePath = Path.Combine(_memoryDir, $"{sessionName}.md");
            var content = string.Join("\n", entries.Select(e => e.ToMarkdown()));
            File.WriteAllText(filePath, content);
            Console.WriteLine($"📁 세션 파일을 저장했습니다: {sessionName}.md");
        }
    }

    /// <summary>
    /// 기억 삭제
    /// </summary>
    public bool DeleteMemory(string id)
    {
        lock (_lock)
        {
            var entry = _memories.FirstOrDefault(m => m.Id == id);
            if (entry != null)
            {
                _memories.Remove(entry);
                Console.WriteLine($"🗑️  기억을 삭제했습니다: {entry.Title}");
                return true;
            }
            return false;
        }
    }
}
