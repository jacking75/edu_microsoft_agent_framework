namespace _11A_ServerMetricMonitor.Services;

using System.Reactive.Subjects;
using System.Reactive.Linq;

/// <summary>
/// 서버 메트릭 데이터
/// </summary>
public class ServerMetric
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public double Fps { get; set; }
    public double Ping { get; set; }
    public int PlayerCount { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
}

/// <summary>
/// 스트리밍 데이터 서비스 - 실시간 메트릭 수집
/// </summary>
public class StreamingDataService
{
    private readonly Subject<ServerMetric> _metricSubject = new();
    private readonly List<ServerMetric> _history = new();
    private Timer? _simulationTimer;

    /// <summary>
    /// 메트릭 스트림 구독
    /// </summary>
    public IObservable<ServerMetric> MetricStream => _metricSubject.AsObservable();

    /// <summary>
    /// 스트리밍 시작 (데모용 시뮬레이션)
    /// </summary>
    public void StartStreaming()
    {
        Console.WriteLine("📡 스트리밍 시작...");
        
        _simulationTimer = new Timer(SimulateMetric, null, 0, 2000);
    }

    /// <summary>
    /// 스트리밍 중지
    /// </summary>
    public void StopStreaming()
    {
        _simulationTimer?.Dispose();
        Console.WriteLine("📡 스트리밍 중지");
    }

    /// <summary>
    /// 메트릭 발행
    /// </summary>
    public void PublishMetric(ServerMetric metric)
    {
        _history.Add(metric);
        
        // 최근 100 개만 유지
        if (_history.Count > 100)
        {
            _history.RemoveAt(0);
        }
        
        _metricSubject.OnNext(metric);
    }

    /// <summary>
    /// 최근 메트릭 가져오기
    /// </summary>
    public List<ServerMetric> GetRecentMetrics(int count = 10)
    {
        return _history.TakeLast(count).ToList();
    }

    /// <summary>
    /// 데모용 메트릭 시뮬레이션
    /// </summary>
    private void SimulateMetric(object? state)
    {
        var random = new Random();
        var metric = new ServerMetric
        {
            Timestamp = DateTime.Now,
            Fps = 55 + random.NextDouble() * 10,
            Ping = 20 + random.NextDouble() * 30,
            PlayerCount = 100 + random.Next(50),
            CpuUsage = 40 + random.NextDouble() * 30,
            MemoryUsage = 60 + random.NextDouble() * 20
        };

        PublishMetric(metric);
        
        Console.WriteLine($"📊 [{metric.Timestamp:HH:mm:ss}] FPS: {metric.Fps:F1}, Ping: {metric.Ping:F1}ms, Players: {metric.PlayerCount}");
    }
}

/// <summary>
/// 알림 서비스
/// </summary>
public class AlertService
{
    public event EventHandler<AlertEventArgs>? AlertRaised;

    public void RaiseAlert(string message, string severity = "INFO")
    {
        var icon = severity switch
        {
            "CRITICAL" => "🔴",
            "WARNING" => "🟡",
            "INFO" => "🔵",
            _ => "⚪"
        };

        Console.WriteLine($"{icon} [{severity}] {message}");
        AlertRaised?.Invoke(this, new AlertEventArgs(message, severity));
    }
}

public class AlertEventArgs : EventArgs
{
    public string Message { get; }
    public string Severity { get; }

    public AlertEventArgs(string message, string severity)
    {
        Message = message;
        Severity = severity;
    }
}
