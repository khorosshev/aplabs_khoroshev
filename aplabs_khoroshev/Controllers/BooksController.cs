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

namespace aplabs_khoroshev.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize(Roles = "Administrator")]
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
        [HttpHead]
        public async Task<IActionResult> GetBooksAsync()
        {
            var books = await _repository.Book.GetAllBooksAsync(trackChanges: false);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(booksDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookAsync(Guid id)
        {
            var book = await _repository.Book.GetBookAsync(id, trackChanges: false);
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
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto book)
        {
            var bookEntity = _mapper.Map<Book>(book);
            _repository.Book.CreateBook(bookEntity);
            await _repository.SaveAsync();
            var bookToReturn = _mapper.Map<BookDto>(bookEntity);
            return CreatedAtRoute("BookById", new { id = bookToReturn.Id },
            bookToReturn);
        }

        [HttpGet("collection/({ids})", Name = "BookCollection")]
        public async Task<IActionResult> GetBookCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var bookEntities = await _repository.Book.GetByIdsAsync(ids, trackChanges: false);

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
        public async Task<IActionResult> CreateBookCollection([FromBody]
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
            await _repository.SaveAsync();
            var bookCollectionToReturn =
            _mapper.Map<IEnumerable<BookDto>>(bookEntities);
            var ids = string.Join(",", bookCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("BookCollection", new { ids },
            bookCollectionToReturn);
        }
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateBookExistsAttribute))]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var book = HttpContext.Items["book"] as Book;
            _repository.Book.DeleteBook(book);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateBookExistsAttribute))]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookForUpdateDto
        book)
        {
            var bookEntity = HttpContext.Items["book"] as Book;
            _mapper.Map(book, bookEntity);
            await _repository.SaveAsync();
            return NoContent();
        }
        
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateBook(Guid id,
        [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            var bookEntity = await _repository.Book.GetBookAsync(id,
           trackChanges: true);
            if (bookEntity == null)
            {
                _logger.LogInfo($"Book with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var bookToPatch = _mapper.Map<BookForUpdateDto>(bookEntity);
            patchDoc.ApplyTo(bookToPatch, ModelState);
            TryValidateModel(bookToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(bookToPatch, bookEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }

    }
}
