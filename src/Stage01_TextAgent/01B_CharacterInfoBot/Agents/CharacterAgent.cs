using System.Text.Json;
using System.Text.Json.Nodes;

namespace _01B_CharacterInfoBot.Agents;

/// <summary>
/// JSON 캐릭터 정보를 읽고 질문에 답변하는 에이전트
/// System.Text.Json 라이브러리를 사용하여 JSON 파싱
/// </summary>
public class CharacterAgent
{
    private readonly JsonNode _characterData;
    private readonly string _systemPrompt;

    /// <summary>
    /// CharacterAgent 생성자
    /// JSON 캐릭터 파일을 로드
    /// </summary>
    public CharacterAgent(string jsonPath)
    {
        var jsonContent = File.ReadAllText(jsonPath);
        _characterData = JsonNode.Parse(jsonContent) 
            ?? throw new InvalidOperationException("JSON 파일을 파싱할 수 없습니다.");

        _systemPrompt = """
            당신은 게임 캐릭터 정보 어시스턴트입니다.
            
            당신의 역할:
            1. 캐릭터 스탯, 스킬, 성장률 정보를 정확하게 답변합니다
            2. 클래스 간 상성 관계를 설명합니다
            3. 숫자 값은 그대로 전달하고, 필요한 경우 비교합니다
            4. 캐릭터 추천이 필요한 경우 플레이스타일을 고려합니다
            
            사용 가능한 정보:
            - 5 개 클래스: 전사, 마법사, 궁수, 성직자, 암살자
            - 각 클래스의 기본 스탯 (HP, MP, 공격, 방어 등)
            - 성장률 (레벨업 시 증가량)
            - 보유 스킬 목록
            - 클래스 상성 관계
            """;
    }

    public string GetSystemPrompt() => _systemPrompt;

    public string GetCharacterDataAsText()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        return _characterData.ToJsonString(options);
    }

    public List<string> GetAllClassNames()
    {
        var names = new List<string>();
        
        var characters = _characterData["characters"]?.AsArray();
        if (characters != null)
        {
            foreach (var character in characters)
            {
                var name = character?["name"]?.GetValue<string>();
                if (!string.IsNullOrEmpty(name))
                {
                    names.Add(name);
                }
            }
        }
        
        return names;
    }
}
