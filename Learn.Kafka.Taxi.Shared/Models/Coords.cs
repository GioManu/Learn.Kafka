using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.Kafka.Taxi.Shared.Models
{
    public struct Coords(double x, double y)
    {
        public double X { get; set; } = x;
        public double Y { get; set; } = y;
    }
}
