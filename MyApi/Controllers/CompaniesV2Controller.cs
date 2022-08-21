using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiVersion("2.0" , Deprecated =true)]
     //[Route("api/{v:apiversion}/companies")]
    [Route("api/companies")]

    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]

    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        public CompaniesV2Controller(IRepositoryManager repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompaniesAsync()
        {
            var companies = await
            _repository.Company.GetAllCompaniesAsync(trackChanges:
             false);
            return Ok(companies);
        }
    }

}
