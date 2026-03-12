namespace _13C_BottleneckAnalyzer.Agents;

using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using System.ClientModel;
using _13C_BottleneckAnalyzer.Services;

/// <summary>
/// 병목 분석 에이전트
/// </summary>
public class BottleneckAnalyzerAgent
{
    private readonly AIAgent _agent;

    public BottleneckAnalyzerAgent(string apiKey, string baseUrl)
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        _agent = chatClient.AsAIAgent(
            instructions: """
                당신은 생산성 병목 분석 전문가입니다.
                
                당신의 역할:
                1. 스프린트 메트릭을 분석하여 병목 지점을 식별합니다
                2. 추세를 분석하여 악화/개선 여부를 판단합니다
                3. 근본 원인을 파악합니다
                4. 개선 방안을 제안합니다
                
                분석 항목:
                - 완료율 (Completed/Planned)
                - 작업 완료율 (Tasks Completed)
                - 버그 발생률
                - 팀 사기
                - 벨로시티 추세
                - 개인별 생산성 편차
                
                병목 유형:
                - 인력 부족: 작업량 대비 팀원 부족
                - 스킬 갭: 필요한 기술 부재
                - 프로세스: 승인/검토 지연
                - 기술부채: 버그 수정에 시간 소요
                - 번아웃: 팀 사기 저하
                
                출력 형식:
                - 병목지점: [식별된 문제]
                - 추세분석: [악화/개선/유지]
                - 근본원인: [Root Cause]
                - 영향도: [High/Medium/Low]
                - 개선방안: [Action Items]
                - 우선순위: [즉시/단기/장기]
                """,
            name: "BottleneckAnalyzerAgent"
        );
    }

    public async Task<string> AnalyzeBottlenecksAsync(List<SprintMetric> metrics, List<MemberProductivity> memberMetrics)
    {
        var metricsText = string.Join("\n", metrics.Select(m => 
            $"{m.SprintId}: 계획 {m.PlannedPoints}점 → 완료 {m.CompletedPoints}점 ({m.CompletionRate:P1}), " +
            $"작업 {m.CompletedTasks}/{m.TotalTasks}, 버그 {m.BugCount}개, 사기 {m.TeamMorale}/5"
        ));

        var memberText = string.Join("\n", memberMetrics.Select(m => 
            $"{m.Name}: 완료 {m.CompletedTasks}작업, 평균 {m.AverageHours}h, 품질 {m.QualityScore}/5"
        ));

        var prompt = $"""
            다음 스프린트 메트릭을 분석하여 병목 지점을 찾아주세요:
            
            스프린트 메트릭:
            {metricsText}
            
            팀원별 생산성:
            {memberText}
            
            병목 지점, 추세, 근본 원인, 개선 방안을 상세히 분석해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }

    public async Task<string> GenerateImprovementPlanAsync(string bottleneckAnalysis, List<SprintMetric> metrics)
    {
        var trendText = metrics.Count >= 2 
            ? $"최근 2 개 스프린트 완료율: {metrics[^2].CompletionRate:P1} → {metrics[^1].CompletionRate:P1}"
            : "충분한 데이터 없음";

        var prompt = $"""
            다음 병목 분석을 바탕으로 개선 계획을 수립해주세요:
            
            병목 분석:
            {bottleneckAnalysis}
            
            추세:
            {trendText}
            
            실행 가능한 개선 계획을 30-60-90 일 플랜으로 수립해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }

    public async Task<string> PredictNextSprintAsync(List<SprintMetric> metrics, int plannedPoints)
    {
        var metricsText = string.Join("\n", metrics.Select(m => 
            $"{m.SprintId}: {m.PlannedPoints}점 → {m.CompletedPoints}점 ({m.CompletionRate:P1})"
        ));

        var prompt = $"""
            과거 스프린트 데이터를 바탕으로 다음 스프린트를 예측해주세요:
            
            과거 데이터:
            {metricsText}
            
            다음 스프린트 계획: {plannedPoints}점
            
            예상 완료 포인트, 리스크, 권장사항을 제안해주세요.
            """;
        
        var response = await _agent.RunAsync(prompt);
        return response.ToString() ?? "";
    }
}
