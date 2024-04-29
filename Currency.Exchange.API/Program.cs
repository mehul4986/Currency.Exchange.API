using Currency.Exchange.API.Repositories;
using Currency.Exchange.API.Repositories.Interfaces;
using Currency.Exchange.API.Services;
using Currency.Exchange.API.Services.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency Exchange Manager API", Version = "v1" });
});

builder.Services.AddLogging((loggingBuilder) =>
{
    loggingBuilder.SetMinimumLevel(LogLevel.Error); // Update default MEL-filter
});

builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IExchangeRepository, ExchangeRepository>();
builder.Services.AddSingleton<IExchangeService, ExchangeService>();

//builder.Services.AddCors();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllOrigins",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllOrigins");

app.MapControllers();

app.Run();
