using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PillMedTech.Models
{
    public class Logger
    {
        // teacher "Millan" solution to previous implementation poco "Sequence" => using System.ComponentModel.DataAnnotations;
        // Implement [Key] on Id attribute. This allows Id to auto increment when a new Logger is added to context. Annotation [Key] is essenstial and is a req for app to work.
        [Key]
        public int Id { get; set; }
        public string Time { get; set; }
        public string EmployeeId { get; set; }
        public string Ip { get; set; }
        public string Action { get; set; }

    }
}
