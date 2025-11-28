using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Dtos;
using TodoApp.Todos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace TodoApp.Controllers
{
    [Route("api/todos")]
    [ApiController]
    public class TodoController : AbpController
    {
        private readonly ITodoAppService _todoAppService;

        public TodoController(ITodoAppService todoAppService)
        {
            _todoAppService = todoAppService;
        }

        // GET: api/todos
        [HttpGet]
        public async Task<PagedResultDto<TodoDto>> GetListAsync([FromQuery] GetTodoListDto input)
        {
            return await _todoAppService.GetListAsync(input);
        }

        // GET: api/todos/{id}
        [HttpGet("{id}")]
        public async Task<TodoDto> GetAsync(Guid id)
        {
            return await _todoAppService.GetAsync(id);
        }

        // POST: api/todos
        [HttpPost]
        public async Task<TodoDto> CreateAsync([FromBody] CreateUpdateTodoDto input)
        {
            return await _todoAppService.CreateAsync(input);
        }

        // PUT: api/todos/{id}
        [HttpPut("{id}")]
        public async Task<TodoDto> UpdateAsync(Guid id, [FromBody] CreateUpdateTodoDto input)
        {
            return await _todoAppService.UpdateAsync(id, input);
        }

        // DELETE: api/todos/{id}
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _todoAppService.DeleteAsync(id);
        }

        // POST: api/todos/{id}/complete
        [HttpPost("{id}/complete")]
        public async Task<TodoDto> CompleteAsync(Guid id)
        {
            return await _todoAppService.CompleteAsync(id);
        }

        // POST: api/todos/{id}/reopen
        [HttpPost("{id}/reopen")]
        public async Task<TodoDto> ReopenAsync(Guid id)
        {
            return await _todoAppService.ReopenAsync(id);
        }
    }
}
