using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DataShaping;

namespace aplabs_khoroshev.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/{v:apiversion}/employees")]
    [ApiController]
    public class EmployeesV2Controller : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IDataShaper<EmployeeDto> _dataShaper;
        public EmployeesV2Controller(IRepositoryManager repository, ILoggerManager
       logger, IMapper mapper, IDataShaper<EmployeeDto> dataShaper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _dataShaper = dataShaper;
        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
        [FromQuery] EmployeeParameters employeeParameters)
        {
            if (!employeeParameters.ValidAgeRange)
                return BadRequest("Max age can't be less than min age.");
            var company = await _repository.Company.GetCompanyAsync(companyId,
           trackChanges:
            false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeesFromDb = _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(employeesFromDb.MetaData));
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return Ok(_dataShaper.ShapeData(employeesDto, employeeParameters.Fields));
        }
    }
}
