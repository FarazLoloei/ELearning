using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure configuration sources
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        b => b.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(settings => settings.WithDictionaryHandle());

// Add controllers
builder.Services.AddControllers();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseCors("CorsPolicy");

// Configure endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Configure Ocelot middleware
await app.UseOcelot();

// Run the app
app.Run();