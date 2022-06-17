using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.DataContext;
using System.IO;
using Microsoft.AspNetCore.Http;
using TextAppApi.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TextAppApi.Queries;
using HotChocolate;
using HotChocolate.AspNetCore;
using TextAppApi.QueryTypes;
using MongoDB.Bson;
using HotChocolate.Types;
using TextAppApi.Mutations;
using HotChocolate.Execution.Configuration;
using TextAppApi.ErrorFilter;
using TextAppApi.Subscriptions;
using TextAppApi.Middlewares;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace TextAppApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = TokenService.IssuerAddr,
                    ValidAudience = TokenService.AudienceAddr,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenService.TokenKey))
                };
            });
            services.AddTransient<ITokenService, TokenService>();
            services.AddSingleton<IDbContext, DbContext>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TextAppApi", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.<br>
                                    Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddHttpContextAccessor();
            services.AddGraphQLServer()
                .AddSocketSessionInterceptor<SubscriptionAuthMiddleware>()
                .AddInMemorySubscriptions()
                .AddErrorFilter<GraphQLErrorFilter>()
                .AddQueryType<DbQueries>()
                .AddMutationType<DbMutations>()
                .AddSubscriptionType<DbSubscriptions>()
                .AddTypes(typeof(ChatType), typeof(UserType), typeof(MessageType), typeof(DBRefType))
                .AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://localhost:3000")
                    .AllowCredentials());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TextAppApi v1"));
            }
            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseMiddleware<RedirectMiddleware>();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.UseWebSockets();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
        }
    }
}
