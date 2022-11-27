using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NCoreUtils.Resources;

namespace NCoreUtils
{
    public static class OptionsBuilderAspNetCoreResourceExtensions
    {
        public static OptionsBuilder<CompositeResourceFactoryConfiguration> AddAspNetCoreResourceFactory(
            this OptionsBuilder<CompositeResourceFactoryConfiguration> optionsBuilder)
        {
            optionsBuilder.Configure<IHttpContextAccessor>((config, httpContextAccessor) =>
            {
                config.AddFactory(new AspNetCoreResourceFactory(httpContextAccessor));
            });
            return optionsBuilder;
        }
    }
}