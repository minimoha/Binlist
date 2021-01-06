using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace bin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeanController : ControllerBase
    {
        private readonly ILogger<BeanController> _logger;

        public BeanController(ILogger<BeanController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{nin}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, NoStore = false)]
        [Authorize]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetByIdAync(int nin)
        {
            try
            {
                if (nin.ToString().Length != 6 && nin.ToString().Length != 8)
                {
                    return BadRequest("You are to enter 6 or 8 digits of your payment card number");
                }
                var client = new RestClient($"https://lookup.binlist.net/{nin}");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await client.ExecuteAsync(request);


                var data = response.Content;

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("An error occured");
            }
            
        }
    }
}
