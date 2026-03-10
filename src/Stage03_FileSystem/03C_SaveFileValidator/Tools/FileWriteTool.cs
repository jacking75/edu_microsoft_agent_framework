namespace _03C_SaveFileValidator.Tools;

using System.Text.Json;

/// <summary>
/// 세이브 파일 검수를 위한 Tool
/// </summary>
public class FileWriteTool
{
    /// <summary>
    /// JSON 파일의 무결성을 검증합니다
    /// </summary>
    public ValidationResult ValidateJsonFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new ValidationResult { IsValid = false, ErrorMessage = "File not found" };
        }

        try
        {
            var json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);
            return new ValidationResult { IsValid = true, Message = "Valid JSON format" };
        }
        catch (JsonException ex)
        {
            return new ValidationResult { IsValid = false, ErrorMessage = $"JSON Parse Error: {ex.Message}" };
        }
        catch (Exception ex)
        {
            return new ValidationResult { IsValid = false, ErrorMessage = $"Error: {ex.Message}" };
        }
    }

    /// <summary>
    /// 세이브 파일의 필수 필드를 검사합니다
    /// </summary>
    public ValidationResult CheckRequiredFields(string filePath, string[] requiredFields)
    {
        if (!File.Exists(filePath))
        {
            return new ValidationResult { IsValid = false, ErrorMessage = "File not found" };
        }

        try
        {
            var json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var missingFields = new List<string>();
            foreach (var field in requiredFields)
            {
                if (!root.TryGetProperty(field, out _))
                {
                    missingFields.Add(field);
                }
            }

            if (missingFields.Any())
            {
                return new ValidationResult 
                { 
                    IsValid = false, 
                    ErrorMessage = $"Missing fields: {string.Join(", ", missingFields)}" 
                };
            }

            return new ValidationResult { IsValid = true, Message = "All required fields present" };
        }
        catch (Exception ex)
        {
            return new ValidationResult { IsValid = false, ErrorMessage = ex.Message };
        }
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Message { get; set; }
}
