using Learn.Kafka.Taxi.Shared.Interfaces;
using Learn.Kafka.Taxi.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.Kafka.Taxi.Shared
{
    public class DistanceCalculatorService : IDistanceCalculatorService
    {
        private readonly Dictionary<Guid, Coords> _storage = new();
        
        public double CalculateDistance(Guid vehicleId, Coords currentCoords)
        {
            var previousCoords = _storage.ContainsKey(vehicleId) ? _storage[vehicleId] : new Coords(0, 0);
            _storage[vehicleId] = currentCoords;

            return Calculate(previousCoords, currentCoords);
        }

        private double Calculate(Coords previousCoords, Coords currentCoords)
        {
            double deltaX = currentCoords.X - previousCoords.X;
            double deltaY = currentCoords.Y - previousCoords.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
