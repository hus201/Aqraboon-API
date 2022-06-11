using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [ForeignKey("request")]
        public int? RequestId { get; set; }
        [ForeignKey("service")]
        public int? ServiceId { get; set; }
        [Required]
        [ForeignKey("_user")]
        public Guid UserId { get; set; }
        [Required]
        [ForeignKey("user_reported")]
        public Guid UserReportedId { get; set; }

        public virtual User _user { get; set; }
        public virtual Service service { get; set; }
        public virtual User user_reported { get; set; }
        public virtual Request request { get; set; }
    }
}
