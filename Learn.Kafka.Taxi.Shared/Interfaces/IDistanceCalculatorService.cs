using Learn.Kafka.Taxi.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.Kafka.Taxi.Shared.Interfaces
{
    public interface IDistanceCalculatorService
    {
        double CalculateDistance(Guid vehicleId, Coords currentCoords);
    }
}
