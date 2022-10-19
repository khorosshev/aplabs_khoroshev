using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aplabs_khoroshev.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/{v:apiversion}/books")]
    [ApiController]
    public class BooksV2Controller : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        public BooksV2Controller(IRepositoryManager repository)
        {
            _repository = repository;
        }
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetBooks()
        {
            var books = await
           _repository.Book.GetAllBooksAsync(trackChanges:
            false);
            return Ok(books);
        }
    }
}
