using System;
using System.Collections.Generic;
using System.Text;

namespace TodoApp.Dtos
{
    public class CreateUpdateTodoDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
