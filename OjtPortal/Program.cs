using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OjtPortal.Context;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using OjtPortal.Services;
using Serilog;
using Serilog.Events;
using SeriLogThemesLibrary;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add view support
builder.Services.AddRazorPages();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add logging configuration
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .WriteTo.AzureApp(
                outputTemplate: "{Message:lj}{NewLine}{Exception}"
             )
            .WriteTo.Console(
                theme: SeriLogCustomThemes.Theme1(), 
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
            .CreateLogger();
builder.Host.UseSerilog();

// Load Secrets
Env.Load();
builder.Configuration.AddUserSecrets<Program>();

// Connect database to container 
builder.Services.AddDbContext<OjtPortalContext>(db =>
    db.UseNpgsql(builder.Configuration["DBCONNECTION"]));

// Identity Framework Configuration
builder.Services.AddIdentityApiEndpoints<User>(option =>
{
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireUppercase = false;
})
    .AddSignInManager<SignInManager<User>>()
    .AddEntityFrameworkStores<OjtPortalContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDataProtection();

builder.Services.AddHttpContextAccessor();

// Add HttpClient
builder.Services.AddHttpClient<HolidayService>();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Singleton services
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton(TimeProvider.System);

// Transient services
builder.Services.AddTransient<IHolidayService, HolidayService>();
builder.Services.AddTransient<IShiftRecordService, ShiftService>();
builder.Services.AddTransient<IStudentService, StudentService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEmailSender, EmailService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IDegreeProgramService, DegreeProgramService>();
builder.Services.AddTransient<IMentorService, MentorService>();
builder.Services.AddTransient<ITeacherService, TeacherService>();
builder.Services.AddTransient<IChairService, ChairService>();
builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddTransient<ILogbookEntryService, LogbookEntryService>();

// Scoped repositories
builder.Services.AddScoped<IHolidayRepo, HolidayRepo>();
builder.Services.AddScoped<IDepartmentRepo, DepartmentRepo>();
builder.Services.AddScoped<IDegreeProgramRepo, DegreeProgramRepo>();
builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IMentorRepo, MentorRepo>();
builder.Services.AddScoped<ICompanyRepo, CompanyRepo>();
builder.Services.AddScoped<ITeacherRepo, TeacherRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IChairRepo, ChairRepo>();
builder.Services.AddScoped<IAdminRepo, AdminRepo>();
builder.Services.AddScoped<IOtpRepo, OtpRepo>();
builder.Services.AddScoped<IAttendanceRepo, AttendanceRepo>();
builder.Services.AddScoped<ILogbookEntryRepo, LogbookEntryRepo>();


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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

app.UseRouting();

app.MapRazorPages();

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();

app.UseCors(options =>
    options.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(origin => true)
);

app.Run();
