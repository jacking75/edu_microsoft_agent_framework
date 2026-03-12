namespace _13B_ResourceAllocator.Services;

/// <summary>
/// 리소스 배분 데이터 통합 서비스
/// </summary>
public class ResourceDataService
{
    /// <summary>
    /// 팀원 정보 가져오기
    /// </summary>
    public List<TeamMember> GetTeamMembers()
    {
        return new List<TeamMember>
        {
            new() 
            { 
                Id = "TM001", 
                Name = "김개발", 
                Role = "시니어 개발자", 
                Skills = new[] { "C#", ".NET", "Azure" },
                Capacity = 1.0,
                CurrentWorkload = 0.6
            },
            new() 
            { 
                Id = "TM002", 
                Name = "이디자인", 
                Role = "UI/UX 디자이너", 
                Skills = new[] { "Figma", "Photoshop", "UI Design" },
                Capacity = 1.0,
                CurrentWorkload = 0.4
            },
            new() 
            { 
                Id = "TM003", 
                Name = "박테스터", 
                Role = "QA 엔지니어", 
                Skills = new[] { "Testing", "Automation", "Selenium" },
                Capacity = 0.5,
                CurrentWorkload = 0.3
            },
            new() 
            { 
                Id = "TM004", 
                Name = "최주니어", 
                Role = "주니어 개발자", 
                Skills = new[] { "JavaScript", "React", "Node.js" },
                Capacity = 0.8,
                CurrentWorkload = 0.5
            }
        };
    }

    /// <summary>
    /// 작업 아이템 가져오기
    /// </summary>
    public List<WorkItem> GetWorkItems()
    {
        return new List<WorkItem>
        {
            new() 
            { 
                Id = "WORK-001", 
                Title = "로그인 모듈 개발", 
                RequiredSkills = new[] { "C#", ".NET" },
                EstimatedHours = 40,
                Priority = "High",
                Deadline = DateTime.Now.AddDays(10)
            },
            new() 
            { 
                Id = "WORK-002", 
                Title = "대시보드 UI 디자인", 
                RequiredSkills = new[] { "Figma", "UI Design" },
                EstimatedHours = 24,
                Priority = "Medium",
                Deadline = DateTime.Now.AddDays(7)
            },
            new() 
            { 
                Id = "WORK-003", 
                Title = "API 통합 테스트", 
                RequiredSkills = new[] { "Testing", "Automation" },
                EstimatedHours = 16,
                Priority = "High",
                Deadline = DateTime.Now.AddDays(5)
            },
            new() 
            { 
                Id = "WORK-004", 
                Title = "프론트엔드 리팩토링", 
                RequiredSkills = new[] { "JavaScript", "React" },
                EstimatedHours = 32,
                Priority = "Low",
                Deadline = DateTime.Now.AddDays(14)
            },
            new() 
            { 
                Id = "WORK-005", 
                Title = "Azure 배포 설정", 
                RequiredSkills = new[] { "C#", "Azure" },
                EstimatedHours = 20,
                Priority = "Medium",
                Deadline = DateTime.Now.AddDays(8)
            }
        };
    }

    /// <summary>
    /// 과거 배분 기록 가져오기
    /// </summary>
    public List<AllocationHistory> GetAllocationHistory()
    {
        return new List<AllocationHistory>
        {
            new() 
            { 
                SprintId = "SPR-001", 
                MemberId = "TM001", 
                WorkItemId = "WORK-010",
                ActualHours = 45,
                EstimatedHours = 40,
                Completed = true,
                Quality = 4.5
            },
            new() 
            { 
                SprintId = "SPR-001", 
                MemberId = "TM002", 
                WorkItemId = "WORK-011",
                ActualHours = 20,
                EstimatedHours = 24,
                Completed = true,
                Quality = 4.8
            },
            new() 
            { 
                SprintId = "SPR-002", 
                MemberId = "TM003", 
                WorkItemId = "WORK-012",
                ActualHours = 18,
                EstimatedHours = 16,
                Completed = true,
                Quality = 4.2
            }
        };
    }
}

/// <summary>
/// 팀원 정보
/// </summary>
public class TeamMember
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string[] Skills { get; set; } = Array.Empty<string>();
    public double Capacity { get; set; } // 1.0 = 풀타임
    public double CurrentWorkload { get; set; } // 0.0 ~ 1.0
    public double AvailableCapacity => Capacity - CurrentWorkload;
}

/// <summary>
/// 작업 아이템
/// </summary>
public class WorkItem
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string[] RequiredSkills { get; set; } = Array.Empty<string>();
    public int EstimatedHours { get; set; }
    public string Priority { get; set; } = "";
    public DateTime Deadline { get; set; }
}

/// <summary>
/// 배분 이력
/// </summary>
public class AllocationHistory
{
    public string SprintId { get; set; } = "";
    public string MemberId { get; set; } = "";
    public string WorkItemId { get; set; } = "";
    public int ActualHours { get; set; }
    public int EstimatedHours { get; set; }
    public bool Completed { get; set; }
    public double Quality { get; set; } // 1-5 점
}
