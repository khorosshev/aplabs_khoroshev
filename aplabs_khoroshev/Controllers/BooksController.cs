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
    [ApiController, Authorize(Roles = "Manager")]
    [ApiExplorerSettings(GroupName = "v1")]
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

        /// <summary>
        /// Получает список всех книг
        /// </summary>
        /// <returns>Список книг</returns>.
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetBooksAsync()
        {
            var books = await _repository.Book.GetAllBooksAsync(trackChanges: false);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(booksDto);
        }

        /// <summary>
        /// Получает книгу по ID
        /// </summary>
        /// <returns>Книга</returns>.
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

        /// <summary>
        /// Создает вновь созданную книгу
        /// </summary>
        /// <param name="book"></param>.
        /// <returns>Вновь созданная книга</returns>.
        /// <response code="201"> Возвращает только что созданный элемент</response>.
        /// <response code="400"> Если элемент равен null</response>.
        /// <responce code="422"> Если модель недействительна</responce>.
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

        /// <summary>
        /// Получает коллекцию книг
        /// </summary>
        /// <returns>Список книга</returns>.
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

        /// <summary>
        /// Создает коллекцию книг
        /// </summary>
        /// <returns>Вновь созданная коллекция книг</returns>.
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

        /// <summary>
        /// Удаляет книгу
        /// </summary>
        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateBookExistsAttribute))]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var book = HttpContext.Items["book"] as Book;
            _repository.Book.DeleteBook(book);
            await _repository.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// Изменяет книгу (стирая старые свойства)
        /// </summary>
        /// <returns>Измененная книга</returns>.
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

        /// <summary>
        /// Изменяет книгу (сохраняя старые свойства)
        /// </summary>
        /// <returns>Измененная книга</returns>.
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

        /// <summary>
        /// Получает параметры для книги, не затрагивая сам ресурс
        /// </summary>
        /// <returns>Параметры книги</returns>.
        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }

    }
}
