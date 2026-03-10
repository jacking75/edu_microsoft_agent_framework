namespace _05A_JiraIssueBot.Tools;

/// <summary>
/// Jira API 연동 Tool (모의 구현)
/// </summary>
public class JiraApiTool
{
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public JiraApiTool(string baseUrl, string apiKey)
    {
        _baseUrl = baseUrl;
        _apiKey = apiKey;
    }

    public string CreateIssue(string summary, string description, string issueType = "Task")
    {
        // 실제 API 호출 대신 모의 응답 반환
        return $"""
            ✅ Jira 이슈가 생성되었습니다 (모의 응답):
            
            - 요약: {summary}
            - 타입: {issueType}
            - 설명: {description[..50]}...
            
            실제 구현 시 다음 API 를 사용합니다:
            POST {_baseUrl}/rest/api/3/issue
            Authorization: Bearer {_apiKey[..10]}...
            """;
    }

    public string SearchIssues(string query)
    {
        return $"""
            JQL 검색 결과 (모의 응답):
            
            검색어: {query}
            
            발견된 이슈:
            - PROJ-001: 로그인 페이지 개선 (In Progress)
            - PROJ-002: 성능 최적화 (To Do)
            - PROJ-003: 버그 수정 - null 참조 (Done)
            
            실제 구현 시:
            GET {_baseUrl}/rest/api/3/search?jql={query}
            """;
    }
}
