using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace AdapativeCardExperiments.Controllers
{
    [ApiController]
    [Authorize]
    public class BotApiController : ControllerBase
    {
        private ITokenAcquisition _tokenAcquisition;

        public BotApiController(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        [HttpGet("api/v1/health")]
        public async Task<string> Health()
        {
            var result = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "User.Read" }, HttpContext.User.GetTenantId());
            return await Task.FromResult("Health Check Ok");
        }
    }
}
