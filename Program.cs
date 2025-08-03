using Microsoft.EntityFrameworkCore;
using WhatsappWebHook.Data;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add EF Core SQL Server support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Middleware pipeline
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapGet("/hello", (ILogger<Program> logger) =>
{
    logger.LogInformation("GET /hello endpoint was hit.");
    return "Hello World!";
});

app.MapControllers();

app.Run();
