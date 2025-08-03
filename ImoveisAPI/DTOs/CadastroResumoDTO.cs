namespace ImoveisAPI.DTOs
{
    public class CadastroResumoDTO
    {
        public string NomeProprietario { get; set; }
        public string DocumentoProprietario { get; set; }
        public string Status { get; set; }
        public List<ImovelDTO> Imoveis { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
