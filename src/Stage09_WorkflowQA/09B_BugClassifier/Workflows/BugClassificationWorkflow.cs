namespace _09B_BugClassifier.Workflows;

using _09B_BugClassifier.Agents;

/// <summary>
/// 버그 분류 처리 결과
/// </summary>
public class BugClassificationResult
{
    public string OriginalReport { get; set; } = "";
    public string LogAnalysis { get; set; } = "";
    public string DuplicateCheck { get; set; } = "";
    public string PriorityAssignment { get; set; } = "";
    public DateTime ProcessedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 버그 분류 워크플로우 (Graph Workflow)
/// LogAnalyzer → DuplicateChecker → PriorityAssigner
/// </summary>
public class BugClassificationWorkflow
{
    private readonly LogAnalyzerAgent _logAnalyzerAgent;
    private readonly DuplicateCheckerAgent _duplicateCheckerAgent;
    private readonly PriorityAssignerAgent _priorityAssignerAgent;

    public BugClassificationWorkflow(
        LogAnalyzerAgent logAnalyzerAgent,
        DuplicateCheckerAgent duplicateCheckerAgent,
        PriorityAssignerAgent priorityAssignerAgent)
    {
        _logAnalyzerAgent = logAnalyzerAgent;
        _duplicateCheckerAgent = duplicateCheckerAgent;
        _priorityAssignerAgent = priorityAssignerAgent;
    }

    /// <summary>
    /// Graph 워크플로우 실행
    /// </summary>
    public async Task<BugClassificationResult> ExecuteAsync(string bugReport)
    {
        Console.WriteLine($"\n🐛 버그 분류 처리 시작");
        Console.WriteLine(new string('-', 50));

        var result = new BugClassificationResult { OriginalReport = bugReport };

        // Node 1: 로그 분석
        Console.WriteLine("\n🔍 Node 1: 로그 분석 중...");
        result.LogAnalysis = await _logAnalyzerAgent.AnalyzeAsync(bugReport);
        Console.WriteLine($"✅ 로그 분석 완료");
        Console.WriteLine($"   {Truncate(result.LogAnalysis, 80)}");

        // Node 2: 중복 체크 (로그 분석 완료 후 실행)
        Console.WriteLine("\n🔄 Node 2: 중복 체크 중...");
        result.DuplicateCheck = await _duplicateCheckerAgent.CheckDuplicateAsync(bugReport);
        Console.WriteLine($"✅ 중복 체크 완료");
        Console.WriteLine($"   {Truncate(result.DuplicateCheck, 80)}");

        // Node 3: 우선순위 결정 (로그 분석 결과를 기반으로)
        Console.WriteLine("\n📊 Node 3: 우선순위 결정 중...");
        result.PriorityAssignment = await _priorityAssignerAgent.AssignPriorityAsync(bugReport, result.LogAnalysis);
        Console.WriteLine($"✅ 우선순위 결정 완료");
        Console.WriteLine($"   {Truncate(result.PriorityAssignment, 80)}");

        Console.WriteLine("\n" + new string('-', 50));
        Console.WriteLine("✅ 버그 분류 처리 완료!");

        return result;
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
