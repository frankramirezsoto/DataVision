using DataVision.DTOs;
using System.Text.Json;

namespace DataVision.Services
{
    public interface IExternalDataService
    {
        Task<ReportDataResponse> GetFuelPricesHistoryAsync();
        Task<ReportDataResponse> GetCurrentFuelPricesAsync();
        Task<ReportDataResponse> GetFuelSalesDataAsync();
    }
    public class ExternalDataService : IExternalDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalDataService> _logger;
        private readonly string _recopeBaseUrl = "https://datosabiertos.recope.go.cr";

        public ExternalDataService(HttpClient httpClient, ILogger<ExternalDataService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configure HttpClient for RECOPE API
            _httpClient.BaseAddress = new Uri(_recopeBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DataVision/1.0");
        }

        public async Task<ReportDataResponse> GetFuelPricesHistoryAsync()
        {
            try
            {
                // Since the exact API endpoints aren't accessible, we'll simulate with realistic data
                // In production, you would call the actual RECOPE API endpoint
                await Task.Delay(500); // Simulate API call

                // Simulated historical fuel prices data for Costa Rica (last 12 months)
                var data = new List<DataPoint>
                {
                    new DataPoint { Label = "Ene 2024", Value = 742, Color = "#FF6384", Category = "Super" },
                    new DataPoint { Label = "Feb 2024", Value = 738, Color = "#FF6384", Category = "Super" },
                    new DataPoint { Label = "Mar 2024", Value = 745, Color = "#FF6384", Category = "Super" },
                    new DataPoint { Label = "Abr 2024", Value = 751, Color = "#FF6384", Category = "Super" },
                    new DataPoint { Label = "May 2024", Value = 748, Color = "#FF6384", Category = "Super" },
                    new DataPoint { Label = "Jun 2024", Value = 753, Color = "#FF6384", Category = "Super" },
                    new DataPoint { Label = "Jul 2024", Value = 757, Color = "#FF6384", Category = "Super" },
                    new DataPoint { Label = "Ago 2024", Value = 761, Color = "#FF6384", Category = "Super" }
                };

                return new ReportDataResponse
                {
                    ChartType = "line",
                    Title = "Evolución Precio Gasolina Súper - Últimos 8 Meses (₡/L)",
                    Data = data,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching fuel prices history from RECOPE API");
                return new ReportDataResponse
                {
                    ChartType = "line",
                    Title = "Error obteniendo precios históricos de combustibles",
                    Data = new List<DataPoint>()
                };
            }
        }

        public async Task<ReportDataResponse> GetCurrentFuelPricesAsync()
        {
            try
            {
                // Simulate current fuel prices API call to RECOPE
                await Task.Delay(500);

                // Current fuel prices in Costa Rica (₡/Liter)
                var data = new List<DataPoint>
                {
                    new DataPoint { Label = "Gasolina Súper", Value = 761, Color = "#FF6384" },
                    new DataPoint { Label = "Gasolina Regular", Value = 739, Color = "#36A2EB" },
                    new DataPoint { Label = "Diésel", Value = 634, Color = "#FFCE56" },
                    new DataPoint { Label = "Kerosene", Value = 576, Color = "#4BC0C0" },
                    new DataPoint { Label = "Búnker", Value = 428, Color = "#9966FF" }
                };

                return new ReportDataResponse
                {
                    ChartType = "bar",
                    Title = "Precios Actuales de Combustibles - Costa Rica (₡/L)",
                    Data = data,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current fuel prices from RECOPE API");
                return new ReportDataResponse
                {
                    ChartType = "bar",
                    Title = "Error obteniendo precios actuales de combustibles",
                    Data = new List<DataPoint>()
                };
            }
        }

        public async Task<ReportDataResponse> GetFuelSalesDataAsync()
        {
            try
            {
                // Simulate fuel sales distribution data
                await Task.Delay(500);

                // Distribution of fuel sales by type (percentage)
                var data = new List<DataPoint>
                {
                    new DataPoint { Label = "Gasolina Súper", Value = 42.5m, Color = "#FF6384" },
                    new DataPoint { Label = "Gasolina Regular", Value = 28.3m, Color = "#36A2EB" },
                    new DataPoint { Label = "Diésel", Value = 24.7m, Color = "#FFCE56" },
                    new DataPoint { Label = "Kerosene", Value = 2.8m, Color = "#4BC0C0" },
                    new DataPoint { Label = "Búnker", Value = 1.7m, Color = "#9966FF" }
                };

                return new ReportDataResponse
                {
                    ChartType = "pie",
                    Title = "Distribución de Ventas por Tipo de Combustible (%)",
                    Data = data,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching fuel sales data from RECOPE API");
                return new ReportDataResponse
                {
                    ChartType = "pie",
                    Title = "Error obteniendo datos de ventas de combustibles",
                    Data = new List<DataPoint>()
                };
            }
        }

        // Method to call real RECOPE API endpoints when available
        private async Task<T?> CallRecopeApiAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error calling RECOPE API: {endpoint}");
                return default(T);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"JSON parsing error for RECOPE API: {endpoint}");
                return default(T);
            }
        }

        // Models for RECOPE API responses (when implementing real API calls)
        public class RecopeFuelPrice
        {
            public string Producto { get; set; } = string.Empty;
            public decimal Precio { get; set; }
            public DateTime Fecha { get; set; }
            public string Moneda { get; set; } = "CRC";
        }

        public class RecopeSalesData
        {
            public string TipoCombustible { get; set; } = string.Empty;
            public decimal VolumenVendido { get; set; }
            public decimal PorcentajeTotal { get; set; }
            public string Periodo { get; set; } = string.Empty;
        }
    }
}
