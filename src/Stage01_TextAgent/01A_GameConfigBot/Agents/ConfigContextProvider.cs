// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using Microsoft.Agents.AI;
using OpenAI.Chat;

namespace _01A_GameConfigBot.Agents;

/// <summary>
/// 게임 설정 데이터를 LLM 컨텍스트에 주입하는 Context Provider
/// 
/// Microsoft Agent Framework 에서 AIContextProvider 를 상속하여:
/// 1. ProvideAIContextAsync: LLM 호출 전에 컨텍스트 데이터 주입
/// 2. StoreAIContextAsync: LLM 호출 후 결과 저장 (선택사항)
/// 
/// 이 클래스는 로드된 게임 설정 JSON 을 직렬화하여
/// 각 LLM 호출마다 시스템 프롬프트에 포함시킵니다.
/// </summary>
public class ConfigContextProvider : AIContextProvider
{
    private readonly Dictionary<string, object> _configData;
    private readonly string _configJson;

    /// <summary>
    /// ConfigContextProvider 생성자
    /// </summary>
    /// <param name="configData">게임 설정 데이터 (사전에 로드된 JSON)</param>
    public ConfigContextProvider(Dictionary<string, object> configData)
        : base(null, null)  // AIContextProvider 기본 생성자 호출
    {
        _configData = configData;
        
        // JSON 을 미리 직렬화하여 성능 최적화
        // WriteIndented = true: 가독성 좋은 형식 (LLM 이 이해하기 쉬움)
        _configJson = JsonSerializer.Serialize(configData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// LLM 호출 전에 호출되는 메서드
    /// 설정 데이터를 AIContext.Instructions 에 포함시켜 LLM 이 접근할 수 있게 함
    /// </summary>
    /// <param name="context">호출 컨텍스트 (세션 정보 등 포함)</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>LLM 에 전달할 추가 컨텍스트 (지침, 메시지, 도구 등)</returns>
    protected override ValueTask<AIContext> ProvideAIContextAsync(
        InvokingContext context, 
        CancellationToken cancellationToken = default)
    {
        // 설정 데이터를 LLM 이 참조할 수 있도록 지침에 포함
        // 이 텍스트는 모든 사용자 메시지에 자동으로 추가됩니다
        var configInstructions = $"""
            === 현재 로드된 게임 설정 데이터 ===
            
            다음 JSON 데이터를 참고하여 사용자의 질문에 정확하게 답변하세요.
            숫자 값, 비율, 설정 등을 이 데이터에서 찾아서 사용하세요.
            
            ```json
            {_configJson}
            ```
            
            === 사용 가이드 ===
            - 사용자가 특정 값을 물어보면 위 JSON 에서 해당 키를 찾아 정확한 값을 반환하세요
            - 계산이 필요한 경우 JSON 의 값을 사용하여 계산하세요
            - 예: "크리티컬 데미지는?" → "criticalMultiplier: 2.0" 을 참고하여 답변
            - 예: "최대 레벨은?" → "maxLevel: 99" 를 참고하여 답변
            """;

        // AIContext 에_instructions 를 설정하여 LLM 에 전달
        return new ValueTask<AIContext>(new AIContext
        {
            Instructions = configInstructions
        });
    }

    /// <summary>
    /// (선택사항) LLM 호출 후에 호출되는 메서드
    /// 대화 기록을 저장하거나 상태를 업데이트할 때 사용
    /// 현재 구현에서는 특별한 저장 작업이 필요하지 않아 기본 동작 사용
    /// </summary>
    protected override ValueTask StoreAIContextAsync(
        InvokedContext context, 
        CancellationToken cancellationToken = default)
    {
        // 기본 동작: 아무 작업도 하지 않음
        // 필요한 경우 여기서 대화 기록을 저장할 수 있음
        return ValueTask.CompletedTask;
    }
}
