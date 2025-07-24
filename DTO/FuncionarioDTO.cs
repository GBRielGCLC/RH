using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public Cargo? Cargo { get; set; }

        public DateOnly DataAdmissao { get; set; }
        public bool Ativo { get; set; }
    }

    public class CargoDto : Cargo { }

}