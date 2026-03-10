namespace _05C_SlackNotifier.Tools;

/// <summary>
/// Slack Webhook 연동 Tool (모의 구현)
/// </summary>
public class SlackWebhookTool
{
    private readonly string _webhookUrl;

    public SlackWebhookTool(string webhookUrl)
    {
        _webhookUrl = webhookUrl;
    }

    public string SendMessage(string channel, string message, string username = "Bot")
    {
        return $$"""
            ✅ Slack 메시지가 전송되었습니다 (모의 응답):
            
            채널: {{channel}}
            사용자: {{username}}
            메시지: {{message[..50]}}...
            
            Webhook URL: {{_webhookUrl[..30]}}...
            
            실제 구현 시:
            POST {{_webhookUrl}}
            Content-Type: application/json
            {
                "channel": "{{channel}}",
                "username": "{{username}}",
                "text": "{{message}}"
            }
            """;
    }

    public string SendNotification(string title, string message, string level = "info")
    {
        var emoji = level switch
        {
            "error" => "🔴",
            "warning" => "🟡",
            "success" => "🟢",
            _ => "🔵"
        };

        return $"""
            {emoji} 알림 전송됨 (모의 응답):
            
            제목: {title}
            레벨: {level}
            내용: {message[..50]}...
            
            실제 구현 시:
            - error: 채널에 긴급 알림
            - warning: 주의 필요한 사항
            - success: 작업 완료 알림
            - info: 일반 정보
            """;
    }
}
