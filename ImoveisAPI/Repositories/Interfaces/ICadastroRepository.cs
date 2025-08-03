using ImoveisAPI.Models;

namespace ImoveisAPI.Repositories.Interfaces
{
    public interface ICadastroRepository
    {
        Task<Cadastro?> GetByIdAsync(int id);
        Task AddAsync(Cadastro cadastro);
        Task UpdateAsync(Cadastro cadastro);
    }
}
