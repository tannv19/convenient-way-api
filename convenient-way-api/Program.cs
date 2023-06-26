using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ship_convenient.Config;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = Encoding.GetEncoding("UTF-8");
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddAzureWebAppDiagnostics().AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);
#region Extension
builder.Services.AddFirebaseApp();
builder.Services.AddDIService();
builder.Services.AddCorsApp();
builder.Services.AddSwaggerApp();
builder.Services.AddHTTPLogingExtension();
builder.Services.AddBackgroundServices();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpLogging();
// app.UseHttpsRedirection(); 
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
