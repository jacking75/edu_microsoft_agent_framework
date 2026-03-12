namespace _11B_AnomalyDetector.Services;

using System.Reactive.Subjects;
using System.Reactive.Linq;

/// <summary>
/// 게임 메트릭 데이터
/// </summary>
public class GameMetric
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public double Fps { get; set; }
    public double Ping { get; set; }
    public int PlayerCount { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public string? Zone { get; set; }
}

/// <summary>
/// 스트리밍 데이터 서비스 - 실시간 메트릭 수집
/// </summary>
public class StreamingDataService
{
    private readonly Subject<GameMetric> _metricSubject = new();
    private readonly List<GameMetric> _history = new();
    private Timer? _simulationTimer;
    private readonly Random _random = new();
    private bool _anomalyMode = false;

    /// <summary>
    /// 메트릭 스트림 구독
    /// </summary>
    public IObservable<GameMetric> MetricStream => _metricSubject.AsObservable();

    /// <summary>
    /// 이상 모드 설정 (데모용)
    /// </summary>
    public void SetAnomalyMode(bool enabled)
    {
        _anomalyMode = enabled;
        Console.WriteLine($"✅ 이상 모드: {(enabled ? "활성화 (이상 징후 발생)" : "비활성화 (정상 상태)")}");
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
    public void PublishMetric(GameMetric metric)
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
    public List<GameMetric> GetRecentMetrics(int count = 10)
    {
        return _history.TakeLast(count).ToList();
    }

    /// <summary>
    /// 전체 히스토리 가져오기 (패턴 분석용)
    /// </summary>
    public List<GameMetric> GetAllHistory()
    {
        return new List<GameMetric>(_history);
    }

    /// <summary>
    /// 데모용 메트릭 시뮬레이션
    /// </summary>
    private void SimulateMetric(object? state)
    {
        var metric = new GameMetric
        {
            Timestamp = DateTime.Now,
            Fps = _anomalyMode ? 25 + _random.NextDouble() * 10 : 55 + _random.NextDouble() * 10,
            Ping = _anomalyMode ? 80 + _random.NextDouble() * 60 : 20 + _random.NextDouble() * 30,
            PlayerCount = 100 + _random.Next(50),
            CpuUsage = _anomalyMode ? 75 + _random.NextDouble() * 20 : 40 + _random.NextDouble() * 30,
            MemoryUsage = _anomalyMode ? 80 + _random.NextDouble() * 15 : 60 + _random.NextDouble() * 20,
            Zone = GetRandomZone()
        };

        PublishMetric(metric);
        
        var indicator = _anomalyMode ? "🔴" : "📊";
        Console.WriteLine($"{indicator} [{metric.Timestamp:HH:mm:ss}] FPS: {metric.Fps:F1}, Ping: {metric.Ping:F1}ms, Players: {metric.PlayerCount}");
    }

    private string GetRandomZone()
    {
        var zones = new[] { "로드리아", "아르데타인", "카라인", "오딘", "발록" };
        return zones[_random.Next(zones.Length)];
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
            "HIGH" => "🟠",
            "MEDIUM" => "🟡",
            "LOW" => "🔵",
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
