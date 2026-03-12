namespace _12A_UIStyleChecker.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _12A_UIStyleChecker.Tools;

/// <summary>
/// UI 스타일 검수 에이전트
/// </summary>
public class StyleAgent
{
    private readonly AIAgent _agent;

    public StyleAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 UI 스타일 검수 전문가입니다.
                
                당신의 역할:
                1. UI 스크린샷을 분석하여 스타일가이드 준수 여부를 확인합니다
                2. 색상, 폰트, 레이아웃 일관성을 검사합니다
                3. 개선 사항을 제안합니다
                4. 문제점을 상세히 보고합니다
                
                검수 항목:
                - 색상 팔레트 준수
                - 폰트 일관성 (사이즈, 패밀리)
                - 레이아웃 정렬
                - 아이콘 스타일 통일
                - 여백 및 간격
                - 대비 및 가독성
                
                출력 형식:
                - 준수사항: [목록]
                - 문제점: [목록]
                - 개선제안: [목록]
                """,
            name: "StyleAgent"
        );
    }

    public async Task<string> CheckStyleAsync(string imagePath, string styleGuide)
    {
        var prompt = $"""
            다음 UI 스크린샷을 스타일가이드와 비교하여 검수해주세요:
            
            이미지: {imagePath}
            
            스타일가이드:
            {styleGuide}
            
            상세한 검수 결과를 보고해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
