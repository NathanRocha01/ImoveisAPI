using ImoveisAPI.Data;
using ImoveisAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ImoveisAPI.Repositories
{
    public class ProprietarioRepository
    {
        private readonly AppDbContext _context;

        public ProprietarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Proprietario> GetByIdAsync(int id)
        {
            return await _context.Proprietarios
                .Include(p => p.Imoveis)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Proprietario> GetByDocumentoAsync(string documento)
        {
            return await _context.Proprietarios
                .Include(p => p.Imoveis)
                .FirstOrDefaultAsync(p => p.Documento == documento);
        }

        public async Task AddAsync(Proprietario proprietario)
        {
            await _context.Proprietarios.AddAsync(proprietario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Proprietario proprietario)
        {
            _context.Proprietarios.Update(proprietario);
            await _context.SaveChangesAsync();
        }
    }
}
