﻿using aplabs_khoroshev.ActionFilters;
using aplabs_khoroshev.ModelBinders;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace aplabs_khoroshev.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) 
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges:false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }

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


        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = HttpContext.Items["company"] as Company;
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
            return NoContent();
        }



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

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody]
        IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
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
            var companyCollectionToReturn =
            _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids },
            companyCollectionToReturn);
        }
    }
}
