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

        [HttpGet("{id}")]
        public async Task<IActionResult> UpdateDeliveringTime(Guid id)
        {
            try
            {
                await _service.UpdateDeliveringTimeAsync(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the delivering time: {ex.Message}");
            }
        }

    }
}
