using System;
using System.Collections.Generic;
using System.Text;

namespace PostgresTestApp
{
    public class Student
    {
        public int Id { get; set; }           // Primary Key
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
    }

}
