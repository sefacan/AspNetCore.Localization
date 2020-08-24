using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AspNetCore.Localization.Localization
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // languages
            modelBuilder.Entity<Language>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<Language>()
                .Property(entity => entity.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<Language>()
                .Property(entity => entity.Culture)
                .HasMaxLength(10);

            modelBuilder.Entity<Language>()
                .Property(entity => entity.TwoLetterIsoCode)
                .HasMaxLength(2);

            modelBuilder.Entity<Language>()
                .Property(entity => entity.ThreeLetterIsoCode)
                .HasMaxLength(3);

            modelBuilder.Entity<Language>()
                .HasMany(entity => entity.LanguageResources);

            modelBuilder.Entity<Language>()
                .HasData(new[] {
                    new Language { Id = 1, Name = "English", Culture = "en-US", TwoLetterIsoCode = "en", ThreeLetterIsoCode = "eng" },
                    new Language { Id = 2, Name = "French", Culture = "fr-FR", TwoLetterIsoCode = "fr", ThreeLetterIsoCode = "fre" },
                    new Language { Id = 3, Name = "German", Culture = "de-DE", TwoLetterIsoCode = "de", ThreeLetterIsoCode = "ger" }
                });

            // resources
            modelBuilder.Entity<LanguageResource>()
                .HasKey(entity => entity.Id);

            modelBuilder.Entity<LanguageResource>()
                .Property(entity => entity.Key)
                .HasMaxLength(255);

            modelBuilder.Entity<LanguageResource>()
                .Property(entity => entity.Value)
                .HasMaxLength(3000);

            modelBuilder.Entity<LanguageResource>()
            .HasOne(entity => entity.Language);

            modelBuilder.Entity<LanguageResource>()
                .HasData(new[] {
                    new LanguageResource { Id = 1, Key = "menu.homepage", Value = "Home Page", LanguageId = 1 },
                    new LanguageResource { Id = 2, Key = "menu.homepage", Value = "Page D'accueil", LanguageId = 2 },
                    new LanguageResource { Id = 3, Key = "menu.homepage", Value = "Startseite", LanguageId = 3 }
                });
        }

        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageResource> LanguageResources { get; set; }
    }

    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Culture { get; set; }
        public string TwoLetterIsoCode { get; set; }
        public string ThreeLetterIsoCode { get; set; }

        public List<LanguageResource> LanguageResources { get; set; } = new List<LanguageResource>();
    }

    public class LanguageResource
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public Language Language { get; set; }
    }
}