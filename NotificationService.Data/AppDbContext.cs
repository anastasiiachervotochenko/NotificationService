using Microsoft.EntityFrameworkCore;
using NotificationService.Data.Entity;

namespace NotificationService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<NotificationStatus> NotificationStatuses { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserConfig> UserConfigs { get; set; }
}