using ImoveisAPI.Data;
using ImoveisAPI.Models;
using ImoveisAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImoveisAPI.Repositories
{
    public class ProprietarioRepository : IProprietarioRepository
    {
        private readonly AppDbContext _context;

        public ProprietarioRepository(AppDbContext context)
        {
            _context = context;
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
    }
}
