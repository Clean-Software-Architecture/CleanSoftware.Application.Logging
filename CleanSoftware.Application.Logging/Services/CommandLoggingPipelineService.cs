using CleanSoftware.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanSoftware.Application.Logging.Services
{
    internal class CommandLoggingPipelineService<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<CommandLoggingPipelineService<TRequest, TResponse>> _logger;

        public CommandLoggingPipelineService(
            ICurrentUser currentUser,
            ILogger<CommandLoggingPipelineService<TRequest, TResponse>> logger)
        {
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is ICommand<TResponse> == false)
            {
                return await next();
            }

            var commandName = request.GetType().Name;

            try
            {
                _logger.LogInformation(
                    "[CurrentUser: {@CurrentUser}] Handling command {@RequestName} {@Request}",
                    _currentUser.Identifier,
                    commandName,
                    request);

                var result = await next();

                _logger.LogInformation(
                    "[CurrentUser: {@CurrentUser}] Command {@RequestName} handled - result: {@Result}",
                    _currentUser.Identifier,
                    commandName,
                    result);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "[CurrentUser: {@CurrentUser}] Failed handling command {RequestName}. Exception: {@Exception}",
                    _currentUser.Identifier,
                    commandName,
                    e);

                throw;
            }
        }
    }
}