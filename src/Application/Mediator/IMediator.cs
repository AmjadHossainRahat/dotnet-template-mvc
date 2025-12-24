using System.Threading.Tasks;

namespace Application.Mediator
{
    /// <summary>
    /// Minimal mediator contract used by application handlers and controllers.
    /// A concrete mediator will be implemented in the Application layer (custom implementation, not MediatR).
    /// </summary>
    public interface IMediator
    {
        Task<TResponse> SubmitAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task<TResponse> SendAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>;
    }
}
