using ImoveisAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImoveisAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProprietarioController : ControllerBase
    {
        private readonly ProprietarioService _proprietarioService;

        public ProprietarioController(ProprietarioService proprietarioService)
        {
            _proprietarioService = proprietarioService;
        }

        // Buscar proprietário por documento
        [HttpGet("{documento}")]
        public async Task<IActionResult> BuscarPorDocumento(string documento)
        {
            var proprietario = await _proprietarioService.BuscarPorDocumentoAsync(documento);
            return Ok(proprietario);
        }

        // Calcular valor total dos imóveis
        [HttpGet("{documento}/valor-total")]
        public async Task<IActionResult> CalcularValorTotal(string documento)
        {
            var valorTotal = await _proprietarioService.CalcularValorTotalImoveisAsync(documento);
            return Ok(new { Documento = documento, ValorTotal = valorTotal });
        }
    }
}
