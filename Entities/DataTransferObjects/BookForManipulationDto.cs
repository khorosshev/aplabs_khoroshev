using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public abstract class BookForManipulationDto
    {
        [Required(ErrorMessage = "Book name is a required field.")]
        [MaxLength(40, ErrorMessage = "Maximum length for the Name is 40 characters.")]
        public string Name { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public int Pages { get; set; }
    }
}
