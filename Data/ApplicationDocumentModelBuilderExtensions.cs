using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace linc.Data
{
    public static class ApplicationDocumentModelBuilderExtensions
    {
        public static EntityTypeBuilder<ApplicationDocument> MapDossiersTable(this EntityTypeBuilder<ApplicationDocument> builder)
        {
            builder
                .HasMany(document => document.Dossiers)
                .WithMany(dossier => dossier.Documents)
                .UsingEntity<Dictionary<string, object>>(
                    "dossier_documents",
                    join => join
                        .HasOne<ApplicationDossier>()
                        .WithMany()
                        .HasForeignKey("dossier_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_dossier_documents_dossier_id"),
                    join => join
                        .HasOne<ApplicationDocument>()
                        .WithMany()
                        .HasForeignKey("document_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_dossier_documents_document_id")
                );

            return builder;
        }

        public static EntityTypeBuilder<ApplicationDocument> MapIssuesTable(this EntityTypeBuilder<ApplicationDocument> builder)
        {
            builder
                .HasMany(document => document.Issues)
                .WithMany(issue => issue.Documents)
                .UsingEntity<Dictionary<string, object>>(
                    "issue_documents",
                    join => join
                        .HasOne<ApplicationIssue>()
                        .WithMany()
                        .HasForeignKey("issue_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_issue_documents_issue_id"),
                    join => join
                        .HasOne<ApplicationDocument>()
                        .WithMany()
                        .HasForeignKey("document_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_issue_documents_document_id")
                );

            return builder;
        }
    }
}
