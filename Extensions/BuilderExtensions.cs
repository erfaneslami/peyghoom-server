using Peyghoom.Services.AuthService;
using Peyghoom.Services.CacheService;

namespace Peyghoom.Extensions;

public static class BuilderExtensions
{
   public static WebApplicationBuilder AddWebAppConfigs(this WebApplicationBuilder builder)
   {

      builder.Services.AddScoped<IAuthService, AuthService>();
      builder.Services.AddSingleton<ICacheService, CacheService>();
      return builder;
   }
}