using MediatR;
using TransportSystem.Core.Application.Common.Enums;
using TransportSystem.Core.Application.Common.Exceptions;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Domain.Transportation.Aggregates;
using TransportSystem.Core.Domain.Transportation.Repositories;
using TransportSystem.Core.Domain.Transportation.ValueObjects;

namespace TransportSystem.Core.Application.Features.Transportation.Commands.CreateTravelRequest
{
    public class CreateTravelRequestCommandHandler : IRequestHandler<CreateTravelRequestCommand, Guid>
    {
        private readonly ITravelRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public CreateTravelRequestCommandHandler(ITravelRequestRepository requestRepository,
            IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreateTravelRequestCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsInAnyRole(UserRole.Admin, UserRole.Operator))
                throw new ForbiddenException("crear solicitudes de transporte", "Administrador u Operador");

            var year = DateTime.UtcNow.Year;
            var sequence = await _requestRepository.GetNextSequenceAsync(year, cancellationToken);

            var destination = Destination.Create(command.Destination);
            var timeSlot = TimeSlot.Create(command.DepartureDateTime, command.ReturnDateTime);

            var request = TravelRequest.Create(
                sequence,
                command.RequestingArea,
                _currentUser.Id,
                destination,
                command.PassengerCount,
                timeSlot,
                command.TripPurpose);

            await _requestRepository.AddAsync(request, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}
