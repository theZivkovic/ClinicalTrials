using ClinicalTrialsApi.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialsApi
{
    public class ClinicalTrialsContext : DbContext
    {
        public ClinicalTrialsContext() { }

        public DbSet<ClinicalTrialMetadata> ClinicalTrialMetadatas { get; set; }
        public DbSet<ValidationSchema> ValidationSchemas { get; set; }

        public ClinicalTrialsContext(DbContextOptions<ClinicalTrialsContext> options)
       : base(options)
        { }
    }
}
