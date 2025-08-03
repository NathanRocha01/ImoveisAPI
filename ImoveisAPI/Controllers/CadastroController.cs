using ImoveisAPI.DTOs;
using ImoveisAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImoveisAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CadastroController : ControllerBase
    {
        private readonly CadastroService _cadastroService;

        public CadastroController(CadastroService cadastroService)
        {
            _cadastroService = cadastroService;
        }

        // Iniciar cadastro
        [HttpPost("iniciar")]
        public async Task<IActionResult> IniciarCadastro([FromBody] ProprietarioDTO proprietarioDto)
        {
            var cadastro = await _cadastroService.IniciarCadastroAsync(proprietarioDto);
            return CreatedAtAction(nameof(RetomarCadastro), new { id = cadastro.Id }, cadastro);
        }

        // Adicionar imóvel
        [HttpPost("{id}/imovel")]
        public async Task<IActionResult> AdicionarImovel(int id, [FromBody] ImovelDTO imovelDto)
        {
            var cadastro = await _cadastroService.AdicionarImovelAsync(id, imovelDto);
            return Ok(new
            {
                cadastro.Id,
                cadastro.Status,
                Proprietario = new
                {
                    cadastro.Proprietario.Nome,
                    cadastro.Proprietario.Documento,
                    Imoveis = cadastro.Proprietario.Imoveis.Select(i => new { i.Area, i.Endereco })
                }
            });
        }

        // Finalizar cadastro
        [HttpPost("{id}/finalizar")]
        public async Task<IActionResult> FinalizarCadastro(int id)
        {
            await _cadastroService.FinalizarCadastroAsync(id);
            return Ok(new { message = "Cadastro finalizado com sucesso!" });
        }

        // Obter resumo do cadastro finalizado
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterResumo(int id)
        {
            var resumo = await _cadastroService.ObterResumoAsync(id);
            return Ok(resumo);
        }

        // Retomar cadastro (pegar status atual)
        [HttpGet("{id}/retomar")]
        public async Task<IActionResult> RetomarCadastro(int id)
        {
            var statusDto = await _cadastroService.RetomarCadastroAsync(id);
            return Ok(statusDto);
        }
    }
}
