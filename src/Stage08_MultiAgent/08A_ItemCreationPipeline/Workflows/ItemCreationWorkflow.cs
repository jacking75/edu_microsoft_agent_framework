namespace _08A_ItemCreationPipeline.Workflows;

using _08A_ItemCreationPipeline.Agents;

/// <summary>
/// 아이템 생성 워크플로우 결과
/// </summary>
public class ItemCreationResult
{
    public string ItemName { get; set; } = "";
    public string Description { get; set; } = "";
    public string BalanceReview { get; set; } = "";
    public string Localization { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// Sequential 워크플로우: 아이템 생성 파이프라인
/// Description → Balance → Localization
/// </summary>
public class ItemCreationWorkflow
{
    private readonly DescriptionAgent _descriptionAgent;
    private readonly BalanceAgent _balanceAgent;
    private readonly LocalizationAgent _localizationAgent;

    public ItemCreationWorkflow(
        DescriptionAgent descriptionAgent,
        BalanceAgent balanceAgent,
        LocalizationAgent localizationAgent)
    {
        _descriptionAgent = descriptionAgent;
        _balanceAgent = balanceAgent;
        _localizationAgent = localizationAgent;
    }

    /// <summary>
    /// Sequential 워크플로우 실행
    /// </summary>
    public async Task<ItemCreationResult> ExecuteAsync(string itemName)
    {
        Console.WriteLine($"\n🎒 아이템 생성 워크플로우 시작: {itemName}");
        Console.WriteLine(new string('-', 50));

        var result = new ItemCreationResult { ItemName = itemName };

        // Stage 1: 설명 작성
        Console.WriteLine("\n📝 Stage 1: 설명 작성 중...");
        result.Description = await _descriptionAgent.GenerateDescriptionAsync(itemName);
        Console.WriteLine($"✅ 설명 작성 완료");
        Console.WriteLine($"   {result.Description[..100]}...");

        // Stage 2: 밸런스 검토
        Console.WriteLine("\n⚖️ Stage 2: 밸런스 검토 중...");
        result.BalanceReview = await _balanceAgent.ReviewBalanceAsync(result.Description);
        Console.WriteLine($"✅ 밸런스 검토 완료");
        Console.WriteLine($"   {result.BalanceReview[..100]}...");

        // Stage 3: 현지화
        Console.WriteLine("\n🌐 Stage 3: 현지화 중...");
        result.Localization = await _localizationAgent.LocalizeAsync(result.Description);
        Console.WriteLine($"✅ 현지화 완료");
        Console.WriteLine($"   {result.Localization[..100]}...");

        Console.WriteLine("\n" + new string('-', 50));
        Console.WriteLine("✅ 아이템 생성 워크플로우 완료!");

        return result;
    }
}
