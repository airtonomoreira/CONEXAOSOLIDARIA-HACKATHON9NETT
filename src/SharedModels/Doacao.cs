namespace SharedModels;

public class Doacao
{
    public int Id { get; set; }
    public int CampanhaId { get; set; }
    public int DoadorId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataDoacao { get; set; }
    public string Status { get; set; } = "Processada"; // Processada, Pendente
}
