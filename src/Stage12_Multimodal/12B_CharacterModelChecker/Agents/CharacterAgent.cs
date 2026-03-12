namespace _12B_CharacterModelChecker.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _12B_CharacterModelChecker.Tools;

/// <summary>
/// 캐릭터 모델 검수 에이전트
/// </summary>
public class CharacterAgent
{
    private readonly AIAgent _agent;

    public CharacterAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 캐릭터 모델 검수 전문가입니다.
                
                당신의 역할:
                1. 캐릭터 모델의 비율을 분석합니다
                2. 스타일 가이드 준수 여부를 확인합니다
                3. 의상/장비 디테일을 검사합니다
                4. 일관성을 평가합니다
                
                검수 항목:
                - 신체 비율 (머리:신체 = 1:7~8)
                - 의상/장비 스타일 통일
                - 색상 팔레트 준수
                - 텍스처 품질
                - 표정/포즈 자연스러움
                
                출력 형식:
                - 비율분석: [결과]
                - 스타일검사: [결과]
                - 문제점: [목록]
                - 개선제안: [목록]
                """,
            name: "CharacterAgent"
        );
    }

    public async Task<string> CheckCharacterAsync(string imagePath, string styleGuide)
    {
        var prompt = $"""
            다음 캐릭터 모델을 검수해주세요:
            
            이미지: {imagePath}
            
            캐릭터 스타일가이드:
            {styleGuide}
            
            비율과 스타일 일관성을 상세히 검수해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
