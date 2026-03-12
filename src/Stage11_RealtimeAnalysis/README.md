# Stage 11: Real-time Analysis

> **학습 목표**: 스트리밍 데이터 처리와 이벤트 기반 아키텍처

---

## 📁 프로젝트 구성

| 프로젝트 | 설명 | 핵심 기술 |
|----------|------|-----------|
| **11A_ServerMetricMonitor** | ✅ 서버 메트릭 모니터링 | 스트리밍, 이벤트, Reactive |
| **11B_AnomalyDetector** | ⏸️ 이상 징후 탐지 (확장용) | 패턴 분석, 이상 감지 |
| **11C_AutoScaler** | ⏸️ 자동 스케일링 (확장용) | 부하 분석, 알림 |

---

## 🚀 실행 방법

```bash
# 환경 변수 설정
$env:OPENAI_API_KEY="your-api-key"
$env:OPENAI_BASE_URL="https://api.openai.com/v1"

cd src

# 11A 실행 (완전 구현됨)
dotnet run --project Stage11_RealtimeAnalysis/11A_ServerMetricMonitor
```

---

## 🎯 11A_ServerMetricMonitor 학습 내용

### 스트리밍 데이터 아키텍처

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  Metric Source  │────▶│  StreamingData  │────▶│  MetricCollector│
│  (서버/클라이언트)│     │     Service     │     │     Agent       │
└─────────────────┘     └────────┬────────┘     └─────────────────┘
                                 │
                                 ▼
                        ┌─────────────────┐
                        │   AlertService  │
                        │  (알림 발생)     │
                        └─────────────────┘
```

### 1. Reactive Extensions (Rx) 활용

```csharp
// 메트릭 스트림 정의
public class StreamingDataService
{
    private readonly Subject<ServerMetric> _metricSubject = new();
    
    //Observable 스트림 공개
    public IObservable<ServerMetric> MetricStream => _metricSubject.AsObservable();
    
    // 메트릭 발행
    public void PublishMetric(ServerMetric metric)
    {
        _metricSubject.OnNext(metric);
    }
}
```

### 2. 이벤트 기반 필터링

```csharp
// 이상치 감지 구독
streamingService.MetricStream
    .Where(m => m.Fps < 30 || m.CpuUsage > 80)
    .Subscribe(metric =>
    {
        if (metric.Fps < 30)
        {
            alertService.RaiseAlert($"FPS 저하 감지: {metric.Fps:F1}", "WARNING");
        }
        if (metric.CpuUsage > 80)
        {
            alertService.RaiseAlert($"CPU 사용량 높음: {metric.CpuUsage:F1}%", "WARNING");
        }
    });
```

### 3. 실시간 메트릭 분석

```csharp
// 최근 메트릭 수집 및 분석
var recentMetrics = streamingService.GetRecentMetrics(10);
var analysis = await collectorAgent.AnalyzeMetricsAsync(recentMetrics);
```

---

## 💡 실행 예시

```
📊 서버 메트릭 모니터링에 오신 것을 환영합니다!

✅ OpenAI API 키가 설정되었습니다.
✅ 모니터링 서비스가 초기화되었습니다.

📡 메트릭 스트림 구독 중...
✅ 이상치 감지 구독 완료

📡 스트리밍 시작...
📊 [14:30:22] FPS: 58.3, Ping: 35.2ms, Players: 125
📊 [14:30:24] FPS: 55.1, Ping: 42.1ms, Players: 128
📊 [14:30:26] FPS: 28.5, Ping: 125.3ms, Players: 130
🟡 [WARNING] FPS 저하 감지: 28.5

모니터링 중... (중료: 'stop' 또는 'quit')

📈 최근 메트릭 분석 요청...

📊 분석 결과:
지난 10 초간 FPS 가 30 이하로 2 회 하락했습니다.
Ping 은 평균 45ms 에서 125ms 로 급등했으며...
```

---

## 📊 실시간 처리 패턴

### Rx 연산자

| 연산자 | 설명 | 사용 예 |
|--------|------|--------|
| **Where** | 필터링 | `Where(m => m.Fps < 30)` |
| **Buffer** | 배치 처리 | `Buffer(TimeSpan.FromSeconds(10))` |
| **Throttle** | 디바운스 | `Throttle(TimeSpan.FromSeconds(5))` |
| **DistinctUntilChanged** | 중복 제거 | `DistinctUntilChanged(m => m.PlayerCount)` |

### 알림 심각도

| 레벨 | 아이콘 | 조건 |
|------|--------|------|
| **CRITICAL** | 🔴 | FPS < 20, CPU > 90% |
| **WARNING** | 🟡 | FPS < 30, CPU > 80% |
| **INFO** | 🔵 | 일반 정보 |

---

## ✅ 완료된 Stage

- [x] Stage 01: 텍스트 응답
- [x] Stage 02: 단일 Tool
- [x] Stage 03: 파일 시스템
- [x] Stage 04: 다중 Tool
- [x] Stage 05: 외부 API
- [x] Stage 06: 데이터베이스
- [x] Stage 07: RAG
- [x] Stage 08: Multi-Agent (Sequential)
- [x] Stage 09: Workflow QA (Graph)
- [x] Stage 10: Human-in-the-Loop
- [x] **Stage 11: Real-time Analysis**

총 **33 개 프로젝트** 완성! 🎉

---

## 🔗 관련 문서

- **[Reactive Extensions](https://learn.microsoft.com/dotnet/reactive/)**
- **[이벤트 기반 아키텍처](https://learn.microsoft.com/azure/architecture/guide/architecture-styles/event-based)**
- **[실시간 모니터링 패턴](https://learn.microsoft.com/azure/azure-monitor/)**
