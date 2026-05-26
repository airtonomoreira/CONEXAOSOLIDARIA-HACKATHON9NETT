using DonationWorker;
using System;
using System.Text.Json;
using Xunit;

namespace DonationWorker.Tests;

public class DonationWorkerTests
{
    [Fact]
    public void DoacaoEvent_Deserialize_ValidJson_ReturnsObject()
    {
        // Arrange
        var json = @"{
            ""DoacaoId"": 1,
            ""CampanhaId"": 10,
            ""Valor"": 100.50,
            ""DataDoacao"": ""2024-01-15T10:30:00""
        }";

        // Act
        var doacaoEvent = JsonSerializer.Deserialize<DoacaoEvent>(json);

        // Assert
        Assert.NotNull(doacaoEvent);
        Assert.Equal(1, doacaoEvent.DoacaoId);
        Assert.Equal(10, doacaoEvent.CampanhaId);
        Assert.Equal(100.50m, doacaoEvent.Valor);
    }

    [Fact]
    public void DoacaoEvent_Serialize_ValidJson_ReturnsString()
    {
        // Arrange
        var doacaoEvent = new DoacaoEvent
        {
            DoacaoId = 1,
            CampanhaId = 10,
            Valor = 100.50m,
            DataDoacao = new DateTime(2024, 1, 15, 10, 30, 0)
        };

        // Act
        var json = JsonSerializer.Serialize(doacaoEvent);

        // Assert
        Assert.NotNull(json);
        Assert.Contains("DoacaoId", json);
        Assert.Contains("CampanhaId", json);
    }

    [Fact]
    public void DoacaoEvent_DefaultValues_AreDefault()
    {
        // Arrange & Act
        var doacaoEvent = new DoacaoEvent();

        // Assert
        Assert.Equal(0, doacaoEvent.DoacaoId);
        Assert.Equal(0, doacaoEvent.CampanhaId);
        Assert.Equal(0m, doacaoEvent.Valor);
        Assert.Equal(default(DateTime), doacaoEvent.DataDoacao);
    }

    [Fact]
    public void DoacaoEvent_SetValues_ValuesPersist()
    {
        // Arrange
        var doacaoEvent = new DoacaoEvent
        {
            DoacaoId = 5,
            CampanhaId = 20,
            Valor = 500.75m,
            DataDoacao = DateTime.Now
        };

        // Act & Assert
        Assert.Equal(5, doacaoEvent.DoacaoId);
        Assert.Equal(20, doacaoEvent.CampanhaId);
        Assert.Equal(500.75m, doacaoEvent.Valor);
    }
}
