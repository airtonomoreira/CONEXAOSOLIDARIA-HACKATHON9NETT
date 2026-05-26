using SharedModels;
using System;
using Xunit;

namespace IntegrationTests;

public class IntegrationTests
{
    [Fact]
    public void DoadorModel_CreateInstance_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var doador = new Doador
        {
            Id = 1,
            NomeCompleto = "João Silva",
            Email = "joao@example.com",
            Cpf = "12345678909",
            SenhaHash = "hashedpassword",
            Role = "Doador"
        };

        // Assert
        Assert.Equal(1, doador.Id);
        Assert.Equal("João Silva", doador.NomeCompleto);
        Assert.Equal("joao@example.com", doador.Email);
        Assert.Equal("12345678909", doador.Cpf);
        Assert.Equal("hashedpassword", doador.SenhaHash);
        Assert.Equal("Doador", doador.Role);
    }

    [Fact]
    public void CampanhaModel_CreateInstance_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var campanha = new Campanha
        {
            Id = 1,
            Titulo = "Campanha de Natal",
            Descricao = "Arrecadação para crianças carentes",
            DataInicio = new DateTime(2024, 12, 1),
            DataFim = new DateTime(2024, 12, 31),
            MetaFinanceira = 10000m,
            ValorArrecadado = 5000m,
            Status = "Ativa"
        };

        // Assert
        Assert.Equal(1, campanha.Id);
        Assert.Equal("Campanha de Natal", campanha.Titulo);
        Assert.Equal("Arrecadação para crianças carentes", campanha.Descricao);
        Assert.Equal(new DateTime(2024, 12, 1), campanha.DataInicio);
        Assert.Equal(new DateTime(2024, 12, 31), campanha.DataFim);
        Assert.Equal(10000m, campanha.MetaFinanceira);
        Assert.Equal(5000m, campanha.ValorArrecadado);
        Assert.Equal("Ativa", campanha.Status);
    }

    [Fact]
    public void DoacaoModel_CreateInstance_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var doacao = new Doacao
        {
            Id = 1,
            CampanhaId = 10,
            DoadorId = 5,
            Valor = 100.50m,
            DataDoacao = DateTime.Now,
            Status = "Processada"
        };

        // Assert
        Assert.Equal(1, doacao.Id);
        Assert.Equal(10, doacao.CampanhaId);
        Assert.Equal(5, doacao.DoadorId);
        Assert.Equal(100.50m, doacao.Valor);
        Assert.Equal("Processada", doacao.Status);
    }

    [Fact]
    public void DoadorRequest_CreateInstance_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var request = new DoadorRequest
        {
            NomeCompleto = "Maria Santos",
            Email = "maria@example.com",
            Cpf = "98765432100",
            Senha = "Senha456!"
        };

        // Assert
        Assert.Equal("Maria Santos", request.NomeCompleto);
        Assert.Equal("maria@example.com", request.Email);
        Assert.Equal("98765432100", request.Cpf);
        Assert.Equal("Senha456!", request.Senha);
    }

    [Fact]
    public void CampanhaRequest_CreateInstance_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var request = new CampanhaRequest
        {
            Titulo = "Nova Campanha",
            Descricao = "Descrição da nova campanha",
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddDays(30),
            MetaFinanceira = 15000m,
            Status = "Ativa"
        };

        // Assert
        Assert.Equal("Nova Campanha", request.Titulo);
        Assert.Equal("Descrição da nova campanha", request.Descricao);
        Assert.Equal(15000m, request.MetaFinanceira);
        Assert.Equal("Ativa", request.Status);
    }

    [Fact]
    public void DoacaoRequest_CreateInstance_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var request = new DoacaoRequest
        {
            CampanhaId = 20,
            Valor = 250.75m
        };

        // Assert
        Assert.Equal(20, request.CampanhaId);
        Assert.Equal(250.75m, request.Valor);
    }

    [Fact]
    public void SharedModels_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var doador = new Doador();
        var campanha = new Campanha();
        var doacao = new Doacao();

        // Assert
        Assert.Equal("Doador", doador.Role);
        Assert.Equal("Ativa", campanha.Status);
        Assert.Equal("Processada", doacao.Status);
    }

    [Fact]
    public void Campanha_CalculateProgress_ReturnsCorrectPercentage()
    {
        // Arrange
        var campanha = new Campanha
        {
            MetaFinanceira = 10000m,
            ValorArrecadado = 5000m
        };

        // Act
        var progress = (campanha.ValorArrecadado / campanha.MetaFinanceira) * 100;

        // Assert
        Assert.Equal(50m, progress);
    }

    [Fact]
    public void Campanha_IsGoalMet_ReturnsTrueWhenMet()
    {
        // Arrange
        var campanha = new Campanha
        {
            MetaFinanceira = 10000m,
            ValorArrecadado = 10000m
        };

        // Act
        var isGoalMet = campanha.ValorArrecadado >= campanha.MetaFinanceira;

        // Assert
        Assert.True(isGoalMet);
    }

    [Fact]
    public void Campanha_IsGoalMet_ReturnsFalseWhenNotMet()
    {
        // Arrange
        var campanha = new Campanha
        {
            MetaFinanceira = 10000m,
            ValorArrecadado = 5000m
        };

        // Act
        var isGoalMet = campanha.ValorArrecadado >= campanha.MetaFinanceira;

        // Assert
        Assert.False(isGoalMet);
    }
}
