using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PillMedTech.Models
{
    public class Logger
    {
        [Key]
        public int Id { get; set; }
        public string Time { get; set; }
        public string EmployeeId { get; set; }
        public string Ip { get; set; }
        public string Action { get; set; }

    }
}
