using Microsoft.AspNetCore.HttpLogging;

namespace ship_convenient.Config
{
    public static class HTTPLogingExtension
    {
        public static void AddHTTPLogingExtension(this IServiceCollection services)
        {
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestBody | HttpLoggingFields.RequestQuery;
                
            });
        }
    }
}
