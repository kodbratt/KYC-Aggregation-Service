using KYC_Aggregation_Service.Models;
using KYC_Aggregation_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KYC_Aggregation_Service.Controllers
{
    [ApiController]
    [Route("kyc-data")]
    public sealed class KycDataController(
     IKycAggregationService aggregationService) : ControllerBase
    {
        [HttpGet("{ssn}")]
        [ProducesResponseType<AggregatedKycData>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<AggregatedKycData>> GetBySsn(
            string ssn,
            CancellationToken cancellationToken)
        {
            var result = await aggregationService.GetBySsnAsync(
                ssn,
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    error = "Customer data not found for the provided SSN."
                });
            }

            return Ok(result);
        }
    }
}
