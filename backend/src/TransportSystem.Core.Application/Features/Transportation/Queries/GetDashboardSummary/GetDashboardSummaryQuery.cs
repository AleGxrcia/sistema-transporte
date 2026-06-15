using MediatR;
using TransportSystem.Core.Application.Dtos.Dashboard;

namespace TransportSystem.Core.Application.Features.Transportation.Queries.GetDashboardSummary
{
    public record GetDashboardSummaryQuery 
        : IRequest<DashboardSummaryDto>;
}
