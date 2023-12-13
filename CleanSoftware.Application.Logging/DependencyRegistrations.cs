using CleanSoftware.Application.Logging.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CleanSoftware.Application.Logging
{
    public static class DependencyRegistrations
    {
        public static MediatRServiceConfiguration AddCommandLoggingBehavior(
            this MediatRServiceConfiguration configuration)
        {
            configuration.AddBehavior(
                typeof(IPipelineBehavior<,>), 
                typeof(CommandLoggingPipelineService<,>));

            return configuration;
        }

        public static MediatRServiceConfiguration AddQueryLoggingBehavior(
            this MediatRServiceConfiguration configuration)
        {
            configuration.AddBehavior(
                typeof(IPipelineBehavior<,>), 
                typeof(QueryLoggingPipelineService<,>));

            return configuration;
        }
    }
}
