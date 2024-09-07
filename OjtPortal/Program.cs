using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OjtPortal.Context;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Infrastructure.JsonConverters;
using OjtPortal.Repositories;
using OjtPortal.Services;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using SeriLogThemesLibrary;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add Automapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add logging configuration
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .WriteTo.Console(
                theme: SeriLogCustomThemes.Theme1(), 
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
            .CreateLogger();
builder.Host.UseSerilog();

// Connect database to container 
builder.Services.AddDbContext<OjtPortalContext>(db =>
    db.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));

// Identity Framework Configuration
builder.Services.AddIdentityCore<User>(option => {
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<OjtPortalContext>();

// Add HttpClient
builder.Services.AddHttpClient<HolidayService>();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Singleton services
builder.Services.AddSingleton<ICacheService, CacheService>();

// Transient services
builder.Services.AddTransient<IHolidayService, HolidayService>();
builder.Services.AddTransient<IShiftRecordService, ShiftRecordService>();
builder.Services.AddTransient<IStudentService, StudentService>();

// Scoped repositories
builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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


    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
