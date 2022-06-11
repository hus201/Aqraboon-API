using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class UserRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [ForeignKey("request")]
        public int RequestId { get; set; }

        [Required]
        [ForeignKey("user")]
        public Guid UserId { get; set; }

        [Required]
        public double Value { get; set; }
        public bool IsVolunteer { get; set; }

        public virtual User user { get; set; }
        public virtual Request request { get; set; }
    }
}
