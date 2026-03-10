namespace _04C_PerformanceProfiler.Tools;

/// <summary>
/// 병목 감지 Tool
/// </summary>
public class BottleneckDetectorTool
{
    public string DetectBottleneck(Dictionary<string, double> metrics)
    {
        var results = new List<string>();
        
        // CPU 사용량
        if (metrics.TryGetValue("cpu", out var cpu) && cpu > 90)
        {
            results.Add($"⚠️ CPU 병목: 사용량 {cpu:F1}%");
        }
        
        // GPU 사용량
        if (metrics.TryGetValue("gpu", out var gpu) && gpu > 95)
        {
            results.Add($"⚠️ GPU 병목: 사용량 {gpu:F1}%");
        }
        
        // 메모리 사용량
        if (metrics.TryGetValue("memory", out var memory) && memory > 85)
        {
            results.Add($"⚠️ 메모리 병목: 사용량 {memory:F1}%");
        }
        
        // 디스크 I/O
        if (metrics.TryGetValue("disk", out var disk) && disk > 80)
        {
            results.Add($"⚠️ 디스크 I/O 병목: 사용량 {disk:F1}%");
        }

        if (!results.Any())
        {
            return "✅ 병목 현상이 감지되지 않았습니다.";
        }

        return $"""
            병목 현상 분석:
            
            {string.Join("\n", results)}
            
            권장 조치:
            {(cpu > 90 ? "- CPU: 로직 최적화, 멀티스레딩 고려" : "")}
            {(gpu > 95 ? "- GPU: 쉐이더 최적화, LOD 활용" : "")}
            {(memory > 85 ? "- 메모리: 오브젝트 풀링, 리소스 언로드" : "")}
            {(disk > 80 ? "- 디스크: 비동기 로딩, 캐싱 활용" : "")}
            """;
    }

    public string SuggestOptimizations(string bottleneckType)
    {
        return bottleneckType.ToLower() switch
        {
            "cpu" => """
                CPU 최적화 제안:
                1. 불필요한 계산 제거
                2. 캐싱 활용
                3. 데이터 구조 최적화
                4. 병렬 처리 고려
                """,
            "gpu" => """
                GPU 최적화 제안:
                1. 드로우 콜 batching
                2. LOD(Level of Detail) 적용
                3. 쉐이더 복잡도 감소
                4. 오클루전 컬링 활용
                """,
            "memory" => """
                메모리 최적화 제안:
                1. 오브젝트 풀링 구현
                2. 불필요한 할당 제거
                3. Texture 압축
                4. 사용 후 즉시 해제
                """,
            _ => "알 수 없는 병목 타입입니다."
        };
    }
}
