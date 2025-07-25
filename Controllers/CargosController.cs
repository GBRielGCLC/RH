using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_Backend.Data;
using RH_Backend.Models;
using RH_Backend.DTO;

namespace RH_Backend.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class CargosController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public CargosController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cargo>>> Get()
        {
            var cargos = await _appDbContext.Cargos.ToListAsync();
            return Ok(cargos);
        }

        [HttpPost]
        public async Task<ActionResult<Cargo>> Post(PostCargoDTO cargoDto)
        {
            var cargo = new Cargo
            {
                Nome = cargoDto.Nome,
                Salario = cargoDto.Salario
            };

            _appDbContext.Cargos.Add(cargo);
            await _appDbContext.SaveChangesAsync();
            return Ok(cargo);
        }

        [HttpPut]
        public async Task<ActionResult<Cargo>> Put(Cargo cargo)
        {
            var cargoExistente = await _appDbContext.Cargos.FindAsync(cargo.Id);

            if (cargoExistente == null) return NotFound();

            _appDbContext.Entry(cargoExistente).CurrentValues.SetValues(cargo);

            await _appDbContext.SaveChangesAsync();
            return Ok(cargo);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Cargo>> DeleteCargo(int id)
        {
            var cargoExistente = await _appDbContext.Cargos.FindAsync(id);

            if (cargoExistente == null) return NotFound();

            _appDbContext.Cargos.Remove(cargoExistente);
            await _appDbContext.SaveChangesAsync();
            return Ok(cargoExistente);
        }
    }
}