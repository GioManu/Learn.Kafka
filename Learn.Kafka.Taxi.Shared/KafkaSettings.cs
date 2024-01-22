using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.Kafka.Taxi.Shared
{
    public static class KafkaSettings
    {
        public const string BootstrapServers = "localhost:8097,localhost:8098,localhost:8099";
        public const string CalcConsumerGroupId = "calc-consumer-group";
        public const string InputTopic = "input_vehicle_coords_topic";
        public const string OutputTopic = "output_vehicle_coords_topic";
    }
}
