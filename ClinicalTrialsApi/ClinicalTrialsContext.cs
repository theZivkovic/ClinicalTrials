using ClinicalTrialsApi.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialsApi
{
    public class ClinicalTrialsContext : DbContext
    {
        public DbSet<ClinicalTrialMetadata> ClinicalTrialMetadatas { get; set; }

        public ClinicalTrialsContext(DbContextOptions<ClinicalTrialsContext> options)
       : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
