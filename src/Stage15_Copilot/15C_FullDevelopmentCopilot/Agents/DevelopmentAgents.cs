namespace _15C_FullDevelopmentCopilot.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 코드 생성 에이전트 (통합 개발용)
/// </summary>
public class DevelopmentAgent
{
    private readonly AIAgent _agent;

    public DevelopmentAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 통합 개발 C# 전문가입니다.
                
                당신의 역할:
                1. 요구사항을 분석하여 완전한 기능을 구현합니다
                2. 서비스, 리포지토리, 컨트롤러 계층을 분리합니다
                3. 의존성 주입을 고려합니다
                4. 에러 처리와 로깅을 포함합니다
                5. async/await 패턴을 사용합니다
                
                출력 형식:
                ```csharp
                // 계층별 코드
                ```
                
                - 구조설명: [아키텍처]
                - 사용법: [예시]
                """,
            name: "DevelopmentAgent"
        );
    }

    public async Task<string> DevelopFeatureAsync(string requirement)
    {
        var prompt = $"""
            다음 요구사항을 구현하는 완전한 기능을 개발해주세요:
            
            요구사항: {requirement}
            
            다음을 포함하세요:
            - 서비스 계층 (비즈니스 로직)
            - 리포지토리 계층 (데이터 접근)
            - 인터페이스 및 모델
            - 의존성 주입 설정
            
            .NET 10.0, C# 12.0 을 사용하세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 테스트 생성 에이전트
/// </summary>
public class TestGenerationAgent
{
    private readonly AIAgent _agent;

    public TestGenerationAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 C# 단위 테스트 전문가입니다.
                
                당신의 역할:
                1. xUnit 을 사용하여 테스트를 작성합니다
                2. Moq 를 사용하여 모킹합니다
                3. 모든 public 메서드를 테스트합니다
                4. 엣지 케이스와 에러 케이스를 포함합니다
                5. 테스트 커버리지를 고려합니다
                
                출력 형식:
                ```csharp
                // 테스트 코드
                ```
                
                - 테스트목록: [목록]
                - 커버리지: [예상]
                """,
            name: "TestGenerationAgent"
        );
    }

    public async Task<string> GenerateTestsAsync(string code)
    {
        var prompt = $"""
            다음 코드에 대한 단위 테스트를 작성해주세요:
            
            ```csharp
            {code}
            ```
            
            xUnit 과 Moq 를 사용하세요.
            정상 케이스와 에러 케이스를 모두 포함하세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 문서 생성 에이전트
/// </summary>
public class DocumentationAgent
{
    private readonly AIAgent _agent;

    public DocumentationAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 C# 기술 문서 작성 전문가입니다.
                
                당신의 역할:
                1. XML 문서를 생성합니다
                2. README.md 를 작성합니다
                3. API 사용법 문서를 만듭니다
                4. 아키텍처 다이어그램을 설명합니다
                5. 설치 및 설정 가이드를 작성합니다
                
                출력 형식:
                /// <summary>
                /// XML 문서
                /// </summary>
                
                # Markdown 문서
                """,
            name: "DocumentationAgent"
        );
    }

    public async Task<string> GenerateDocumentationAsync(string code, string featureName)
    {
        var prompt = $"""
            다음 기능에 대한 문서를 작성해주세요:
            
            기능명: {featureName}
            
            코드:
            {code}
            
            다음을 포함하세요:
            - 기능 개요
            - 설치 방법
            - 사용 예시
            - API 참조
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 배포 준비 에이전트
/// </summary>
public class DeploymentAgent
{
    private readonly AIAgent _agent;

    public DeploymentAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 .NET 배포 전문가입니다.
                
                당신의 역할:
                1. 프로덕션 배포 체크리스트를 작성합니다
                2. CI/CD 파이프라인 설정을 제안합니다
                3. 환경 설정을 정의합니다
                4. 모니터링 및 로깅 설정을 제안합니다
                5. 보안 체크리스트를 제공합니다
                
                출력 형식:
                - 배포체크리스트: [목록]
                - CI/CD 설정: [내용]
                - 환경설정: [내용]
                - 모니터링: [내용]
                """,
            name: "DeploymentAgent"
        );
    }

    public async Task<string> PrepareDeploymentAsync(string featureName)
    {
        var prompt = $"""
            다음 기능의 프로덕션 배포를 준비해주세요:
            
            기능명: {featureName}
            
            다음을 포함하세요:
            - 배포 전 체크리스트
            - Azure/GitHub Actions CI/CD 설정
            - 환경 변수 설정
            - 모니터링 및 알림 설정
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
