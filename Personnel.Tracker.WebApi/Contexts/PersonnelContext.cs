using Microsoft.EntityFrameworkCore;
using Personnel.Tracker.Model.Auth;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Contexts
{
    public class PersonnelContext : DbContext
    {
        public PersonnelContext()
        {
        }

        public PersonnelContext(DbContextOptions<PersonnelContext> options)
         : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            PrepareInitialValues(modelBuilder); 
        }


        // Db Sets
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Model.Personnel.Personnel> Personnels { get; set; }
        public DbSet<Model.Personnel.PersonnelCheck> PersonnelChecks { get; set; }


        #region SaveChanges overrides 
        /// <summary>
        /// Overrides DbContext SaveChanges method to inject CreationTime and UpdateTime
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                var modificationTimeProp = entityEntry.Properties.FirstOrDefault(x => x.Metadata.Name == "UpdateTime");
                if (modificationTimeProp != null)
                {
                    if (entityEntry.State != EntityState.Added)
                    {
                        modificationTimeProp.CurrentValue = DateTime.Now;
                    }
                }

                if (entityEntry.State == EntityState.Added)
                {
                    var createTimeProp = entityEntry.Properties.FirstOrDefault(x => x.Metadata.Name == "CreationTime");
                    if (createTimeProp != null)
                    {
                        createTimeProp.CurrentValue = DateTime.Now;

                    }
                }
            }

            return base.SaveChanges();
        }

        /// <summary>
        /// Overrides DbContext SaveChangesAsync method to inject CreationTime and UpdateTime
        /// </summary>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            var entries = ChangeTracker
            .Entries()
            .Where(e =>
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {

                var modificationTimeProp = entityEntry.Properties.FirstOrDefault(x => x.Metadata.Name == "UpdateTime");

                if (modificationTimeProp != null)
                {
                    if (entityEntry.State != EntityState.Added)
                    {
                        modificationTimeProp.CurrentValue = DateTime.Now;
                    }
                }

                if (entityEntry.State == EntityState.Added)
                {
                    var createTimeProp = entityEntry.Properties.FirstOrDefault(x => x.Metadata.Name == "CreationTime");
                    if (createTimeProp != null)
                    {
                        createTimeProp.CurrentValue = DateTime.Now;

                    }
                }  
            } 

            return await base.SaveChangesAsync(cancellationToken);
        }
        #endregion 

        /// <summary>
        /// Prepare  initial values to be saved to database
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void PrepareInitialValues(ModelBuilder modelBuilder)
        { 
        }
    }
}
