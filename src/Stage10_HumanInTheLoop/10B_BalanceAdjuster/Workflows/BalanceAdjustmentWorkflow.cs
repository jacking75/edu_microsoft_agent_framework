namespace _10B_BalanceAdjuster.Workflows;

using _10B_BalanceAdjuster.Agents;
using _10B_BalanceAdjuster.Services;

/// <summary>
/// 밸런스 조정 워크플로우 결과
/// </summary>
public class BalanceAdjustmentResult
{
    public string GameData { get; set; } = "";
    public string BalanceProposal { get; set; } = "";
    public string DesignerReview { get; set; } = "";
    public string SimulationResult { get; set; } = "";
    public string FinalRecommendation { get; set; } = "";
    public int TotalCheckpoints { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// Human-in-the-Loop 밸런스 조정 워크플로우
/// AI 제안 → 인간검토 → 시뮬레이션
/// </summary>
public class BalanceAdjustmentWorkflow
{
    private readonly BalanceAgent _balanceAgent;
    private readonly DesignerReviewAgent _designerReviewAgent;
    private readonly SimulationAgent _simulationAgent;
    private readonly CheckpointService _checkpointService;

    public BalanceAdjustmentWorkflow(
        BalanceAgent balanceAgent,
        DesignerReviewAgent designerReviewAgent,
        SimulationAgent simulationAgent,
        CheckpointService checkpointService)
    {
        _balanceAgent = balanceAgent;
        _designerReviewAgent = designerReviewAgent;
        _simulationAgent = simulationAgent;
        _checkpointService = checkpointService;
    }

    /// <summary>
    /// Human-in-the-Loop 워크플로우 실행
    /// </summary>
    public async Task<BalanceAdjustmentResult> ExecuteAsync(string gameData, string designIntent)
    {
        Console.WriteLine($"\n⚖️ 밸런스 조정 워크플로우 시작");
        Console.WriteLine(new string('-', 60));

        var result = new BalanceAdjustmentResult { GameData = gameData };

        // Step 1: AI 밸런스 제안
        Console.WriteLine("\n💡 Step 1: AI 밸런스 제안 중...");
        result.BalanceProposal = await _balanceAgent.ProposeBalanceAsync(gameData);
        
        var checkpoint1 = _checkpointService.CreateCheckpoint(result.BalanceProposal, "Proposal");
        Console.WriteLine($"✅ AI 제안 완료 (체크포인트 {checkpoint1.Id})");
        Console.WriteLine($"   {Truncate(result.BalanceProposal, 80)}");

        // Step 2: 인간 디자이너 검토 (시뮬레이션)
        Console.WriteLine("\n👤 Step 2: 인간 디자이너 검토 중...");
        result.DesignerReview = await _designerReviewAgent.ReviewAsync(result.BalanceProposal, designIntent);
        
        var checkpoint2 = _checkpointService.CreateCheckpoint(result.DesignerReview, "Review");
        Console.WriteLine($"✅ 디자이너 검토 완료 (체크포인트 {checkpoint2.Id})");
        Console.WriteLine($"   {Truncate(result.DesignerReview, 80)}");

        // Step 3: 시뮬레이션
        Console.WriteLine("\n🔬 Step 3: 시뮬레이션 수행 중...");
        result.SimulationResult = await _simulationAgent.SimulateAsync(result.BalanceProposal, result.DesignerReview);
        
        var checkpoint3 = _checkpointService.CreateCheckpoint(result.SimulationResult, "Simulation");
        Console.WriteLine($"✅ 시뮬레이션 완료 (체크포인트 {checkpoint3.Id})");
        Console.WriteLine($"   {Truncate(result.SimulationResult, 80)}");

        // Step 4: 최종 승인
        Console.WriteLine("\n✅ Step 4: 최종 승인 대기 중...");
        _checkpointService.ApproveCheckpoint(checkpoint3.Id);
        
        result.FinalRecommendation = GenerateFinalRecommendation(result.SimulationResult);
        result.TotalCheckpoints = _checkpointService.GetAllCheckpoints().Count;

        Console.WriteLine("\n" + new string('-', 60));
        Console.WriteLine("✅ 밸런스 조정 워크플로우 완료!");
        Console.WriteLine($"   총 체크포인트: {result.TotalCheckpoints}개");

        return result;
    }

    /// <summary>
    /// 시간 이동 기능 데모
    /// </summary>
    public async Task<BalanceAdjustmentResult> TravelAndReReviewAsync(int targetCheckpointId, string newFeedback)
    {
        Console.WriteLine($"\n⏰ 시간 이동: 체크포인트 {targetCheckpointId} 으로");
        
        var checkpoint = _checkpointService.TravelToCheckpoint(targetCheckpointId);
        if (checkpoint == null)
        {
            throw new InvalidOperationException($"체크포인트 {targetCheckpointId} 를 찾을 수 없습니다.");
        }

        // 해당 체크포인트에서 새로운 피드백으로 재검토
        Console.WriteLine($"\n👤 새로운 피드백으로 재검토: {newFeedback}");
        
        string newReview, newSimulation;
        
        if (checkpoint.ContentType == "Proposal")
        {
            newReview = await _designerReviewAgent.ReviewAsync(checkpoint.Content, newFeedback);
            var newCheckpoint = _checkpointService.CreateCheckpoint(newReview, "Review");
            
            newSimulation = await _simulationAgent.SimulateAsync(checkpoint.Content, newReview);
            _checkpointService.CreateCheckpoint(newSimulation, "Simulation");
        }
        else
        {
            newSimulation = await _simulationAgent.SimulateAsync(checkpoint.Content, newFeedback);
            var newCheckpoint = _checkpointService.CreateCheckpoint(newSimulation, "Simulation");
            newReview = newFeedback;
        }

        Console.WriteLine($"✅ 새로운 타임라인 생성 (체크포인트 {_checkpointService.GetAllCheckpoints().Count})");

        return new BalanceAdjustmentResult
        {
            GameData = "시간 이동 재검토",
            BalanceProposal = checkpoint.ContentType == "Proposal" ? checkpoint.Content : "",
            DesignerReview = newReview,
            SimulationResult = newSimulation,
            FinalRecommendation = GenerateFinalRecommendation(newSimulation),
            TotalCheckpoints = _checkpointService.GetAllCheckpoints().Count
        };
    }

    private static string GenerateFinalRecommendation(string simulationResult)
    {
        if (simulationResult.Contains("Pass", StringComparison.OrdinalIgnoreCase))
        {
            return "✅ 시뮬레이션 합격 - 조정안 적용 권장";
        }
        else if (simulationResult.Contains("Fail", StringComparison.OrdinalIgnoreCase))
        {
            return "❌ 시뮬레이션 불합격 - 추가 조정 필요";
        }
        return "⚠️ 조건부 합격 - 모니터링 필요";
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
