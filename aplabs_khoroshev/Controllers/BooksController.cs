using aplabs_khoroshev.ModelBinders;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpPost]
        public IActionResult CreateBook([FromBody] BookForCreationDto book)
        {
            if (book == null)
            {
                _logger.LogError("BookForCreationDto object sent from client is null.");
            return BadRequest("BookForCreationDto object is null");
            }
            var bookEntity = _mapper.Map<Book>(book);
            _repository.Book.CreateBook(bookEntity);
            _repository.Save();
            var bookToReturn = _mapper.Map<BookDto>(bookEntity);
            return CreatedAtRoute("BookById", new { id = bookToReturn.Id },
            bookToReturn);
        }

        [HttpGet("collection/({ids})", Name = "BookCollection")]
        public IActionResult GetBookCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var bookEntities = _repository.Book.GetByIds(ids, trackChanges: false);

            if (ids.Count() != bookEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var booksToReturn =
           _mapper.Map<IEnumerable<BookDto>>(bookEntities);
            return Ok(booksToReturn);
        }

        [HttpPost("collection")]
        public IActionResult CreateBookCollection([FromBody]
IEnumerable<BookForCreationDto> bookCollection)
        {
            if (bookCollection == null)
            {
                _logger.LogError("Book collection sent from client is null.");
                return BadRequest("Book collection is null");
            }
            var bookEntities = _mapper.Map<IEnumerable<Book>>(bookCollection);
            foreach (var book in bookEntities)
            {
                _repository.Book.CreateBook(book);
            }
            _repository.Save();
            var bookCollectionToReturn =
            _mapper.Map<IEnumerable<BookDto>>(bookEntities);
            var ids = string.Join(",", bookCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("BookCollection", new { ids },
            bookCollectionToReturn);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(Guid id)
        {
            var book = _repository.Book.GetBook(id, trackChanges: false);
            if (book == null)
            {
                _logger.LogInfo($"Book with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.Book.DeleteBook(book);
            _repository.Save();
            return NoContent();
        }
        [HttpPut("{id}")]
        public IActionResult UpdateBook(Guid id, [FromBody] BookForUpdateDto
        book)
        {
            if (book == null)
            {
            _logger.LogError("BookForUpdateDto object sent from client is null.");
                return BadRequest("BookForUpdateDto object is null");
            }
            var bookEntity = _repository.Book.GetBook(id, trackChanges: true);
            if (bookEntity == null)
            {
                _logger.LogInfo($"Book with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(book, bookEntity);
            _repository.Save();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBook(Guid id,
[FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            var bookEntity = _repository.Book.GetBook(id,
           trackChanges: true);
            if (bookEntity == null)
            {
                _logger.LogInfo($"Book with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var bookToPatch = _mapper.Map<BookForUpdateDto>(bookEntity);
            patchDoc.ApplyTo(bookToPatch);

            _mapper.Map(bookToPatch, bookEntity);
            _repository.Save();
            return NoContent();
        }

    }
}
