using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdapativeCardExperiments.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace AdapativeCardExperiments.Controllers
{
    [ApiController]
    public class BotApiController : ControllerBase
    {
        private ITokenAcquisition _tokenAcquisition;
        private KycRepository _kycRepository;

        public BotApiController(ITokenAcquisition tokenAcquisition, KycRepository kycRepository)
        {
            _tokenAcquisition = tokenAcquisition;
            _kycRepository = kycRepository;
        }

        [HttpGet("api/v1/health")]
        public async Task<string> Health()
        {
            //var result = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "User.Read" }, HttpContext.User.GetTenantId());
            return await Task.FromResult("Health Check Ok");
        }

        [HttpPost("/api/v1/kycrecord")]
        public async Task<KycRecord> AddKycRecord(KycRecord record)
        {
            record.Id = Guid.NewGuid().ToString("D");
            record.version = DateTime.UtcNow.Ticks;
            //todo: save it somewhere better
            _kycRepository.Add(record);
            return await Task.FromResult(record);
        }

        [HttpGet("/api/v1/kycrecords")]
        public async Task<IEnumerable<KycRecord>> GetKycRecords()
        {
            return await Task.FromResult(_kycRepository.GetRecords(10));
        }
    }
}
