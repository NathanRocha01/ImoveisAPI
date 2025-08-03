using Xunit;
using Moq;
using FluentAssertions;
using ImoveisAPI.Services;
using ImoveisAPI.Repositories;
using ImoveisAPI.Models;
using ImoveisAPI.DTOs;
using System;
using System.Threading.Tasks;
using ImoveisAPI.Repositories.Interfaces;

namespace ImoveisAPI.Tests
{
    public class CadastroServiceTests
    {
        private readonly Mock<ICadastroRepository> _cadastroRepoMock;
        private readonly Mock<IProprietarioRepository> _proprietarioRepoMock;
        private readonly ValorImovelService _valorService;
        private readonly CadastroService _service;

        public CadastroServiceTests()
        {
            _cadastroRepoMock = new Mock<ICadastroRepository>();
            _proprietarioRepoMock = new Mock<IProprietarioRepository>();
            _valorService = new ValorImovelService();
            _service = new CadastroService(_cadastroRepoMock.Object, _proprietarioRepoMock.Object, _valorService);
        }

        [Fact]
        public async Task IniciarCadastro_DeveCriarCadastroQuandoDocumentoNaoExiste()
        {
            // Arrange
            var dto = new ProprietarioDTO { Nome = "João", Documento = "123" };
            _proprietarioRepoMock.Setup(r => r.GetByDocumentoAsync(dto.Documento)).ReturnsAsync((Proprietario)null);
            _proprietarioRepoMock.Setup(r => r.AddAsync(It.IsAny<Proprietario>())).Returns(Task.CompletedTask);
            _cadastroRepoMock.Setup(r => r.AddAsync(It.IsAny<Cadastro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.IniciarCadastroAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(StatusCadastro.Proprietario);
            _proprietarioRepoMock.Verify(r => r.AddAsync(It.IsAny<Proprietario>()), Times.Once);
            _cadastroRepoMock.Verify(r => r.AddAsync(It.IsAny<Cadastro>()), Times.Once);
        }

        [Fact]
        public async Task IniciarCadastro_DeveLancarExcecaoQuandoDocumentoJaExiste()
        {
            // Arrange
            var dto = new ProprietarioDTO { Nome = "João", Documento = "123" };
            _proprietarioRepoMock.Setup(r => r.GetByDocumentoAsync(dto.Documento)).ReturnsAsync(new Proprietario());

            // Act
            Func<Task> act = async () => await _service.IniciarCadastroAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Já existe um proprietário com esse documento.");
        }

        [Fact]
        public async Task AdicionarImovel_DeveAdicionarImovelEAjustarStatus()
        {
            // Arrange
            var cadastro = new Cadastro
            {
                Id = 1,
                Status = StatusCadastro.Proprietario,
                ProprietarioId = 1,
                Proprietario = new Proprietario { Imoveis = new List<Imovel>() }
            };
            var imovelDto = new ImovelDTO { Area = 10, Endereco = "Rua X" };

            _cadastroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cadastro);
            _cadastroRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cadastro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.AdicionarImovelAsync(1, imovelDto);

            // Assert
            result.Proprietario.Imoveis.Should().HaveCount(1);
            result.Status.Should().Be(StatusCadastro.Imovel);
        }

        [Fact]
        public async Task AdicionarImovel_DeveLancarExcecaoQuandoCadastroFinalizado()
        {
            // Arrange
            var cadastro = new Cadastro
            {
                Id = 1,
                Status = StatusCadastro.Finalizado,
                Proprietario = new Proprietario { Imoveis = new List<Imovel>() }
            };
            _cadastroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cadastro);

            // Act
            Func<Task> act = async () => await _service.AdicionarImovelAsync(1, new ImovelDTO());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Cadastro já finalizado. Não é possível adicionar imóveis.");
        }

        [Fact]
        public async Task FinalizarCadastro_DeveAtualizarStatusParaFinalizado()
        {
            // Arrange
            var cadastro = new Cadastro
            {
                Id = 1,
                Proprietario = new Proprietario { Imoveis = new List<Imovel> { new Imovel { Area = 5 } } },
                Status = StatusCadastro.Imovel
            };
            _cadastroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cadastro);
            _cadastroRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cadastro>())).Returns(Task.CompletedTask);

            // Act
            await _service.FinalizarCadastroAsync(1);

            // Assert
            cadastro.Status.Should().Be(StatusCadastro.Finalizado);
        }

        [Fact]
        public async Task FinalizarCadastro_DeveLancarExcecaoQuandoNaoTemImoveis()
        {
            // Arrange
            var cadastro = new Cadastro
            {
                Id = 1,
                Proprietario = new Proprietario { Imoveis = new List<Imovel>() },
                Status = StatusCadastro.Imovel
            };
            _cadastroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cadastro);

            // Act
            Func<Task> act = async () => await _service.FinalizarCadastroAsync(1);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Não é possível finalizar o cadastro sem pelo menos um imóvel.");
        }

        [Fact]
        public async Task RetomarCadastro_DeveRetornarStatus()
        {
            // Arrange
            var cadastro = new Cadastro { Id = 1, Status = StatusCadastro.Imovel };
            _cadastroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cadastro);

            // Act
            var result = await _service.RetomarCadastroAsync(1);

            // Assert
            result.Id.Should().Be(1);
            result.Status.Should().Be(StatusCadastro.Imovel.ToString());
        }

        [Fact]
        public async Task ObterResumo_DeveRetornarResumoQuandoFinalizado()
        {
            // Arrange
            var cadastro = new Cadastro
            {
                Id = 1,
                Status = StatusCadastro.Finalizado,
                Proprietario = new Proprietario
                {
                    Nome = "Maria",
                    Documento = "123",
                    Imoveis = new List<Imovel> { new Imovel { Area = 5, Endereco = "Fazenda" } }
                }
            };
            _cadastroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cadastro);

            // Act
            var result = await _service.ObterResumoAsync(1);

            // Assert
            result.NomeProprietario.Should().Be("Maria");
            result.Imoveis.Should().HaveCount(1);
            result.ValorTotal.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ObterResumo_DeveLancarExcecaoQuandoNaoFinalizado()
        {
            // Arrange
            var cadastro = new Cadastro { Id = 1, Status = StatusCadastro.Imovel };
            _cadastroRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cadastro);

            // Act
            Func<Task> act = async () => await _service.ObterResumoAsync(1);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("O cadastro ainda não foi finalizado.");
        }
    }
}