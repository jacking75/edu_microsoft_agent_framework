namespace _04C_PerformanceProfiler.Tools;

/// <summary>
/// FPS 분석 Tool
/// </summary>
public class FpsAnalyzerTool
{
    public string AnalyzeFpsData(List<int> fpsValues)
    {
        if (!fpsValues.Any())
        {
            return "FPS 데이터가 없습니다.";
        }

        var min = fpsValues.Min();
        var max = fpsValues.Max();
        var avg = fpsValues.Average();
        var low1Percent = fpsValues.OrderBy(x => x).Skip((int)(fpsValues.Count * 0.01)).First();

        return $"""
            FPS 분석 결과:
            
            📊 통계:
            - 최소: {min} FPS
            - 최대: {max} FPS
            - 평균: {avg:F1} FPS
            - 1% Low: {low1Percent} FPS (하위 1% 구간의 최저값)
            
            평가:
            {(avg >= 60 ? "✅ 원활한 성능" : avg >= 30 ? "⚠️ 일반적인 성능" : "❌ 성능 개선 필요")}
            
            {(low1Percent < 30 ? "⚠️ 1% Low 가 낮습니다 - 스터터링 가능성" : "✅ 안정적인 프레임률")}
            """;
    }

    public string DetectDrops(List<int> fpsValues, int threshold = 30)
    {
        var drops = new List<int>();
        
        for (int i = 0; i < fpsValues.Count; i++)
        {
            if (fpsValues[i] < threshold)
            {
                drops.Add(i);
            }
        }

        if (!drops.Any())
        {
            return $"✅ FPS {threshold} 이하 드롭 없음";
        }

        return $"""
            FPS 드롭 감지 (임계값: {threshold} FPS):
            
            - 드롭 발생 횟수: {drops.Count}회
            - 전체 프레임 중: {(double)drops.Count / fpsValues.Count * 100:F2}%
            
            드롭 인덱스: {string.Join(", ", drops.Take(20))}{(drops.Count > 20 ? $"...외 {drops.Count - 20}개" : "")}
            
            {(drops.Count > fpsValues.Count * 0.1 ? "⚠️ 빈번한 드롭 - 최적화 필요" : "✅ 드롭이 적음")}
            """;
    }
}
