using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(int.MaxValue)]
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        public double Lattiud { get; set; }
        public double Longtiud { get; set; }
        [Required]
        [ForeignKey("user")]
        public Guid SenderId { get; set; }
        [Required]
        [ForeignKey("seviceType")]
        public int SeviceTypeId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ExpireTime { get; set; }
        public int status { get; set; } // 0 pendding ,  1 accepted , 2 rejected , 3 delevired

        /* Patient */
        public string PName { get; set; }
        public int PAge { get; set; }
        public int ServiceId { get; set; }
        public string PDescription { get; set; }
        public int PGender { get; set; }
        /* End ,Patient */
        /* Volunteer */
        public int VGender { get; set; }
        /* End ,Volunteer */
        public AcceptedRequest AcceptedInfo { get; set; }
        public FailedRequest FailedInfo { get; set; }
        public DeliveredRequest DeliveredInfo { get; set; }

        public List<Report> Reports { get; set; }

        public virtual User user { get; set; }
        public virtual ServiceType seviceType { get; set; }

    }
}
