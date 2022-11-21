using aplabs_khoroshev.ActionFilters;
using aplabs_khoroshev.ModelBinders;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;

namespace aplabs_khoroshev.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiController, Authorize(Roles = "Manager")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class CompaniesController : ControllerBase
    {
        //auth
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) 
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Получает компанию по ID
        /// </summary>
        /// <returns>Компания</returns>.
        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
        }

        /// <summary>
        /// Получает все компании
        /// </summary>
        /// <returns>Список компаний</returns>.
        [HttpGet(Name = "GetCompanies")]
        /// <summary>
        /// Проверяет существование компаний
        /// </summary>
        /// /// <returns>Флаг существования компаний</returns>.
        [HttpHead]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges:false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }

        /// <summary>
        /// Создает вновь созданную компанию
        /// </summary>
        /// <param name="company"></param>.
        /// <returns>Вновь созданная компания</returns>.
        /// <response code="201"> Возвращает только что созданный элемент</response>.
        /// <response code="400"> Если элемент равен null</response>.
        /// <responce code="422"> Если модель недействительна</responce>.
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();
            
        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id },
            companyToReturn);
        }

        /// <summary>
        /// Удаляет компанию по ID
        /// </summary>
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = HttpContext.Items["company"] as Company;
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// Изменяет компанию по ID (очищая старые свойства)
        /// </summary>
        /// <returns> Измененная компания</returns>.
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            var companyEntity = HttpContext.Items["company"] as Company;
            _mapper.Map(company, companyEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// Получает список компаний
        /// </summary>
        /// <returns>Список компаний</returns>.
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType =
        typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges: false);
            
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn =
           _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        /// <summary>
        /// Создает список компаний
        /// </summary>
        /// <returns>Созданный список компаний</returns>.
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody]
        IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the CompanyForCreationDto object");
                return UnprocessableEntity(ModelState);
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            await _repository.SaveAsync();
            var companyCollectionToReturn =
            _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids },
            companyCollectionToReturn);
        }

        /// <summary>
        /// Изменяет компанию по ID (сохраняя неизмененные свойства)
        /// </summary>
        /// <returns> Измененная компания</returns>.
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateCompany(Guid id,
        [FromBody] JsonPatchDocument<CompanyForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }
           
            var companyEntity = _repository.Company.GetCompanyAsync(id,
           trackChanges:true);
            if (companyEntity == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
            return NotFound();
            }
            var companyToPatch = _mapper.Map<CompanyForUpdateDto>(companyEntity);
            patchDoc.ApplyTo(companyToPatch, ModelState);
            TryValidateModel(companyToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(companyToPatch, companyEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// Получает параметры для компании, не затрагивая сам ресурс
        /// </summary>
        /// <returns>Параметры компании</returns>.
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }
    }
}
