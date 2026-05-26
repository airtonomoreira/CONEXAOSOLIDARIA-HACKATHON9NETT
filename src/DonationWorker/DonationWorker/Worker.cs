using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DonationWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DonationDbContext _context;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;

    public Worker(ILogger<Worker> logger, DonationDbContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "doacoes", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Mensagem recebida: {message}", message);

            try
            {
                var doacaoEvent = JsonSerializer.Deserialize<DoacaoEvent>(message);
                if (doacaoEvent != null)
                {
                    await ProcessDoacao(doacaoEvent);
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar doação");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: "doacoes", autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessDoacao(DoacaoEvent doacaoEvent)
    {
        // Nota: Em produção, o Worker faria uma chamada HTTP ao CampaignService
        // para atualizar o valor arrecadado. Por simplicidade, vamos apenas logar.
        _logger.LogInformation("Processando doação: CampanhaId={CampanhaId}, Valor={Valor}", 
            doacaoEvent.CampanhaId, doacaoEvent.Valor);

        // Aqui seria implementada a lógica de atualização do valor arrecadado
        // via API call ao CampaignService
        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

public class DoacaoEvent
{
    public int DoacaoId { get; set; }
    public int CampanhaId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataDoacao { get; set; }
}
