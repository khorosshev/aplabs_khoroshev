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
using System.Data;

namespace aplabs_khoroshev.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize(Roles = "Administrator")]
    public class ReadersController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public ReadersController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetReaders()
        {
            var readers = await _repository.Reader.GetAllReadersAsync(trackChanges: false);
            var readersDto = _mapper.Map<IEnumerable<ReaderDto>>(readers);
            return Ok(readersDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReader(Guid id)
        {
            var reader = await _repository.Reader.GetReaderAsync(id, trackChanges: false);
            if (reader == null)
            {
                _logger.LogInfo($"Reader with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var readerDto = _mapper.Map<ReaderDto>(reader);
                return Ok(readerDto);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateReader([FromBody] ReaderForCreationDto reader)
        {
            var readerEntity = _mapper.Map<Reader>(reader);
            _repository.Reader.CreateReader(readerEntity);
            await _repository.SaveAsync();
            var readerToReturn = _mapper.Map<ReaderDto>(readerEntity);
            return CreatedAtRoute("ReaderById", new { id = readerToReturn.Id },
            readerToReturn);
        }

        [HttpGet("collection/({ids})", Name = "ReaderCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType =
        typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var readerEntities = await _repository.Reader.GetByIdsAsync(ids, trackChanges: false);

            if (ids.Count() != readerEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var readersToReturn =
           _mapper.Map<IEnumerable<ReaderDto>>(readerEntities);
            return Ok(readersToReturn);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateReaderCollection([FromBody]
        IEnumerable<ReaderForCreationDto> readerCollection)
        {
            if (readerCollection == null)
            {
                _logger.LogError("Reader collection sent from client is null.");
                return BadRequest("Reader collection is null");
            }
            var readerEntities = _mapper.Map<IEnumerable<Reader>>(readerCollection);
            foreach (var reader in readerEntities)
            {
                _repository.Reader.CreateReader(reader);
            }
            await _repository.SaveAsync();
            var readerCollectionToReturn =
            _mapper.Map<IEnumerable<ReaderDto>>(readerEntities);
            var ids = string.Join(",", readerCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("ReaderCollection", new { ids },
            readerCollectionToReturn);
        }
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateReaderExistsAttribute))]
        public async Task<IActionResult> DeleteReader(Guid id)
        {
            var reader = HttpContext.Items["reader"] as Reader;
            _repository.Reader.DeleteReader(reader);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateReaderExistsAttribute))]
        public async Task<IActionResult> UpdateReader(Guid id, [FromBody] ReaderForUpdateDto
        reader)
        {
            var readerEntity = HttpContext.Items["reader"] as Reader;
            _mapper.Map(reader, readerEntity);
           await _repository.SaveAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateReader(Guid id,
        [FromBody] JsonPatchDocument<ReaderForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            var readerEntity = await _repository.Reader.GetReaderAsync(id,
           trackChanges: true);
            if (readerEntity == null)
            {
                _logger.LogInfo($"Reader with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var readerToPatch = _mapper.Map<ReaderForUpdateDto>(readerEntity);
            patchDoc.ApplyTo(readerToPatch, ModelState);
            TryValidateModel(readerToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(readerToPatch, readerEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetReadersOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }
    }
}
