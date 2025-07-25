namespace RH_Backend.DTO
{
    public class FeriasPostDto
    {
        public DateOnly DataInicio { get; set; }
        public DateOnly DataTermino { get; set; }
        public int FuncionarioId { get; set; }
    }

    public class FeriasPutDto : FeriasPostDto
    {
        public int Id { get; set; }
    }

    public class FuncionarioResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class FeriasResponseDto
    {
        public int Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataTermino { get; set; }
        public string Status { get; set; } = string.Empty;
        public FuncionarioResumoDto Funcionario { get; set; } = null!;
    }
}