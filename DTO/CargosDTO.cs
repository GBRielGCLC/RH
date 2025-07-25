using Microsoft.AspNetCore.Mvc;
using RH_Backend.Models;

namespace RH_Backend.DTO
{
    public class PostCargoDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public double Salario { get; set; }
    }
}