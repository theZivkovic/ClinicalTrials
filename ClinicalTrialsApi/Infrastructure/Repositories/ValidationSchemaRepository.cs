using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialsApi.Infrastructure.Repositories
{
    public class ValidationSchemaRepository(ClinicalTrialsContext context) : IValidationSchemaRepository
    {
        public async Task<Option<ValidationSchema>> Get(ValidationSchemaType type)
        {
            var result = await context.Set<ValidationSchema>().FirstOrDefaultAsync(x => x.Type == type);
            return result == null
                ? Option<ValidationSchema>.None
                : Option<ValidationSchema>.Some(result);
        }
    }
}
