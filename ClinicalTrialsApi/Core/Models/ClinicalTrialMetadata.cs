using System.ComponentModel.DataAnnotations;
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
