namespace _13C_BottleneckAnalyzer.Services;

/// <summary>
/// 생산성 메트릭 및 장기 메모리 서비스
/// </summary>
public class ProductivityMetricsService
{
    private readonly LongTermMemory _memory = new();

    /// <summary>
    /// 스프린트 메트릭 가져오기
    /// </summary>
    public List<SprintMetric> GetSprintMetrics()
    {
        return new List<SprintMetric>
        {
            new() 
            { 
                SprintId = "SPR-001", 
                StartDate = DateTime.Now.AddDays(-60),
                EndDate = DateTime.Now.AddDays(-46),
                PlannedPoints = 35,
                CompletedPoints = 30,
                TotalTasks = 12,
                CompletedTasks = 10,
                BugCount = 5,
                TeamMorale = 4.2
            },
            new() 
            { 
                SprintId = "SPR-002", 
                StartDate = DateTime.Now.AddDays(-45),
                EndDate = DateTime.Now.AddDays(-31),
                PlannedPoints = 32,
                CompletedPoints = 28,
                TotalTasks = 10,
                CompletedTasks = 8,
                BugCount = 8,
                TeamMorale = 3.5
            },
            new() 
            { 
                SprintId = "SPR-003", 
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddDays(-16),
                PlannedPoints = 30,
                CompletedPoints = 25,
                TotalTasks = 11,
                CompletedTasks = 8,
                BugCount = 12,
                TeamMorale = 3.0
            },
            new() 
            { 
                SprintId = "SPR-004", 
                StartDate = DateTime.Now.AddDays(-15),
                EndDate = DateTime.Now.AddDays(-1),
                PlannedPoints = 28,
                CompletedPoints = 22,
                TotalTasks = 9,
                CompletedTasks = 6,
                BugCount = 15,
                TeamMorale = 2.8
            }
        };
    }

    /// <summary>
    /// 팀원별 생산성 메트릭
    /// </summary>
    public List<MemberProductivity> GetMemberProductivity()
    {
        return new List<MemberProductivity>
        {
            new() { MemberId = "TM001", Name = "김개발", CompletedTasks = 8, AverageHours = 45, QualityScore = 4.5 },
            new() { MemberId = "TM002", Name = "이디자인", CompletedTasks = 6, AverageHours = 38, QualityScore = 4.8 },
            new() { MemberId = "TM003", Name = "박테스터", CompletedTasks = 5, AverageHours = 35, QualityScore = 4.2 },
            new() { MemberId = "TM004", Name = "최주니어", CompletedTasks = 4, AverageHours = 42, QualityScore = 3.5 }
        };
    }

    /// <summary>
    /// 장기 메모리에 스프린트 저장
    /// </summary>
    public void SaveSprintMetric(SprintMetric metric)
    {
        _memory.SaveSprint(metric);
    }

    /// <summary>
    /// 유사한 과거 스프린트 검색
    /// </summary>
    public SprintMetric? FindSimilarSprint(int plannedPoints, int teamSize)
    {
        return _memory.FindSimilarSprint(plannedPoints, teamSize);
    }

    /// <summary>
    /// 전체 스프린트 이력 가져오기
    /// </summary>
    public List<SprintMetric> GetAllSprintHistory()
    {
        return _memory.GetAllSprints();
    }
}

/// <summary>
/// 스프린트 메트릭
/// </summary>
public class SprintMetric
{
    public string SprintId { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PlannedPoints { get; set; }
    public int CompletedPoints { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int BugCount { get; set; }
    public double TeamMorale { get; set; }
    public double CompletionRate => PlannedPoints > 0 ? (double)CompletedPoints / PlannedPoints : 0;
    public double TaskCompletionRate => TotalTasks > 0 ? (double)CompletedTasks / TotalTasks : 0;
}

/// <summary>
/// 팀원별 생산성
/// </summary>
public class MemberProductivity
{
    public string MemberId { get; set; } = "";
    public string Name { get; set; } = "";
    public int CompletedTasks { get; set; }
    public int AverageHours { get; set; }
    public double QualityScore { get; set; }
}

/// <summary>
/// 장기 메모리 - 스프린트 이력 저장 및 검색
/// </summary>
public class LongTermMemory
{
    private readonly List<SprintMetric> _sprintHistory = new();

    public LongTermMemory()
    {
        // 초기 데이터 로드
        _sprintHistory.AddRange(new List<SprintMetric>
        {
            new() { SprintId = "SPR-000", StartDate = DateTime.Now.AddDays(-90), EndDate = DateTime.Now.AddDays(-76), PlannedPoints = 40, CompletedPoints = 38, TotalTasks = 15, CompletedTasks = 14, BugCount = 3, TeamMorale = 4.5 },
            new() { SprintId = "SPR-001", StartDate = DateTime.Now.AddDays(-75), EndDate = DateTime.Now.AddDays(-61), PlannedPoints = 35, CompletedPoints = 30, TotalTasks = 12, CompletedTasks = 10, BugCount = 5, TeamMorale = 4.2 }
        });
    }

    public void SaveSprint(SprintMetric sprint)
    {
        _sprintHistory.Add(sprint);
    }

    public SprintMetric? FindSimilarSprint(int plannedPoints, int teamSize)
    {
        return _sprintHistory
            .OrderByDescending(s => CalculateSimilarity(s, plannedPoints))
            .FirstOrDefault();
    }

    public List<SprintMetric> GetAllSprints()
    {
        return new List<SprintMetric>(_sprintHistory);
    }

    private double CalculateSimilarity(SprintMetric sprint, int targetPoints)
    {
        var pointsDiff = Math.Abs(sprint.PlannedPoints - targetPoints);
        return 100 - pointsDiff; // 간단한 유사도 계산
    }
}
