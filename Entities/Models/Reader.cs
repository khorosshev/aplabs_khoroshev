using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Reader
    {
        
        [Column("ReaderId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Surname name is a required field.")]
        [MaxLength(40, ErrorMessage = "Maximum length for the surname of cinema is 40 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Name is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the name is 20 digits")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is a required field.")]
        [MaxLength(2, ErrorMessage = "Maximum length for the age is 3 digits")]
        public int Age { get; set; }
       
        [Required(ErrorMessage = "Phone is a required field.")]
        [MaxLength(12, ErrorMessage = "Maximum length for the phone is 12 digits")]
        public string Phone { get; set; }
    
}
}
