namespace SkillSystem2;

public class SkillDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
    public List<FunctionDefinition> Functions { get; set; } = new();
    public List<string> Examples { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
}

public class FunctionDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ParameterDefinition> Parameters { get; set; } = new();
    public string Formula { get; set; } = string.Empty;
}

public class ParameterDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
