using DataVision.DTOs;

namespace DataVision.Services
{
    public interface IRecopeService
    {
        Task<PrecioInternacionalResponse?> GetPrecioInternacionalAsync(string? start = null, string? end = null);
        Task<List<PrecioVentaResponse>?> GetPrecioConsumidorAsync();
        Task<List<PrecioVentaResponse>?> GetPrecioPlantelAsync();
    }

    public class RecopeService : IRecopeService
    {
        private readonly HttpClient _httpClient;

        public RecopeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.recope.go.cr/");
        }

        public async Task<PrecioInternacionalResponse?> GetPrecioInternacionalAsync(string? start = null, string? end = null)
        {
            var url = "precio-internacional";
            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
                url += $"?inicio={start}&fin={end}";
            }

            return await _httpClient.GetFromJsonAsync<PrecioInternacionalResponse>(url);
        }

        public async Task<List<PrecioVentaResponse>?> GetPrecioConsumidorAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<PrecioVentaResponse>>("ventas/precio/consumidor");
        }

        public async Task<List<PrecioVentaResponse>?> GetPrecioPlantelAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<PrecioVentaResponse>>("ventas/precio/plantel");
        }
    }
}
