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
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<DateSession> DateSession { get; set; }
        public virtual DbSet<Departure> Departure { get; set; }
        public virtual DbSet<Error> Error { get; set; }
        public virtual DbSet<Help> Help { get; set; }
        public virtual DbSet<Host> Host { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Media> Media { get; set; }
        public virtual DbSet<Option> Option { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<PriceRule> PriceRule { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductBilling> ProductBilling { get; set; }
        public virtual DbSet<ProductDeparture> ProductDeparture { get; set; }
        public virtual DbSet<ProductHost> ProductHost { get; set; }
        public virtual DbSet<ProductPriceRule> ProductPriceRule { get; set; }
        public virtual DbSet<Push> Push { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<User> User { get; set; }

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
                optionsBuilder.UseMySql(configuration.GetConnectionString("RomanceTourDB"), x => x.ServerVersion("8.0.18-mysql"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasIndex(e => e.DateSessionId)
                    .HasName("DateSessionId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Address)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.BillingBank)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.BillingName)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.BillingNumber)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.HashSalt)
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Password)
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Phone)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasConversion(new EnumToStringConverter<AppointmentStatus>());

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

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
                entity.Property(e => e.Bank)
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Number)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasIndex(e => e.CommentId)
                    .HasName("CommentId");

                entity.HasIndex(e => e.PostId)
                    .HasName("PostId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.CommentNavigation)
                    .WithMany(p => p.InverseCommentNavigation)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("comment_ibfk_3");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("comment_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("comment_ibfk_1");
            });
            modelBuilder.Entity<DateSession>(entity =>
            {
                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasConversion(new EnumToStringConverter<DateSessionStatus>());

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.DateSession)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("DateSession_ibfk_1");
            });
            modelBuilder.Entity<Departure>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });
            modelBuilder.Entity<Error>(entity =>
            {
                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.StackTrace)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });
            modelBuilder.Entity<Help>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Command)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Data)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Ipaddress)
                    .IsRequired()
                    .HasColumnName("IPAddress")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Help)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("help_ibfk_1");
            });
            modelBuilder.Entity<Host>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.HostBank)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.HostBillingNumber)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.HostName)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.HostPhone)
                    .IsRequired()
                    .HasColumnType("varchar(11)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });
            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasColumnName("IpAddress")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Parameter)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("log_ibfk_1");
            });
            modelBuilder.Entity<Media>(entity =>
            {
                entity.HasIndex(e => e.PostId)
                    .HasName("PostId");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Media)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("media_ibfk_1");
            });
            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasIndex(e => e.PersonId)
                    .HasName("PersonId");

                entity.HasIndex(e => e.PriceRuleId)
                    .HasName("PriceRuleId");

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
                entity.HasIndex(e => e.AppointmentId)
                    .HasName("AppointmentId");

                entity.HasIndex(e => e.DepartureId)
                    .HasName("DepartureId");

                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.AppointmentId)
                    .HasConstraintName("person_ibfk_1");

                entity.HasOne(d => d.Departure)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.DepartureId)
                    .HasConstraintName("person_ibfk_2");
            });
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Message)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("post_ibfk_1");
            });
            modelBuilder.Entity<PriceRule>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.RuleName)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.RuleType)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasConversion(new EnumToStringConverter<PriceRuleType>());
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("CategoryId");

                entity.Property(e => e.Form)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SubTitle)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Thumbnail)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("product_ibfk_1");
            });
            modelBuilder.Entity<ProductBilling>(entity =>
            {
                entity.HasIndex(e => e.BillingId)
                    .HasName("BillingId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.HasOne(d => d.Billing)
                    .WithMany(p => p.ProductBilling)
                    .HasForeignKey(d => d.BillingId)
                    .HasConstraintName("ProductBilling_ibfk_2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductBilling)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("ProductBilling_ibfk_1");
            });
            modelBuilder.Entity<ProductDeparture>(entity =>
            {
                entity.HasIndex(e => e.DepartureId)
                    .HasName("DepartureId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.HasOne(d => d.Departure)
                    .WithMany(p => p.ProductDeparture)
                    .HasForeignKey(d => d.DepartureId)
                    .HasConstraintName("ProductDeparture_ibfk_2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductDeparture)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("ProductDeparture_ibfk_1");
            });
            modelBuilder.Entity<ProductHost>(entity =>
            {
                entity.HasIndex(e => e.HostId)
                    .HasName("HostId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.HasOne(d => d.Host)
                    .WithMany(p => p.ProductHost)
                    .HasForeignKey(d => d.HostId)
                    .HasConstraintName("ProductHost_ibfk_2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductHost)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("ProductHost_ibfk_1");
            });
            modelBuilder.Entity<ProductPriceRule>(entity =>
            {
                entity.HasIndex(e => e.PriceRuleId)
                    .HasName("PriceRuleId");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.HasOne(d => d.PriceRule)
                    .WithMany(p => p.ProductPriceRule)
                    .HasForeignKey(d => d.PriceRuleId)
                    .HasConstraintName("ProductPriceRule_ibfk_2");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPriceRule)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("ProductPriceRule_ibfk_1");
            });
            modelBuilder.Entity<Push>(entity =>
            {
                entity.Property(e => e.Group)
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Message)
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("review_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("review_ibfk_2");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.BillingBank)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.BillingName)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.BillingNumber)
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.HashSalt)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci")
                    .HasConversion(new EnumToStringConverter<UserStatus>());

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });
            modelBuilder.UseEncryption(encryptor);
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