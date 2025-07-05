using Kyoushitsu;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<BloggingContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("BloggingContext"))
        .UseSnakeCaseNamingConvention()
);

builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapGet("/posts",
    async (BloggingContext db) =>
    {
        var posts = await db.Posts.OrderByDescending(p => p.Id).ToListAsync();
        return new
        {
            data = posts
        };
    });

app.MapGet("/posts/{slug}", async (BloggingContext db, string slug) =>
{
    var post = await db.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
    if (post == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new
    {
        data = post
    });
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}