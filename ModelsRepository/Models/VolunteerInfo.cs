using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class VolunteerInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("user")]
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }

        public virtual List<Service> Services { get; set; }
        public virtual User user { get; set; }
    }
}
