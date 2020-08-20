using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Skrawl.API.Data;
using System;
using Npgsql;

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
            services.AddDbContext<SkrawlContext>(options => {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                string connectionString;

                if(env == Development) 
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
