namespace _04A_ResourceOptimizer.Tools;

/// <summary>
/// 텍스처 분석을 위한 Tool
/// </summary>
public class TextureAnalyzerTool
{
    public string AnalyzeTexture(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return $"Error: File not found - {filePath}";
        }

        var info = new FileInfo(filePath);
        var sizeMB = info.Length / (1024.0 * 1024.0);
        var extension = info.Extension.ToLower();

        return $"""
            텍스처 분석 결과:
            - 파일: {info.Name}
            - 형식: {extension}
            - 크기: {sizeMB:F2} MB
            - 최종 수정: {info.LastWriteTime}
            
            권장 사항:
            - 4K 텍스처는 8MB 이하 권장
            - 모바일용은 2MB 이하 권장
            - {extension} 형식은 압축 효율이 {(extension == ".png" ? "좋음" : "보통")}
            """;
    }

    public string EstimateMemoryUsage(List<string> texturePaths)
    {
        var totalSize = 0L;
        var results = new List<string>();

        foreach (var path in texturePaths)
        {
            if (File.Exists(path))
            {
                var size = new FileInfo(path).Length;
                totalSize += size;
                results.Add($"{Path.GetFileName(path)}: {size / 1024.0:F1} KB");
            }
        }

        return $"""
            메모리 사용량 추정:
            {string.Join("\n", results)}
            
            총합: {totalSize / (1024.0 * 1024.0):F2} MB
            
            {(totalSize > 100 * 1024 * 1024 ? "⚠️ 경고: 텍스처 메모리 사용량이 많습니다!" : "✅ 정상: 적절한 텍스처 메모리 사용량")}
            """;
    }
}
