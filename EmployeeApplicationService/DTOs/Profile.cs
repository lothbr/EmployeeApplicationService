using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplicationService.DTOs
{
    public class Profile
    {
        
        [Required]
        public string? ProfileId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string? Firstname { get; set; }
        [Required]
        public string? Lastname { get; set;}
        [Required]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Nationality { get; set; }
        public string? CurrentResidence { get; set; }
        [Required]
        public string? IDNumber { get; set; }
        [Required]
        public DateTime DateofBirth { get; set; }
        [Required]
        public string? Gender { get; set; }
       


    }
}
