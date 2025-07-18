using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Peyghoom.Core.Options;
using Peyghoom.Services.AuthService;
using Peyghoom.Services.CacheService;

namespace Peyghoom.Core.Extensions;

public static class BuilderExtensions
{
   public static WebApplicationBuilder AddWebAppConfigs(this WebApplicationBuilder builder)
   {

      builder.AddServices();
      builder.AddAuthenticationAuthorization();
      builder.AddOptions(); 
      
      return builder;
   }


   private static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
   {
      builder.Services.AddMemoryCache();
      
      builder.Services.AddScoped<IAuthService, AuthService>();
      builder.Services.AddSingleton<ICacheService, CacheService>();

      return builder;
   }
   private static WebApplicationBuilder AddAuthenticationAuthorization(this WebApplicationBuilder builder)
   {
      var tokenOption = builder.Configuration.GetSection(TokenOption.Token).Get<TokenOption>();

      if (tokenOption is null)
      {
         // TODO: log critical here
         throw new ArgumentNullException();
      }
      
      builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
         {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
               ValidateIssuer = true,
               ValidIssuer = tokenOption.Issuer,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ClockSkew = TimeSpan.Zero,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecretKey))
            };
         });

      return builder;
   }

   private static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
   {
      builder.Services.Configure<TokenOption>(builder.Configuration.GetSection(TokenOption.Token));

      return builder;
   }
}