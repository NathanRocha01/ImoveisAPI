namespace ImoveisAPI.DTOs
{
    public class ProprietarioDetalhadoDTO
    {
        public string Nome { get; set; }
        public string Documento { get; set; }
        public List<ImovelDTO> Imoveis { get; set; }
    }
}
