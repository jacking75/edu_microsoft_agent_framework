namespace _12C_IconConsistencyChecker.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _12C_IconConsistencyChecker.Tools;

/// <summary>
/// 아이콘 일관성 검수 에이전트
/// </summary>
public class IconAgent
{
    private readonly AIAgent _agent;

    public IconAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 아이콘 일관성 검수 전문가입니다.
                
                당신의 역할:
                1. 아이콘 세트의 일관성을 분석합니다
                2. 스타일 가이드 준수 여부를 확인합니다
                3. 색상/스트로크/크기 통일을 검사합니다
                4. 문제 아이콘을 식별합니다
                
                검수 항목:
                - 스타일 통일 (플랫/리얼/픽셀 등)
                - 색상 팔레트 준수
                - 스트로크 두께 (2px 통일)
                - 크기 비율 (정사각형 기준)
                - 그림자/하이라이트 방향
                - 대비 및 가독성
                
                출력 형식:
                - 일관성점수: [0-100]
                - 스타일검사: [결과]
                - 문제아이콘: [목록]
                - 개선제안: [목록]
                """,
            name: "IconAgent"
        );
    }

    public async Task<string> CheckConsistencyAsync(List<string> imagePaths, string styleGuide)
    {
        var images = string.Join(", ", imagePaths);
        
        var prompt = $"""
            다음 아이콘 세트의 일관성을 검수해주세요:
            
            이미지 목록: {images}
            
            아이콘 스타일가이드:
            {styleGuide}
            
            일관성 점수와 문제점을 상세히 보고해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }

    public async Task<string> CompareTwoIconsAsync(string imagePath1, string imagePath2)
    {
        var prompt = $"""
            다음 두 아이콘을 비교하여 일관성을 검수해주세요:
            
            이미지 1: {imagePath1}
            이미지 2: {imagePath2}
            
            스타일, 색상, 스트로크, 크기 등을 비교해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
