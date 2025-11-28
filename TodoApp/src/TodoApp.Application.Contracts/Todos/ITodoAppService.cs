using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TodoApp.Todos
{
    public interface ITodoAppService : IApplicationService
    {
        Task<PagedResultDto<TodoDto>> GetListAsync(GetTodoListDto input);
        Task<TodoDto> GetAsync(Guid id);
        Task<TodoDto> CreateAsync(CreateUpdateTodoDto input);
        Task<TodoDto> UpdateAsync(Guid id, CreateUpdateTodoDto input);
        Task DeleteAsync(Guid id);
        Task<TodoDto> CompleteAsync(Guid id);
        Task<TodoDto> ReopenAsync(Guid id);
    }
}
