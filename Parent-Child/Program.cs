//using System.Text;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.FileProviders;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using Parent_Child.Models;
//using Parent_Child.Services;

//var builder = WebApplication.CreateBuilder(args);


//// Bind JwtSettings
//var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
//builder.Services.Configure<JwtSettings>(jwtSettingsSection);
//var jwtSettings = jwtSettingsSection.Get<JwtSettings>();

//// Add authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,

//        ValidIssuer = jwtSettings.Issuer,
//        ValidAudience = jwtSettings.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
//    };
//});


//// ✅ Role-based authorization policies
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("ParentPolicy", policy => policy.RequireRole("Parent"));
//    options.AddPolicy("ChildPolicy", policy => policy.RequireRole("Child"));
//});


//// Services
//builder.Services.AddControllers();

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        new MySqlServerVersion(new Version(8, 0, 36))
//    ));

//builder.Services.AddControllers()
//    .AddJsonOptions(x =>
//        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);


//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<IRewardService, RewardService>();
//builder.Services.AddScoped<ITaskService, TaskService>();
//builder.Services.AddScoped<IFamilyService, FamilyService>();
//builder.Services.AddScoped<IStatsService, StatisticsService>();
//builder.Services.AddScoped<IAchievementService, AchievementService>();


//// Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Parent-Child API",
//        Version = "v1",
//        Description = "API for Chore Management, Rewards, and Family Dashboard"
//    });
//    c.EnableAnnotations();
//});

//var app = builder.Build();

//// Middleware
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parent-Child API v1");
//        c.RoutePrefix = "swagger";
//    });
//}

//app.UseAuthentication(); // IMPORTANT: before UseAuthorization
//app.UseAuthorization();
//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
//app.UseStaticFiles();

//// Ensure uploads folder exists
//var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
//if (!Directory.Exists(uploadsPath))
//{
//    Directory.CreateDirectory(uploadsPath);
//}

//// Serve static files from uploads
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(uploadsPath),
//    RequestPath = "/uploads"
//});

//app.Run();


using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Parent_Child.Models;
using Parent_Child.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔑 1. Bind JwtSettings from appsettings.json
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();

// 🔑 2. Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// 🔑 3. Role-based authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ParentPolicy", policy => policy.RequireRole("Parent"));
    options.AddPolicy("ChildPolicy", policy => policy.RequireRole("Child"));
    options.AddPolicy("ChildOrParentPolicy", policy =>
       policy.RequireRole("Child", "Parent"));
});

// 📦 4. Register services and DB context
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IFamilyService, FamilyService>();
builder.Services.AddScoped<IStatsService, StatisticsService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();

// 🔧 5. Configure Swagger with JWT Bearer support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Parent-Child API",
        Version = "v1",
        Description = "API for Chore Management, Rewards, and Family Dashboard"
    });
    c.EnableAnnotations();

    // 🔑 JWT Bearer config for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer eyJhb...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// 🌐 6. Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parent-Child API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();


// 🚀 8. Map controllers
app.MapControllers();

app.Run();
