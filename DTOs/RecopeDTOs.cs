namespace DataVision.DTOs
{
    public class PeriodoDto
    {
        public string Desde { get; set; } = string.Empty;
        public string Hasta { get; set; } = string.Empty;
    }

    public class MaterialDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nomprod { get; set; } = string.Empty;
        public List<decimal> Precios { get; set; } = new();
    }

    public class PrecioInternacionalResponse
    {
        public List<PeriodoDto> Periodos { get; set; } = new();
        public List<MaterialDto> Materiales { get; set; } = new();
    }

    public class PrecioVentaResponse
    {
        public string Fecha { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Impuesto { get; set; } = string.Empty;
        public string Precsinimp { get; set; } = string.Empty;
        public string Fechaupd { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Preciototal { get; set; } = string.Empty;
        public string Nomprod { get; set; } = string.Empty;
        public string Margenpromedio { get; set; } = string.Empty;
    }
}
