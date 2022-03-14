using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Common
{
    /// <summary>
    /// Services related extensions for <see cref="ServiceCollection"/>.
    /// </summary>
    public static class ServicesExtensions
    {
        /// <summary>
        /// Configures cross-origin resource sharing services to the specified <see cref="ServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="ServiceCollection"/> instance this method extends.</param>
        /// <param name="configuration">Set of key/value configuration values.</param>
        /// <param name="policy">Cross-origin policy to be used.</param>
        public static void ConfigureCors(this IServiceCollection services, 
            IConfiguration configuration, string policy)
        {
            var hosts = configuration.GetSection("AllowedOrigin").Get<List<string>>();
            services.AddCors(options =>
            {
                options.AddPolicy(policy,
                    builder => builder.WithOrigins(hosts.ToArray())
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }
        
        /// <summary>
        /// Configures utility classes to the specified <see cref="ServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="ServiceCollection"/> instance this method extends.</param>
        /// <param name="config">Set of key/value configuration values.</param>
        public static void ConfigureUtilities(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSettings>(config.GetSection(nameof(JwtSettings)));
        }
        
        /// <summary>
        /// Configures authorization policies to the specified <see cref="ServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="ServiceCollection"/> instance this method extends.</param>
        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policy.Admin, policy =>
                    policy
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .RequireClaim(AuthClaimNames.IsAdmin, "True", "true"));
                
                config.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });
        }
        
        /// <summary>
        /// Configures Jwt Token authentication to the specified <see cref="ServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="ServiceCollection"/> instance this method extends.</param>
        /// <param name="config">Set of key/value configuration values.</param>
        public static void ConfigureAuthentication(this IServiceCollection services,
            IConfiguration config)
        {
            var jwtSettings = new JwtSettings();
            config.Bind(nameof(JwtSettings), jwtSettings);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
    }
}