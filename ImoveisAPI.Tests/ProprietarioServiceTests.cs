using FluentAssertions;
using ImoveisAPI.Models;
using ImoveisAPI.Repositories.Interfaces;
using ImoveisAPI.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImoveisAPI.Tests
{
    public class ProprietarioServiceTests
    {

        private readonly Mock<IProprietarioRepository> _repoMock;
        private readonly ValorImovelService _valorImovelService;
        private readonly ProprietarioService _service;

        public ProprietarioServiceTests()
        {
            _repoMock = new Mock<IProprietarioRepository>();
            _valorImovelService = new ValorImovelService();
            _service = new ProprietarioService(_repoMock.Object, _valorImovelService);
        }

        [Fact]
        public async Task BuscarPorDocumento_DeveRetornarProprietarioDetalhado()
        {
            // Arrange
            var documento = "123";
            var proprietario = new Proprietario
            {
                Nome = "João",
                Documento = documento,
                Imoveis = new List<Imovel>
                {
                    new Imovel { Area = 10, Endereco = "Fazenda A" }
                }
            };
            _repoMock.Setup(r => r.GetByDocumentoAsync(documento)).ReturnsAsync(proprietario);

            // Act
            var result = await _service.BuscarPorDocumentoAsync(documento);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("João");
            result.Documento.Should().Be("123");
            result.Imoveis.Should().HaveCount(1);
        }

        [Fact]
        public async Task BuscarPorDocumento_DeveLancarExcecaoQuandoNaoEncontrar()
        {
            // Arrange
            var documento = "999";
            _repoMock.Setup(r => r.GetByDocumentoAsync(documento)).ReturnsAsync((Proprietario)null);

            // Act
            Func<Task> act = async () => await _service.BuscarPorDocumentoAsync(documento);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Proprietário não encontrado.");
        }

        [Fact]
        public async Task CalcularValorTotalImoveis_DeveRetornarSomaCorreta()
        {
            // Arrange
            var documento = "123";
            var proprietario = new Proprietario
            {
                Documento = documento,
                Imoveis = new List<Imovel>
                {
                    new Imovel { Area = 5 },   // 5 * 26.000 = 130.000
                    new Imovel { Area = 20 },  // 20 * 24.000 = 480.000
                    new Imovel { Area = 60 }   // 60 * 23.000 = 1.380.000
                }
            };
            _repoMock.Setup(r => r.GetByDocumentoAsync(documento)).ReturnsAsync(proprietario);

            // Act
            var result = await _service.CalcularValorTotalImoveisAsync(documento);

            // Assert
            result.Should().Be(130000 + 480000 + 1380000);
        }

        [Fact]
        public async Task CalcularValorTotalImoveis_DeveLancarExcecaoQuandoNaoEncontrar()
        {
            // Arrange
            var documento = "999";
            _repoMock.Setup(r => r.GetByDocumentoAsync(documento)).ReturnsAsync((Proprietario)null);

            // Act
            Func<Task> act = async () => await _service.CalcularValorTotalImoveisAsync(documento);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Proprietário não encontrado.");
        }
    }
}
