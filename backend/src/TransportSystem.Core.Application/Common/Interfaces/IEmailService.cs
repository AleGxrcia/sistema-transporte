using TransportSystem.Core.Application.Common.Dtos;

namespace TransportSystem.Core.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request, CancellationToken cancellationToken = default);
    }
}
