using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class ServiceAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("service")]
        public int ServiceId { get; set; }
        public string Attachment { get; set; }

        [JsonIgnore]
        public virtual Service service { get; set; }
    }
}
