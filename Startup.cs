using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Skrawl.API.Data;
using System;
using Npgsql;
using System.Text;
using Skrawl.API.Infrastructure;
using Skrawl.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Skrawl.API.Data.Models;

namespace Skrawl.API
{
    public class Startup
    {
        private const string Production = "Production";
        private const string Development = "Development";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SkrawlContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Development;
                string connectionString;

                if (env == Development)
                {
                    connectionString = this.Configuration.GetConnectionString("SkrawlContext");
                }
                else
                {
                    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                    Uri databaseUri = new Uri(databaseUrl);

                    var userInfo = databaseUri.UserInfo.Split(':');

                    var builder = new NpgsqlConnectionStringBuilder
                    {
                        Host = databaseUri.Host,
                        Port = databaseUri.Port,
                        Username = userInfo[0],
                        Password = userInfo[1],
                        Database = databaseUri.LocalPath.TrimStart('/'),
                        SslMode = SslMode.Prefer,
                        TrustServerCertificate = true
                    };

                    connectionString = builder.ToString();
                }

                options.UseNpgsql(connectionString);
            });

            services.AddControllers();

            var jwtTokenConfig = Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
            });

            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INoteService, NoteService>();
            
            /** 
             * TODO 
             * Move refresh tokens to database, hitting Heroku's RAM limits
             * Set more reasonable timers for the cleanup process
             */
            // services.AddHostedService<JwtRefreshTokenCache>();
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
