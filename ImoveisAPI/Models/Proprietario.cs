using System.ComponentModel.DataAnnotations;

namespace ImoveisAPI.Models
{
    public class Proprietario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nome { get; set; }

        [Required]
        [StringLength(20)]
        public string Documento { get; set; }

        public ICollection<Imovel> Imoveis { get; set; } = new List<Imovel>();
    }
}
