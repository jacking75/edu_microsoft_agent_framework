namespace _08C_DialogueGenerator.Workflows;

using _08C_DialogueGenerator.Agents;

/// <summary>
/// 대화 생성 워크플로우 결과
/// </summary>
public class DialogueGenerationResult
{
    public string NpcName { get; set; } = "";
    public string NpcRole { get; set; } = "";
    public string Draft { get; set; } = "";
    public string Tone { get; set; } = "";
    public string Branches { get; set; } = "";
    public string DesiredTone { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// Sequential 워크플로우: 대화 생성 파이프라인
/// Draft → Tone → Branch
/// </summary>
public class DialogueGenerationWorkflow
{
    private readonly DraftAgent _draftAgent;
    private readonly ToneAgent _toneAgent;
    private readonly BranchAgent _branchAgent;

    public DialogueGenerationWorkflow(
        DraftAgent draftAgent,
        ToneAgent toneAgent,
        BranchAgent branchAgent)
    {
        _draftAgent = draftAgent;
        _toneAgent = toneAgent;
        _branchAgent = branchAgent;
    }

    /// <summary>
    /// Sequential 워크플로우 실행
    /// </summary>
    public async Task<DialogueGenerationResult> ExecuteAsync(string npcName, string npcRole, string desiredTone)
    {
        Console.WriteLine($"\n💬 대화 생성 워크플로우 시작: {npcName} ({npcRole})");
        Console.WriteLine(new string('-', 50));

        var result = new DialogueGenerationResult
        {
            NpcName = npcName,
            NpcRole = npcRole,
            DesiredTone = desiredTone
        };

        // Stage 1: 초안 작성
        Console.WriteLine("\n✏️ Stage 1: 대화 초안 작성 중...");
        result.Draft = await _draftAgent.GenerateDraftAsync(npcName, npcRole);
        Console.WriteLine($"✅ 초안 작성 완료");
        Console.WriteLine($"   {result.Draft[..100]}...");

        // Stage 2: 톤 조정
        Console.WriteLine("\n🎭 Stage 2: 톤 조정 중...");
        result.Tone = await _toneAgent.AdjustToneAsync(result.Draft, desiredTone);
        Console.WriteLine($"✅ 톤 조정 완료");
        Console.WriteLine($"   {result.Tone[..100]}...");

        // Stage 3: 분기 생성
        Console.WriteLine("\n🌳 Stage 3: 분기 생성 중...");
        result.Branches = await _branchAgent.CreateBranchesAsync(result.Tone);
        Console.WriteLine($"✅ 분기 생성 완료");
        Console.WriteLine($"   {result.Branches[..100]}...");

        Console.WriteLine("\n" + new string('-', 50));
        Console.WriteLine("✅ 대화 생성 워크플로우 완료!");

        return result;
    }
}
