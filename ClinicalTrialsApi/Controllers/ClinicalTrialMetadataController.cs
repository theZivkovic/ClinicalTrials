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

        // GET api/<ClinicalTrialMetadataController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ClinicalTrialMetadataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ClinicalTrialMetadataController>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] JsonElement value)
        {
            var result = await clinicalTrialMetadataService.Create(value);
            if (result.IsError)
            {
                return BadRequest(result.Error);
            }
            else
            {
                return Ok(result.Entity);
            }
        }

        // DELETE api/<ClinicalTrialMetadataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
