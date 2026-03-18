using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

namespace SkillSystem2;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🎯 Skill-based Agent System v2 (Dynamic Skill Selection)");
        Console.WriteLine("========================================================\n");

        var skillsDir = Path.Combine(AppContext.BaseDirectory, "skills");
        var skillLoader = new SkillLoader(skillsDir);
        
        Console.WriteLine("📚 모든 스킬을 사전에 로딩합니다...\n");
        var allSkills = skillLoader.LoadAllSkills();
        
        if (allSkills.Count == 0)
        {
            Console.WriteLine("⚠️  로드된 스킬이 없습니다. 프로그램을 종료합니다.");
            return;
        }

        Console.WriteLine($"\n✅ 총 {allSkills.Count}개의 스킬을 사용 가능\n");

        var skillTools = new SkillTools();
        var genericTools = new GenericTools();

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
                var skillMatcher = new SkillMatcher(allSkills);
                var relevantSkills = skillMatcher.FindRelevantSkills(userInput, threshold: 1);

                var tools = new List<AITool>();
                var toolNames = new List<string>();
                
                foreach (var skill in relevantSkills)
                {
                    foreach (var func in skill.Functions)
                    {
                        var tool = CreateToolForFunction(func, skillTools, genericTools);
                        if (tool != null)
                        {
                            tools.Add(tool);
                            toolNames.Add(func.Name);
                        }
                    }
                }

                Console.WriteLine($"\n🔧 {tools.Count}개의 Tool 을 활성화했습니다.\n");

                if (relevantSkills.Count == 0)
                {
                    Console.WriteLine("⚠️  일치하는 스킬이 없어 기본 프롬프트로 응답합니다.\n");
                }

                var skillDescriptions = relevantSkills.Count > 0
                    ? string.Join("\n", relevantSkills.Select(s => $"- {s.Name}: {s.Description}"))
                    : "없음";

                var enabledSkills = relevantSkills.Count > 0
                    ? string.Join(", ", relevantSkills.Select(s => s.Name))
                    : "활성화된 스킬 없음";

                Console.WriteLine($"📌 활성화된 스킬: {enabledSkills}\n");

                var agent = chatClient.AsAIAgent(
                    instructions: $"""
                        당신은 스킬 기반 에이전트입니다.
                        
                        현재 활성화된 스킬:
                        {skillDescriptions}
                        
                        사용 가능한 Tool:
                        {string.Join(", ", toolNames)}
                        
                        가이드라인:
                        1. 사용자의 질문에 맞는 Tool 을 선택하여 호출하세요.
                        2. Tool 이 없는 경우 일반 지식으로 답변하세요.
                        3. 계산 결과는 수식과 함께 표시하세요.
                        4. Tool 호출이 불가능한 경우 그 이유를 설명하세요.
                        
                        활성화된 스킬 예시:
                        {string.Join("\n", relevantSkills.SelectMany(s => s.Examples.Take(1)))}
                        """,
                    name: "SkillAgent",
                    tools: tools
                );

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
            "calculate_dps" => AIFunctionFactory.Create(skillTools.CalculateDps),
            "calculate_crit_damage" => AIFunctionFactory.Create(skillTools.CalculateCritDamage),
            "calculate_expected_dps" => AIFunctionFactory.Create(skillTools.CalculateExpectedDps),
            
            "get_current_weather" => AIFunctionFactory.Create(skillTools.GetCurrentWeather),
            "get_weekend_forecast" => AIFunctionFactory.Create(skillTools.GetWeekendForecast),
            "compare_weather" => AIFunctionFactory.Create(skillTools.CompareWeather),
            
            "translate_ko_to_ja" => AIFunctionFactory.Create(skillTools.TranslateKoToJa),
            "translate_ja_to_ko" => AIFunctionFactory.Create(skillTools.TranslateJaToKo),
            "translate_game_term" => AIFunctionFactory.Create(skillTools.TranslateGameTerm),
            
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
            
            _ => null
        };
    }
}
