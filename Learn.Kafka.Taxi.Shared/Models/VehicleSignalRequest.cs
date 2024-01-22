using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.Kafka.Taxi.Shared.Models
{
    public class VehicleSignalRequest(Guid id, Coords coords)
    {
        public Guid Id { get; set; } = id;
        public Coords Coords { get; set; } = coords;
    }

    public class VehicleSignalRequestValidator : AbstractValidator<VehicleSignalRequest>
    {
        public VehicleSignalRequestValidator()
        {
            RuleFor(x => x.Coords)
                .Must(ValidateCoords)
                .WithMessage("Incorrect Vehicle Coordinates.");
        }

        private bool ValidateCoords(Coords coords)
        {
            // skipping validation part for now.
            return true;
        }
    }

}
