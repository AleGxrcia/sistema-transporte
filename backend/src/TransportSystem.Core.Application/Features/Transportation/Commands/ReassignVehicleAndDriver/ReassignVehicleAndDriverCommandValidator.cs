using FluentValidation;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.ReassignVehicleAndDriver
{
    public class ReassignVehicleAndDriverCommandValidator : AbstractValidator<ReassignVehicleAndDriverCommand>
    {
        public ReassignVehicleAndDriverCommandValidator()
        {
            RuleFor(x => x.RequestId).NotEmpty().WithMessage("La solicitud es requerida.");
            RuleFor(x => x.VehicleId).NotEmpty().WithMessage("El vehículo es requerido.");
            RuleFor(x => x.DriverId).NotEmpty().WithMessage("El conductor es requerido.");
        }
    }
}
