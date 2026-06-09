using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.UpdateVehicle
{
    public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
    {
        public UpdateVehicleCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Type).IsInEnum().WithMessage("Tipo de vehículo inválido.");
        }
    }
}
