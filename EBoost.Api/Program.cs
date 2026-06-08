using EBoost.Api.Extensions;
using EBoost.Api.Swagger;
using EBoost.Application.Common.Responses;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Application.Mappings;
using EBoost.Application.Services;
using EBoost.Infrastructure.Data;
using EBoost.Infrastructure.Data.Seed;
using EBoost.Infrastructure.Identity;
using EBoost.Infrastructure.Repositories;
using EBoost.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// ── Global crash guards ──────────────────────────────────────────────────────
// Catches unhandled exceptions on background threads (e.g. Razorpay SDK internals)
// and logs them instead of silently terminating the process.
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine("=== [FATAL] Unhandled AppDomain Exception ===");
    Console.WriteLine(e.ExceptionObject?.ToString());
};

// Catches fire-and-forget Task exceptions that were never awaited
TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Console.WriteLine("=== [FATAL] Unobserved Task Exception ===");
    Console.WriteLine(e.Exception?.ToString());
    e.SetObserved(); // Prevents process termination
};
// ────────────────────────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EBoost API",
        Version = "v1"
    });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
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

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SchemaFilter<ClearExamplesSchemaFilter>();


});


//for ApiResponces
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var response = ApiResponse<string>.FailureResponse(
            "Validation failed",
            errors
        );

        return new BadRequestObjectResult(response);
    };
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_AZURE_SQL_CONNECTION_STRING"))
{
    connectionString = "Server=.;Database=EBoost_EcommerceDb;Trusted_Connection=True;TrustServerCertificate=True";
}

builder.Services.AddDbContext<EBoostDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddAutoMapper(typeof(EBoost.Application.Mappings.ProductProfile).Assembly);




builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IShippingAddressRepository, ShippingAddressRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, EBoost.Infrastructure.Payments.StripePaymentService>();
builder.Services.AddScoped<IPasswordResetOtpRepository, PasswordResetOtpRepository>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IEmailService, EmailService>();




builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("roleId", "2"));

    options.AddPolicy("UserOnly", policy =>
    policy.RequireClaim("roleId", "1"));
});


var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? new[]
    {
        "http://localhost:5173",
        "https://eboost-ecommerce.vercel.app",
        "https://eboost.vercel.app"
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ResponseHeaderMiddleware>();


using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<EBoostDbContext>();
        
        // Automatically apply pending EF Core migrations on startup
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await AdminSeeder.SeedAsync(context, hasher);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Database migration or admin seeding failed:");
        Console.WriteLine(ex.Message);
    }
}


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
