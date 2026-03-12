namespace _09A_CrashReportProcessor.Workflows;

using _09A_CrashReportProcessor.Agents;

/// <summary>
/// 크래시 리포트 처리 결과
/// </summary>
public class CrashReportResult
{
    public string OriginalReport { get; set; } = "";
    public string CollectedInfo { get; set; } = "";
    public string Analysis { get; set; } = "";
    public string Severity { get; set; } = "";
    public DateTime ProcessedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 크래시 리포트 처리 워크플로우 (Graph Workflow)
/// Collector → Analyzer → Severity (병렬 가능)
/// </summary>
public class CrashReportWorkflow
{
    private readonly CollectorAgent _collectorAgent;
    private readonly AnalyzerAgent _analyzerAgent;
    private readonly SeverityAgent _severityAgent;

    public CrashReportWorkflow(
        CollectorAgent collectorAgent,
        AnalyzerAgent analyzerAgent,
        SeverityAgent severityAgent)
    {
        _collectorAgent = collectorAgent;
        _analyzerAgent = analyzerAgent;
        _severityAgent = severityAgent;
    }

    /// <summary>
    /// Graph 워크플로우 실행
    /// </summary>
    public async Task<CrashReportResult> ExecuteAsync(string crashReport)
    {
        Console.WriteLine($"\n💥 크래시 리포트 처리 시작");
        Console.WriteLine(new string('-', 50));

        var result = new CrashReportResult { OriginalReport = crashReport };

        // Node 1: 수집
        Console.WriteLine("\n📥 Node 1: 정보 수집 중...");
        result.CollectedInfo = await _collectorAgent.CollectAsync(crashReport);
        Console.WriteLine($"✅ 수집 완료");
        Console.WriteLine($"   {Truncate(result.CollectedInfo, 80)}");

        // Node 2: 분석 (수집 완료 후 실행)
        Console.WriteLine("\n🔍 Node 2: 심층 분석 중...");
        result.Analysis = await _analyzerAgent.AnalyzeAsync(crashReport);
        Console.WriteLine($"✅ 분석 완료");
        Console.WriteLine($"   {Truncate(result.Analysis, 80)}");

        // Node 3: 심각도 평가 (분석 완료 후 실행)
        Console.WriteLine("\n⚠️ Node 3: 심각도 평가 중...");
        result.Severity = await _severityAgent.EvaluateSeverityAsync(result.Analysis);
        Console.WriteLine($"✅ 평가 완료");
        Console.WriteLine($"   {Truncate(result.Severity, 80)}");

        Console.WriteLine("\n" + new string('-', 50));
        Console.WriteLine("✅ 크래시 리포트 처리 완료!");

        return result;
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
