using ClinicalTrialsApi.Core.Models;

namespace ClinicalTrialsApi.Core.DTOs
{
    public class ClinicalTrialMetadataFilter
    {
        public IEnumerable<string> TrialIds { get; set; } = [];
        public IEnumerable<string> Titles { get; set; } = [];
        public IEnumerable<int> Participants { get; set; } = [];
        public IEnumerable<ClinicalTrialStatus> Statuses { get; set; } = [];
    }
}
