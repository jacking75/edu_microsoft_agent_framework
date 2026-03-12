namespace _11C_AutoScaler.Services;

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
    public string? ServerId { get; set; }
}

/// <summary>
/// 스트리밍 데이터 서비스 - 실시간 메트릭 수집
/// </summary>
public class StreamingDataService
{
    private readonly Subject<ServerMetric> _metricSubject = new();
    private readonly List<ServerMetric> _history = new();
    private Timer? _simulationTimer;
    private readonly Random _random = new();
    private int _loadLevel = 1; // 1:Low, 2:Normal, 3:High, 4:Critical

    /// <summary>
    /// 메트릭 스트림 구독
    /// </summary>
    public IObservable<ServerMetric> MetricStream => _metricSubject.AsObservable();

    /// <summary>
    /// 부하 레벨 설정 (데모용)
    /// </summary>
    public void SetLoadLevel(int level)
    {
        _loadLevel = Math.Clamp(level, 1, 4);
        var levelName = _loadLevel switch
        {
            1 => "Low (여유)",
            2 => "Normal (정상)",
            3 => "High (높음)",
            4 => "Critical (심각)",
            _ => "Unknown"
        };
        Console.WriteLine($"✅ 부하 레벨: {levelName}");
    }

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
    /// 전체 히스토리 가져오기
    /// </summary>
    public List<ServerMetric> GetAllHistory()
    {
        return new List<ServerMetric>(_history);
    }

    /// <summary>
    /// 데모용 메트릭 시뮬레이션
    /// </summary>
    private void SimulateMetric(object? state)
    {
        var (cpuBase, memBase, playerBase) = _loadLevel switch
        {
            1 => (25.0, 40.0, 300),    // Low
            2 => (50.0, 60.0, 800),    // Normal
            3 => (75.0, 80.0, 1500),   // High
            4 => (90.0, 92.0, 2500),   // Critical
            _ => (50.0, 60.0, 800)
        };

        var metric = new ServerMetric
        {
            Timestamp = DateTime.Now,
            Fps = 55 + _random.NextDouble() * 10 - (_loadLevel * 5),
            Ping = 20 + _random.NextDouble() * 30 + (_loadLevel * 20),
            PlayerCount = playerBase + _random.Next(100) - 50,
            CpuUsage = cpuBase + _random.NextDouble() * 15 - 7.5,
            MemoryUsage = memBase + _random.NextDouble() * 10 - 5,
            ServerId = $"GameServer-{_random.Next(1, 5):D2}"
        };

        PublishMetric(metric);
        
        var indicator = _loadLevel switch
        {
            4 => "🔴",
            3 => "🟠",
            2 => "🟢",
            1 => "🔵",
            _ => "⚪"
        };
        
        Console.WriteLine($"{indicator} [{metric.Timestamp:HH:mm:ss}] CPU: {metric.CpuUsage:F1}%, Mem: {metric.MemoryUsage:F1}%, Players: {metric.PlayerCount}");
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
            "WARNING" => "🟠",
            "HIGH" => "🟡",
            "MEDIUM" => "🔵",
            "INFO" => "⚪",
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
