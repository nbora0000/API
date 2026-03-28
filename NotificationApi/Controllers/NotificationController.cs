using Microsoft.AspNetCore.Mvc;

namespace NotificationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(ILogger<NotificationController> logger)
        {
            _logger = logger;
        }

        [HttpPost("send")]
        public IActionResult Send([FromBody] NotificationRequest request)
        {
            // placeholder: in real app use template engine and provider integrations
            _logger.LogInformation("Sending notification to {Recipient}: {Message}", request.Recipient, request.Message);
            return Ok();
        }
    }

    public class NotificationRequest
    {
        public string Recipient { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Channel { get; set; } = "email";
    }
}