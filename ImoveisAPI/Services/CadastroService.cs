using ImoveisAPI.DTOs;
using ImoveisAPI.Models;
using ImoveisAPI.Repositories;
using ImoveisAPI.Repositories.Interfaces;

namespace ImoveisAPI.Services
{
    public class CadastroService
    {
        private readonly ICadastroRepository _cadastroRepository;
        private readonly IProprietarioRepository _proprietarioRepository;
        private readonly ValorImovelService _valorImovelService;

        public CadastroService(ICadastroRepository cadastroRepository, IProprietarioRepository proprietarioRepository, ValorImovelService valorImovelService)
        {
            _cadastroRepository = cadastroRepository;
            _proprietarioRepository = proprietarioRepository;
            _valorImovelService = valorImovelService;
        }

        // 1 - Iniciar cadastro
        public async Task<Cadastro> IniciarCadastroAsync(ProprietarioDTO proprietarioDto)
        {
            var proprietarioExistente = await _proprietarioRepository.GetByDocumentoAsync(proprietarioDto.Documento);
            if (proprietarioExistente != null)
                throw new InvalidOperationException("Já existe um proprietário com esse documento.");

            var novoProprietario = new Proprietario
            {
                Nome = proprietarioDto.Nome,
                Documento = proprietarioDto.Documento
            };
            await _proprietarioRepository.AddAsync(novoProprietario);

            var cadastro = new Cadastro
            {
                ProprietarioId = novoProprietario.Id,
                Status = StatusCadastro.Proprietario
            };
            await _cadastroRepository.AddAsync(cadastro);

            return cadastro;
        }

        // 2 - Adicionar imóvel
        public async Task<Cadastro> AdicionarImovelAsync(int cadastroId, ImovelDTO imovelDto)
        {
            var cadastro = await _cadastroRepository.GetByIdAsync(cadastroId)
                ?? throw new KeyNotFoundException("Cadastro não encontrado.");

            if (cadastro.Status == StatusCadastro.Finalizado)
                throw new InvalidOperationException("Cadastro já finalizado. Não é possível adicionar imóveis.");

            var imovel = new Imovel
            {
                ProprietarioId = cadastro.ProprietarioId,
                Area = imovelDto.Area,
                Endereco = imovelDto.Endereco
            };

            cadastro.Proprietario.Imoveis.Add(imovel);

            // Atualiza status para "Imovel" se ainda está em Proprietario
            if (cadastro.Status == StatusCadastro.Proprietario)
                cadastro.Status = StatusCadastro.Imovel;

            cadastro.AtualizadoEm = DateTime.UtcNow;
            await _cadastroRepository.UpdateAsync(cadastro);

            return cadastro;
        }

        // 3 - Finalizar cadastro
        public async Task FinalizarCadastroAsync(int cadastroId)
        {
            var cadastro = await _cadastroRepository.GetByIdAsync(cadastroId)
                ?? throw new KeyNotFoundException("Cadastro não encontrado.");

            if (!cadastro.Proprietario.Imoveis.Any())
                throw new InvalidOperationException("Não é possível finalizar o cadastro sem pelo menos um imóvel.");

            cadastro.Status = StatusCadastro.Finalizado;
            cadastro.AtualizadoEm = DateTime.UtcNow;

            await _cadastroRepository.UpdateAsync(cadastro);
        }

        // 4 - Retomar cadastro
        public async Task<CadastroStatusDTO> RetomarCadastroAsync(int cadastroId)
        {
            var cadastro = await _cadastroRepository.GetByIdAsync(cadastroId)
            ?? throw new KeyNotFoundException("Cadastro não encontrado.");

            return new CadastroStatusDTO
            {
                Id = cadastro.Id,
                Status = cadastro.Status.ToString()
            };
        }

        // 5 - Obter resumo do cadastro
        public async Task<CadastroResumoDTO> ObterResumoAsync(int cadastroId)
        {
            var cadastro = await _cadastroRepository.GetByIdAsync(cadastroId)
            ?? throw new KeyNotFoundException("Cadastro não encontrado.");

            if (cadastro.Status != StatusCadastro.Finalizado)
                throw new InvalidOperationException("O cadastro ainda não foi finalizado.");

            var valorTotal = _valorImovelService.CalcularValorTotal(cadastro.Proprietario.Imoveis.Select(i => i.Area));

            return new CadastroResumoDTO
            {
                NomeProprietario = cadastro.Proprietario.Nome,
                DocumentoProprietario = cadastro.Proprietario.Documento,
                Status = cadastro.Status.ToString(),
                Imoveis = cadastro.Proprietario.Imoveis.Select(i => new ImovelDTO
                {
                    Area = i.Area,
                    Endereco = i.Endereco
                }).ToList(),
                ValorTotal = valorTotal
            };
        }
    }
}
