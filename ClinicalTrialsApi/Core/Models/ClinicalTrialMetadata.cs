using System.ComponentModel.DataAnnotations;

namespace ClinicalTrialsApi.Core.Models
{
    public enum ClinicalTrialStatus
    {
        NotStarted,
        Ongoing,
        Completed
    }

    public class ClinicalTrialMetadata
    {
        [Required]
        [Key]
        public string TrialId { get; set; } = null!;
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Participants { get; set; }
        [Required]
        public ClinicalTrialStatus Status { get; set; }

    }
}
