namespace _09C_QAReportGenerator.Workflows;

using _09C_QAReportGenerator.Agents;

/// <summary>
/// QA 리포트 생성 결과
/// </summary>
public class QAReportResult
{
    public string OriginalTestResults { get; set; } = "";
    public string Analysis { get; set; } = "";
    public string Summary { get; set; } = "";
    public string DeploymentDecision { get; set; } = "";
    public DateTime ProcessedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// QA 리포트 생성 워크플로우 (Graph Workflow)
/// TestAnalyzer → Summarizer → DeploymentDecider
/// </summary>
public class QAReportWorkflow
{
    private readonly TestAnalyzerAgent _testAnalyzerAgent;
    private readonly SummarizerAgent _summarizerAgent;
    private readonly DeploymentDeciderAgent _deploymentDeciderAgent;

    public QAReportWorkflow(
        TestAnalyzerAgent testAnalyzerAgent,
        SummarizerAgent summarizerAgent,
        DeploymentDeciderAgent deploymentDeciderAgent)
    {
        _testAnalyzerAgent = testAnalyzerAgent;
        _summarizerAgent = summarizerAgent;
        _deploymentDeciderAgent = deploymentDeciderAgent;
    }

    /// <summary>
    /// Graph 워크플로우 실행
    /// </summary>
    public async Task<QAReportResult> ExecuteAsync(string testResults)
    {
        Console.WriteLine($"\n📊 QA 리포트 생성 시작");
        Console.WriteLine(new string('-', 50));

        var result = new QAReportResult { OriginalTestResults = testResults };

        // Node 1: 테스트 결과 분석
        Console.WriteLine("\n🔍 Node 1: 테스트 결과 분석 중...");
        result.Analysis = await _testAnalyzerAgent.AnalyzeAsync(testResults);
        Console.WriteLine($"✅ 분석 완료");
        Console.WriteLine($"   {Truncate(result.Analysis, 80)}");

        // Node 2: 요약 리포트 생성 (분석 결과 기반)
        Console.WriteLine("\n📝 Node 2: 요약 리포트 생성 중...");
        result.Summary = await _summarizerAgent.SummarizeAsync(testResults, result.Analysis);
        Console.WriteLine($"✅ 요약 완료");
        Console.WriteLine($"   {Truncate(result.Summary, 80)}");

        // Node 3: 배포 결정 (분석 + 요약 기반)
        Console.WriteLine("\n🚀 Node 3: 배포 결정 중...");
        result.DeploymentDecision = await _deploymentDeciderAgent.DecideAsync(result.Analysis, result.Summary);
        Console.WriteLine($"✅ 배포 결정 완료");
        Console.WriteLine($"   {Truncate(result.DeploymentDecision, 80)}");

        Console.WriteLine("\n" + new string('-', 50));
        Console.WriteLine("✅ QA 리포트 생성 완료!");

        return result;
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
