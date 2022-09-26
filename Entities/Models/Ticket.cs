using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Ticket
    {
        
        [Column("TicketId")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Cinema name is a required field.")]
        [MaxLength(40, ErrorMessage = "Maximum length for the name of cinema is 40 characters.")]
        public string Cinema { get; set; }
        [Required(ErrorMessage = "Film is a required field.")]
        [MaxLength(40, ErrorMessage = "Maximum length for the film is 40 digits")]
        public string Film { get; set; }
        [Required(ErrorMessage = "year is a required field.")]
        [MaxLength(3, ErrorMessage = "Maximum length for the year is 3 digits")]
        public int Year { get; set; }
        [Required(ErrorMessage = "sit is a required field.")]
        [MaxLength(2, ErrorMessage = "Maximum length for the sit is 2 digits")]
        public int Sit { get; set; }
    
}
}
