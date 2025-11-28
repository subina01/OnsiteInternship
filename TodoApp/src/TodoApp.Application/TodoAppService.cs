using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Dtos;
using TodoApp.Todos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using System.Linq;

namespace TodoApp
{
    public class TodoAppService : ApplicationService, ITodoAppService
    {
        private readonly IRepository<Todo, Guid> _todoRepository;

        public TodoAppService(IRepository<Todo, Guid> todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task<PagedResultDto<TodoDto>> GetListAsync(GetTodoListDto input)
        {
            var queryable = await _todoRepository.GetQueryableAsync();

            // Apply filters
            if (input.IsCompleted.HasValue)
            {
                queryable = queryable.Where(x => x.IsCompleted == input.IsCompleted.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                queryable = queryable.Where(x =>
                    x.Title.Contains(input.Filter) ||
                    x.Description.Contains(input.Filter));
            }

            // Get total count
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            // Apply sorting
            queryable = queryable
                .OrderBy(x => x.IsCompleted)
                .ThenByDescending(x => x.CreationTime);

            // Apply paging
            queryable = queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var todos = await AsyncExecuter.ToListAsync(queryable);

            return new PagedResultDto<TodoDto>(
                totalCount,
                ObjectMapper.Map<System.Collections.Generic.List<Todo>, System.Collections.Generic.List<TodoDto>>(todos)
            );
        }

        public async Task<TodoDto> GetAsync(Guid id)
        {
            var todo = await _todoRepository.GetAsync(id);
            return ObjectMapper.Map<Todo, TodoDto>(todo);
        }

        public async Task<TodoDto> CreateAsync(CreateUpdateTodoDto input)
        {
            var todo = new Todo(
                GuidGenerator.Create(),
                input.Title,
                input.Description,
                input.DueDate
            );

            await _todoRepository.InsertAsync(todo);
            return ObjectMapper.Map<Todo, TodoDto>(todo);
        }

        public async Task<TodoDto> UpdateAsync(Guid id, CreateUpdateTodoDto input)
        {
            var todo = await _todoRepository.GetAsync(id);

            todo.UpdateTitle(input.Title);
            todo.UpdateDescription(input.Description);
            todo.UpdateDueDate(input.DueDate);

            await _todoRepository.UpdateAsync(todo);
            return ObjectMapper.Map<Todo, TodoDto>(todo);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _todoRepository.DeleteAsync(id);
        }

        public async Task<TodoDto> CompleteAsync(Guid id)
        {
            var todo = await _todoRepository.GetAsync(id);
            todo.Complete();
            await _todoRepository.UpdateAsync(todo);
            return ObjectMapper.Map<Todo, TodoDto>(todo);
        }

        public async Task<TodoDto> ReopenAsync(Guid id)
        {
            var todo = await _todoRepository.GetAsync(id);
            todo.Reopen();
            await _todoRepository.UpdateAsync(todo);
            return ObjectMapper.Map<Todo, TodoDto>(todo);
        }
    }
}
