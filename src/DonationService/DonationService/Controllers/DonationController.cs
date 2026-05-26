using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace DonationService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DonationController : ControllerBase
{
    private readonly DonationDbContext _context;
    private readonly IRabbitMQService _rabbitMQService;

    public DonationController(DonationDbContext context, IRabbitMQService rabbitMQService)
    {
        _context = context;
        _rabbitMQService = rabbitMQService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DoacaoRequest request)
    {
        // Validar se campanha existe e está ativa
        // Nota: Em produção, isso seria feito via API call ao CampaignService
        // Por simplicidade, vamos assumir que a validação é feita no Worker

        var doacao = new Doacao
        {
            CampanhaId = request.CampanhaId,
            DoadorId = 1, // Em produção, viria do token JWT
            Valor = request.Valor,
            DataDoacao = DateTime.Now,
            Status = "Pendente"
        };

        _context.Doacoes.Add(doacao);
        await _context.SaveChangesAsync();

        // Publicar evento no RabbitMQ
        var doacaoEvent = new
        {
            DoacaoId = doacao.Id,
            CampanhaId = doacao.CampanhaId,
            Valor = doacao.Valor,
            DataDoacao = doacao.DataDoacao
        };

        _rabbitMQService.PublishMessage(System.Text.Json.JsonSerializer.Serialize(doacaoEvent));

        return Ok(new { message = "Doação registrada com sucesso", doacaoId = doacao.Id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var doacoes = await _context.Doacoes.ToListAsync();
        return Ok(doacoes);
    }
}
