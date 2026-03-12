namespace _13A_SprintPlanner.Services;

/// <summary>
/// 데이터 통합 서비스 (데모)
/// 실제 구현 시 Jira, Git API 연동
/// </summary>
public class DataIntegrationService
{
    /// <summary>
    /// Jira 에서 이슈 가져오기 (데모)
    /// </summary>
    public List<JiraIssue> FetchJiraIssues(string projectId)
    {
        // 데모 데이터
        return new List<JiraIssue>
        {
            new() { Id = "PROJ-001", Title = "로그인 시스템 구현", StoryPoints = 8, Status = "To Do" },
            new() { Id = "PROJ-002", Title = "대시보드 UI 설계", StoryPoints = 5, Status = "To Do" },
            new() { Id = "PROJ-003", Title = "API 엔드포인트 개발", StoryPoints = 13, Status = "In Progress" },
            new() { Id = "PROJ-004", Title = "단위 테스트 작성", StoryPoints = 5, Status = "To Do" },
            new() { Id = "PROJ-005", Title = "성능 최적화", StoryPoints = 8, Status = "Backlog" }
        };
    }

    /// <summary>
    /// Git 커밋 이력 가져오기 (데모)
    /// </summary>
    public List<GitCommit> FetchGitCommits(string repo)
    {
        return new List<GitCommit>
        {
            new() { Hash = "abc123", Author = "dev1", Message = "로그인 기능 추가", Date = DateTime.Now.AddDays(-1) },
            new() { Hash = "def456", Author = "dev2", Message = "UI 버그 수정", Date = DateTime.Now.AddDays(-2) },
            new() { Hash = "ghi789", Author = "dev1", Message = "API 리팩토링", Date = DateTime.Now.AddDays(-3) }
        };
    }
}

public class JiraIssue
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public int StoryPoints { get; set; }
    public string Status { get; set; } = "";
}

public class GitCommit
{
    public string Hash { get; set; } = "";
    public string Author { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime Date { get; set; }
}
