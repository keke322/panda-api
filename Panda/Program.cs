using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Panda.Data;
using Panda.Models;
using Panda.Validators;
using Panda.Repositories;
using Panda.Services;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Panda.OperationFilters;

var builder = WebApplication.CreateBuilder(args);

// --- Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// --- EF Core (SQLite example, swap easily to SqlServer or Postgres)
builder.Services.AddDbContext<PandaDbContext>(options =>
    options.UseSqlite("Data Source=SQLite\\panda.db"));

// --- Dependency Injection
builder.Services.AddScoped<IRepository<Patient>, PatientRepository>();
builder.Services.AddScoped<IValidator<Patient>, PatientValidator>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IRepository<Appointment>, AppointmentRepository>();
builder.Services.AddScoped<IValidator<Appointment>, AppointmentValidator>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// --- AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// --- Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// --- Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // keep PascalCase if needed
    })
    .AddDataAnnotationsLocalization()
    .AddViewLocalization();

// --- Swagger for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "PANDA API",
        Description = "Patient Appointment Network Data Application MVP"
    });
    options.OperationFilter<AcceptLanguageHeaderOperationFilter>();
});

var app = builder.Build();

// --- Migrate DB on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PandaDbContext>();
    db.Database.Migrate();
}

var supportedCultures = new[] { "en", "fr", "es" };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en"),
    SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList(),
    SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList()
});

// --- Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
