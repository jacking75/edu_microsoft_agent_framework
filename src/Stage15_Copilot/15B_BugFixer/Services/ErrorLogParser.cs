namespace _15B_BugFixer.Services;

/// <summary>
/// 에러 로그 파싱 서비스
/// </summary>
public class ErrorLogParser
{
    /// <summary>
    /// 에러 로그를 파싱하여 구조화된 정보 추출
    /// </summary>
    public ParsedError Parse(string errorLog)
    {
        var parsedError = new ParsedError
        {
            RawLog = errorLog,
            ErrorMessage = ExtractErrorMessage(errorLog),
            ErrorType = ExtractErrorType(errorLog),
            StackTrace = ExtractStackTrace(errorLog),
            Source = ExtractSource(errorLog)
        };

        return parsedError;
    }

    private string ExtractErrorMessage(string log)
    {
        var lines = log.Split('\n');
        foreach (var line in lines)
        {
            if (line.Contains("Exception") || line.Contains("Error:"))
            {
                return line.Trim();
            }
        }
        return lines.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l)) ?? "Unknown Error";
    }

    private string ExtractErrorType(string log)
    {
        var exceptionTypes = new[]
        {
            "NullReferenceException", "ArgumentException", "InvalidOperationException",
            "IndexOutOfRangeException", "KeyNotFoundException", "TimeoutException",
            "UnauthorizedAccessException", "FileNotFoundException", "TaskCanceledException"
        };

        foreach (var type in exceptionTypes)
        {
            if (log.Contains(type))
            {
                return type;
            }
        }

        return "UnknownException";
    }

    private string ExtractStackTrace(string log)
    {
        var stackTraceStart = log.IndexOf("at ");
        if (stackTraceStart >= 0)
        {
            return log.Substring(stackTraceStart);
        }
        return "";
    }

    private string ExtractSource(string log)
    {
        var stackTrace = ExtractStackTrace(log);
        var firstLine = stackTrace.Split('\n').FirstOrDefault(l => l.Contains("at "));
        if (!string.IsNullOrEmpty(firstLine))
        {
            var match = System.Text.RegularExpressions.Regex.Match(firstLine, @"in (.+):line (\d+)");
            if (match.Success)
            {
                return $"{match.Groups[1].Value}:{match.Groups[2].Value}";
            }
        }
        return "Unknown Source";
    }
}

/// <summary>
/// 파싱된 에러 정보
/// </summary>
public class ParsedError
{
    public string RawLog { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
    public string ErrorType { get; set; } = "";
    public string StackTrace { get; set; } = "";
    public string Source { get; set; } = "";
}

/// <summary>
/// 버그 수정 이력
/// </summary>
public class BugFixHistory
{
    public DateTime FixedAt { get; set; } = DateTime.Now;
    public string BugDescription { get; set; } = "";
    public string OriginalCode { get; set; } = "";
    public string FixedCode { get; set; } = "";
    public string Diagnosis { get; set; } = "";
    public bool Verified { get; set; }
}

/// <summary>
/// 로컬 디버깅 서비스 (데모)
/// </summary>
public class LocalDebugService
{
    private readonly List<BugFixHistory> _history = new();

    /// <summary>
    /// 에러 시뮬레이션 (데모용)
    /// </summary>
    public string SimulateError(string errorType)
    {
        return errorType switch
        {
            "NullReference" => """
                System.NullReferenceException: Object reference not set to an instance of an object.
                   at UserService.GetUserById(String id) in Services/UserService.cs:line 45
                   at AccountController.GetProfile(String userId) in Controllers/AccountController.cs:line 28
                """,

            "Argument" => """
                System.ArgumentException: Value does not fall within the expected range.
                   at PaymentService.ProcessPayment(Decimal amount) in Services/PaymentService.cs:line 67
                   at CheckoutController.SubmitOrder(OrderDto order) in Controllers/CheckoutController.cs:line 112
                """,

            "Timeout" => """
                System.TimeoutException: The operation has timed out.
                   at DatabaseService.ExecuteQueryAsync(String sql) in Services/DatabaseService.cs:line 89
                   at ReportGenerator.GenerateReportAsync(ReportParams parameters) in Services/ReportGenerator.cs:line 34
                """,

            _ => """
                System.InvalidOperationException: Operation is not valid due to the current state of the object.
                   at GameStateMachine.ChangeState(GameState newState) in Core/GameStateMachine.cs:line 56
                   at GameManager.StartGame() in Core/GameManager.cs:line 23
                """
        };
    }

    /// <summary>
    /// 수정 이력 저장
    /// </summary>
    public void SaveFix(BugFixHistory fix)
    {
        _history.Add(fix);
    }

    /// <summary>
    /// 수정 이력 가져오기
    /// </summary>
    public List<BugFixHistory> GetHistory()
    {
        return new List<BugFixHistory>(_history);
    }
}
