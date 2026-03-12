namespace _15C_FullDevelopmentCopilot.Services;

/// <summary>
/// 개발 워크플로우 서비스
/// </summary>
public class DevelopmentWorkflowService
{
    private readonly List<DevelopmentStage> _stages = new();
    private int _currentStage = 0;

    /// <summary>
    /// 개발 워크플로우 시작
    /// </summary>
    public void StartWorkflow(string featureName)
    {
        _stages.Clear();
        _currentStage = 0;

        _stages.Add(new DevelopmentStage { Name = "요구사항 분석", Status = "Pending" });
        _stages.Add(new DevelopmentStage { Name = "코드 생성", Status = "Pending" });
        _stages.Add(new DevelopmentStage { Name = "테스트 생성", Status = "Pending" });
        _stages.Add(new DevelopmentStage { Name = "코드 리뷰", Status = "Pending" });
        _stages.Add(new DevelopmentStage { Name = "문서 생성", Status = "Pending" });
        _stages.Add(new DevelopmentStage { Name = "배포 준비", Status = "Pending" });
    }

    /// <summary>
    /// 다음 스테이지로 이동
    /// </summary>
    public string NextStage()
    {
        if (_currentStage < _stages.Count)
        {
            _stages[_currentStage].Status = "Completed";
            _currentStage++;
            
            if (_currentStage < _stages.Count)
            {
                _stages[_currentStage].Status = "In Progress";
                return _stages[_currentStage].Name;
            }
        }
        return "Completed";
    }

    /// <summary>
    /// 현재 스테이지 가져오기
    /// </summary>
    public DevelopmentStage? GetCurrentStage()
    {
        if (_currentStage < _stages.Count)
        {
            return _stages[_currentStage];
        }
        return null;
    }

    /// <summary>
    /// 전체 워크플로우 표시
    /// </summary>
    public void DisplayWorkflow()
    {
        Console.WriteLine("\n🚀 개발 워크플로우:");
        Console.WriteLine(new string('-', 50));
        
        for (int i = 0; i < _stages.Count; i++)
        {
            var stage = _stages[i];
            var icon = stage.Status switch
            {
                "Completed" => "✅",
                "In Progress" => "🔄",
                "Pending" => "⏳",
                _ => "⚪"
            };
            
            var marker = i == _currentStage ? "▶" : " ";
            Console.WriteLine($"{marker} {icon} {stage.Name,-20} [{stage.Status}]");
        }
        
        Console.WriteLine(new string('-', 50));
    }

    /// <summary>
    /// 완료 여부 확인
    /// </summary>
    public bool IsComplete()
    {
        return _stages.All(s => s.Status == "Completed");
    }
}

/// <summary>
/// 개발 스테이지
/// </summary>
public class DevelopmentStage
{
    public string Name { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public string? Output { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
}

/// <summary>
/// 프로젝트 컨텍스트
/// </summary>
public class ProjectContext
{
    public string ProjectName { get; set; } = "";
    public string FeatureName { get; set; } = "";
    public string GeneratedCode { get; set; } = "";
    public string GeneratedTests { get; set; } = "";
    public string GeneratedDocs { get; set; } = "";
    public string DeploymentPlan { get; set; } = "";
    public List<string> ReviewComments { get; set; } = new();
    public bool IsProductionReady { get; set; }
}

/// <summary>
/// 코드 품질 체커 (데모)
/// </summary>
public class CodeQualityChecker
{
    /// <summary>
    /// 코드 품질 점수 계산 (데모)
    /// </summary>
    public CodeQualityReport CheckQuality(string code)
    {
        var lines = code.Split('\n');
        var lineCount = lines.Length;
        var hasComments = lines.Any(l => l.Trim().StartsWith("//") || l.Trim().StartsWith("///"));
        var hasAsync = code.Contains("async") || code.Contains("await");
        var hasErrorHandling = code.Contains("try") || code.Contains("throw");
        var hasInterfaces = code.Contains("interface") || code.Contains("interface ");

        return new CodeQualityReport
        {
            LineCount = lineCount,
            HasComments = hasComments,
            HasAsync = hasAsync,
            HasErrorHandling = hasErrorHandling,
            HasInterfaces = hasInterfaces,
            OverallScore = CalculateScore(hasComments, hasAsync, hasErrorHandling, hasInterfaces)
        };
    }

    private int CalculateScore(bool comments, bool async, bool errorHandling, bool interfaces)
    {
        var score = 50;
        if (comments) score += 15;
        if (async) score += 15;
        if (errorHandling) score += 15;
        if (interfaces) score += 5;
        return Math.Min(100, score);
    }
}

/// <summary>
/// 코드 품질 리포트
/// </summary>
public class CodeQualityReport
{
    public int LineCount { get; set; }
    public bool HasComments { get; set; }
    public bool HasAsync { get; set; }
    public bool HasErrorHandling { get; set; }
    public bool HasInterfaces { get; set; }
    public int OverallScore { get; set; }
    
    public string Rating => OverallScore switch
    {
        >= 90 => "🟢 Excellent",
        >= 75 => "🟢 Good",
        >= 60 => "🟡 Fair",
        >= 40 => "🟠 Poor",
        _ => "🔴 Bad"
    };
}
