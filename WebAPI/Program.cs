using DependencyResolution;
using Main.Base.Instance;
using Main.Localization;
using Main.Persistence.PersistenceBase.AdoNet;
using Main.Security.Jwt;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using System.Globalization;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Localization

builder.Services.Configure<JsonLocalizationOptions>(options =>
{
    options.ResourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
});
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddSingleton<IStringLocalizer, JsonStringLocalizer>();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "az-AZ", "tr-TR", "ru-RU" };
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("TokenOptions"));

builder.Services.AddBaseRegistration(builder.Configuration);

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddSwaggerGen(swagger =>
{
    // This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Feedback API",
        Description = ".NET 8 API for Feedback"
    });

    // To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below."
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

var app = builder.Build();

var supportedCultures = new[] { "en-US", "az-AZ", "tr-TR", "ru-RU" };

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList()
};

app.UseRequestLocalization(localizationOptions);

var serviceFactory = app.Services.GetRequiredService<ServiceFactory>();
QueryBuilderUtils.Initialize(serviceFactory);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
