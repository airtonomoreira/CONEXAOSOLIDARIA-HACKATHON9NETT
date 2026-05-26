namespace SharedModels;

public class CampanhaRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal MetaFinanceira { get; set; }
    public string Status { get; set; } = "Ativa";
}
