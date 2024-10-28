using System.Collections.Generic;

namespace Qltt.Models
{
    public class Teacher : User
    {
        public List<Class> Classes { get; set; }

        public string FullName => $"{LastName} {FirstName}";
    }
}
