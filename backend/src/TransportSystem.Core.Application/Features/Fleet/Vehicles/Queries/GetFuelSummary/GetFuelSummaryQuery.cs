using MediatR;
using TransportSystem.Core.Application.Dtos.Fuel;

namespace TransportSystem.Core.Application.Features.Fleet.Vehicles.Queries.GetFuelSummary
{
    public record GetFuelSummaryQuery(int Year, int Month) : IRequest<FuelSummaryDto>;
}
