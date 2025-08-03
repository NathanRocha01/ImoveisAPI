using ImoveisAPI.DTOs;
using ImoveisAPI.Models;
using ImoveisAPI.Repositories;
using ImoveisAPI.Repositories.Interfaces;

namespace ImoveisAPI.Services
{
    public class ProprietarioService
    {
        private readonly IProprietarioRepository _proprietarioRepository;
        private readonly ValorImovelService _valorImovelService;

        public ProprietarioService(IProprietarioRepository proprietarioRepository, ValorImovelService valorImovelService)
        {
            _proprietarioRepository = proprietarioRepository;
            _valorImovelService = valorImovelService;
        }

        // Buscar proprietário por documento
        public async Task<ProprietarioDetalhadoDTO> BuscarPorDocumentoAsync(string documento)
        {
            var proprietario = await _proprietarioRepository.GetByDocumentoAsync(documento)
            ?? throw new KeyNotFoundException("Proprietário não encontrado.");

            return new ProprietarioDetalhadoDTO
            {
                Nome = proprietario.Nome,
                Documento = proprietario.Documento,
                Imoveis = proprietario.Imoveis.Select(i => new ImovelDTO
                {
                    Area = i.Area,
                    Endereco = i.Endereco
                }).ToList()
            };
        }

        // Calcular valor total de todos os imóveis de um proprietário
        public async Task<decimal> CalcularValorTotalImoveisAsync(string documento)
        {
            var proprietario = await _proprietarioRepository.GetByDocumentoAsync(documento)
                ?? throw new KeyNotFoundException("Proprietário não encontrado.");

            return _valorImovelService.CalcularValorTotal(proprietario.Imoveis.Select(i => i.Area));
        }
    }
}
