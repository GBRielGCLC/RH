namespace RH_Backend.Models
{
    public class Funcionario
    {
        public int Id { get; set; }
        public required string Nome { get; set; }

        public int? CargoId { get; set; }
        public Cargo? Cargo { get; set; }

        public DateOnly DataAdmissao { get; set; }
        public bool Ativo { get; set; }
    }
}