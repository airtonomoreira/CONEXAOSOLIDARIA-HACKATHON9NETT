using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace TransparencyService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransparencyController : ControllerBase
{
    private readonly TransparencyDbContext _context;

    public TransparencyController(TransparencyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveCampaigns()
    {
        var campanhas = await _context.Campanhas
            .Where(c => c.Status == "Ativa")
            .Select(c => new
            {
                c.Id,
                c.Titulo,
                c.MetaFinanceira,
                c.ValorArrecadado
            })
            .ToListAsync();

        return Ok(campanhas);
    }
}
