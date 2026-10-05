using TravelBay.Common.Services.CryptoService;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services;
using TravelBay.Services.Database;
using TravelBay.Services.Hubs;
using TravelBay.Services.Validators;
using TravelBay.WebAPI.Filters;
using TravelBay.WebAPI.Services;
using TravelBay.WebAPI.Services.AccessManager;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text;

// In Docker the env vars come from docker-compose's env_file (no .env file inside the image),
// so a missing .env file here (e.g. when running from the published container) is expected.
try
{
    DotNetEnv.Env.TraversePath().Load();
}
catch (FileNotFoundException)
{
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticatedUserAccessor, HttpAuthenticatedUserAccessor>();

builder.Services.AddControllers(
   options => options.Filters.Add<ExceptionFilter>()
);

builder.Services.AddSignalR();

// Add Entity Framework Core DbContext
// Connection string is composed from .env, never hardcoded/committed (DB_HOST differs between host and docker network).
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id=sa;Password={dbPassword};TrustServerCertificate=True;";
builder.Services.AddDbContext<TravelBayDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Explicit allow-list, never a wildcard - override via CORS_ALLOWED_ORIGINS (.env) for other client origins.
var corsOrigins = (Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS") ?? "http://localhost:3000,http://localhost:5173")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("TravelBayClients", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// register Mapster for object mapping
builder.Services.AddMapster();

// configure a few mappings explicitly if needed (optional)
// Mapster will automatically map same-named properties, but configuration
// ensures any custom rules or future needs can be added here.
TypeAdapterConfig<Destination, DestinationResponse>.NewConfig().IgnoreNullValues(true);
TypeAdapterConfig<Category, CategoryResponse>.NewConfig().IgnoreNullValues(true);
TypeAdapterConfig<User, UserResponse>.NewConfig().IgnoreNullValues(true);
TypeAdapterConfig<UserUpdateRequest, User>.NewConfig().IgnoreNullValues(true);
TypeAdapterConfig<Asset, AssetResponse>.NewConfig().IgnoreNullValues(true);
TypeAdapterConfig<Review, ReviewResponse>.NewConfig()
    .Map(dest => dest.ReviewerDisplayName, src => $"{src.User.FirstName} {src.User.LastName}".Trim());
TypeAdapterConfig<City, CityResponse>.NewConfig()
    .Map(dest => dest.CountryName, src => src.Country.Name);


// register application services
builder.Services.AddScoped<IDestinationService, DestinationService>();

// category / reference data services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<INewsService, NewsService>();

// user service
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAssetService, AssetService>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddScoped<IAccessManager, AccessManager>();

builder.Services.AddScoped<ICryptoService, CryptoService>();

builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewStateMachine, ReviewStateMachine>();

builder.Services.AddScoped<ITripPlanService, TripPlanService>();
builder.Services.AddScoped<ITripPlanStateMachine, TripPlanStateMachine>();

builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<ISavedDestinationService, SavedDestinationService>();
builder.Services.AddScoped<IUserPreferenceService, UserPreferenceService>();
builder.Services.AddScoped<IViewHistoryService, ViewHistoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();

builder.Services.AddScoped<IValidator<DestinationInsertRequest>, DestinationInsertValidator>();
builder.Services.AddScoped<IValidator<DestinationUpdateRequest>, DestinationUpdateValidator>();
builder.Services.AddScoped<IValidator<CategoriesInsertRequest>, CategoryInsertValidator>();
builder.Services.AddScoped<IValidator<CategoriesUpdateRequest>, CategoryUpdateValidator>();
builder.Services.AddScoped<IValidator<CountryInsertRequest>, CountryInsertValidator>();
builder.Services.AddScoped<IValidator<CountryUpdateRequest>, CountryUpdateValidator>();
builder.Services.AddScoped<IValidator<CityInsertRequest>, CityInsertValidator>();
builder.Services.AddScoped<IValidator<CityUpdateRequest>, CityUpdateValidator>();
builder.Services.AddScoped<IValidator<NewsInsertRequest>, NewsInsertValidator>();
builder.Services.AddScoped<IValidator<NewsUpdateRequest>, NewsUpdateValidator>();
builder.Services.AddScoped<IValidator<UserInsertRequest>, UserInsertValidator>();
builder.Services.AddScoped<IValidator<UserUpdateRequest>, UserUpdateValidator>();
builder.Services.AddScoped<IValidator<AssetInsertRequest>, AssetInsertValidator>();
builder.Services.AddScoped<IValidator<AssetUpdateRequest>, AssetUpdateValidator>();
builder.Services.AddScoped<IValidator<ReviewInsertRequest>, ReviewInsertValidator>();
builder.Services.AddScoped<IValidator<ReviewUpdateRequest>, ReviewUpdateValidator>();
builder.Services.AddScoped<IValidator<ReviewRejectRequest>, ReviewRejectValidator>();
builder.Services.AddScoped<IValidator<TripPlanInsertRequest>, TripPlanInsertValidator>();
builder.Services.AddScoped<IValidator<TripPlanUpdateRequest>, TripPlanUpdateValidator>();
builder.Services.AddScoped<IValidator<CollectionInsertRequest>, CollectionInsertValidator>();
builder.Services.AddScoped<IValidator<CollectionUpdateRequest>, CollectionUpdateValidator>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(options => // dodavanje authentfikacije i autorizacije u projekat
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtToken:Issuer"],
        ValidAudience = builder.Configuration["JwtToken:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:SecretKey"] ?? string.Empty)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        // AccessManager issues the role claim as ClaimNames.Role ("Role"), not the default
        // ClaimTypes.Role URI - without this, User.IsInRole/[Authorize(Roles=...)] never match.
        RoleClaimType = TravelBay.Model.Constants.ClaimNames.Role,
        NameClaimType = TravelBay.Model.Constants.ClaimNames.Id
    };

    // SignalR's browser/WebSocket client can't set an Authorization header, so it sends the
    // JWT as ?access_token=... instead - accept it there, and only there.
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "TravelBay API",
            Description = "API for managing travel destinations, reviews and trip planning in the TravelBay application"
        });

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();


    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors("TravelBayClients");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
