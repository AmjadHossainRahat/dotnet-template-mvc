using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mediator
{
    public interface IRequestHandler<TRequest, TResult>
        where TRequest : IRequest<TResult>
    {
        Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
