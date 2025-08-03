using ImoveisAPI.Data;
using ImoveisAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ImoveisAPI.Repositories
{
    public class CadastroRepository
    {
        private readonly AppDbContext _context;

        public CadastroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cadastro> GetByIdAsync(int id)
        {
            return await _context.Cadastros
                .Include(c => c.Proprietario)
                .ThenInclude(p => p.Imoveis)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cadastro>> GetAllAsync()
        {
            return await _context.Cadastros
                .Include(c => c.Proprietario)
                .ThenInclude(p => p.Imoveis)
                .ToListAsync();
        }

        public async Task AddAsync(Cadastro cadastro)
        {
            await _context.Cadastros.AddAsync(cadastro);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cadastro cadastro)
        {
            _context.Cadastros.Update(cadastro);
            await _context.SaveChangesAsync();
        }
    }
}
