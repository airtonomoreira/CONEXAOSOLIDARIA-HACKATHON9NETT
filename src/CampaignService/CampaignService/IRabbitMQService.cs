namespace CampaignService;

public interface IRabbitMQService
{
    void PublishMessage(string message);
}
