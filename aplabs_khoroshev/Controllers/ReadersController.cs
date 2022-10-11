using aplabs_khoroshev.ModelBinders;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aplabs_khoroshev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public IActionResult GetReaders()
        {
            var readers = _repository.Reader.GetAllReaders(trackChanges: false);
            var readersDto = _mapper.Map<IEnumerable<ReaderDto>>(readers);
            return Ok(readersDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetReader(Guid id)
        {
            var reader = _repository.Reader.GetReader(id, trackChanges: false);
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
        public IActionResult CreateReader([FromBody] ReaderForCreationDto reader)
        {
            if (reader == null)
            {
                _logger.LogError("ReaderForCreationDto object sent from client is null.");
            return BadRequest("ReaderForCreationDto object is null");
            }
            var readerEntity = _mapper.Map<Reader>(reader);
            _repository.Reader.CreateReader(readerEntity);
            _repository.Save();
            var readerToReturn = _mapper.Map<ReaderDto>(readerEntity);
            return CreatedAtRoute("ReaderById", new { id = readerToReturn.Id },
            readerToReturn);
        }

        [HttpGet("collection/({ids})", Name = "ReaderCollection")]
        public IActionResult GetCompanyCollection([ModelBinder(BinderType =
typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var readerEntities = _repository.Reader.GetByIds(ids, trackChanges: false);

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
        public IActionResult CreateReaderCollection([FromBody]
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
            _repository.Save();
            var readerCollectionToReturn =
            _mapper.Map<IEnumerable<ReaderDto>>(readerEntities);
            var ids = string.Join(",", readerCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("ReaderCollection", new { ids },
            readerCollectionToReturn);
        }
    }
}
