using System.Reflection;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using ProCredit_task.Contracts;
using ProCredit_task.Services;

namespace ProCredit_task;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        logger.Debug("Logger initialized");

        var builder = WebApplication.CreateBuilder(args);
        //var connString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Add services to the container.
        builder.Services.AddScoped<IDatabase>(sp =>
        {
            var nlogLogger = sp.GetRequiredService<ILogger<Database>>();
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            return new Database(nlogLogger, connString);
        });

        // NLog: Setup NLog for Dependency injection
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Version = "v1",
                Title = "Swift Parser API",
                Description =
                    "A simple API used for parsing a swift (type \"MT799\") message and saving the parsed result into an SQLite database"
            });
            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
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


        app.MapControllers();

        app.Run();
    }
}