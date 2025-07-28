using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using RH_Backend.Data;
using RH_Backend.DTO;
using RH_Backend.Models;

using QuestPDF.Helpers;
using System.Globalization;

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
        [Produces("application/pdf")]
        public async Task<IActionResult> GerarRelatorioFuncionarios()
        {
            var funcionarios = await _appDbContext.Funcionarios
                .Include(f => f.Cargo)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Row(row =>
                    {
                        row.RelativeColumn().Text("Relatório de Funcionários")
                            .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                        row.ConstantColumn(100).Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                            .FontSize(10).AlignRight().FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Nome
                            columns.RelativeColumn(2); // Cargo
                            columns.RelativeColumn(2); // Data Admissão
                            columns.RelativeColumn(2); // Salário
                            columns.RelativeColumn(1); // Status
                        });

                        // Cabeçalho da tabela
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nome").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cargo").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Data Admissão").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Salário").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Status").Bold();
                        });

                        // Linhas
                        int index = 0;
                        foreach (var f in funcionarios)
                        {
                            var background = index++ % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                            table.Cell().Background(background).Padding(5).Text(f.Nome);
                            table.Cell().Background(background).Padding(5).Text(f.Cargo?.Nome ?? "N/A");
                            table.Cell().Background(background).Padding(5).Text(f.DataAdmissao.ToString("dd/MM/yyyy"));
                            table.Cell().Background(background).Padding(5).Text(f.Cargo?.Salario.ToString("C", new CultureInfo("pt-BR")));
                            table.Cell().Padding(5).AlignCenter().Element(container =>
                            {
                                container
                                    .Background(f.Ativo ? Colors.Green.Lighten3 : Colors.Red.Lighten3)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text(f.Ativo ? "Ativo" : "Inativo")
                                    .FontColor(f.Ativo ? Colors.Green.Darken3 : Colors.Red.Darken3)
                                    .SemiBold()
                                    .FontSize(10);
                            });

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