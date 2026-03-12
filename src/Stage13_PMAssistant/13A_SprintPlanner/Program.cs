// Copyright (c) Microsoft. All rights reserved.

using _13A_SprintPlanner.Agents;
using _13A_SprintPlanner.Services;

// ==========================================
// 13 단계 A: 스프린트 계획 어시스턴트
// ==========================================
// 학습 목표:
// 1. 다중 데이터 소스 통합 (Jira + Git)
// 2. 상태 관리 및 추적
// 3. 장기 메모리 구현
// ==========================================

Console.WriteLine("📋 스프린트 계획 어시스턴트에 오신 것을 환영합니다!");
Console.WriteLine("Jira 와 Git 데이터를 통합하여 스프린트를 계획합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var dataService = new DataIntegrationService();
var planningAgent = new SprintPlanningAgent(apiKey, baseUrl);

Console.WriteLine("✅ 스프린트 계획 도구가 초기화되었습니다.\n");

// 데모용 스프린트 설정
var projectId = "PROJ";
var sprintVelocity = 30; // 스토리 포인트

Console.WriteLine("📊 백로그 아이템을 가져오는 중...");
var issues = dataService.FetchJiraIssues(projectId);
Console.WriteLine($"✅ {issues.Count}개 이슈를 가져왔습니다.\n");

Console.WriteLine("백로그:");
foreach (var issue in issues)
{
    Console.WriteLine($"  {issue.Id}: {issue.Title} ({issue.StoryPoints}점)");
}
Console.WriteLine();

Console.WriteLine("📋 스프린트 계획 수립 중...");
var sprintPlan = await planningAgent.CreateSprintPlanAsync(issues, sprintVelocity);

Console.WriteLine($"\n📊 스프린트 계획:\n{sprintPlan}\n");

Console.WriteLine("계속하려면 아무 키나 누르세요...");
Console.ReadKey();
