using ClinicalTrialsApi.Core.Models;
using LanguageExt;

namespace ClinicalTrialsApi.Core.Interfaces
{
    public interface IValidationSchemaRepository
    {
        public Task<Option<ValidationSchema>> Get(ValidationSchemaType type);
    }
}
