using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace CampaignService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "GestorONG")]
public class CampaignController : ControllerBase
{
    private readonly CampaignDbContext _context;

    public CampaignController(CampaignDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var campanhas = await _context.Campanhas.ToListAsync();
        return Ok(campanhas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var campanha = await _context.Campanhas.FindAsync(id);
        if (campanha == null)
            return NotFound();
        return Ok(campanha);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CampanhaRequest request)
    {
        // Validações de negócio
        if (request.DataFim < DateTime.Now)
            return BadRequest("A data de término não pode ser no passado");

        if (request.MetaFinanceira <= 0)
            return BadRequest("A meta financeira deve ser maior que zero");

        if (request.DataInicio >= request.DataFim)
            return BadRequest("A data de início deve ser anterior à data de término");

        var campanha = new Campanha
        {
            Titulo = request.Titulo,
            Descricao = request.Descricao,
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            MetaFinanceira = request.MetaFinanceira,
            ValorArrecadado = 0,
            Status = request.Status
        };

        _context.Campanhas.Add(campanha);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = campanha.Id }, campanha);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CampanhaRequest request)
    {
        var campanha = await _context.Campanhas.FindAsync(id);
        if (campanha == null)
            return NotFound();

        campanha.Titulo = request.Titulo;
        campanha.Descricao = request.Descricao;
        campanha.DataInicio = request.DataInicio;
        campanha.DataFim = request.DataFim;
        campanha.MetaFinanceira = request.MetaFinanceira;
        campanha.Status = request.Status;

        await _context.SaveChangesAsync();

        return Ok(campanha);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var campanha = await _context.Campanhas.FindAsync(id);
        if (campanha == null)
            return NotFound();

        _context.Campanhas.Remove(campanha);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
