using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aplabs_khoroshev.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/{v:apiversion}/readers")]
    [ApiController]
    public class ReadersV2Controller : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        public ReadersV2Controller(IRepositoryManager repository)
        {
            _repository = repository;
        }
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetReaders()
        {
            var readers = await
           _repository.Reader.GetAllReadersAsync(trackChanges:
            false);
            return Ok(readers);
        }
    }
}
