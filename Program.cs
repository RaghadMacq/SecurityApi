using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SecurityApi.Data;
using SecurityApi.Filters;
using SecurityApi.Infrastructure;
using SecurityApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt =>
{
    //opt.Filters.Add(new MyLogging());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configure rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();
// Configure Kestrel server to hide the Server header
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

//Add Authentication
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

//Add Autherization
builder.Services.AddAuthorizationBuilder();

//Configer DbContext
builder.Services
    .AddDbContext<AppDbContext>(opt => opt.UseSqlServer("Server=HPG10;Database=SecurityApi;Trusted_Connection=True;TrustServerCertificate=True") );

builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>().AddApiEndpoints();

// initialise the service in the DI
//builder.Services.AddExceptionHandler<GlobalExeptionHandler>();
//builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapIdentityApi<AppUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();
//app.UseExceptionHandler();

app.MapControllers();
// Use rate limiting middleware
app.UseIpRateLimiting();

app.Run();
