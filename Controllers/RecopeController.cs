using DataVision.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataVision.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints in this controller
    public class RecopeController : ControllerBase
    {
        private readonly IRecopeService _recopeService;

        public RecopeController(IRecopeService recopeService)
        {
            _recopeService = recopeService;
        }

        [HttpGet("precio-internacional")]
        public async Task<IActionResult> GetPrecioInternacional([FromQuery] string? inicio, [FromQuery] string? fin)
        {
            var result = await _recopeService.GetPrecioInternacionalAsync(inicio, fin);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("precio-consumidor")]
        public async Task<IActionResult> GetPrecioConsumidor()
        {
            var result = await _recopeService.GetPrecioConsumidorAsync();
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("precio-plantel")]
        public async Task<IActionResult> GetPrecioPlantel()
        {
            var result = await _recopeService.GetPrecioPlantelAsync();
            return result != null ? Ok(result) : NotFound();
        }
    }
}
