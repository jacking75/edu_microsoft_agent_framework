using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.Speech.Recognition;
using System.Speech.Synthesis;

// OpenAI 클라이언트 설정
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));
var chatClientInner = openAiClient.GetChatClient(model);
IChatClient chatClient = chatClientInner.AsIChatClient();

// 음성 인식 및 합성 설정
var speechRecognizer = new SpeechRecognitionEngine();
speechRecognizer.SetInputToDefaultAudioDevice();

// 간단한 문법 설정 (명령어 인식용)
var grammarBuilder = new GrammarBuilder();
grammarBuilder.Append("안녕하세요");
grammarBuilder.Append("안녕");
grammarBuilder.Append("종료해");
grammarBuilder.Append("그만");
var grammar = new Grammar(grammarBuilder);
speechRecognizer.LoadGrammar(grammar);

speechRecognizer.SpeechRecognized += (s, e) =>
{
    Console.WriteLine($"\n🎤 인식됨: {e.Result.Text}");
};

var speechSynthesizer = new SpeechSynthesizer();
speechSynthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);

// 대화 히스토리
var conversationHistory = new List<Microsoft.Extensions.AI.ChatMessage>();

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║     Stage 10 - Voice & Multimodal          ║");
Console.WriteLine("║     '시작' - 음성 인식 시작                ║");
Console.WriteLine("║     '종료' - 음성 인식 중지                ║");
Console.WriteLine("║     'quit' - 프로그램 종료                 ║");
Console.WriteLine("╚════════════════════════════════════════════╝");
Console.WriteLine();

var isListening = false;

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input))
        continue;
    
    if (input.ToLower() == "quit")
        break;
    
    if (input.ToLower() == "시작")
    {
        if (!isListening)
        {
            isListening = true;
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            Console.WriteLine("[음성 인식 시작 - 말해주세요]");
        }
        continue;
    }
    
    if (input.ToLower() == "종료")
    {
        if (isListening)
        {
            speechRecognizer.RecognizeAsyncCancel();
            isListening = false;
            Console.WriteLine("[음성 인식 중지]");
        }
        continue;
    }
    
    // 텍스트 채팅 처리
    await ProcessMessageAsync(input, chatClient, conversationHistory, speechSynthesizer);
}

Console.WriteLine("\nGoodbye!");

// 음성 인식 이벤트 처리
speechRecognizer.SpeechRecognized += async (s, e) =>
{
    if (isListening)
    {
        var text = e.Result.Text;
        if (text == "종료해" || text == "그만")
        {
            speechRecognizer.RecognizeAsyncCancel();
            isListening = false;
            Console.WriteLine("[음성 인식 중지]");
            return;
        }
        
        await ProcessMessageAsync(text, chatClient, conversationHistory, speechSynthesizer);
    }
};

Console.WriteLine();
Console.WriteLine("음성 인식 팁:");
Console.WriteLine("- '시작'이라고 말하면 음성 인식이 시작됩니다");
Console.WriteLine("- '종료해' 또는 '그만'이라고 말하면 중지됩니다");
Console.WriteLine("- Windows 음성 인식 기능이 필요합니다");

await Task.Delay(Timeout.Infinite);

static async Task ProcessMessageAsync(
    string message, 
    IChatClient chatClient, 
    List<Microsoft.Extensions.AI.ChatMessage> history,
    SpeechSynthesizer synthesizer)
{
    // 사용자 메시지 추가
    history.Add(new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, message));
    
    // AI 응답 생성
    var response = await chatClient.GetResponseAsync(
        history,
        new Microsoft.Extensions.AI.ChatOptions { MaxOutputTokens = 500 }
    );
    
    var responseText = response.Text;
    
    // 화면에 출력
    Console.WriteLine($"Agent: {responseText}");
    
    // 음성으로 읽기 (TTS)
    synthesizer.SpeakAsync(responseText);
    
    // 히스토리에 추가
    history.Add(new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.Assistant, responseText));
    
    // 히스토리 제한 (최근 10 개 메시지만 유지)
    while (history.Count > 10)
    {
        history.RemoveAt(0);
    }
}
