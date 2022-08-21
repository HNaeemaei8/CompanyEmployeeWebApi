using AutoMapper;
using Contracts.IServices;
using Entities;
using Entities.DataTransferObjects;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Infrastructure.ActionFilters;
using WebApi.Infrastructure.ModelBinders;

namespace WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/Companies")]
    [ApiController]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    [ApiExplorerSettings(GroupName ="v1")]
    public class CompaniesController : Controller
    {

        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        public CompaniesController(ILoggerManager logger, IRepositoryManager repository , IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        //[HttpGet]
        //[ResponseCache(Duration = 60)]
        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns>The companies list</returns>
        [HttpGet(Name = "GetCompanies"), Authorize(Roles = "Manager")]

        public async Task<IActionResult> GetCompaniesAsync()
        {
            //try
            //{
                var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges:false);
                var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            //var companiesDto = companies.Select(c => new CompanyDto
            //{
            //    Id = c.Id,
            //    Name = c.Name,
            //    FullAddress = string.Join(' ', c.Address, c.Country)
            //}
            // ).ToList();
            //  return Ok(companiesDto);
            //throw new Exception("Exception");
            return Ok(companiesDto);

            //  }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(($"Something went wrong in the { nameof(GetCompanies)} action { ex}"));

            //            return StatusCode(500, "Internal Server Error");
            //}
        }

        [HttpGet("{Id}" , Name ="CompanyById")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompanyAsync(Guid id)
        {
            var company =await _repository.Company.GetCompanyAsync(id, trackchanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var companyDto = _mapper.Map<CompanyDto>(company);
            return Ok(companyDto);
        }


        /// <summary>
        /// Creates a newly created company
        /// </summary>
        /// <param name="company"></param>
        /// <returns>A newly created company</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompanyAsync([FromBody] CompanyForCreationDto company)
        {

            //if (company == null)
            //{
            //    _logger.LogError("companyForCreationDto object sent from client is null");

            //    return BadRequest("CompanyForCreationDto object is null");
            //}
            //if (!ModelState.IsValid)
            //{
            //    _logger.LogError("Invalid model state for the CompanyForCreationDto object");

            //    return UnprocessableEntity(ModelState);

            //}
            var CompanyEntity = _mapper.Map<Company>(company);

            _repository.Company.CreateCompany(CompanyEntity);
            await  _repository.SaveAsync();

            var CompanyToReturn = _mapper.Map<CompanyDto>(CompanyEntity);
            return CreatedAtRoute("CompanyById", new { Id = CompanyToReturn.Id }, CompanyToReturn);

        }

        [HttpGet("collection/({ids})" , Name = "CompanyCollection") ]
        public async Task<IActionResult> GetCompanyCollectionAsync([ModelBinder(BinderType =typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if (ids== null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities =await _repository.Company.GetByIdsAsync(ids, trackchanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();

            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollectionAsync([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection ==null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
           await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(" ", companyCollectionToReturn.Select(x => x.Id));
            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteCompanyAsync(Guid id)
        {
            //var company =await _repository.Company.GetCompanyAsync(id, trackchanges: false);

            //if (company==null)
            //{
            //    _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

            //     return NotFound();
            //}

            var company = HttpContext.Items["company"] as Company;
            _repository.Company.DeleteCompany(company);
           await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]

        public async Task<IActionResult> UpdateCompanyAsync(Guid id , [FromBody] CompanyForUpdateDto company)
        {
            //if (company==null)
            //{
            //    _logger.LogError("CompanyForUpdateDto object sent from client is null.");

            //     return BadRequest("CompanyForUpdateDto object is null");
            //}
            //if (! ModelState.IsValid)
            //{
            //    _logger.LogError("Invalid model state for the EmployeeForCreationDto object");

            //    return UnprocessableEntity(ModelState);
            //}
            ////var companyEntity = await _repository.Company.GetCompanyAsync(id, trackchanges: true);
            ////if (companyEntity == null)
            ////{
            ////_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
            ////  return NotFound();
            ////  }
            var companyEntity = HttpContext.Items["company"] as Company;
            _mapper.Map(company ,companyEntity);
           await _repository.SaveAsync();
            return NoContent();
        }
   }
}

