using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class FailedRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("request")]
        public int RequestId { get; set; }
        public string Reason { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public virtual Request request { get; set; }

    }
}
