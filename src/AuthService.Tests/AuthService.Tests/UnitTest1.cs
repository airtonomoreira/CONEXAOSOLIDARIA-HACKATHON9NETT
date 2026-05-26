using SharedModels;
using Xunit;

namespace AuthService.Tests;

public class AuthServiceTests
{
    [Fact]
    public void IsValidCpf_ValidCpf_ReturnsTrue()
    {
        // Arrange
        var validCpf = "12345678909";

        // Act
        var result = IsValidCpf(validCpf);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidCpf_InvalidCpf_ReturnsFalse()
    {
        // Arrange
        var invalidCpf = "12345678900";

        // Act
        var result = IsValidCpf(invalidCpf);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidCpf_AllSameDigits_ReturnsFalse()
    {
        // Arrange
        var invalidCpf = "11111111111";

        // Act
        var result = IsValidCpf(invalidCpf);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidCpf_Empty_ReturnsFalse()
    {
        // Arrange
        var invalidCpf = "";

        // Act
        var result = IsValidCpf(invalidCpf);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidCpf_WithSpecialChars_ReturnsTrue()
    {
        // Arrange
        var validCpf = "123.456.789-09";

        // Act
        var result = IsValidCpf(validCpf);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidCpf_TooShort_ReturnsFalse()
    {
        // Arrange
        var invalidCpf = "123456789";

        // Act
        var result = IsValidCpf(invalidCpf);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidCpf_TooLong_ReturnsFalse()
    {
        // Arrange
        var invalidCpf = "123456789012";

        // Act
        var result = IsValidCpf(invalidCpf);

        // Assert
        Assert.False(result);
    }

    private bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        cpf = new string(cpf.Where(char.IsDigit).ToArray());
        if (cpf.Length != 11)
            return false;

        if (cpf.All(c => c == cpf[0]))
            return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito = digito + resto.ToString();
        return cpf.EndsWith(digito);
    }
}
