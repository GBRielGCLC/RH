using System.ComponentModel.DataAnnotations.Schema;

namespace RH_Backend.Models
{
    public class Ferias
    {
        public int Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataTermino { get; set; }
        public int FuncionarioId { get; set; }
        public Funcionario Funcionario { get; set; } = null!;

        // NÃO SALVA NO BD
        [NotMapped]
        public string Status
        {
            get
            {
                var hoje = DateOnly.FromDateTime(DateTime.Today);

                if (hoje < DataInicio)
                    return "Pendente";
                else if (hoje >= DataInicio && hoje <= DataTermino)
                    return "Em andamento";
                else
                    return "Concluídas";
            }
        }
    }
}