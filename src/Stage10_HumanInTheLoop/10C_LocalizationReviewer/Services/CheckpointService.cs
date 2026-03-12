namespace _10C_LocalizationReviewer.Services;

/// <summary>
/// 체크포인트 데이터
/// </summary>
public class Checkpoint
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public string HumanFeedback { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsApproved { get; set; }
    public string ContentType { get; set; } = ""; // Translation, Review, Revision
}

/// <summary>
/// 체크포인트 서비스 - 상태 저장 및 복원
/// </summary>
public class CheckpointService
{
    private readonly List<Checkpoint> _checkpoints = new();
    private int _currentId = 0;

    /// <summary>
    /// 새로운 체크포인트 생성
    /// </summary>
    public Checkpoint CreateCheckpoint(string content, string contentType = "General")
    {
        var checkpoint = new Checkpoint
        {
            Id = ++_currentId,
            Content = content,
            ContentType = contentType,
            CreatedAt = DateTime.Now
        };
        
        _checkpoints.Add(checkpoint);
        Console.WriteLine($"💾 체크포인트 {_currentId} 저장됨 ({contentType})");
        
        return checkpoint;
    }

    /// <summary>
    /// 체크포인트에 인간 피드백 추가
    /// </summary>
    public void AddFeedback(int checkpointId, string feedback)
    {
        var checkpoint = _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
        if (checkpoint != null)
        {
            checkpoint.HumanFeedback = feedback;
            Console.WriteLine($"✏️ 체크포인트 {checkpointId} 에 피드백 추가");
        }
    }

    /// <summary>
    /// 체크포인트 승인
    /// </summary>
    public void ApproveCheckpoint(int checkpointId)
    {
        var checkpoint = _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
        if (checkpoint != null)
        {
            checkpoint.IsApproved = true;
            Console.WriteLine($"✅ 체크포인트 {checkpointId} 승인됨");
        }
    }

    /// <summary>
    /// 특정 체크포인트로 시간 이동 (Time-travel)
    /// </summary>
    public Checkpoint? TravelToCheckpoint(int checkpointId)
    {
        var checkpoint = _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
        if (checkpoint != null)
        {
            Console.WriteLine($"⏰ 체크포인트 {checkpointId} 으로 시간 이동");
            return checkpoint;
        }
        
        Console.WriteLine($"❌ 체크포인트 {checkpointId} 를 찾을 수 없음");
        return null;
    }

    /// <summary>
    /// 모든 체크포인트 목록
    /// </summary>
    public List<Checkpoint> GetAllCheckpoints()
    {
        return new List<Checkpoint>(_checkpoints);
    }

    /// <summary>
    /// 최신 체크포인트 가져오기
    /// </summary>
    public Checkpoint? GetLatestCheckpoint()
    {
        return _checkpoints.LastOrDefault();
    }
}
