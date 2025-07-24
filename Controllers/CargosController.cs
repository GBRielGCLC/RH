using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_Backend.Data;
using RH_Backend.Models;

namespace RH_Backend.Controllers
{
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
        public async Task<IActionResult> GetCargos()
        {
            var cargos = await _appDbContext.Cargos.ToListAsync();
            return Ok(cargos);
        }

        [HttpPost]
        public async Task<IActionResult> AddCargo(Cargo cargo)
        {
            _appDbContext.Cargos.Add(cargo);
            await _appDbContext.SaveChangesAsync();
            return Ok(cargo);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCargo(Cargo cargo)
        {
            var cargoExistente = await _appDbContext.Cargos.FindAsync(cargo.Id);

            if (cargoExistente == null) return NotFound();

            _appDbContext.Entry(cargoExistente).CurrentValues.SetValues(cargo);

            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCargo(int id)
        {
            var cargoExistente = await _appDbContext.Cargos.FindAsync(id);

            if (cargoExistente == null) return NotFound();

            _appDbContext.Cargos.Remove(cargoExistente);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}