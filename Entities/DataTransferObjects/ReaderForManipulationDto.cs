using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public abstract class ReaderForManipulationDto
    {
        [Required(ErrorMessage = "Reader surname is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the surname is 30 characters.")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Reader name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string Name { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }
    }
}
