using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace linc.Data
{
    public static class ApplicationAuthorModelBuilderExtensions
    {
        public static EntityTypeBuilder<ApplicationAuthor> MapDossiersTable(this EntityTypeBuilder<ApplicationAuthor> builder)
        {
            builder
                .HasMany(author => author.Dossiers)
                .WithMany(dossier => dossier.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "dossier_authors",
                    join => join
                        .HasOne<ApplicationDossier>()
                        .WithMany()
                        .HasForeignKey("dossier_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_dossier_authors_dossier_id"),
                    join => join
                        .HasOne<ApplicationAuthor>()
                        .WithMany()
                        .HasForeignKey("author_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_dossier_authors_author_id")
                );

            return builder;
        }

        public static EntityTypeBuilder<ApplicationAuthor> MapSourcesTable(this EntityTypeBuilder<ApplicationAuthor> builder)
        {
            builder
                .HasMany(author => author.Sources)
                .WithMany(dossier => dossier.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "source_authors",
                    join => join
                        .HasOne<ApplicationSource>()
                        .WithMany()
                        .HasForeignKey("source_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_source_authors_source_id"),
                    join => join
                        .HasOne<ApplicationAuthor>()
                        .WithMany()
                        .HasForeignKey("author_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_source_authors_author_id")
                );

            return builder;
        }
    }
}
