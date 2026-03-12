namespace _15A_CodeGenerator.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 코드 생성 에이전트
/// </summary>
public class CodeAgent
{
    private readonly AIAgent _agent;

    public CodeAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 C# 전문 코드 생성 AI 입니다.
                
                당신의 역할:
                1. 사용자 요구사항을 분석하여 C# 코드를 생성합니다
                2. 코딩 컨벤션을 준수합니다 (C# 12.0)
                3. 주석과 XML 문서를 작성합니다
                4. 에러 처리를 포함합니다
                5. 단위 테스트도 함께 생성합니다
                
                출력 형식:
                ```csharp
                // 코드 내용
                ```
                """,
            name: "CodeAgent"
        );
    }

    public async Task<string> GenerateCodeAsync(string requirement)
    {
        var prompt = $"""
            다음 요구사항을 구현하는 C# 코드를 생성해주세요:
            
            요구사항: {requirement}
            
            .NET 10.0 을 사용하세요.
            최신 C# 기능 (pattern matching, nullable 등) 을 활용하세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 코드 리뷰 에이전트
/// </summary>
public class ReviewAgent
{
    private readonly AIAgent _agent;

    public ReviewAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 시니어 C# 개발자로서 코드 리뷰를 수행합니다.
                
                당신의 역할:
                1. 코드의 정확성을 검증합니다
                2. 성능 문제를 찾습니다
                3. 보안 취약점을 확인합니다
                4. 가독성과 유지보수성을 평가합니다
                5. 개선 방안을 제안합니다
                
                출력 형식:
                - ✅ 좋은점: [목록]
                - ⚠️ 개선필요: [목록]
                - ❌ 문제점: [목록]
                - 💡 제안: [목록]
                """,
            name: "ReviewAgent"
        );
    }

    public async Task<string> ReviewCodeAsync(string code)
    {
        var prompt = $"""
            다음 C# 코드를 리뷰해주세요:
            
            ```csharp
            {code}
            ```
            
            코딩 컨벤션, 성능, 보안, 가독성 측면에서 평가하세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
