namespace _10C_LocalizationReviewer.Workflows;

using _10C_LocalizationReviewer.Agents;
using _10C_LocalizationReviewer.Services;

/// <summary>
/// 로컬라이제이션 검수 워크플로우 결과
/// </summary>
public class LocalizationReviewResult
{
    public string OriginalText { get; set; } = "";
    public string TargetLanguage { get; set; } = "";
    public string Translation { get; set; } = "";
    public string HumanReview { get; set; } = "";
    public string RevisedTranslation { get; set; } = "";
    public string FinalQuality { get; set; } = "";
    public int TotalCheckpoints { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// Human-in-the-Loop 로컬라이제이션 검수 워크플로우
/// AI 번역 → 인간검토 → 수정반영
/// </summary>
public class LocalizationReviewWorkflow
{
    private readonly TranslatorAgent _translatorAgent;
    private readonly HumanReviewerAgent _humanReviewerAgent;
    private readonly RevisionAgent _revisionAgent;
    private readonly CheckpointService _checkpointService;

    public LocalizationReviewWorkflow(
        TranslatorAgent translatorAgent,
        HumanReviewerAgent humanReviewerAgent,
        RevisionAgent revisionAgent,
        CheckpointService checkpointService)
    {
        _translatorAgent = translatorAgent;
        _humanReviewerAgent = humanReviewerAgent;
        _revisionAgent = revisionAgent;
        _checkpointService = checkpointService;
    }

    /// <summary>
    /// Human-in-the-Loop 워크플로우 실행
    /// </summary>
    public async Task<LocalizationReviewResult> ExecuteAsync(string originalText, string targetLanguage)
    {
        Console.WriteLine($"\n🌐 로컬라이제이션 검수 워크플로우 시작");
        Console.WriteLine(new string('-', 60));

        var result = new LocalizationReviewResult 
        { 
            OriginalText = originalText,
            TargetLanguage = targetLanguage
        };

        // Step 1: AI 번역
        Console.WriteLine($"\n🔤 Step 1: AI 번역 중... ({targetLanguage})");
        result.Translation = await _translatorAgent.TranslateAsync(originalText, targetLanguage);
        
        var checkpoint1 = _checkpointService.CreateCheckpoint(result.Translation, "Translation");
        Console.WriteLine($"✅ AI 번역 완료 (체크포인트 {checkpoint1.Id})");
        Console.WriteLine($"   {Truncate(result.Translation, 80)}");

        // Step 2: 인간 검수 (시뮬레이션)
        Console.WriteLine("\n👤 Step 2: 인간 검수자 검토 중...");
        result.HumanReview = await _humanReviewerAgent.ReviewAsync(originalText, result.Translation, targetLanguage);
        
        var checkpoint2 = _checkpointService.CreateCheckpoint(result.HumanReview, "Review");
        Console.WriteLine($"✅ 인간 검수 완료 (체크포인트 {checkpoint2.Id})");
        Console.WriteLine($"   {Truncate(result.HumanReview, 80)}");

        // Step 3: 수정 및 완성
        Console.WriteLine("\n✏️ Step 3: 수정 및 완성 중...");
        result.RevisedTranslation = await _revisionAgent.ReviseAsync(originalText, result.Translation, result.HumanReview);
        
        var checkpoint3 = _checkpointService.CreateCheckpoint(result.RevisedTranslation, "Revision");
        Console.WriteLine($"✅ 수정 완료 (체크포인트 {checkpoint3.Id})");
        Console.WriteLine($"   {Truncate(result.RevisedTranslation, 80)}");

        // Step 4: 최종 승인
        Console.WriteLine("\n✅ Step 4: 최종 승인 대기 중...");
        _checkpointService.ApproveCheckpoint(checkpoint3.Id);
        
        result.FinalQuality = ExtractQualityAssessment(result.RevisedTranslation);
        result.TotalCheckpoints = _checkpointService.GetAllCheckpoints().Count;

        Console.WriteLine("\n" + new string('-', 60));
        Console.WriteLine("✅ 로컬라이제이션 검수 워크플로우 완료!");
        Console.WriteLine($"   총 체크포인트: {result.TotalCheckpoints}개");

        return result;
    }

    /// <summary>
    /// 시간 이동 기능 데모
    /// </summary>
    public async Task<LocalizationReviewResult> TravelAndReviseAsync(
        int targetCheckpointId, 
        string newFeedback,
        string originalText,
        string targetLanguage,
        string translation)
    {
        Console.WriteLine($"\n⏰ 시간 이동: 체크포인트 {targetCheckpointId} 으로");
        
        var checkpoint = _checkpointService.TravelToCheckpoint(targetCheckpointId);
        if (checkpoint == null)
        {
            throw new InvalidOperationException($"체크포인트 {targetCheckpointId} 를 찾을 수 없습니다.");
        }

        // 해당 체크포인트에서 새로운 피드백으로 재수정
        Console.WriteLine($"\n✏️ 새로운 피드백으로 재수정: {newFeedback}");
        
        string newRevision;
        
        if (checkpoint.ContentType == "Translation")
        {
            // 번역 체크포인트면 검수부터 다시
            var newReview = await _humanReviewerAgent.ReviewAsync(
                originalText, 
                checkpoint.Content, 
                targetLanguage);
            _checkpointService.CreateCheckpoint(newReview, "Review");
            
            newRevision = await _revisionAgent.ReviseAsync(
                originalText, 
                checkpoint.Content, 
                newReview + "\n추가피드백: " + newFeedback);
        }
        else
        {
            // 검수 또는 수정 체크포인트면 바로 재수정
            newRevision = await _revisionAgent.ReviseAsync(
                originalText,
                translation,
                checkpoint.Content + "\n추가피드백: " + newFeedback);
        }
        
        var newCheckpoint = _checkpointService.CreateCheckpoint(newRevision, "Revision");
        Console.WriteLine($"✅ 새로운 타임라인 생성 (체크포인트 {newCheckpoint.Id})");

        return new LocalizationReviewResult
        {
            OriginalText = originalText,
            TargetLanguage = targetLanguage,
            Translation = translation,
            HumanReview = newFeedback,
            RevisedTranslation = newRevision,
            FinalQuality = ExtractQualityAssessment(newRevision),
            TotalCheckpoints = _checkpointService.GetAllCheckpoints().Count
        };
    }

    private static string ExtractQualityAssessment(string revisedTranslation)
    {
        if (revisedTranslation.Contains("배포 권고", StringComparison.OrdinalIgnoreCase) ||
            revisedTranslation.Contains("완료", StringComparison.OrdinalIgnoreCase))
        {
            return "✅ 품질 승인 - 배포 가능";
        }
        else if (revisedTranslation.Contains("수정 필요", StringComparison.OrdinalIgnoreCase))
        {
            return "⚠️ 추가 수정 필요";
        }
        return "✅ 검수 완료";
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
