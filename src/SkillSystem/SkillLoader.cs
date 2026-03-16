using System.Text.RegularExpressions;

namespace SkillSystem;

public class SkillLoader
{
    private readonly string _skillsDirectory;

    public SkillLoader(string skillsDirectory)
    {
        _skillsDirectory = skillsDirectory;
    }

    public List<SkillDefinition> LoadAllSkills()
    {
        var skills = new List<SkillDefinition>();
        
        if (!Directory.Exists(_skillsDirectory))
        {
            Console.WriteLine($"⚠️  스킬 디렉토리를 찾을 수 없습니다: {_skillsDirectory}");
            return skills;
        }

        var skillDirs = Directory.GetDirectories(_skillsDirectory);
        
        foreach (var dir in skillDirs)
        {
            var skillFile = Path.Combine(dir, "skill.md");
            if (File.Exists(skillFile))
            {
                try
                {
                    var skill = ParseSkillFile(skillFile, dir);
                    skills.Add(skill);
                    Console.WriteLine($"✅ 스킬 로드 완료: {skill.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 스킬 로드 실패: {dir} - {ex.Message}");
                }
            }
        }

        return skills;
    }

    private SkillDefinition ParseSkillFile(string filePath, string directory)
    {
        var content = File.ReadAllText(filePath);
        var skill = new SkillDefinition
        {
            Directory = Path.GetFileName(directory)
        };

        var lines = content.Split('\n');
        string? currentSection = null;
        FunctionDefinition? currentFunction = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            
            if (trimmed.StartsWith("# Skill:"))
            {
                skill.Name = trimmed.Replace("# Skill:", "").Trim();
            }
            else if (trimmed.StartsWith("## Description"))
            {
                currentSection = "Description";
            }
            else if (trimmed.StartsWith("## Functions"))
            {
                currentSection = "Functions";
            }
            else if (trimmed.StartsWith("## Examples"))
            {
                currentSection = "Examples";
                
                if (currentFunction != null)
                {
                    skill.Functions.Add(currentFunction);
                    currentFunction = null;
                }
            }
            else if (trimmed.StartsWith("### ") && (currentSection == "Functions" || currentSection == "FunctionDetails"))
            {
                if (currentFunction != null)
                {
                    skill.Functions.Add(currentFunction);
                }
                
                currentFunction = new FunctionDefinition
                {
                    Name = trimmed.Replace("### ", "").Trim()
                };
                currentSection = "FunctionDetails";
            }
            else if (currentFunction != null && 
                    (currentSection == "Functions" || currentSection == "FunctionDetails"))
            {
                if (trimmed.StartsWith("- **설명**:"))
                {
                    currentFunction.Description = trimmed.Replace("- **설명**:", "").Trim();
                }
                else if (trimmed.StartsWith("- **파라미터**:"))
                {
                    currentSection = "FunctionDetails";
                }
                else if (trimmed.StartsWith("- **수식**:"))
                {
                    currentFunction.Formula = trimmed.Replace("- **수식**:", "").Trim();
                }
                else if (trimmed.StartsWith("- ") && trimmed.Contains(":"))
                {
                    var paramMatch = Regex.Match(trimmed, @"- (\w+):\s*(\w+)\s*\(([^)]+)\)");
                    if (paramMatch.Success)
                    {
                        currentFunction.Parameters.Add(new ParameterDefinition
                        {
                            Name = paramMatch.Groups[1].Value,
                            Type = paramMatch.Groups[2].Value,
                            Description = paramMatch.Groups[3].Value
                        });
                    }
                }
            }
            else if (currentSection == "Description" && !string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith("##"))
            {
                skill.Description += trimmed + " ";
            }
            else if (currentSection == "Examples" && trimmed.StartsWith("- "))
            {
                skill.Examples.Add(trimmed.TrimStart('-', ' ').Trim('"'));
            }
        }

        if (currentFunction != null)
        {
            skill.Functions.Add(currentFunction);
        }

        skill.Description = skill.Description.Trim();
        
        return skill;
    }
}
