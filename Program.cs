using StackExchange.Redis;

namespace UrlShortener.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var frontendCorsPolicy = "frontendCorsPolicy";
        builder.Services.AddCors(options => options.AddPolicy(frontendCorsPolicy, corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins(builder.Configuration["Frontend:Url"])
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));
        
        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{builder.Configuration["RedisSettings:Url"]}"));

        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.MapControllers();

        app.UseCors(frontendCorsPolicy);

        app.Run();
    }
}