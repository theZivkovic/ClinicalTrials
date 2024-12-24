using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ClinicalTrialsApi.Core.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter<ClinicalTrialStatus>))]
    public enum ClinicalTrialStatus
    {
        [EnumMember(Value = "Not Started")]
        NotStarted,
        [EnumMember(Value = "Ongoing")]
        Ongoing,
        [EnumMember(Value = "Completed")]
        Completed
    }

    public class ClinicalTrialMetadata
    {
        [Required]
        [Key]
        [MaxLength(50)]
        public string TrialId { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Participants { get; set; }
        [Required]
        public ClinicalTrialStatus Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int DurationInDays
        {
            get {
                if (EndDate == null) throw new NullReferenceException(" EndDate must be set in order calculated DurationInDays,");
                return (int)(EndDate.Value - StartDate).TotalDays; }
            private set {  }
        }

        public void AdjustEndDate()
        {
            if (EndDate == null)
            {
                EndDate = StartDate.AddMonths(1);
                Status = ClinicalTrialStatus.Ongoing;
            }
        }

    }
}
