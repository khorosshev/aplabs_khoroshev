using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aplabs_khoroshev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public BooksController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = _repository.Book.GetAllBooks(trackChanges: false);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(booksDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetBook(Guid id)
        {
            var book = _repository.Book.GetBook(id, trackChanges: false);
            if (book == null)
            {
                _logger.LogInfo($"Book with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var bookDto = _mapper.Map<BookDto>(book);
                return Ok(bookDto);
            }
        }

    }
}
