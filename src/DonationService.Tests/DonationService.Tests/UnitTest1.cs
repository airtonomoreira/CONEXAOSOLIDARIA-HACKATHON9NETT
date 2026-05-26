using DonationService.Controllers;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DonationService.Tests;

public class DonationServiceTests
{
    private readonly DonationDbContext _context;
    private readonly Mock<IRabbitMQService> _mockRabbitMQService;

    public DonationServiceTests()
    {
        var options = new DbContextOptionsBuilder<DonationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new DonationDbContext(options);
        _mockRabbitMQService = new Mock<IRabbitMQService>();
    }

    [Fact]
    public async Task Create_ValidData_ReturnsSuccess()
    {
        // Arrange
        var controller = new DonationController(_context, _mockRabbitMQService.Object);
        var request = new DoacaoRequest
        {
            CampanhaId = 1,
            Valor = 100.00m
        };

        // Act
        var result = await controller.Create(request);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        _mockRabbitMQService.Verify(x => x.PublishMessage(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Create_ZeroValue_ReturnsSuccess()
    {
        // Arrange
        var controller = new DonationController(_context, _mockRabbitMQService.Object);
        var request = new DoacaoRequest
        {
            CampanhaId = 1,
            Valor = 0
        };

        // Act
        var result = await controller.Create(request);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Create_NegativeValue_ReturnsSuccess()
    {
        // Arrange
        var controller = new DonationController(_context, _mockRabbitMQService.Object);
        var request = new DoacaoRequest
        {
            CampanhaId = 1,
            Valor = -50.00m
        };

        // Act
        var result = await controller.Create(request);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsAllDonations()
    {
        // Arrange
        _context.Doacoes.AddRange(new[]
        {
            new Doacao { Id = 1, CampanhaId = 1, DoadorId = 1, Valor = 100.00m, DataDoacao = DateTime.Now, Status = "Processada" },
            new Doacao { Id = 2, CampanhaId = 2, DoadorId = 2, Valor = 200.00m, DataDoacao = DateTime.Now, Status = "Pendente" }
        });
        await _context.SaveChangesAsync();

        var controller = new DonationController(_context, _mockRabbitMQService.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var donations = Assert.IsType<System.Collections.Generic.List<Doacao>>(okResult.Value);
        Assert.Equal(2, donations.Count);
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var controller = new DonationController(_context, _mockRabbitMQService.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var donations = Assert.IsType<System.Collections.Generic.List<Doacao>>(okResult.Value);
        Assert.Empty(donations);
    }

    [Fact]
    public async Task Create_SetsPendingStatus()
    {
        // Arrange
        var controller = new DonationController(_context, _mockRabbitMQService.Object);
        var request = new DoacaoRequest
        {
            CampanhaId = 1,
            Valor = 150.00m
        };

        // Act
        await controller.Create(request);
        var donation = await _context.Doacoes.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(donation);
        Assert.Equal("Pendente", donation.Status);
    }

    [Fact]
    public async Task Create_SetsCurrentDate()
    {
        // Arrange
        var controller = new DonationController(_context, _mockRabbitMQService.Object);
        var request = new DoacaoRequest
        {
            CampanhaId = 1,
            Valor = 150.00m
        };

        // Act
        await controller.Create(request);
        var donation = await _context.Doacoes.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(donation);
        Assert.Equal(DateTime.Now.Date, donation.DataDoacao.Date);
    }
}
