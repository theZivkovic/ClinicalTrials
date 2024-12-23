using ClinicalTrialsApi.Application.Extensions;
using ClinicalTrialsApi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicalTrialsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicalTrialMetadataController(
        IClinicalTrialMetadataService clinicalTrialMetadataService) : ControllerBase
    {
        // GET: api/<ClinicalTrialMetadataController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ClinicalTrialMetadataController>/trial123
        [HttpGet("{trialId}")]
        public async Task<IActionResult> Get(string trialId)
        {
            return (await clinicalTrialMetadataService.GetATrial(trialId)).ToResponse();
        }

        // PUT api/<ClinicalTrialMetadataController>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] JsonElement value)
        {
            return (await clinicalTrialMetadataService.CreateOrUpdateATrial(value)).ToResponse();
        }
    }
}
