using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class RequestReceivers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("user")]
        public Guid UserId { get; set; }
        [Required]
        [ForeignKey("request")]
        public int RequestId { get; set; }

        public double distance { get; set; }

        public virtual User user { get; set; }
        public virtual Request request { get; set; }
    }
}
