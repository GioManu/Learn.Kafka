using Learn.Kafka.Taxi.Shared;
using Learn.Kafka.Taxi.Shared.Interfaces;
using Learn.Kafka.Taxi.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Learn.Kafka.TaxWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class VehicleController : ControllerBase
    {
        private readonly ILogger<VehicleController> _logger;
        private readonly IVehicleProducer _producer;

        public VehicleController(ILogger<VehicleController> logger, IVehicleProducer producer)
        {
            _producer = producer;
            _logger = logger;
        }

        [HttpPost(Name = "SignalCoordinates")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> SignalCoordinates([FromBody] VehicleSignalRequest request)
        {
            await _producer.ProduceAsync(KafkaSettings.InputTopic, request.Id, JsonSerializer.Serialize(request), nameof(VehicleSignalRequest));
            return Accepted();
        }
    }
}