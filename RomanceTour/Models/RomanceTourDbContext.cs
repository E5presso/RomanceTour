using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

using RomanceTour.Middlewares;

using System.Linq;

namespace RomanceTour.Models
{
    /// <summary>
    /// 모델의 운용모드입니다.
    /// </summary>
    public enum EntityMode
    {
        /// <summary>
        /// 최초로 암호화를 설정할 때 사용합니다. 이 경우 기존의 평문 데이터가 암호문으로 변경됩니다.
        /// </summary>
        Initialize,
        /// <summary>
        /// 암호화 키를 변경할 때 사용됩니다. 이 경우 기존의 암호문이 새로운 키를 기준으로 다시 암호화됩니다.
        /// </summary>
        Migration,
        /// <summary>
        /// 평상 운용 시 사용됩니다. 사용 중인 키를 이용하여 암복호화를 수행합니다.
        /// </summary>
        Normal,
        /// <summary>
        /// 데이터베이스에 설정된 암호화의 해제 시 사용됩니다. 이 경우 이미 설정된 데이터가 평문으로 복호화됩니다.
        /// </summary>
        Revert
    }
    public partial class RomanceTourDbContext : DbContext
    {
        public IEncryptionProvider encryptor;
        private string Key => XmlConfiguration.Key;
        private readonly IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<Billing> Billing { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<DateSession> DateSession { get; set; }
        public virtual DbSet<Departure> Departure { get; set; }
        public virtual DbSet<Error> Error { get; set; }
        public virtual DbSet<Help> Help { get; set; }
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
        public virtual DbSet<Push> Push { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Verification> Verification { get; set; }

        public RomanceTourDbContext()
        {
            encryptor = new EntityEncryptor(Key, Key);
        }
        public RomanceTourDbContext(DbContextOptions<RomanceTourDbContext> options) : base(options)
        {
            encryptor = new EntityEncryptor(Key, Key);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("RomanceTourDb"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(e => e.Address).HasColumnType("text");

                entity.Property(e => e.BillingBank).HasColumnType("text");

                entity.Property(e => e.BillingName).HasColumnType("text");

                entity.Property(e => e.BillingNumber).HasColumnType("text");

                entity.Property(e => e.HashSalt)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasColumnType("text");

                entity.Property(e => e.Password)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasColumnType("text");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasConversion(new EnumToStringConverter<AppointmentStatus>());

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.DateSession)
                    .WithMany(p => p.Appointment)
                    .HasForeignKey(d => d.DateSessionId)
                    .HasConstraintName("FK__Appointme__DateS__06CD04F7");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Appointment)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Appointme__UserI__07C12930");
            });
            modelBuilder.Entity<Billing>(entity =>
            {
                entity.Property(e => e.Bank)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Number).HasColumnType("text");
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<DateSession>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasConversion(new EnumToStringConverter<DateSessionStatus>());

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.DateSession)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__DateSessi__Produ__00200768");
            });
            modelBuilder.Entity<Departure>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<Error>(entity =>
            {
                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.StackTrace)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });
            modelBuilder.Entity<Help>(entity =>
            {
                entity.Property(e => e.Command)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Data).HasColumnType("text");

                entity.Property(e => e.Ipaddress)
                    .IsRequired()
                    .HasColumnName("IPAddress")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Help)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Help__UserId__395884C4");
            });
            modelBuilder.Entity<Host>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.HostBank)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.HostBillingNumber)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.HostName)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.HostPhone)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Parameter).HasColumnType("text");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Log__UserId__3C34F16F");
            });
            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Option)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK__Option__PersonId__0E6E26BF");

                entity.HasOne(d => d.PriceRule)
                    .WithMany(p => p.Option)
                    .HasForeignKey(d => d.PriceRuleId)
                    .HasConstraintName("FK__Option__PriceRul__0F624AF8");
            });
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.AppointmentId)
                    .HasConstraintName("FK__Person__Appointm__0A9D95DB");

                entity.HasOne(d => d.Departure)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.DepartureId)
                    .HasConstraintName("FK__Person__Departur__0B91BA14");
            });
            modelBuilder.Entity<PriceRule>(entity =>
            {
                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.RuleName)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.RuleType)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasConversion(new EnumToStringConverter<PriceRuleType>());
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Form)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.SubTitle).HasColumnType("text");

                entity.Property(e => e.Thumbnail)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Product__Expose__66603565");
            });
            modelBuilder.Entity<ProductBilling>(entity =>
            {
                entity.HasOne(d => d.Billing)
                    .WithMany(p => p.ProductBilling)
                    .HasForeignKey(d => d.BillingId)
                    .HasConstraintName("FK__ProductBi__Billi__6C190EBB");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductBilling)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductBi__Produ__6B24EA82");
            });
            modelBuilder.Entity<ProductDeparture>(entity =>
            {
                entity.HasOne(d => d.Departure)
                    .WithMany(p => p.ProductDeparture)
                    .HasForeignKey(d => d.DepartureId)
                    .HasConstraintName("FK__ProductDe__Depar__7D439ABD");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductDeparture)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductDe__Produ__7C4F7684");
            });
            modelBuilder.Entity<ProductHost>(entity =>
            {
                entity.HasOne(d => d.Host)
                    .WithMany(p => p.ProductHost)
                    .HasForeignKey(d => d.HostId)
                    .HasConstraintName("FK__ProductHo__HostI__778AC167");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductHost)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductHo__Produ__76969D2E");
            });
            modelBuilder.Entity<ProductPriceRule>(entity =>
            {
                entity.HasOne(d => d.PriceRule)
                    .WithMany(p => p.ProductPriceRule)
                    .HasForeignKey(d => d.PriceRuleId)
                    .HasConstraintName("FK__ProductPr__Price__71D1E811");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPriceRule)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductPr__Produ__70DDC3D8");
            });
            modelBuilder.Entity<Push>(entity =>
            {
                entity.Property(e => e.Group)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Message)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Review__ProductI__02FC7413");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Review__UserId__03F0984C");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.BillingBank).HasColumnType("text");

                entity.Property(e => e.BillingName).HasColumnType("text");

                entity.Property(e => e.BillingNumber).HasColumnType("text");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.HashSalt)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasConversion(new EnumToStringConverter<UserStatus>());

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<Verification>(entity =>
            {
                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public void ChangeMode(EntityMode mode)
        {
            void UpdateEntities()
            {
                var users = User.Where(x => true);
                var appointments = Appointment.Where(x => true);
                User.UpdateRange(users);
                Appointment.UpdateRange(appointments);
                SaveChanges();
            }
            switch (mode)
            {
                case EntityMode.Initialize:
                {
                    XmlConfiguration.Key = Core.Security.Key.GenerateString(32);
                    XmlConfiguration.SaveChanges();
                    ((EntityEncryptor)encryptor).EncryptionKey = Key;
                    ((EntityEncryptor)encryptor).DecryptionKey = string.Empty;
                    UpdateEntities();
                    break;
                }
                case EntityMode.Migration:
                {
                    string newKey = Core.Security.Key.GenerateString(32);
                    string oldKey = Key;
                    XmlConfiguration.Key = newKey;
                    XmlConfiguration.SaveChanges();
                    ((EntityEncryptor)encryptor).EncryptionKey = newKey;
                    ((EntityEncryptor)encryptor).DecryptionKey = oldKey;
                    UpdateEntities();
                    break;
                }
                case EntityMode.Normal:
                {
                    ((EntityEncryptor)encryptor).EncryptionKey = Key;
                    ((EntityEncryptor)encryptor).DecryptionKey = Key;
                    break;
                }
                case EntityMode.Revert:
                {
                    string oldKey = Key;
                    XmlConfiguration.Key = string.Empty;
                    XmlConfiguration.SaveChanges();
                    ((EntityEncryptor)encryptor).EncryptionKey = Key;
                    ((EntityEncryptor)encryptor).DecryptionKey = oldKey;
                    UpdateEntities();
                    break;
                }
            }
        }
    }
}