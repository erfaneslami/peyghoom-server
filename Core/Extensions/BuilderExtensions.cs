using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Peyghoom.Core.Options;
using Peyghoom.Repositories.AuthRepository;
using Peyghoom.Repositories.UserRepository;
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
      builder.AddMongoDbConfig();
      
      return builder;
   }


   private static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
   {
      builder.Services.AddMemoryCache();
      
      builder.Services.AddScoped<IAuthService, AuthService>();
      builder.Services.AddScoped<IAuthRepository, AuthRepository>();
      builder.Services.AddScoped<IUserRepository, UserRepository>();
      builder.Services.AddSingleton<ICacheService, CacheService>();

      return builder;
   }

   private static WebApplicationBuilder AddMongoDbConfig(this WebApplicationBuilder builder)
   {
      // TODO:README
      builder.Services.AddSingleton<IMongoClient>(sp =>
      {
         var settings = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;
         return new MongoClient(settings.PeyghoomMongoDb);
      });

      builder.Services.AddSingleton<IMongoDatabase>(sp =>
      {
         var settings = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;
         var client = sp.GetRequiredService<IMongoClient>();
         Console.WriteLine(settings.PeyghoomMongoDb);
         return client.GetDatabase(settings.DatabaseName);
      });
      
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
               ClockSkew = TimeSpan.Zero,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecretKey))
            };
         });

      builder.Services.AddAuthorization(options =>
      {
         options.AddPolicy("OTPVerify", policy => policy.RequireClaim("purpose", "otp"));
         options.AddPolicy("REGISTER", policy => policy.RequireClaim("purpose", "register"));
      });
      return builder;
   }

   private static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
   {
      builder.Services.Configure<TokenOption>(builder.Configuration.GetSection(TokenOption.Token));
      builder.Services.Configure<ConnectionStringsOptions>(builder.Configuration.GetSection(ConnectionStringsOptions.ConnectionString));

      return builder;
   }
}