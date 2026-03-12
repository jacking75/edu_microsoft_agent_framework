namespace _10C_LocalizationReviewer.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;

/// <summary>
/// 번역 에이전트
/// </summary>
public class TranslatorAgent
{
    private readonly AIAgent _agent;

    public TranslatorAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 게임 로컬라이제이션 전문 번역가입니다.
                
                당신의 역할:
                1. 게임 텍스트를 목표 언어로 번역합니다
                2. 게임 용어와 톤을 일관되게 유지합니다
                3. 문화적 맥락을 고려하여 적절히_localize 합니다
                4. 플레이어에게 자연스러운 표현을 사용합니다
                
                번역 원칙:
                - 직역보다 의역을 우선합니다
                - 게임 세계관의 톤을 유지합니다
                - 문화적 금기를 피합니다
                - UI 길이 제한을 고려합니다
                
                출력 형식:
                - 원문: [원본 텍스트]
                - 번역: [번역문]
                - 용어설명: [주요 용어 설명]
                - 문화적고려: [문화적 조정 사항]
                """,
            name: "TranslatorAgent"
        );
    }

    public async Task<string> TranslateAsync(string sourceText, string targetLanguage)
    {
        var prompt = $$"""
            다음 게임 텍스트를 {{targetLanguage}}로 번역해주세요.
            
            원문:
            {{sourceText}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 인간 검수자 에이전트 (시뮬레이션)
/// </summary>
public class HumanReviewerAgent
{
    private readonly AIAgent _agent;

    public HumanReviewerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 인간 로컬라이제이션 검수자로서 AI 번역을 검토합니다.
                
                당신의 역할:
                1. 번역의 정확성을 평가합니다
                2. 자연스러움을 확인합니다
                3. 오류와 부적절한 표현을 지적합니다
                4. 수정안을 제시합니다
                
                검수 기준:
                - 문법적 정확성
                - 자연스러운 표현
                - 게임 톤앤매너 일치
                - 문화적 적절성
                - UI 제약 준수
                
                출력 형식:
                - 검수의견: [상세 의견]
                - 수정사항: [구체적 수정안]
                - 오류목록: [발견된 오류]
                - 승인여부: [승인/수정필요/재번역]
                """,
            name: "HumanReviewerAgent"
        );
    }

    public async Task<string> ReviewAsync(string originalText, string translation, string targetLanguage)
    {
        var prompt = $$"""
            AI 번역을 검수해주세요.
            
            원문:
            {{originalText}}
            
            AI 번역 ({{targetLanguage}}):
            {{translation}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}

/// <summary>
/// 수정 및 완성 에이전트
/// </summary>
public class RevisionAgent
{
    private readonly AIAgent _agent;

    public RevisionAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 로컬라이제이션 수정 전문가입니다.
                
                당신의 역할:
                1. 인간 검수자의 피드백을 반영합니다
                2. 번역을 수정하고 완성합니다
                3. 최종 품질을 보장합니다
                4. 일관성을 검증합니다
                
                수정 원칙:
                - 모든 피드백을 반영합니다
                - 원문의 의미를 왜곡하지 않습니다
                - 목표 언어의 자연스러움을 우선합니다
                - 게임 용어집과 일치시킵니다
                
                출력 형식:
                - 수정번역: [최종 번역문]
                - 수정내용: [변경 사항 설명]
                - 품질평가: [품질 점수 및 평가]
                - 최종권고: [배포 권고 여부]
                """,
            name: "RevisionAgent"
        );
    }

    public async Task<string> ReviseAsync(string originalText, string translation, string reviewFeedback)
    {
        var prompt = $$"""
            검수 피드백을 반영하여 번역을 수정해주세요.
            
            원문:
            {{originalText}}
            
            AI 번역:
            {{translation}}
            
            검수자 피드백:
            {{reviewFeedback}}
            """;
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
