namespace SharedModels;

public class Campanha
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal MetaFinanceira { get; set; }
    public decimal ValorArrecadado { get; set; }
    public string Status { get; set; } = "Ativa"; // Ativa, Concluida, Cancelada
}
