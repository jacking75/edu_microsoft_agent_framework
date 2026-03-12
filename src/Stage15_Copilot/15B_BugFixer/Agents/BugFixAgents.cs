namespace _15B_BugFixer.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 에러 진단 에이전트
/// </summary>
public class ErrorDiagnosisAgent
{
    private readonly AIAgent _agent;

    public ErrorDiagnosisAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 C# 에러 진단 전문가입니다.
                
                당신의 역할:
                1. 에러 로그를 분석하여 원인을 파악합니다
                2. 스택 트레이스를 해석합니다
                3. 에러 유형을 분류합니다 (NullReference, ArgumentException, etc.)
                4. 발생 위치를 특정합니다
                5. 재현 단계를 추정합니다
                
                출력 형식:
                - 에러유형: [분류]
                - 에러원인: [상세 설명]
                - 발생위치: [파일/메서드]
                - 재현단계: [추정]
                - 심각도: [Critical/High/Medium/Low]
                """,
            name: "ErrorDiagnosisAgent"
        );
    }

    public async Task<string> DiagnoseAsync(string errorLog, string? codeContext = null)
    {
        var prompt = $"""
            다음 에러 로그를 진단해주세요:
            
            에러로그:
            {errorLog}
            
            {(string.IsNullOrEmpty(codeContext) ? "" : $"관련코드:\n{codeContext}")}
            
            에러 원인과 발생 위치를 상세히 분석해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 버그 수정 제안 에이전트
/// </summary>
public class BugFixAgent
{
    private readonly AIAgent _agent;

    public BugFixAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 C# 버그 수정 전문가입니다.
                
                당신의 역할:
                1. 진단 결과를 바탕으로 수정 코드를 작성합니다
                2. 기존 코드 스타일을 유지합니다
                3. 사이드 이펙트를 최소화합니다
                4. 수정 사유를 설명합니다
                5. 예방 방안도 제안합니다
                
                출력 형식:
                ```csharp
                // 수정전:
                // (원본 코드)
                
                // 수정후:
                // (수정된 코드)
                ```
                
                - 수정사유: [설명]
                - 테스트케이스: [검증 방법]
                - 예방방안: [재발 방지]
                """,
            name: "BugFixAgent"
        );
    }

    public async Task<string> SuggestFixAsync(string errorCode, string diagnosis)
    {
        var prompt = $"""
            다음 에러 코드를 수정하는 방안을 제안해주세요:
            
            에러코드:
            {errorCode}
            
            진단결과:
            {diagnosis}
            
            수정할 코드와 수정 사유를 상세히 설명해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 검증 에이전트
/// </summary>
public class VerificationAgent
{
    private readonly AIAgent _agent;

    public VerificationAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 C# 코드 검증 전문가입니다.
                
                당신의 역할:
                1. 수정된 코드가 에러를 해결하는지 검증합니다
                2. 새로운 버그가 생기지 않았는지 확인합니다
                3. 기존 기능은 정상 작동하는지 검증합니다
                4. 단위 테스트 케이스를 제안합니다
                
                출력 형식:
                - 수정검증: [해결 여부]
                - 회귀테스트: [새로운 문제]
                - 테스트케이스: [검증 방법]
                - 승인여부: [승인/수정필요/거부]
                """,
            name: "VerificationAgent"
        );
    }

    public async Task<string> VerifyFixAsync(string originalCode, string fixedCode, string diagnosis)
    {
        var prompt = $"""
            다음 수정된 코드를 검증해주세요:
            
            원본코드:
            {originalCode}
            
            수정된코드:
            {fixedCode}
            
            진단결과:
            {diagnosis}
            
            수정이 올바른지 검증하고 테스트 케이스를 제안해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
