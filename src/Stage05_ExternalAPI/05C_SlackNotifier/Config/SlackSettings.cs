namespace _05C_SlackNotifier.Config;

public class SlackSettings
{
    public string WebhookUrl { get; set; } = "";
    public string DefaultChannel { get; set; } = "#general";
    public string BotUsername { get; set; } = "Agent Bot";
}
