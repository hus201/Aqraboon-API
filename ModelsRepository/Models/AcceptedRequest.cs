using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ModelsRepository.Models
{
    public class AcceptedRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("request")]
        public int RequestId { get; set; }

        [Required]
        [ForeignKey("user")]
        public Guid VolunteerId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [JsonIgnore]
        public virtual Request request { get; set; }
        public virtual User user { get; set; }
    }
}
