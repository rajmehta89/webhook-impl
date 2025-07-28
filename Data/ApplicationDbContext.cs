using Microsoft.EntityFrameworkCore;
using WhatsappWebHook.Models;

namespace WhatsappWebHook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<WhatsAppMessage> WhatsAppMessages { get; set; }
    }
}
