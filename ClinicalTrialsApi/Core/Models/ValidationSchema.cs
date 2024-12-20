using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ClinicalTrialsApi.Core.Models
{
    public enum ValidationSchemaType
    {
        ClinicalTrial
    }

    public class ValidationSchema
    {
        [Key]
        public ValidationSchemaType Type { get; set; }
        public JsonElement Schema { get; set; }
    }
}
