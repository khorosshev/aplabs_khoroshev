using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
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
    }
}
