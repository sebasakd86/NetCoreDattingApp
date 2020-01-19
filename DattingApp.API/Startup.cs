using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DattingApp.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using DattingApp.API.Helpers;
using AutoMapper;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.AspNetCore.Identity;
using DattingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace DattingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //This Run automatically since it's conventioned base (just like controllers)
        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(
                x => 
                {
                    x.UseLazyLoadingProxies(); //To enable LazyLoading avoiding includes.
                    x.UseMySql(Configuration.GetConnectionString("MySQLConnetionString"));
                } 
            );

            ConfigureServices(services);
        }
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(
                x => 
                {
                    x.UseLazyLoadingProxies();
                    x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
                }
            );

            ConfigureServices(services);
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Identity CFG
            IdentityBuilder builder = services.AddIdentityCore<User>(opt => 
            {
                //To use "password" as password, non production cfg.
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer
            (options => {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddControllers(opt => 
            {
                //So that every user has to authenticate in each method in our api
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                opt.Filters.Add(new AuthorizeFilter(policy));services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer
            (options => {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            });
            // To add json functionality to netCore 3.0 without System.Text.Json // To avoid the circular reference issue.
            services.AddControllers().AddNewtonsoftJson(
                x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            //So automapper can look for the profile in our assembly
            services.AddAutoMapper(typeof(DatingRepository).Assembly);
            //The service is added one per request within the scope.
            //services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();            
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //To catch global errors in production environment
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());            

            app.UseAuthentication();

            app.UseAuthorization();

            // To serve static files
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                //To route the angular app
                endpoints.MapFallbackToController("Index","Fallback");
            });
        }
    }
}
