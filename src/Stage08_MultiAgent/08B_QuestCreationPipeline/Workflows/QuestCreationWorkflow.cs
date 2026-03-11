namespace _08B_QuestCreationPipeline.Workflows;

using _08B_QuestCreationPipeline.Agents;

/// <summary>
/// 퀘스트 생성 워크플로우 결과
/// </summary>
public class QuestCreationResult
{
    public string QuestTitle { get; set; } = "";
    public string Story { get; set; } = "";
    public string Reward { get; set; } = "";
    public string Difficulty { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// Sequential 워크플로우: 퀘스트 생성 파이프라인
/// Story → Reward → Difficulty
/// </summary>
public class QuestCreationWorkflow
{
    private readonly StoryAgent _storyAgent;
    private readonly RewardAgent _rewardAgent;
    private readonly DifficultyAgent _difficultyAgent;

    public QuestCreationWorkflow(
        StoryAgent storyAgent,
        RewardAgent rewardAgent,
        DifficultyAgent difficultyAgent)
    {
        _storyAgent = storyAgent;
        _rewardAgent = rewardAgent;
        _difficultyAgent = difficultyAgent;
    }

    /// <summary>
    /// Sequential 워크플로우 실행
    /// </summary>
    public async Task<QuestCreationResult> ExecuteAsync(string questTitle)
    {
        Console.WriteLine($"\n📜 퀘스트 생성 워크플로우 시작: {questTitle}");
        Console.WriteLine(new string('-', 50));

        var result = new QuestCreationResult { QuestTitle = questTitle };

        // Stage 1: 스토리 작성
        Console.WriteLine("\n📖 Stage 1: 스토리 작성 중...");
        result.Story = await _storyAgent.GenerateStoryAsync(questTitle);
        Console.WriteLine($"✅ 스토리 작성 완료");
        Console.WriteLine($"   {result.Story[..100]}...");

        // Stage 2: 보상 설계
        Console.WriteLine("\n💰 Stage 2: 보상 설계 중...");
        result.Reward = await _rewardAgent.DesignRewardAsync(result.Story);
        Console.WriteLine($"✅ 보상 설계 완료");
        Console.WriteLine($"   {result.Reward[..100]}...");

        // Stage 3: 난이도 조정
        Console.WriteLine("\n⚔️ Stage 3: 난이도 조정 중...");
        result.Difficulty = await _difficultyAgent.AdjustDifficultyAsync(result.Story, result.Reward);
        Console.WriteLine($"✅ 난이도 조정 완료");
        Console.WriteLine($"   {result.Difficulty[..100]}...");

        Console.WriteLine("\n" + new string('-', 50));
        Console.WriteLine("✅ 퀘스트 생성 워크플로우 완료!");

        return result;
    }
}
