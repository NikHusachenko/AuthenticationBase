using AuthenticationBase.EntityFramework;
using AuthenticationBase.EntityFramework.Repository;
using AuthenticationBase.Infrastructure.Events;
using AuthenticationBase.Infrastructure.Middleware;
using AuthenticationBase.Services.AuthenticationServices;
using AuthenticationBase.Services.JwtServices;
using AuthenticationBase.Services.JwtServices.Options;
using AuthenticationBase.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    string securityKey = builder.Configuration["JwtOptions:Key"] ?? throw new InvalidOperationException("Security key not available.");
    TokenValidator tokenValidator = new TokenValidator(securityKey);
    options.TokenValidationParameters = tokenValidator.GetValidationParameters();
    options.Events = new AuthenticationEvent();
});

services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AuthenticationDb"));
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

services.AddHttpContextAccessor();
services.AddScoped<AuthenticationEvent>();
services.AddTransient<IAuthenticationService, AuthenticationService>();
services.AddTransient<IJwtService, JwtService>();
services.AddTransient<IUserService, UserService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseMiddleware<AuthenticationProtectionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();