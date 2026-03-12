// Copyright (c) Microsoft. All rights reserved.

using _13B_ResourceAllocator.Agents;
using _13B_ResourceAllocator.Services;

// ==========================================
// 13 단계 B: 리소스 배분 최적화
// ==========================================
// 학습 목표:
// 1. 팀원 역량과 작업량 분석
// 2. 스킬 기반 매칭
// 3. 작업량 균형 최적화
// 4. 과거 이력 활용
// ==========================================

Console.WriteLine("👥 리소스 배분 최적화 시스템에 오신 것을 환영합니다!");
Console.WriteLine("팀원 역량과 작업량을 분석하여 최적 배정을 제안합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var resourceService = new ResourceDataService();
var allocationAgent = new ResourceAllocationAgent(apiKey, baseUrl);

Console.WriteLine("✅ 리소스 배분 도구가 초기화되었습니다.\n");

// 데이터 로드
Console.WriteLine("📊 팀원 정보를 가져오는 중...");
var teamMembers = resourceService.GetTeamMembers();
Console.WriteLine($"✅ {teamMembers.Count}명의 팀원을 로드했습니다.\n");

Console.WriteLine("팀원 현황:");
foreach (var member in teamMembers)
{
    Console.WriteLine($"  {member.Id}: {member.Name} ({member.Role})");
    Console.WriteLine($"     스킬: {string.Join(", ", member.Skills)}");
    Console.WriteLine($"     가용률: {member.AvailableCapacity:P0} (현재부하: {member.CurrentWorkload:P0})");
}
Console.WriteLine();

Console.WriteLine("📋 작업 아이템을 가져오는 중...");
var workItems = resourceService.GetWorkItems();
Console.WriteLine($"✅ {workItems.Count}개의 작업을 로드했습니다.\n");

Console.WriteLine("작업 목록:");
foreach (var work in workItems)
{
    Console.WriteLine($"  {work.Id}: {work.Title}");
    Console.WriteLine($"     필수스킬: {string.Join(", ", work.RequiredSkills)}, {work.EstimatedHours}h");
    Console.WriteLine($"     우선순위: {work.Priority}, 마감: {work.Deadline:MM/dd}");
}
Console.WriteLine();

Console.WriteLine("📈 과거 배분 이력을 가져오는 중...");
var history = resourceService.GetAllocationHistory();
Console.WriteLine($"✅ {history.Count}개의 이력을 로드했습니다.\n");

Console.WriteLine("👥 리소스 배분 최적화 시작...");
var allocationResult = await allocationAgent.AllocateResourcesAsync(teamMembers, workItems, history);

Console.WriteLine($"\n📊 리소스 배분 결과:\n{allocationResult}\n");

Console.WriteLine("\n📈 작업량 분석:");
var workloadAnalysis = await allocationAgent.AnalyzeWorkloadAsync(teamMembers);
Console.WriteLine($"\n{workloadAnalysis}\n");

Console.WriteLine("\n계속하려면 아무 키나 누르세요...");
Console.ReadKey();
