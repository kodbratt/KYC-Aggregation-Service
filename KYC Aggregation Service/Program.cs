using KYC_Aggregation_Service.ErrorHandling;
using KYC_Aggregation_Service.Persistence;
using KYC_Aggregation_Service.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("KycDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'KycDatabase' is not configured.");

builder.Services.AddDbContext<KycDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ICustomerDataClient, CustomerDataClient>(
    (serviceProvider, httpClient) =>
    {
        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        var baseUrl = configuration["CustomerDataApi:BaseUrl"]
            ?? throw new InvalidOperationException(
                "CustomerDataApi:BaseUrl is not configured.");

        httpClient.BaseAddress = new Uri(baseUrl);
    });

builder.Services.AddScoped<IAggregatedKycStore, EfAggregatedKycStore>();
builder.Services.AddScoped<IKycAggregationService, KycAggregationService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "KYC Aggregation API v1");
    options.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();