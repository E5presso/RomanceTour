using Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using RomanceTour.Extensions;
using RomanceTour.Middlewares;
using RomanceTour.Middlewares.DataEncryption;
using RomanceTour.Middlewares.DataEncryption.Providers;

namespace RomanceTour.Models
{
    public partial class RomanceTourDbContext : DbContext
    {
        private readonly IDataEncryptionProvider encryptor = new GcmEncryptionProvider(new Gcm(XmlConfiguration.SecretKey));
        private readonly IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<Billing> Billing { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<DateSession> DateSession { get; set; }
        public virtual DbSet<Departure> Departure { get; set; }
        public virtual DbSet<Error> Error { get; set; }
        public virtual DbSet<Host> Host { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Option> Option { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PriceRule> PriceRule { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductBilling> ProductBilling { get; set; }
        public virtual DbSet<ProductDeparture> ProductDeparture { get; set; }
        public virtual DbSet<ProductHost> ProductHost { get; set; }
        public virtual DbSet<ProductPriceRule> ProductPriceRule { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Verification> Verification { get; set; }

        public RomanceTourDbContext()
        {
        }
        public RomanceTourDbContext(DbContextOptions<RomanceTourDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(configuration.GetConnectionString("RomanceTourDb"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("appointment");

                entity.HasIndex(e => e.DateSessionId)
                    .HasName("DateSessionId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Ammount).HasColumnType("int(11)");

                entity.Property(e => e.BillingBank)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BillingName)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BillingNumber)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DateSessionId).HasColumnType("int(11)");

                entity.Property(e => e.HashSalt)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Link)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Password)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Phone)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci")
                    .HasConversion(new EnumToStringConverter<AppointmentStatus>());

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.DateSession)
                    .WithMany(p => p.Appointment)
                    .HasForeignKey(d => d.DateSessionId)
                    .HasConstraintName("appointment_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Appointment)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("appointment_ibfk_2");
            });
            modelBuilder.Entity<Billing>(entity =>
            {
                entity.ToTable("billing");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Bank)
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Number)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
            modelBuilder.Entity<DateSession>(entity =>
            {
                entity.ToTable("datesession");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Paid).HasColumnType("int(11)");

                entity.Property(e => e.ProductId).HasColumnType("int(11)");

                entity.Property(e => e.Reserved).HasColumnType("int(11)");

                entity.Property(e => e.Sales).HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci")
                    .HasConversion(new EnumToStringConverter<DateSessionStatus>());

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.DateSession)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("datesession_ibfk_1");
            });
            modelBuilder.Entity<Departure>(entity =>
            {
                entity.ToTable("departure");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
            modelBuilder.Entity<Error>(entity =>
            {
                entity.ToTable("error");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Code).HasColumnType("int(11)");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StackTrace)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });
            modelBuilder.Entity<Host>(entity =>
            {
                entity.ToTable("host");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.HostBank)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.HostBillingNumber)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.HostName)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.HostPhone)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type).HasColumnType("int(11)");
            });
            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("log");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Parameter)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("log_ibfk_1");
            });
            modelBuilder.Entity<Option>(entity =>
            {
                entity.ToTable("option");

                entity.HasIndex(e => e.PersonId)
                    .HasName("PersonId");

                entity.HasIndex(e => e.PriceRuleId)
                    .HasName("PriceRuleId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.PriceRuleId).HasColumnType("int(11)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Option)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("option_ibfk_1");

                entity.HasOne(d => d.PriceRule)
                    .WithMany(p => p.Option)
                    .HasForeignKey(d => d.PriceRuleId)
                    .HasConstraintName("option_ibfk_2");
            });
            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("person");

                entity.HasIndex(e => e.AppointmentId)
                    .HasName("AppointmentId");

                entity.HasIndex(e => e.DepartureId)
                    .HasName("DepartureId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Ammount).HasColumnType("int(11)");

                entity.Property(e => e.AppointmentId).HasColumnType("int(11)");

                entity.Property(e => e.DepartureId).HasColumnType("int(11)");

                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.AppointmentId)
                    .HasConstraintName("person_ibfk_1");

                entity.HasOne(d => d.Departure)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.DepartureId)
                    .HasConstraintName("person_ibfk_2");
            });
            modelBuilder.Entity<PriceRule>(entity =>
            {
                entity.ToTable("pricerule");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.RuleName)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RuleType)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci")
                    .HasConversion(new EnumToStringConverter<PriceRuleType>());
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("CategoryId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Form)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.Rating).HasColumnType("int(11)");

                entity.Property(e => e.SubTitle)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Thumbnail)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("product_ibfk_1");
            });
            modelBuilder.Entity<ProductBilling>(entity =>
            {
                entity.ToTable("productbilling");

                entity.HasIndex(e => e.BillingId)
                    .HasName("BillingId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BillingId).HasColumnType("int(11)");

                entity.Property(e => e.ProductId).HasColumnType("int(11)");

                entity.HasOne(d => d.Billing)
                    .WithMany(p => p.ProductBilling)
                    .HasForeignKey(d => d.BillingId)
                    .HasConstraintName("productbilling_ibfk_2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductBilling)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("productbilling_ibfk_1");
            });
            modelBuilder.Entity<ProductDeparture>(entity =>
            {
                entity.ToTable("productdeparture");

                entity.HasIndex(e => e.DepartureId)
                    .HasName("DepartureId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.DepartureId).HasColumnType("int(11)");

                entity.Property(e => e.ProductId).HasColumnType("int(11)");

                entity.HasOne(d => d.Departure)
                    .WithMany(p => p.ProductDeparture)
                    .HasForeignKey(d => d.DepartureId)
                    .HasConstraintName("productdeparture_ibfk_2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductDeparture)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("productdeparture_ibfk_1");
            });
            modelBuilder.Entity<ProductHost>(entity =>
            {
                entity.ToTable("producthost");

                entity.HasIndex(e => e.HostId)
                    .HasName("HostId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.HostId).HasColumnType("int(11)");

                entity.Property(e => e.ProductId).HasColumnType("int(11)");

                entity.HasOne(d => d.Host)
                    .WithMany(p => p.ProductHost)
                    .HasForeignKey(d => d.HostId)
                    .HasConstraintName("producthost_ibfk_2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductHost)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("producthost_ibfk_1");
            });
            modelBuilder.Entity<ProductPriceRule>(entity =>
            {
                entity.ToTable("productpricerule");

                entity.HasIndex(e => e.PriceRuleId)
                    .HasName("PriceRuleId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PriceRuleId).HasColumnType("int(11)");

                entity.Property(e => e.ProductId).HasColumnType("int(11)");

                entity.HasOne(d => d.PriceRule)
                    .WithMany(p => p.ProductPriceRule)
                    .HasForeignKey(d => d.PriceRuleId)
                    .HasConstraintName("productpricerule_ibfk_2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPriceRule)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("productpricerule_ibfk_1");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BillingBank)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BillingName)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BillingNumber)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.HashSalt)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci")
                    .HasConversion(new EnumToStringConverter<UserStatus>());

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
            modelBuilder.Entity<Verification>(entity =>
            {
                entity.ToTable("verification");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });
            modelBuilder.UseEncryption(encryptor);
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}