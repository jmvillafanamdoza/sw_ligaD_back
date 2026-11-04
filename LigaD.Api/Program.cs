using FluentValidation;
using LigaD.Api.Config;
using LigaD.Api.EndPoints;
using LigaD.Api.Filtros;
using LigaD.Api.Repositorios;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// 1. Configuración (appsettings -> Config/)
// ---------------------------------------------------------------------
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
    throw new InvalidOperationException(
        "Falta la cadena de conexión 'ConnectionStrings:DefaultConnection'. En desarrollo está en " +
        "appsettings.Development.json; en el servidor use la variable de entorno ConnectionStrings__DefaultConnection.");

// ---------------------------------------------------------------------
// 2. CORS (orígenes del front en appsettings: Cors:OrigenesPermitidos)
// ---------------------------------------------------------------------
const string corsPolicyName = "FrontLigaD";
var origenes = builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddPolicy(corsPolicyName, policy =>
        policy.WithOrigins(origenes).AllowAnyMethod().AllowAnyHeader()));

// ---------------------------------------------------------------------
// 3. OpenAPI nativo + Scalar
// ---------------------------------------------------------------------
builder.Services.AddLigaDOpenApi();

// ---------------------------------------------------------------------
// 4. Acceso a datos (Dapper + SPs PRLGD_*) y repositorios
// ---------------------------------------------------------------------
builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<ITipoValorRepositorio, TipoValorRepositorio>();

// ---------------------------------------------------------------------
// 5. FluentValidation (validadores en Validaciones/)
// ---------------------------------------------------------------------
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// ---------------------------------------------------------------------
// Pipeline HTTP
// ---------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                      // /openapi/v1.json
    app.MapScalarApiReference(options =>   // UI en /scalar/v1
    {
        options.Title = "Liga D API";
        options.Theme = ScalarTheme.DeepSpace;
    });
}

app.UseMiddleware<ManejoExcepcionesMiddleware>();
app.UseHttpsRedirection();
app.UseCors(corsPolicyName);

// Endpoints (Minimal API) agrupados por dominio
app.MapSaludEndpoints();
app.MapCatalogoEndpoints();

app.Run();
