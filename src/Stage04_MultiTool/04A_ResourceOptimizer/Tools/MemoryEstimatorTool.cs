namespace _04A_ResourceOptimizer.Tools;

/// <summary>
/// 메모리 추정 Tool
/// </summary>
public class MemoryEstimatorTool
{
    public string EstimateBuildMemory(string projectPath)
    {
        if (!Directory.Exists(projectPath))
        {
            return $"Error: Directory not found - {projectPath}";
        }

        var csFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
        var totalLines = 0;
        var fileCount = 0;

        foreach (var file in csFiles)
        {
            try
            {
                var lines = File.ReadAllLines(file);
                totalLines += lines.Length;
                fileCount++;
            }
            catch { }
        }

        // 대략적인 컴파일 메모리 추정 (줄당 ~1KB)
        var estimatedMemoryKB = totalLines;
        var estimatedMemoryMB = estimatedMemoryKB / 1024.0;

        return $"""
            빌드 메모리 추정:
            - C# 파일 수: {fileCount}개
            - 총 코드 줄수: {totalLines:N0}줄
            
            예상 컴파일 메모리: {estimatedMemoryMB:F1} MB
            
            {(estimatedMemoryMB > 2048 ? "⚠️ 경고: 많은 메모리가 필요합니다." : "✅ 정상: 일반적인 빌드 메모리")}
            """;
    }
}
