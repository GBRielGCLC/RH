using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RH_Backend.DTO
{
    public class FuncionarioCargoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public CargoDto Cargo { get; set; } = null!;
    }

}