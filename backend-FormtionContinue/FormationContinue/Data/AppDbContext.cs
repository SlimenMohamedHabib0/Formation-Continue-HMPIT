using FormationContinue.Models;
using Microsoft.EntityFrameworkCore;

namespace FormationContinue.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<Choix> Choix { get; set; } = null!;
        public DbSet<TentativeQcm> TentativesQcm { get; set; } = null!;
        public DbSet<ChoixSelectionne> ChoixSelectionnes { get; set; } = null!;
        public DbSet<ResultatQuestion> ResultatQuestions { get; set; } = null!;
        public DbSet<CourseProfessor> CourseProfessors { get; set; } = null!;
        public DbSet<QuestionCourse> CourseQuestions { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<CourseProgress> Progress { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FullName).HasMaxLength(64).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(120).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(20).IsRequired();
                entity.Property(u => u.PasswordHash).HasMaxLength(400).IsRequired();
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(u => u.Libelle).IsUnique();
                entity.Property(u => u.Libelle).HasMaxLength(100).IsRequired();

            });
            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(c => c.Titre).HasMaxLength(200).IsRequired();
                entity.Property(c => c.Description).IsRequired();
                entity.Property(c => c.MotsCles).HasMaxLength(255).IsRequired();
                entity.Property(c => c.Etat).HasMaxLength(50).IsRequired();
                entity.Property(c => c.NomFichierPdf).HasMaxLength(255);
                entity.Property(c => c.VideoFileName).HasMaxLength(255);
                entity.Property(c => c.VideoPath).HasMaxLength(600);
                entity.Property(c => c.VideoMimeType).HasMaxLength(100);


                entity.Property(c => c.ContenuPdf).HasColumnType("BLOB");

                entity.HasOne(c => c.Category)
                    .WithMany()
                    .HasForeignKey(c => c.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CourseProfessor>(entity =>
            {
                entity.HasKey(cp => new { cp.CourseId, cp.ProfessorId });

                entity.HasOne(cp => cp.Course)
                    .WithMany(c => c.CourseProfessors)
                    .HasForeignKey(cp => cp.CourseId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(cp => cp.Professor)
                    .WithMany()
                    .HasForeignKey(cp => cp.ProfessorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(q => q.Enonce).HasMaxLength(2000).IsRequired();
                entity.Property(q => q.Points).IsRequired();
            });

            modelBuilder.Entity<Choix>(entity =>
            {
                entity.Property(c => c.Libelle).HasMaxLength(500).IsRequired();
                entity.Property(c => c.EstCorrect)
                      .IsRequired()
                      .HasConversion<int>();


                entity.HasOne(c => c.Question)
                    .WithMany(q => q.Choix)
                    .HasForeignKey(c => c.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction);

            });

            modelBuilder.Entity<TentativeQcm>(entity =>
            {
                entity.Property(t => t.StatutTentative).HasMaxLength(50).IsRequired();
                entity.Property(t => t.DateTentative).IsRequired();

                entity.HasOne(t => t.User)
                    .WithMany()
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(t => t.Course)
                    .WithMany(c => c.TentativesQcm)
                    .HasForeignKey(t => t.CourseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ChoixSelectionne>(entity =>
            {
                entity.HasKey(cs => new { cs.TentativeQcmId, cs.ChoixId });

                entity.HasOne(cs => cs.TentativeQcm)
                    .WithMany(t => t.ChoixSelectionnes)
                    .HasForeignKey(cs => cs.TentativeQcmId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(cs => cs.Choix)
                    .WithMany(c => c.ChoixSelectionnes)
                    .HasForeignKey(cs => cs.ChoixId)
                    .OnDelete(DeleteBehavior.NoAction);
            });


            modelBuilder.Entity<ResultatQuestion>(entity =>
            {
                entity.Property(r => r.PointsObtenus).IsRequired();
                entity.Property(r => r.EstCorrect)
                      .IsRequired()
                      .HasConversion<int>();


                entity.HasIndex(r => new { r.TentativeQcmId, r.QuestionId }).IsUnique();

                entity.HasOne(r => r.TentativeQcm)
                    .WithMany(t => t.ResultatQuestions)
                    .HasForeignKey(r => r.TentativeQcmId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.Question)
                    .WithMany(q => q.ResultatQuestions)
                    .HasForeignKey(r => r.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction);

            });

            modelBuilder.Entity<QuestionCourse>(entity =>
            {
                entity.HasKey(qc => new { qc.CourseId, qc.QuestionId });

                entity.HasOne(qc => qc.Course)
                    .WithMany(c => c.CourseQuestions)
                    .HasForeignKey(qc => qc.CourseId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(qc => qc.Question)
                    .WithMany(q => q.QuestionCourses)
                    .HasForeignKey(qc => qc.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.Property(e => e.Statut).HasMaxLength(20).IsRequired();

                entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Course)
                    .WithMany()
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.DecisionProfessor)
                    .WithMany()
                    .HasForeignKey(e => e.DecisionProfessorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CourseProgress>(entity =>
            {
                entity.HasIndex(p => p.EnrollmentId).IsUnique();

                entity.HasOne(p => p.Enrollment)
                    .WithMany()
                    .HasForeignKey(p => p.EnrollmentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });




        }
    }
}
