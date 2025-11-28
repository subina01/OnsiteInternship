using System;
using System.Collections.Generic;
using System.Text;

namespace sample_redistestapp.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
