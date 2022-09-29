using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Book
    {
        [Column("BookId")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Book name is a required field.")]
        [MaxLength(40, ErrorMessage = "Maximum length for the name of book name is 40 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Author is a required field.")]
        [MaxLength(40, ErrorMessage = "Maximum length for the author is 40 digits")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Year is a required field.")]
        [MaxLength(4, ErrorMessage = "Maximum length for the year is 4 digits")]
        public int Year { get; set; }
        [Required(ErrorMessage = "Pages is a required field.")]
        [MaxLength(100, ErrorMessage = "Maximum length for the pages is 4 digits")]
        public int Pages { get; set; }
    }
}
