using System.Security.Claims;
using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Mappers;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;
using Indiego_Backend.Services;
using Indiego_Backend.Settings;
using Indiego_Backend.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Configurations
builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection(nameof(DatabaseSetting)));
builder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<DatabaseSetting>>().Value);

builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection(nameof(JwtSetting)));
builder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<JwtSetting>>().Value);

//Database
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

//Authentication
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context => {
            var service = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
            service.ConfigureJwtOptions(options);
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"))
    .AddPolicy("CustomerOrDeveloper", policy => policy.RequireClaim(ClaimTypes.Role, "Customer", "Developer"))
    .AddPolicy("Customer", policy => policy.RequireClaim(ClaimTypes.Role, "Customer"))
    .AddPolicy("Developer", policy => policy.RequireClaim(ClaimTypes.Role, "Developer"))
    .AddPolicy("Subscribed", policy => policy.RequireClaim("subscription", "true"))
    .AddPolicy("NotSubscribed", policy => policy.RequireClaim("subscription", "false"))
    .AddPolicy("AdminWithManageAdmins", policy => policy.RequireClaim(ClaimTypes.Role, "Admin").RequireClaim("CanManageAdmins", "true"))
    .AddPolicy("AdminWithManageGames", policy => policy.RequireClaim(ClaimTypes.Role, "Admin").RequireClaim("CanManageGames", "true"))
    .AddPolicy("AdminWithManageSubscriptions", policy => policy.RequireClaim(ClaimTypes.Role, "Admin").RequireClaim("CanManageSubscriptions", "true"))
    .AddPolicy("DeveloperOrAdminWithManageGames", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Developer") ||
            (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin") &&
             context.User.HasClaim(c => c.Type == "CanManageGames" && c.Value == "true"))
        ))
    .AddPolicy("DeveloperOrAdminWithManagePosts", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Developer") ||
            (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin") &&
             context.User.HasClaim(c => c.Type == "CanManagePosts" && c.Value == "true"))
        ))
    .AddPolicy("CustomerOrDeveloperOrAdminWithManageReviews", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Customer") ||
            context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Developer") ||
            (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin") &&
             context.User.HasClaim(c => c.Type == "CanManageReviews" && c.Value == "true"))
        ));

//Repositories
builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddSingleton<IGenreRepository, GenreRepository>();
builder.Services.AddSingleton<IPostRepository, PostRepository>();
builder.Services.AddSingleton<IReviewRepository, ReviewRepository>();
builder.Services.AddSingleton<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddSingleton<ISubscriptionTypeRepository, SubscriptionTypeRepository>();
builder.Services.AddSingleton<IUserRepository<User>, UserRepository<User>>();
builder.Services.AddSingleton<IUserRepository<Admin>, UserRepository<Admin>>();
builder.Services.AddSingleton<IUserRepository<Customer>, UserRepository<Customer>>();
builder.Services.AddSingleton<IUserRepository<Developer>, UserRepository<Developer>>();

builder.Services.AddHostedService<SubscriptionExpiryService>();

//AutoMappers
builder.Services.AddAutoMapper(typeof(GameMapper));
builder.Services.AddAutoMapper(typeof(GenreMapper));
builder.Services.AddAutoMapper(typeof(PostMapper));
builder.Services.AddAutoMapper(typeof(ReviewMapper));
builder.Services.AddAutoMapper(typeof(SubscriptionMapper));
builder.Services.AddAutoMapper(typeof(SubscriptionTypeMapper));
builder.Services.AddAutoMapper(typeof(UserMapper));

//Validators
builder.Services.AddSingleton<IValidator<CreateGameContract>, CreateGameValidator>();
builder.Services.AddSingleton<IValidator<UpdateGameContract>, UpdateGameValidator>();
builder.Services.AddSingleton<IValidator<CreateGenreContract>, CreateGenreValidator>();
builder.Services.AddSingleton<IValidator<CreatePostContract>, CreatePostValidator>();
builder.Services.AddSingleton<IValidator<UpdatePostContract>, UpdatePostValidator>();
builder.Services.AddSingleton<IValidator<CreateReviewContract>, CreateReviewValidator>();
builder.Services.AddSingleton<IValidator<UpdateReviewContract>, UpdateReviewValidator>();
builder.Services.AddSingleton<IValidator<CreateSubscriptionContract>, CreateSubscriptionValidator>();
builder.Services.AddSingleton<IValidator<CreateSubscriptionTypeContract>, CreateSubscriptionTypeValidator>();
builder.Services.AddSingleton<IValidator<UpdateSubscriptionTypeContract>, UpdateSubscriptionTypeValidator>();
builder.Services.AddSingleton<IValidator<CreateUserContract>, CreateUserValidator>();
builder.Services.AddSingleton<IValidator<UpdateUserContract>, UpdateUserValidator>();
builder.Services.AddSingleton<IValidator<CreateAdminContract>, CreateAdminValidator>();
builder.Services.AddSingleton<IValidator<UpdateAdminContract>, UpdateAdminValidator>();
builder.Services.AddSingleton<IValidator<CreateCustomerContract>, CreateCustomerValidator>();
builder.Services.AddSingleton<IValidator<UpdateCustomerContract>, UpdateCustomerValidator>();
builder.Services.AddSingleton<IValidator<CreateDeveloperContract>, CreateDeveloperValidator>();
builder.Services.AddSingleton<IValidator<UpdateDeveloperContract>, UpdateDeveloperValidator>();

//Services
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IGenreService, GenreService>();
builder.Services.AddSingleton<IPostService, PostService>();
builder.Services.AddSingleton<IReviewService, ReviewService>();
builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();
builder.Services.AddSingleton<ISubscriptionTypeService, SubscriptionTypeService>();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddControllers();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Indiego API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                Array.Empty<string>()
            }
        });
    }
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
