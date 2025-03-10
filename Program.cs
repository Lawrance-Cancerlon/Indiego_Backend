using AutoMapper;
using FluentValidation;
using Indiego_Backend.Contracts.Users;
using Indiego_Backend.Mappings;
using Indiego_Backend.Models.Users;
using Indiego_Backend.Repositories;
using Indiego_Backend.Services;
using Indiego_Backend.Settings;
using Indiego_Backend.Validations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//Configurations
builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection(nameof(DatabaseSetting)));
builder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<DatabaseSetting>>().Value);

builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection(nameof(JwtSetting)));
builder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<JwtSetting>>().Value);

//AutoMapper
builder.Services.AddAutoMapper(typeof(UserMapping));

//Repositories
builder.Services.AddScoped<IUserRepository<User>, UserRepository<User>>();
builder.Services.AddScoped<IUserRepository<Admin>, UserRepository<Admin>>();
builder.Services.AddScoped<IUserRepository<Customer>, UserRepository<Customer>>();
builder.Services.AddScoped<IUserRepository<Developer>, UserRepository<Developer>>();

//Validations
builder.Services.AddSingleton<IValidator<CreateUserContract>, CreateUserValidation>();
builder.Services.AddSingleton<IValidator<CreateAdminContract>, CreateAdminValidation>();
builder.Services.AddSingleton<IValidator<CreateCustomerContract>, CreateCustomerValidation>();
builder.Services.AddSingleton<IValidator<CreateDeveloperContract>, CreateDeveloperValidation>();

//Services
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton(serviceProvider => 
    new AuthService(
        serviceProvider.GetRequiredService<IDistributedCache>(),
        serviceProvider.GetRequiredService<JwtSetting>()
    )
);
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context => {
            var service = context.HttpContext.RequestServices.GetRequiredService<AuthService>();
            service.ConfigureJwtOptions(options);
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"))
    .AddPolicy("Customer", policy => policy.RequireClaim("Role", "Customer"))
    .AddPolicy("Developer", policy => policy.RequireClaim("Role", "Developer"))
    .AddPolicy("Subscribed", policy => policy.RequireClaim("Subscription", "True"))
    .AddPolicy("AdminWithManageAdmins", policy => policy.RequireClaim("Role", "Admin").RequireClaim("CanManageAdmins", "True"))
    .AddPolicy("AdminWithManageUsers", policy => policy.RequireClaim("Role", "Admin").RequireClaim("CanManageUsers", "True"))
    .AddPolicy("AdminWithManageGames", policy => policy.RequireClaim("Role", "Admin").RequireClaim("CanManageGames", "True"))
    .AddPolicy("AdminWithManagePosts", policy => policy.RequireClaim("Role", "Admin").RequireClaim("CanManagePosts", "True"))
    .AddPolicy("AdminWithManageReviews", policy => policy.RequireClaim("Role", "Admin").RequireClaim("CanManageReviews", "True"))
    .AddPolicy("AdminWithManageSubscriptions", policy => policy.RequireClaim("Role", "Admin").RequireClaim("CanManageSubscriptions", "True"))
    .AddPolicy("AdminWithManageTransactions", policy => policy.RequireClaim("Role", "Admin").RequireClaim("CanManageTransactions", "True"));

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
