using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VigilantMeerkat.Db;
using VigilantMeerkat.Gateway.Auth;
using VaultSharp.V1.AuthMethods;
using Microsoft.Extensions.Options;
using VaultSharp;
using VigilantMeerkat.Micro.Base.Vault;
using VaultSharp.V1.AuthMethods.Token;

namespace VigilantMeerkat.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigHelper.ConnectionString.Add(nameof(AppDbContext), Configuration.GetConnectionString("App"));

            services.AddControllers();

            services.AddSwaggerGen();

            services.AddCors(cors =>
            {
                cors.AddPolicy("Allow", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            services.Configure<VaultConfig>(Configuration.GetSection("Vault"));
            services.Configure<ConfigDefinition>(Configuration);
            services.Configure<RabbitConfig>(Configuration.GetSection("Rabbit"));
            services.Configure<MongoConfig>(Configuration.GetSection("Mongo"));

            services.AddSingleton<IAuthMethodInfo>((serviceProvider) =>
            {
                var cfg = serviceProvider.GetRequiredService<IOptions<VaultConfig>>();

                return new TokenAuthMethodInfo(cfg.Value.Token);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var cfg = serviceProvider.GetService<IOptions<VaultConfig>>();
                var auth = serviceProvider.GetService<IAuthMethodInfo>();

                return new VaultClientSettings(cfg.Value.Host, auth);
            });

            services.AddTransient<IVaultClient, VaultClient>();
            services.AddTransient<VaultService>();

            services.AddTransient<JwtProvider>();
            services.AddSingleton<IRabbitConnection, RabbitConnection>();
            services.AddTransient<AmqpService>();

            services.AddDbContext<UserDbContext>(opts =>
            {
                opts.UseNpgsql(Configuration.GetConnectionString("User"));
            });

            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(Configuration.GetConnectionString("App"));
            });

            services.AddAuthentication(x =>
                    {
                        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(opts =>
                    {
                        opts.IncludeErrorDetails = true;

                        opts.TokenValidationParameters = new TokenValidationParameters
                        {
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Key"])),
                            ValidAudience = "https://localhost:4200",
                            ValidIssuer = "https://localhost:11000",
                            RequireSignedTokens = true,
                            RequireExpirationTime = true,
                            ValidateLifetime = false,
                            ValidateAudience = false,
                            ValidateIssuer = false
                        };
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

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseCors("Allow");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}