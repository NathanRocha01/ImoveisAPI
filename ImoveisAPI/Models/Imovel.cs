using System.ComponentModel.DataAnnotations;

namespace ImoveisAPI.Models
{
    public class Imovel
    {
        public int Id { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Área deve ser maior que zero.")]
        public decimal Area { get; set; }

        [Required]
        [StringLength(500)]
        public string Endereco { get; set; }

        public int ProprietarioId { get; set; }
        public Proprietario Proprietario { get; set; }
    }
}
