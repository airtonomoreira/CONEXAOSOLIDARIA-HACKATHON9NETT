using TransparencyService.Controllers;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TransparencyService.Tests;

public class TransparencyServiceTests
{
    private readonly TransparencyDbContext _context;

    public TransparencyServiceTests()
    {
        var options = new DbContextOptionsBuilder<TransparencyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TransparencyDbContext(options);
    }

    [Fact]
    public async Task GetActiveCampaigns_ReturnsOnlyActiveCampaigns()
    {
        // Arrange
        _context.Campanhas.AddRange(new[]
        {
            new Campanha { Id = 1, Titulo = "Campanha Ativa 1", Descricao = "Descrição", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 10000, ValorArrecadado = 5000, Status = "Ativa" },
            new Campanha { Id = 2, Titulo = "Campanha Ativa 2", Descricao = "Descrição", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 20000, ValorArrecadado = 10000, Status = "Ativa" },
            new Campanha { Id = 3, Titulo = "Campanha Concluída", Descricao = "Descrição", DataInicio = DateTime.Now.AddDays(-60), DataFim = DateTime.Now.AddDays(-30), MetaFinanceira = 15000, ValorArrecadado = 15000, Status = "Concluida" },
            new Campanha { Id = 4, Titulo = "Campanha Cancelada", Descricao = "Descrição", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 5000, ValorArrecadado = 0, Status = "Cancelada" }
        });
        await _context.SaveChangesAsync();

        var controller = new TransparencyController(_context);

        // Act
        var result = await controller.GetActiveCampaigns();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var campaigns = okResult.Value as System.Collections.IList;
        Assert.NotNull(campaigns);
        Assert.Equal(2, campaigns.Count);
    }

    [Fact]
    public async Task GetActiveCampaigns_ReturnsCorrectFields()
    {
        // Arrange
        _context.Campanhas.Add(new Campanha 
        { 
            Id = 1, 
            Titulo = "Campanha Teste", 
            Descricao = "Descrição completa não deve aparecer", 
            DataInicio = DateTime.Now, 
            DataFim = DateTime.Now.AddDays(30), 
            MetaFinanceira = 10000, 
            ValorArrecadado = 5000, 
            Status = "Ativa" 
        });
        await _context.SaveChangesAsync();

        var controller = new TransparencyController(_context);

        // Act
        var result = await controller.GetActiveCampaigns();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var campaigns = okResult.Value as System.Collections.IList;
        Assert.NotNull(campaigns);
        Assert.Single(campaigns);
    }

    [Fact]
    public async Task GetActiveCampaigns_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var controller = new TransparencyController(_context);

        // Act
        var result = await controller.GetActiveCampaigns();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var campaigns = okResult.Value as System.Collections.IList;
        Assert.NotNull(campaigns);
        Assert.Empty(campaigns);
    }

    [Fact]
    public async Task GetActiveCampaigns_AllInactive_ReturnsEmptyList()
    {
        // Arrange
        _context.Campanhas.AddRange(new[]
        {
            new Campanha { Id = 1, Titulo = "Campanha Concluída", Descricao = "Descrição", DataInicio = DateTime.Now.AddDays(-60), DataFim = DateTime.Now.AddDays(-30), MetaFinanceira = 15000, ValorArrecadado = 15000, Status = "Concluida" },
            new Campanha { Id = 2, Titulo = "Campanha Cancelada", Descricao = "Descrição", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 5000, ValorArrecadado = 0, Status = "Cancelada" }
        });
        await _context.SaveChangesAsync();

        var controller = new TransparencyController(_context);

        // Act
        var result = await controller.GetActiveCampaigns();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var campaigns = okResult.Value as System.Collections.IList;
        Assert.NotNull(campaigns);
        Assert.Empty(campaigns);
    }

    [Fact]
    public async Task GetActiveCampaigns_MultipleActive_ReturnsAll()
    {
        // Arrange
        _context.Campanhas.AddRange(new[]
        {
            new Campanha { Id = 1, Titulo = "Campanha 1", Descricao = "Descrição", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 10000, ValorArrecadado = 0, Status = "Ativa" },
            new Campanha { Id = 2, Titulo = "Campanha 2", Descricao = "Descrição", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 20000, ValorArrecadado = 15000, Status = "Ativa" },
            new Campanha { Id = 3, Titulo = "Campanha 3", Descricao = "Descrição", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 30000, ValorArrecadado = 30000, Status = "Ativa" }
        });
        await _context.SaveChangesAsync();

        var controller = new TransparencyController(_context);

        // Act
        var result = await controller.GetActiveCampaigns();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var campaigns = okResult.Value as System.Collections.IList;
        Assert.NotNull(campaigns);
        Assert.Equal(3, campaigns.Count);
    }
}
