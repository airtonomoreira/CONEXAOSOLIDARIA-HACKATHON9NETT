using CampaignService.Controllers;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CampaignService.Tests;

public class CampaignServiceTests
{
    private readonly CampaignDbContext _context;

    public CampaignServiceTests()
    {
        var options = new DbContextOptionsBuilder<CampaignDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new CampaignDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsAllCampaigns()
    {
        // Arrange
        _context.Campanhas.AddRange(new[]
        {
            new Campanha { Id = 1, Titulo = "Campanha 1", Descricao = "Descrição 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 10000, Status = "Ativa" },
            new Campanha { Id = 2, Titulo = "Campanha 2", Descricao = "Descrição 2", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 20000, Status = "Ativa" }
        });
        await _context.SaveChangesAsync();

        var controller = new CampaignController(_context);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var campaigns = Assert.IsType<System.Collections.Generic.List<Campanha>>(okResult.Value);
        Assert.Equal(2, campaigns.Count);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsCampaign()
    {
        // Arrange
        var campaign = new Campanha { Id = 1, Titulo = "Campanha 1", Descricao = "Descrição 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 10000, Status = "Ativa" };
        _context.Campanhas.Add(campaign);
        await _context.SaveChangesAsync();

        var controller = new CampaignController(_context);

        // Act
        var result = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var returnedCampaign = Assert.IsType<Campanha>(okResult.Value);
        Assert.Equal("Campanha 1", returnedCampaign.Titulo);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var controller = new CampaignController(_context);

        // Act
        var result = await controller.GetById(999);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ValidData_ReturnsCreatedCampaign()
    {
        // Arrange
        var controller = new CampaignController(_context);
        var request = new CampanhaRequest
        {
            Titulo = "Nova Campanha",
            Descricao = "Descrição da nova campanha",
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddDays(30),
            MetaFinanceira = 15000,
            Status = "Ativa"
        };

        // Act
        var result = await controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(result);
        var campaign = Assert.IsType<Campanha>(createdResult.Value);
        Assert.Equal("Nova Campanha", campaign.Titulo);
        Assert.Equal(0, campaign.ValorArrecadado);
    }

    [Fact]
    public async Task Create_PastEndDate_ReturnsBadRequest()
    {
        // Arrange
        var controller = new CampaignController(_context);
        var request = new CampanhaRequest
        {
            Titulo = "Campanha Inválida",
            Descricao = "Descrição",
            DataInicio = DateTime.Now.AddDays(-10),
            DataFim = DateTime.Now.AddDays(-5),
            MetaFinanceira = 10000,
            Status = "Ativa"
        };

        // Act
        var result = await controller.Create(request);

        // Assert
        var badRequest = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        Assert.Equal("A data de término não pode ser no passado", badRequest.Value);
    }

    [Fact]
    public async Task Create_ZeroMetaFinanceira_ReturnsBadRequest()
    {
        // Arrange
        var controller = new CampaignController(_context);
        var request = new CampanhaRequest
        {
            Titulo = "Campanha Inválida",
            Descricao = "Descrição",
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddDays(30),
            MetaFinanceira = 0,
            Status = "Ativa"
        };

        // Act
        var result = await controller.Create(request);

        // Assert
        var badRequest = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        Assert.Equal("A meta financeira deve ser maior que zero", badRequest.Value);
    }

    [Fact]
    public async Task Create_StartDateAfterEndDate_ReturnsBadRequest()
    {
        // Arrange
        var controller = new CampaignController(_context);
        var request = new CampanhaRequest
        {
            Titulo = "Campanha Inválida",
            Descricao = "Descrição",
            DataInicio = DateTime.Now.AddDays(30),
            DataFim = DateTime.Now.AddDays(10),
            MetaFinanceira = 10000,
            Status = "Ativa"
        };

        // Act
        var result = await controller.Create(request);

        // Assert
        var badRequest = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        Assert.Equal("A data de início deve ser anterior à data de término", badRequest.Value);
    }

    [Fact]
    public async Task Update_ExistingId_ReturnsUpdatedCampaign()
    {
        // Arrange
        var campaign = new Campanha { Id = 1, Titulo = "Campanha Original", Descricao = "Descrição Original", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 10000, Status = "Ativa" };
        _context.Campanhas.Add(campaign);
        await _context.SaveChangesAsync();

        var controller = new CampaignController(_context);
        var request = new CampanhaRequest
        {
            Titulo = "Campanha Atualizada",
            Descricao = "Descrição Atualizada",
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddDays(30),
            MetaFinanceira = 20000,
            Status = "Ativa"
        };

        // Act
        var result = await controller.Update(1, request);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var updatedCampaign = Assert.IsType<Campanha>(okResult.Value);
        Assert.Equal("Campanha Atualizada", updatedCampaign.Titulo);
        Assert.Equal(20000, updatedCampaign.MetaFinanceira);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var controller = new CampaignController(_context);
        var request = new CampanhaRequest
        {
            Titulo = "Campanha",
            Descricao = "Descrição",
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddDays(30),
            MetaFinanceira = 10000,
            Status = "Ativa"
        };

        // Act
        var result = await controller.Update(999, request);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var campaign = new Campanha { Id = 1, Titulo = "Campanha", Descricao = "Descrição", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(30), MetaFinanceira = 10000, Status = "Ativa" };
        _context.Campanhas.Add(campaign);
        await _context.SaveChangesAsync();

        var controller = new CampaignController(_context);

        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(result);
        Assert.Null(await _context.Campanhas.FindAsync(1));
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var controller = new CampaignController(_context);

        // Act
        var result = await controller.Delete(999);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }
}
