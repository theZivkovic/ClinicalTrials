using System.Text.Json;

namespace ClinicalTrialsApi.Application.Extensions
{
    public static class FormFileExtensions
    {
        public static async Task<JsonElement> ToJson(this IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var fileContent = await reader.ReadToEndAsync();
            return JsonDocument.Parse(fileContent).RootElement;
        }
    }
}
