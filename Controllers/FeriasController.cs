using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_Backend.Data;
using RH_Backend.DTO;
using RH_Backend.Models;

namespace RH_Backend.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class FeriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FeriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<FeriasResponseDto>> Get()
        {
            var ferias = await _context.Ferias
                .Include(f => f.Funcionario)
                .ToListAsync();

            var hoje = DateOnly.FromDateTime(DateTime.Today);

            var result = ferias.Select(f => new FeriasResponseDto
            {
                Id = f.Id,
                DataInicio = f.DataInicio,
                DataTermino = f.DataTermino,
                Status = f.Status,
                Funcionario = new FuncionarioResumoDto
                {
                    Id = f.Funcionario.Id,
                    Nome = f.Funcionario.Nome
                }
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Ferias>> Post(FeriasPostDto dto)
        {
            if (!await _context.Funcionarios.AnyAsync(f => f.Id == dto.FuncionarioId))
                return BadRequest("Funcionário não encontrado.");

            var ferias = new Ferias
            {
                FuncionarioId = dto.FuncionarioId,
                DataInicio = dto.DataInicio,
                DataTermino = dto.DataTermino
            };

            _context.Ferias.Add(ferias);
            await _context.SaveChangesAsync();

            return Ok(ferias);
        }

        [HttpPut]
        public async Task<ActionResult<Ferias>> Put(FeriasPutDto dto)
        {
            var ferias = await _context.Ferias.FindAsync(dto.Id);
            if (ferias == null) return NotFound();

            if (!await _context.Funcionarios.AnyAsync(f => f.Id == dto.FuncionarioId))
                return BadRequest("Funcionário não encontrado.");

            ferias.DataInicio = dto.DataInicio;
            ferias.DataTermino = dto.DataTermino;
            ferias.FuncionarioId = dto.FuncionarioId;

            await _context.SaveChangesAsync();

            return Ok(ferias);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Ferias>> Delete(int id)
        {
            var ferias = await _context.Ferias.FindAsync(id);
            if (ferias == null) return NotFound();

            _context.Ferias.Remove(ferias);
            await _context.SaveChangesAsync();

            return Ok(ferias);
        }
    }
}