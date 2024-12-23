using ClinicalTrialsApi.Application.Extensions;
using ClinicalTrialsApi.Application.Factories;
using ClinicalTrialsApi.Application.Services;
using ClinicalTrialsApi.Core.DTOs;
using ClinicalTrialsApi.Core.Models;
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
        // GET: api/<ClinicalTrialMetadata>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string[] trialId, [FromQuery] ClinicalTrialStatus[] status)
        {
            var filter = new ClinicalTrialMetadataFilter()
            {
                TrialIds = trialId,
                Statuses = status
            };

            var pagination = Pagination.ParseFromRequest(Request.Headers);

            return await pagination.MatchAsync(
                async p => {
                    Response.Headers.Append("x-pagination-limit", p.Limit.ToString());
                    Response.Headers.Append("x-pagination-offset", p.Offset.ToString());
                    return (await clinicalTrialMetadataService.GetAllTrials(filter, p)).ToResponse();
                },
                () => Task.FromResult(ServiceResultFactory
                    .CreateBadRequest<IEnumerable<ClinicalTrialMetadata>>("Pagination is not valid. Specify it using headers: x-pagination-offset (any whole number) and x-pagination-limit (whole number from 0 to 50)").ToResponse()));

        }

        // GET api/<ClinicalTrialMetadata>/trial123
        [HttpGet("{trialId}")]
        public async Task<IActionResult> Get(string trialId)
        {
            return (await clinicalTrialMetadataService.GetATrial(trialId)).ToResponse();
        }

        [HttpPut]
        public async Task<IActionResult> Put(IFormFile file)
        {
            return (await clinicalTrialMetadataService.CreateOrUpdateATrial(file)).ToResponse();
        }
    }
}
