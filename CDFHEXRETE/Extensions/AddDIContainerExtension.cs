using CDFHEXRETE.Interfaces;
using CDFHEXRETE.Services;

namespace CDFHEXRETE.Extensions
{
    public static class AddDIContainerExtension
    {
        public static IServiceCollection AddDiContainer(this IServiceCollection services)
        {
            // 新增主頁服務
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IScheduleRateService, ScheduleRateService>();
            return services;
        }
    }
}
