using Microsoft.EntityFrameworkCore;
using WhatsappWebHook.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 📦 Add EF Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🚀 Build the app
var app = builder.Build();

// 🌐 Middleware pipeline
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization(); // ✅ Required to activate controller routing

// 🛠️ Minimal API test endpoint
app.MapGet("/hello", (ILogger<Program> logger) =>
{
    logger.LogInformation("GET /hello endpoint was hit.");
    return "Hello World!";
});

// 🚀 Enable attribute-based controllers like [Route("webhook")]
app.MapControllers();

app.Run();
