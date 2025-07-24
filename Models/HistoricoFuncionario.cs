namespace RH_Backend.Models
{
    public class HistoricoFuncionario
    {
        public int Id { get; set; }

        public int FuncionarioId { get; set; }
        public string CampoAlterado { get; set; } = string.Empty;
        public string ValorAntigo { get; set; } = string.Empty;
        public string ValorNovo { get; set; } = string.Empty;
        public DateTime DataAlteracao { get; set; }

        public Funcionario Funcionario { get; set; } = null!;
    }
}