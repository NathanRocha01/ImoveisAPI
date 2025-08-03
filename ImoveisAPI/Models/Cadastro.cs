using System.ComponentModel.DataAnnotations;

namespace ImoveisAPI.Models
{
    public class Cadastro
    {
        public int Id { get; set; }

        // FK para Proprietario
        public int ProprietarioId { get; set; }
        public Proprietario Proprietario { get; set; }

        [Required]
        public StatusCadastro Status { get; set; } = StatusCadastro.Proprietario;

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
    }
}
