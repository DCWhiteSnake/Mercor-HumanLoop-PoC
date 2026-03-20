using System.Text;
using FluentValidation;
using HumanHands.Common.Behaviors;
using HumanHands.Common.Middleware;
using HumanHands.Features.Auth;
using HumanHands.Features.Tasks;
using HumanHands.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── MediatR + pipeline behaviors ────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// ── FluentValidation ─────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ── In-memory stores (singletons) ────────────────────────────────────────────
builder.Services.AddSingleton<InMemoryTaskStore>();
builder.Services.AddSingleton<InMemoryUsageStore>();

// ── IHttpContextAccessor (used by CreateTaskHandler) ─────────────────────────
builder.Services.AddHttpContextAccessor();

// ── JWT Authentication ────────────────────────────────────────────────────────
// Must match the signing key in GetTokenHandler.
const string jwtSigningKey = "humanhands-mock-signing-key-32bytes!!";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "humanhands-api",
            ValidateAudience = true,
            ValidAudience = "humanhands-clients",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ── Exception handling ───────────────────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HumanHands API",
        Version = "v1",
        Description =
            "A Human-in-the-Loop REST API that allows an LLM to delegate physical tasks " +
            "(errands, deliveries, manual labour) to human agents. " +
            "Authenticate via POST /api/auth/token, then use the bearer token on task endpoints."
    });

    // Bearer token UI support in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your token here. Obtain one from POST /api/auth/token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            []
        }
    });

    // Pull in XML doc comments from all endpoints and DTOs
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.EnableAnnotations();
});

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HumanHands API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Usage tracking runs after auth so JWT claims are resolved.
app.UseMiddleware<UsageTrackingMiddleware>();

// ── Endpoint registration ─────────────────────────────────────────────────────
app.MapAuthEndpoints();
app.MapTaskEndpoints();

app.Run();
