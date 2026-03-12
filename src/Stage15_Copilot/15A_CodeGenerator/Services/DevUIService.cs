namespace _15A_CodeGenerator.Services;

/// <summary>
/// DevUI 서비스 (데모)
/// 실제 구현 시 Visual Studio 확장과 연동
/// </summary>
public class DevUIService
{
    /// <summary>
    /// 코드 에디터에 코드 표시 (데모)
    /// </summary>
    public void DisplayCode(string code, string fileName = "GeneratedCode.cs")
    {
        Console.WriteLine($"\n📝 [{fileName}] 코드 미리보기:");
        Console.WriteLine(new string('-', 60));
        
        // 코드 줄번호 추가
        var lines = code.Split('\n');
        for (int i = 0; i < lines.Length && i < 20; i++)
        {
            Console.WriteLine($"{i + 1,3}: {lines[i]}");
        }
        
        if (lines.Length > 20)
        {
            Console.WriteLine($"... ({lines.Length - 20} 줄 더 있음)");
        }
        
        Console.WriteLine(new string('-', 60));
    }
    
    /// <summary>
    /// 코드 적용 (데모)
    /// </summary>
    public void ApplyCode(string code, string filePath)
    {
        Console.WriteLine($"\n✅ 코드가 {filePath} 에 적용되었습니다.");
    }
}
