using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

namespace SkillSystem;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🎯 Skill-based Agent System");
        Console.WriteLine("==========================\n");

        var skillsDir = Path.Combine(AppContext.BaseDirectory, "skills");
        var skillLoader = new SkillLoader(skillsDir);
        
        Console.WriteLine("📚 스킬 로딩 중...\n");
        var skills = skillLoader.LoadAllSkills();
        
        if (skills.Count == 0)
        {
            Console.WriteLine("⚠️  로드된 스킬이 없습니다. 프로그램을 종료합니다.");
            return;
        }

        Console.WriteLine($"\n✅ 총 {skills.Count}개의 스킬을 로드했습니다.\n");

        var skillTools = new SkillTools();
        var genericTools = new GenericTools();
        var tools = new List<AITool>();

        foreach (var skill in skills)
        {
            Console.WriteLine($"  📌 {skill.Name}: {skill.Functions.Count}개 함수");
            
            foreach (var func in skill.Functions)
            {
                Console.WriteLine($"    - {func.Name}: {func.Description}");
                var tool = CreateToolForFunction(func, skillTools, genericTools);
                if (tool != null)
                {
                    tools.Add(tool);
                }
            }
        }

        Console.WriteLine($"\n🔧 총 {tools.Count}개의 Tool 이 등록되었습니다.\n");

        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL") 
            ?? "https://api.openai.com/v1";

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("❌ OPENAI_API_KEY 환경 변수가 설정되지 않았습니다.");
            Console.WriteLine("다음 명령어로 설정하세요:");
            Console.WriteLine("  $env:OPENAI_API_KEY=\"your-api-key\"");
            return;
        }

        var options = new OpenAIClientOptions { Endpoint = new Uri(baseUrl) };
        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);
        var chatClient = client.GetChatClient("gpt-4o-mini");

        var skillDescriptions = string.Join("\n", 
            skills.Select(s => $"- {s.Name}: {s.Description}"));

        var agent = chatClient.AsAIAgent(
            instructions: $"""
                당신은 스킬 기반 에이전트입니다.
                
                사용 가능한 스킬:
                {skillDescriptions}
                
                가이드라인:
                1. 사용자의 질문에 맞는 적절한 스킬과 함수를 선택하세요.
                2. Tool 을 호출하여 결과를 계산하거나 정보를 조회하세요.
                3. 결과를 친절하게 설명하여 답변하세요.
                4. 계산 결과는 수식과 함께 표시하세요.
                5. 시스템 명령어 실행 시 보안에 유의하세요.
                
                사용 예시:
                {string.Join("\n", skills.SelectMany(s => s.Examples.Take(2)))}
                """,
            name: "SkillAgent",
            tools: tools
        );

        Console.WriteLine("💬 에이전트가 준비되었습니다. 질문을 입력하세요 (종료: 'quit')\n");

        while (true)
        {
            Console.Write("👤 사용자: ");
            var userInput = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(userInput) || 
                userInput.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            
            try
            {
                Console.Write("🤖 에이전트: ");
                var response = await agent.RunAsync(userInput);
                Console.WriteLine(response);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ 오류: {ex.Message}\n");
            }
        }
    }

    private static AIFunction? CreateToolForFunction(FunctionDefinition func, SkillTools skillTools, GenericTools genericTools)
    {
        var funcName = func.Name.ToLower();
        
        return funcName switch
        {
            // Calculator 스킬
            "calculate_dps" => AIFunctionFactory.Create(skillTools.CalculateDps),
            "calculate_crit_damage" => AIFunctionFactory.Create(skillTools.CalculateCritDamage),
            "calculate_expected_dps" => AIFunctionFactory.Create(skillTools.CalculateExpectedDps),
            
            // Weather 스킬
            "get_current_weather" => AIFunctionFactory.Create(skillTools.GetCurrentWeather),
            "get_weekend_forecast" => AIFunctionFactory.Create(skillTools.GetWeekendForecast),
            "compare_weather" => AIFunctionFactory.Create(skillTools.CompareWeather),
            
            // Translator 스킬
            "translate_ko_to_ja" => AIFunctionFactory.Create(skillTools.TranslateKoToJa),
            "translate_ja_to_ko" => AIFunctionFactory.Create(skillTools.TranslateJaToKo),
            "translate_game_term" => AIFunctionFactory.Create(skillTools.TranslateGameTerm),
            
            // SystemTools 스킬 (GenericTools)
            "execute_command" => AIFunctionFactory.Create(genericTools.ExecuteCommand),
            "download_file" => AIFunctionFactory.Create(genericTools.DownloadFile),
            "install_package" => AIFunctionFactory.Create(genericTools.InstallPackage),
            "search_package" => AIFunctionFactory.Create(genericTools.SearchPackage),
            "get_system_info" => AIFunctionFactory.Create(genericTools.GetSystemInfo),
            "get_process_list" => AIFunctionFactory.Create(genericTools.GetProcessList),
            "search_web" => AIFunctionFactory.Create(genericTools.SearchWeb),
            "get_url_content" => AIFunctionFactory.Create(genericTools.GetUrlContent),
            "read_file" => AIFunctionFactory.Create(genericTools.ReadFile),
            "write_file" => AIFunctionFactory.Create(genericTools.WriteFile),
            "delete_file" => AIFunctionFactory.Create(genericTools.DeleteFile),
            "list_directory" => AIFunctionFactory.Create(genericTools.ListDirectory),
            
            // WebTools 스킬 (GenericTools 와 공유)
            "fetch_url" => AIFunctionFactory.Create(genericTools.GetUrlContent),
            "search_google" => AIFunctionFactory.Create(genericTools.SearchWeb),
            "search_github" => AIFunctionFactory.Create((string query) => 
                genericTools.SearchWeb($"site:github.com {query}")),
            "check_website_status" => AIFunctionFactory.Create(CheckWebsiteStatus),
            "download_file_web" => AIFunctionFactory.Create(genericTools.DownloadFile),
            
            _ => null
        };
    }

    private static string CheckWebsiteStatus(string url)
    {
        try
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            var response = client.GetAsync(url).Result;
            
            if (response.IsSuccessStatusCode)
            {
                return $"✅ {url} 이 (가) 정상 응답합니다.\n상태 코드: {response.StatusCode}\n응답 시간: {response.Headers.Date?.LocalDateTime.ToString() ?? "N/A"}";
            }
            else
            {
                return $"⚠️ {url} 이 (가) 응답하지만 오류 상태입니다.\n상태 코드: {response.StatusCode}";
            }
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
        {
            return $"❌ {url} 으로 연결할 수 없습니다 (시간 초과).";
        }
        catch (Exception ex)
        {
            return $"❌ {url} 으로 연결할 수 없습니다.\n오류: {ex.Message}";
        }
    }
}
