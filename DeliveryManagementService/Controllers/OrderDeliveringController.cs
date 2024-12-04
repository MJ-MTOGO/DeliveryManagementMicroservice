using DeliveryManagementService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDeliveringController : ControllerBase
    {
        private readonly OrderDeliveringService _service;

        public OrderDeliveringController(OrderDeliveringService service)
        {
            _service = service;
        }

        [HttpPut("{id}/update-delivering-time")]
        public async Task<IActionResult> UpdateDeliveringTime(Guid id)
        {
            try
            {
                await _service.UpdateDeliveringTimeAsync(id, DateTime.UtcNow);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
