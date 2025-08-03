using ImoveisAPI.Models;

namespace ImoveisAPI.Repositories.Interfaces
{
    public interface IProprietarioRepository
    {
        Task<Proprietario?> GetByDocumentoAsync(string documento);
        Task AddAsync(Proprietario proprietario);
    }
}
