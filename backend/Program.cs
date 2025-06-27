using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using DeepSeekChatApi.Services;
using DeepSeekChatApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS - Flexible configuration for development and production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: Allow localhost and common development IPs
            policy.WithOrigins(
                    "http://localhost:4200",
                    "http://127.0.0.1:4200",
                    "http://localhost:3000",
                    "http://127.0.0.1:3000"
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // Production: Allow any origin (for Raspberry Pi deployment)
            // In a real production environment, you should specify exact origins
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// Register DeepSeek Chat Service
builder.Services.AddScoped<IDeepSeekChatService, DeepSeekChatService>();

// Configure DeepSeek settings
builder.Services.Configure<DeepSeekSettings>(
    builder.Configuration.GetSection("DeepSeek"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run(); 