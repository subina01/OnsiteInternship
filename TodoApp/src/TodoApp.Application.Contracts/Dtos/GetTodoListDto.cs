using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace TodoApp.Dtos
{
    public class GetTodoListDto : PagedAndSortedResultRequestDto
    {
        public bool? IsCompleted { get; set; }
        public string Filter { get; set; } = string.Empty;
    }
}
