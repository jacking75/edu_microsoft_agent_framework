namespace SoulAgent.Soul;

/// <summary>
/// 에이전트의 핵심 가치와 원칙을 정의하는 Soul 클래스
/// </summary>
public class SoulDefinition
{
    /// <summary>
    /// 에이전트의 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 에이전트의 정체성/페르소나
    /// </summary>
    public string Identity { get; set; } = string.Empty;

    /// <summary>
    /// 핵심 가치 목록
    /// </summary>
    public List<string> CoreValues { get; set; } = new();

    /// <summary>
    /// 행동 원칙 목록
    /// </summary>
    public List<string> Principles { get; set; } = new();

    /// <summary>
    /// 말투 및 언어 스타일
    /// </summary>
    public string SpeakingStyle { get; set; } = string.Empty;

    /// <summary>
    /// 금지 사항
    /// </summary>
    public List<string> Taboos { get; set; } = new();

    /// <summary>
    /// 미션 및 목적
    /// </summary>
    public string Mission { get; set; } = string.Empty;
}
