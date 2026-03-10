namespace _05B_GitHubPRReviewer.Tools;

/// <summary>
/// GitHub API 연동 Tool (모의 구현)
/// </summary>
public class GitHubApiTool
{
    private readonly string _token;

    public GitHubApiTool(string token)
    {
        _token = token;
    }

    public string GetPullRequest(string repo, int prNumber)
    {
        return $"""
            PR 정보 (모의 응답):
            
            저장소: {repo}
            PR # {prNumber}
            
            제목: "기능 추가: 사용자 인증 시스템"
            작성자: @developer123
            상태: Open
            변경 파일: 15 개
            추가: +1,234 라인
            삭제: -567 라인
            
            실제 API:
            GET https://api.github.com/repos/{repo}/pulls/{prNumber}
            Authorization: token {_token[..10]}...
            """;
    }

    public string GetChangedFiles(string repo, int prNumber)
    {
        return $"""
            변경된 파일 목록 (모의 응답):
            
            1. src/Auth/LoginService.cs (+150, -20)
            2. src/Auth/TokenProvider.cs (+200, -50)
            3. tests/AuthTests.cs (+300, -10)
            ...
            
            실제 API:
            GET https://api.github.com/repos/{repo}/pulls/{prNumber}/files
            """;
    }

    public string AddComment(string repo, int prNumber, string comment)
    {
        return $"""
            ✅ 댓글이 추가되었습니다 (모의 응답):
            
            PR #{prNumber} 에 댓글:
            "{comment[..50]}..."
            
            실제 API:
            POST https://api.github.com/repos/{repo}/issues/{prNumber}/comments
            """;
    }
}
