namespace DonationService;

public interface IRabbitMQService
{
    void PublishMessage(string message);
}
