using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.Dtos;
using TodoApp.Todos;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;

namespace TodoApp
{
    public class TodoAppApplicationAutoMapperProfile : Profile
    {
        public TodoAppApplicationAutoMapperProfile()
        {
            CreateMap<Todo, TodoDto>();
            CreateMap<CreateUpdateTodoDto, Todo>().ReverseMap();
        }
    }
}
