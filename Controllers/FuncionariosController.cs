using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using RH_Backend.Data;
using RH_Backend.DTO;
using RH_Backend.Models;

namespace RH_Backend.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionariosController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public FuncionariosController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<FuncionarioResponseGet>> Get()
        {
            var funcionarios = await _appDbContext.Funcionarios
                .Include(f => f.Cargo)
                .ToListAsync();

            var funcionariosDto = funcionarios.Select(f =>
            new FuncionarioResponseDto
            {
                Id = f.Id,
                Nome = f.Nome,
                Cargo = f.Cargo == null ? null : new CargoDto
                {
                    Id = f.Cargo.Id,
                    Nome = f.Cargo.Nome,
                    Salario = f.Cargo.Salario
                },
                DataAdmissao = f.DataAdmissao,
                Ativo = f.Ativo
            });

            var response = new FuncionarioResponseGet
            {
                Funcionarios = funcionariosDto,
                QuantidadeFuncionarios = funcionariosDto.Count(),
                QuantidadeFuncionariosAtivos = funcionarios.Count(f => f.Ativo),
                QuantidadeFuncionariosInativos = funcionarios.Count(f => !f.Ativo),
                MediaSalarial = funcionarios.Average(f => f.Cargo == null ? 0 : f.Cargo.Salario),
                SalarioTotal = funcionarios.Sum(f =>
                {
                    return f.Cargo == null ? 0 : f.Cargo.Salario;
                })
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Funcionario>> Post(FuncionarioPostDto dto)
        {
            if (dto.CargoId != null)
            {
                var cargoExiste = await _appDbContext.Cargos.AnyAsync(c => c.Id == dto.CargoId);
                if (!cargoExiste)
                    return BadRequest($"Cargo com Id {dto.CargoId} não existe.");
            }

            var funcionario = new Funcionario
            {
                Nome = dto.Nome,
                CargoId = dto.CargoId,
                DataAdmissao = dto.DataAdmissao,
                Ativo = dto.Ativo
            };

            _appDbContext.Funcionarios.Add(funcionario);
            await _appDbContext.SaveChangesAsync();

            // Retorna o DTO de resposta com dados do cargo carregados
            await _appDbContext.Entry(funcionario).Reference(f => f.Cargo).LoadAsync();

            var responseDto = new FuncionarioResponseDto
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Cargo = funcionario.Cargo,
                DataAdmissao = funcionario.DataAdmissao,
                Ativo = funcionario.Ativo
            };

            return Ok(responseDto);
        }

        [HttpPut]
        public async Task<ActionResult<FuncionarioPutDto>> Put(FuncionarioPutDto funcionario)
        {
            var funcionarioExistente = await _appDbContext.Funcionarios.FindAsync(funcionario.Id);
            if (funcionarioExistente == null) return NotFound();

            if (funcionario.CargoId != null)
            {
                var cargoExiste = await _appDbContext.Cargos.AnyAsync(c => c.Id == funcionario.CargoId);
                if (!cargoExiste)
                    return BadRequest($"Cargo com Id {funcionario.CargoId} não existe.");
            }

            var historicos = new List<HistoricoFuncionario>();
            DateTime agora = DateTime.Now;

            if (funcionarioExistente.Nome != funcionario.Nome)
            {
                historicos.Add(new HistoricoFuncionario
                {
                    FuncionarioId = funcionarioExistente.Id,
                    CampoAlterado = "Nome",
                    ValorAntigo = funcionarioExistente.Nome,
                    ValorNovo = funcionario.Nome,
                    DataAlteracao = agora
                });

                funcionarioExistente.Nome = funcionario.Nome;
            }

            if (funcionarioExistente.CargoId != funcionario.CargoId)
            {
                string nomeCargoAntigo = "";
                string nomeCargoNovo = "";

                if (funcionarioExistente.CargoId != null)
                {
                    var cargoAntigo = await _appDbContext.Cargos.FindAsync(funcionarioExistente.CargoId);
                    nomeCargoAntigo = cargoAntigo?.Nome ?? "";
                }

                if (funcionario.CargoId != null)
                {
                    var cargoNovo = await _appDbContext.Cargos.FindAsync(funcionario.CargoId);
                    nomeCargoNovo = cargoNovo?.Nome ?? "";
                }

                historicos.Add(new HistoricoFuncionario
                {
                    FuncionarioId = funcionarioExistente.Id,
                    CampoAlterado = "Cargo",
                    ValorAntigo = nomeCargoAntigo,
                    ValorNovo = nomeCargoNovo,
                    DataAlteracao = agora
                });

                funcionarioExistente.CargoId = funcionario.CargoId;
            }

            if (funcionarioExistente.DataAdmissao != funcionario.DataAdmissao)
            {
                historicos.Add(new HistoricoFuncionario
                {
                    FuncionarioId = funcionarioExistente.Id,
                    CampoAlterado = "DataAdmissao",
                    ValorAntigo = funcionarioExistente.DataAdmissao.ToString("yyyy-MM-dd"),
                    ValorNovo = funcionario.DataAdmissao.ToString("yyyy-MM-dd"),
                    DataAlteracao = agora
                });

                funcionarioExistente.DataAdmissao = funcionario.DataAdmissao;
            }

            if (funcionarioExistente.Ativo != funcionario.Ativo)
            {
                historicos.Add(new HistoricoFuncionario
                {
                    FuncionarioId = funcionarioExistente.Id,
                    CampoAlterado = "Ativo",
                    ValorAntigo = funcionarioExistente.Ativo.ToString(),
                    ValorNovo = funcionario.Ativo.ToString(),
                    DataAlteracao = agora
                });

                funcionarioExistente.Ativo = funcionario.Ativo;
            }

            await _appDbContext.SaveChangesAsync();

            // Salva históricos, se houver
            if (historicos.Count > 0)
            {
                _appDbContext.HistoricoFuncionarios.AddRange(historicos);
                await _appDbContext.SaveChangesAsync();
            }

            return Ok(funcionario);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Funcionario>> Delete(int id)
        {
            var funcionarioExistente = await _appDbContext.Funcionarios.FindAsync(id);

            if (funcionarioExistente == null) return NotFound();

            _appDbContext.Funcionarios.Remove(funcionarioExistente);
            await _appDbContext.SaveChangesAsync();
            return Ok(funcionarioExistente);
        }

        [HttpGet("Relatorio")]
        public async Task<IActionResult> GerarRelatorioFuncionarios()
        {
            var funcionarios = await _appDbContext.Funcionarios
                .Include(f => f.Cargo)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("Relatório de Funcionários").FontSize(20).Bold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            // Nome
                            columns.RelativeColumn(2);
                            // Cargo
                            columns.RelativeColumn(2);
                            // Data
                            columns.RelativeColumn(2);
                            // Salário
                            columns.RelativeColumn(2);
                            // Status
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Nome").Bold();
                            header.Cell().Text("Cargo").Bold();
                            header.Cell().Text("Data Admissão").Bold();
                            header.Cell().Text("Salário").Bold();
                            header.Cell().Text("Status").Bold();
                        });

                        foreach (var f in funcionarios)
                        {
                            table.Cell().Text(f.Nome);
                            table.Cell().Text(f.Cargo?.Nome ?? "N/A");
                            table.Cell().Text(f.DataAdmissao.ToString("dd/MM/yyyy"));
                            table.Cell().Text(f.Cargo?.Salario.ToString("C", new System.Globalization.CultureInfo("pt-BR")));
                            table.Cell().Text(f.Ativo ? "Ativo" : "Inativo");
                        }
                    });
                });
            });

            var pdfStream = new MemoryStream();
            document.GeneratePdf(pdfStream);
            pdfStream.Position = 0;

            return File(pdfStream, "application/pdf", "relatorio-funcionarios.pdf");
        }
    }
}