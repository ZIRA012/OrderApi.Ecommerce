using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ECommmerce.SharedLibrary.DependencyInjection
{
    public static class JWTAuthenticationscheme
    {

        public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration config)
        {
            //add JWT services
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", options =>
             {
                 var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
                 string issuer = config.GetSection("Authentication:Issuer").Value!;
                 string audience = config.GetSection("Authentication:Audience").Value!;

                 options.RequireHttpsMetadata = false;
                 options.SaveToken = true;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = false,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = issuer,
                     ValidAudience = audience,
                     IssuerSigningKey = new SymmetricSecurityKey(key)
                 };
             });
            return services;
        }
    }
}
