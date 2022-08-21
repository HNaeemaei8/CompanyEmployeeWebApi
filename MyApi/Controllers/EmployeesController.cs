using AutoMapper;
using Contracts.IServices;
using Entities;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Infrastructure.ActionFilters;

namespace WebApi.Controllers
{
    [Route("api/companies/{CompanyId}/employees")]
    [ApiController]
    public class EmployeesController : Controller
    {

        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IDataShaper<EmployeeDto> _dataShaper;

        public EmployeesController(ILoggerManager logger, IRepositoryManager repository, IMapper mapper, IDataShaper<EmployeeDto> dataShaper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _dataShaper = dataShaper;
        }
        [HttpGet(Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeesForCompanyAsync(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackchanges: false);
            if (!employeeParameters.ValidAgeRange)
            {
                return BadRequest("Max age can't be less than min age.");
            }
            if (company == null)
            {
                _logger.LogInfo($"company with Id {companyId} dosent exist in the database");

                return NotFound();
            }
            //var emploeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackchanges: false);
            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            Response.Headers.Add("X-Pagination",
            JsonConvert.SerializeObject(employeesFromDb.MetaData));

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            //return Ok(emploeesDto);

            return Ok(_dataShaper.ShapeData(employeesDto,
            employeeParameters.Fields));
        }
        [HttpGet("{Id}")]
        //[HttpGet(Name= "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompanyAsync(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackchanges: false);
            if (company == null)
            {
                _logger.LogInfo($"company with Id {companyId} dosent exist in the database");

                return NotFound();
            }
            var employeeDb =await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if (employeeDb == null)
            {
                _logger.LogInfo($"employee with Id {id} dosent exist in the database");

                return NotFound();
            }
            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompanyAsync(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            if (employee == null)
            {
                _logger.LogError("EmployeeForCreationDto object sent from client is null.");
                return BadRequest("EmployeeForCreationDto object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EmployeeForCreationDto object");
                return UnprocessableEntity(ModelState);          
            }

            var company =await  _repository.Company.GetCompanyAsync(companyId, trackchanges:
            false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeEntity = _mapper.Map<Employee>(employee);
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
           await _repository.SaveAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return
            CreatedAtRoute("GetEmployeeForCompany",
            new
            {
                companyId,
                id = employeeToReturn.Id
            },
            employeeToReturn);
        }

        [HttpDelete("{id}")]

        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]

        public async Task< IActionResult> DeleteEmployeeForCompanyAsync(Guid companyId, Guid id)
        {
            //var company =await _repository.Company.GetCompanyAsync(companyId, trackchanges: false);
            //if (company == null)
            //{
            //    _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

            //    return NotFound();
            //}
            //var employeeForCompany =await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            //if (employeeForCompany == null)
            //{
            //    _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
            //    return NotFound();
            //}
             var employeeForCompany = HttpContext.Items["employee"] as Employee;
            _repository.Employee.DeleteEmployee(employeeForCompany);
          await  _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]

        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]

        public async Task<IActionResult> UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            //if (employee == null)
            //{
            //    _logger.LogError("EmployeeForUpdateDto object sent from client is null.");

            //    return BadRequest("EmployeeForUpdateDto object is null");
            //}
            //if (!ModelState.IsValid)
            //{
            //    _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");

            //    return UnprocessableEntity(ModelState);
            //}

            //var company =await _repository.Company.GetCompanyAsync(companyId, trackchanges: false);
            //if (company == null)
            //{
            //    _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
            //    return NotFound();
            //}
            //var employeeEntity =await _repository.Employee.GetEmployeeAsync(companyId, id,
            //trackChanges: true);
            //if (employeeEntity == null)
            //{
            //    _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
            //    return NotFound();
            //}
            var employeeEntity = HttpContext.Items["employee"] as Employee;
            _mapper.Map(employee,employeeEntity);

           await _repository.SaveAsync();
            return NoContent();

        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task< IActionResult> PartiallyUpdateEmployeeForCompanyAsync(Guid companyId ,Guid id , [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }
            //var company =await  _repository.Company.GetCompanyAsync(companyId, trackchanges: false);
            //if (company == null)
            //{
            //    _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
            //    return NotFound();
            //}
            //var employeeEntity =await  _repository.Employee.GetEmployeeAsync(companyId,id,trackChanges:true);
            //if (employeeEntity == null)
            //{
            //    _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
            //    return NotFound();
            //}
            var employeeEntity = HttpContext.Items["employee"] as Employee;
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            patchDoc.ApplyTo(employeeToPatch , ModelState);
            TryValidateModel(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");

                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(employeeToPatch,employeeEntity);
          await  _repository.SaveAsync();
            return NoContent();
        }
    }
}

