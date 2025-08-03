namespace ImoveisAPI.Services
{
    public class ValorImovelService
    {
        public decimal CalcularValor(decimal area)
        {
            if (area <= 10) return area * 26000;
            if (area <= 50) return area * 24000;
            return area * 23000;
        }

        public decimal CalcularValorTotal(IEnumerable<decimal> areas)
        {
            return areas.Sum(area => CalcularValor(area));
        }
    }
}
