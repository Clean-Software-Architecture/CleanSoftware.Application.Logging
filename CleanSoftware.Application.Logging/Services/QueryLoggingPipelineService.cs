using CleanSoftware.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanSoftware.Application.Logging.Services
{
    internal class QueryLoggingPipelineService<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<QueryLoggingPipelineService<TRequest, TResponse>> _logger;

        public QueryLoggingPipelineService(
            ICurrentUser currentUser,
            ILogger<QueryLoggingPipelineService<TRequest, TResponse>> logger)
        {
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is IQuery<TResponse> == false)
            {
                return await next();
            }

            var queryName = request.GetType().Name;

            try
            {
                _logger.LogInformation(
                    "[CurrentUser: {@CurrentUser}] Handling query {@RequestName} {@Request}", 
                    _currentUser.Identifier,
                    queryName,
                    request);

                var response = await next();

                _logger.LogInformation(
                    "[CurrentUser: {@CurrentUser}] Query {@RequestName} handled - response: {@Response}",
                    _currentUser.Identifier,
                    queryName,
                    response);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "[CurrentUser: {@CurrentUser}] Failed handling query {@RequestName}. Exception: {@Exception}",
                    _currentUser.Identifier,
                    queryName, 
                    e);
                throw;
            }
        }
    }
}