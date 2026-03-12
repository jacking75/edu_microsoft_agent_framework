// Copyright (c) Microsoft. All rights reserved.

using _15B_BugFixer.Agents;
using _15B_BugFixer.Services;

// ==========================================
// 15 단계 B: 버그 수정 제안기
// ==========================================
// 학습 목표:
// 1. 에러 로그 분석
// 2. 버그 진단
// 3. 수정 코드 제안
// 4. 검증 및 승인
// ==========================================

Console.WriteLine("🐛 버그 수정 제안기에 오신 것을 환영합니다!");
Console.WriteLine("에러 로그를 분석하고 수정 코드를 제안합니다.\n");

// 환경 변수 확인
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY 환경 변수를 설정해주세요.");

var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
    ?? "https://api.openai.com/v1";

Console.WriteLine($"✅ OpenAI API 키가 설정되었습니다.");
Console.WriteLine($"✅ Base URL: {baseUrl}\n");

// 서비스 및 에이전트 생성
var errorParser = new ErrorLogParser();
var localDebug = new LocalDebugService();
var diagnosisAgent = new ErrorDiagnosisAgent(apiKey, baseUrl);
var bugFixAgent = new BugFixAgent(apiKey, baseUrl);
var verificationAgent = new VerificationAgent(apiKey, baseUrl);

Console.WriteLine("✅ 버그 수정 도구가 초기화되었습니다.\n");

Console.WriteLine("📋 사용 방법:");
Console.WriteLine("  1. 에러 로그를 직접 입력");
Console.WriteLine("  2. 데모 에러 사용 (데모 입력)");
Console.WriteLine("  3. 에러 유형 선택 (NullReference, Argument, Timeout, Invalid)\n");

Console.WriteLine("에러 로그를 입력하세요 (종료: 'quit' 또는 'exit')\n");

while (true)
{
    Console.Write("👤 사용자: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) ||
        input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("\n👋 프로그램을 종료합니다.");
        break;
    }

    try
    {
        string errorLog;

        // 데모 에러 사용
        if (input.Equals("데모", StringComparison.OrdinalIgnoreCase) ||
            input.Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\n에러 유형을 선택하세요:");
            Console.WriteLine("  1. NullReferenceException");
            Console.WriteLine("  2. ArgumentException");
            Console.WriteLine("  3. TimeoutException");
            Console.WriteLine("  4. InvalidOperationException");
            Console.Write("선택: ");
            
            var choice = Console.ReadLine();
            errorLog = choice switch
            {
                "1" => localDebug.SimulateError("NullReference"),
                "2" => localDebug.SimulateError("Argument"),
                "3" => localDebug.SimulateError("Timeout"),
                _ => localDebug.SimulateError("Invalid")
            };
        }
        else
        {
            errorLog = input;
        }

        // Step 1: 에러 로그 파싱
        Console.WriteLine("\n📝 Step 1: 에러 로그 파싱 중...");
        var parsedError = errorParser.Parse(errorLog);
        
        Console.WriteLine($"   에러유형: {parsedError.ErrorType}");
        Console.WriteLine($"   발생위치: {parsedError.Source}");
        Console.WriteLine($"   메시지: {parsedError.ErrorMessage}");

        // Step 2: 에러 진단
        Console.WriteLine("\n🔍 Step 2: 에러 진단 중...");
        var diagnosis = await diagnosisAgent.DiagnoseAsync(errorLog);
        Console.WriteLine($"\n📋 진단 결과:\n{diagnosis}\n");

        // Step 3: 버그 수정 제안
        Console.WriteLine("💡 Step 3: 버그 수정 제안 중...");
        
        // 데모용 에러 코드 (실제로는 관련 코드를 함께 분석)
        var demoErrorCode = """
            public class UserService
            {
                private UserRepository _repository;
                
                public User GetUserById(string id)
                {
                    // Null 체크 없음 - 버그 발생 가능
                    return _repository.Find(id);
                }
            }
            """;
        
        var fixSuggestion = await bugFixAgent.SuggestFixAsync(demoErrorCode, diagnosis);
        Console.WriteLine($"\n📝 수정 제안:\n{fixSuggestion}\n");

        // Step 4: 검증
        Console.WriteLine("수정안을 검증하시겠습니까? (y/n)");
        var verifyChoice = Console.ReadLine();
        
        if (verifyChoice?.Equals("y", StringComparison.OrdinalIgnoreCase) == true)
        {
            Console.WriteLine("\n✅ Step 4: 수정안 검증 중...");
            
            var fixedCode = """
                public class UserService
                {
                    private UserRepository _repository;
                    
                    public User GetUserById(string id)
                    {
                        if (string.IsNullOrEmpty(id))
                            throw new ArgumentException("ID cannot be null or empty", nameof(id));
                        
                        if (_repository == null)
                            throw new InvalidOperationException("Repository not initialized");
                        
                        return _repository.Find(id);
                    }
                }
                """;
            
            var verification = await verificationAgent.VerifyFixAsync(demoErrorCode, fixedCode, diagnosis);
            Console.WriteLine($"\n📋 검증 결과:\n{verification}\n");
        }

        // 수정 이력 저장
        localDebug.SaveFix(new BugFixHistory
        {
            BugDescription = parsedError.ErrorMessage,
            OriginalCode = demoErrorCode,
            FixedCode = "수정안 적용됨",
            Diagnosis = diagnosis,
            Verified = true
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ 오류 발생: {ex.Message}\n");
    }
}

Console.WriteLine("\n💾 저장된 버그 수정 이력:");
foreach (var fix in localDebug.GetHistory())
{
    Console.WriteLine($"  - {fix.BugDescription} ({fix.FixedAt:HH:mm:ss})");
}
