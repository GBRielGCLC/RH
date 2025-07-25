using RH_Backend.Models;

namespace RH_Backend.DTO
{
    public class FuncionarioPostDto
    {
        public required string Nome { get; set; }

        public int? CargoId { get; set; }

        public DateOnly DataAdmissao { get; set; }
        public bool Ativo { get; set; }
    }

    public class FuncionarioPutDto : FuncionarioPostDto
    {
        public required int Id { get; set; }
    }

    public class FuncionarioResponseDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public DateOnly DataAdmissao { get; set; }
        public bool Ativo { get; set; }

        public Cargo? Cargo { get; set; }

    }

    public class FuncionarioResponseGet
    {
        public required IEnumerable<FuncionarioResponseDto> Funcionarios { get; set; }
        public double MediaSalarial { get; set; }
        public double SalarioTotal { get; set; }
        public int QuantidadeFuncionarios { get; set; }
        public int QuantidadeFuncionariosAtivos { get; set; }
        public int QuantidadeFuncionariosInativos { get; set; }
    }

    public class CargoDto : Cargo { }

}