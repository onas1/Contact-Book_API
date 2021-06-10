 using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskContactBook.Data;

namespace TaskContactBook
{
    public class Startup
    {
        public Startup(IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContextPool<TaskContactBookDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("sqlConnection"))
                );
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<TaskContactBookDbContext>();
            
            //To Let the service be aware of the authentication scheme.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration.GetSection("JWT:JWTSigninKey").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                     
                });
            services.AddSwaggerGen(
                option =>
                {
                    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        BearerFormat = "JWT",
                        Description = "Brear authentication using JWT",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        Name = "Authorization"
                    });
                    option.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                new string[] { }
                            }
                        });

                    option.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "TaskContactBook",
                        Version = "v1"
                    });
                 });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, TaskContactBookDbContext context)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(option => option.SwaggerEndpoint("/swagger/v1/swagger.json","TaskContactBook"));


            app.UseAuthentication();
            app.UseAuthorization();
            DataSeeder.SeedData(userManager, roleManager, context).Wait();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
