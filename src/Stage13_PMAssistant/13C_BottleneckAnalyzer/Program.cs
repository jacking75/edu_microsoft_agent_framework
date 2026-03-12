// Copyright (c) Microsoft. All rights reserved.

using _13C_BottleneckAnalyzer.Agents;
using _13C_BottleneckAnalyzer.Services;

// ==========================================
// 13 단계 C: 병목 분석 대시보드
// ==========================================
// 학습 목표:
// 1. 생산성 메트릭 분석
// 2. 장기 메모리 활용
// 3. 병목 지점 식별
// 4. 개선 계획 수립
// ==========================================

Console.WriteLine("📈 병목 분석 대시보드에 오신 것을 환영합니다!");
Console.WriteLine("생산성 메트릭을 분석하여 병목 지점을 식별합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var metricsService = new ProductivityMetricsService();
var bottleneckAgent = new BottleneckAnalyzerAgent(apiKey, baseUrl);

Console.WriteLine("✅ 병목 분석 도구가 초기화되었습니다.\n");

// 데이터 로드
Console.WriteLine("📊 스프린트 메트릭을 가져오는 중...");
var sprintMetrics = metricsService.GetSprintMetrics();
Console.WriteLine($"✅ {sprintMetrics.Count}개의 스프린트 메트릭을 로드했습니다.\n");

Console.WriteLine("스프린트 메트릭:");
foreach (var metric in sprintMetrics)
{
    var status = metric.CompletionRate >= 0.8 ? "🟢" : metric.CompletionRate >= 0.6 ? "🟡" : "🔴";
    Console.WriteLine($"  {status} {metric.SprintId}: {metric.PlannedPoints}점 → {metric.CompletedPoints}점 ({metric.CompletionRate:P1})");
    Console.WriteLine($"     작업: {metric.CompletedTasks}/{metric.TotalTasks}, 버그: {metric.BugCount}개, 사기: {metric.TeamMorale}/5");
}
Console.WriteLine();

Console.WriteLine("📈 팀원별 생산성 메트릭:");
var memberMetrics = metricsService.GetMemberProductivity();
foreach (var member in memberMetrics)
{
    Console.WriteLine($"  {member.Name}: {member.CompletedTasks}작업 완료, 평균 {member.AverageHours}h, 품질 {member.QualityScore}/5");
}
Console.WriteLine();

// 장기 메모리에 저장
Console.WriteLine("💾 장기 메모리에 메트릭 저장 중...");
foreach (var metric in sprintMetrics)
{
    metricsService.SaveSprintMetric(metric);
}
Console.WriteLine("✅ 메트릭 저장 완료\n");

// 병목 분석
Console.WriteLine("🔍 병목 분석 시작...");
var bottleneckAnalysis = await bottleneckAgent.AnalyzeBottlenecksAsync(sprintMetrics, memberMetrics);

Console.WriteLine($"\n📊 병목 분석 결과:\n{bottleneckAnalysis}\n");

// 개선 계획
Console.WriteLine("📋 개선 계획 수립 중...");
var improvementPlan = await bottleneckAgent.GenerateImprovementPlanAsync(bottleneckAnalysis, sprintMetrics);

Console.WriteLine($"\n📈 개선 계획:\n{improvementPlan}\n");

// 다음 스프린트 예측
Console.WriteLine("🔮 다음 스프린트 예측 중...");
var nextPlannedPoints = 30;
var prediction = await bottleneckAgent.PredictNextSprintAsync(sprintMetrics, nextPlannedPoints);

Console.WriteLine($"\n📊 다음 스프린트 예측 (계획: {nextPlannedPoints}점):\n{prediction}\n");

// 유사 스프린트 검색
Console.WriteLine("🔍 유사한 과거 스프린트 검색 중...");
var similarSprint = metricsService.FindSimilarSprint(nextPlannedPoints, 4);
if (similarSprint != null)
{
    Console.WriteLine($"\n✅ 유사 스프린트 발견: {similarSprint.SprintId}");
    Console.WriteLine($"   계획: {similarSprint.PlannedPoints}점 → 완료: {similarSprint.CompletedPoints}점 ({similarSprint.CompletionRate:P1})");
}

Console.WriteLine("\n\n계속하려면 아무 키나 누르세요...");
Console.ReadKey();
