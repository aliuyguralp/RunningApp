namespace RunningApplicationNew.DataLayer
{
    using Microsoft.EntityFrameworkCore;
    using RunningApplicationNew.Entity;
   

   
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }

            // Kullanıcı tablosu
            public DbSet<User> Users { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // User tablosu için ek yapılandırmalar
                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email) // Email alanı için benzersiz index
                    .IsUnique();

                // Varsayılan değerler veya diğer kurallar burada eklenebilir
            }
        }
}
