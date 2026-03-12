namespace _10A_StoryCollaborator.Workflows;

using _10A_StoryCollaborator.Agents;
using _10A_StoryCollaborator.Services;

/// <summary>
/// 스토리 협업 워크플로우 결과
/// </summary>
public class StoryCollaborationResult
{
    public string Theme { get; set; } = "";
    public string InitialDraft { get; set; } = "";
    public List<string> Revisions { get; set; } = new();
    public string FinalStory { get; set; } = "";
    public int TotalCheckpoints { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// Human-in-the-Loop 스토리 협업 워크플로우
/// AI 작성 → 인간 검토/수정 → AI 계속 작성 (반복)
/// </summary>
public class StoryCollaborationWorkflow
{
    private readonly DraftAgent _draftAgent;
    private readonly ContinuationAgent _continuationAgent;
    private readonly CheckpointService _checkpointService;

    public StoryCollaborationWorkflow(
        DraftAgent draftAgent,
        ContinuationAgent continuationAgent,
        CheckpointService checkpointService)
    {
        _draftAgent = draftAgent;
        _continuationAgent = continuationAgent;
        _checkpointService = checkpointService;
    }

    /// <summary>
    /// Human-in-the-Loop 워크플로우 실행
    /// </summary>
    public async Task<StoryCollaborationResult> ExecuteAsync(string theme)
    {
        Console.WriteLine($"\n📖 스토리 협업 워크플로우 시작: {theme}");
        Console.WriteLine(new string('-', 60));

        var result = new StoryCollaborationResult { Theme = theme };

        // Step 1: AI 초안 작성
        Console.WriteLine("\n✍️ Step 1: AI 초안 작성 중...");
        result.InitialDraft = await _draftAgent.WriteDraftAsync(theme);
        
        var checkpoint1 = _checkpointService.CreateCheckpoint(result.InitialDraft);
        Console.WriteLine($"✅ 초안 작성 완료 (체크포인트 {checkpoint1.Id})");
        Console.WriteLine($"   {Truncate(result.InitialDraft, 80)}");

        // Step 2: 인간 검토 및 수정 (여기서 사용자 입력 대기)
        Console.WriteLine("\n👤 Step 2: 인간의 검토를 기다리는 중...");
        Console.WriteLine("   (실제 구현에서는 여기서 사용자 입력을 받습니다)");
        
        // 데모용: 자동으로 피드백 생성
        var humanFeedback = "등장인물의 동기를 더 명확히 해주세요. 배경 설명을 보강해주세요.";
        _checkpointService.AddFeedback(checkpoint1.Id, humanFeedback);
        Console.WriteLine($"   피드백: {humanFeedback}");

        // Step 3: AI 가 수정하여 계속 작성
        Console.WriteLine("\n✍️ Step 3: AI 가 피드백을 반영하여 수정 중...");
        var revisedStory = await _continuationAgent.ContinueStoryAsync(
            result.InitialDraft, 
            humanFeedback);
        
        var checkpoint2 = _checkpointService.CreateCheckpoint(revisedStory);
        result.Revisions.Add(revisedStory);
        Console.WriteLine($"✅ 수정 완료 (체크포인트 {checkpoint2.Id})");
        Console.WriteLine($"   {Truncate(revisedStory, 80)}");

        // Step 4: 인간 최종 승인
        Console.WriteLine("\n✅ Step 4: 최종 승인 대기 중...");
        _checkpointService.ApproveCheckpoint(checkpoint2.Id);
        
        result.FinalStory = revisedStory;
        result.TotalCheckpoints = _checkpointService.GetAllCheckpoints().Count;

        Console.WriteLine("\n" + new string('-', 60));
        Console.WriteLine("✅ 스토리 협업 워크플로우 완료!");
        Console.WriteLine($"   총 체크포인트: {result.TotalCheckpoints}개");

        return result;
    }

    /// <summary>
    /// 시간 이동 기능 데모
    /// </summary>
    public async Task<StoryCollaborationResult> TravelAndContinueAsync(int targetCheckpointId, string newDirection)
    {
        Console.WriteLine($"\n⏰ 시간 이동: 체크포인트 {targetCheckpointId} 으로");
        
        var checkpoint = _checkpointService.TravelToCheckpoint(targetCheckpointId);
        if (checkpoint == null)
        {
            throw new InvalidOperationException($"체크포인트 {targetCheckpointId} 를 찾을 수 없습니다.");
        }

        // 해당 체크포인트에서 새로운 방향으로 계속 작성
        Console.WriteLine($"\n✍️ 새로운 방향으로 계속 작성: {newDirection}");
        var newStory = await _continuationAgent.ContinueStoryAsync(
            checkpoint.StoryContent,
            $"이전 내용을 바탕으로: {newDirection}");

        var newCheckpoint = _checkpointService.CreateCheckpoint(newStory);
        Console.WriteLine($"✅ 새로운 타임라인 생성 (체크포인트 {newCheckpoint.Id})");

        return new StoryCollaborationResult
        {
            Theme = "시간 이동 계속쓰기",
            InitialDraft = checkpoint.StoryContent,
            Revisions = { newStory },
            FinalStory = newStory,
            TotalCheckpoints = _checkpointService.GetAllCheckpoints().Count
        };
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
