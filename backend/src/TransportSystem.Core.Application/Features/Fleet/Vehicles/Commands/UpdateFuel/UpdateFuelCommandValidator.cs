using FluentValidation;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Commands.UpdateFuel
{
    public class UpdateFuelCommandValidator : AbstractValidator<UpdateFuelCommand>
    {
        public UpdateFuelCommandValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.FuelRecordId).NotEmpty();
            RuleFor(x => x.RecordDate).NotEmpty();
            RuleFor(x => x.Gallons).GreaterThan(0).WithMessage("Los galones deben ser mayor a 0.");
            RuleFor(x => x.PricePerGallon).GreaterThan(0).WithMessage("El precio por galón debe ser mayor a 0.");
            RuleFor(x => x.MileageAtRefuel).GreaterThanOrEqualTo(0).WithMessage("El kilometraje no puede ser negativo.");
            RuleFor(x => x.Notes).MaximumLength(300).When(x => x.Notes is not null);
        }
    }
}
