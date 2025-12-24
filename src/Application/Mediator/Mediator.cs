using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Mediator
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            using var scope = _serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            // 1. VALIDATION (Scoped)
            var validators = scopedProvider.GetServices<IValidator<TRequest>>();
            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(request, cancellationToken);
                if (!result.IsValid)
                    throw new ValidationException(result.Errors);
            }
            //await ValidateAsync(request, scopedProvider, cancellationToken);

            // 2. HANDLER RESOLUTION (Scoped)
            var handler = scopedProvider.GetService<IRequestHandler<TRequest, TResponse>>();
            if (handler == null)
                throw new InvalidOperationException($"Handler for {typeof(TRequest).Name} not registered.");

            return await handler.HandleAsync(request, cancellationToken);
        }

        public Task<TResponse> SubmitAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
