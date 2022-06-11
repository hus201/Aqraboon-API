using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsRepository.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(60)]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; }
        public virtual string ConfirmPassword { get; set; }

        public string Role { get; set; }

        [Required, DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime BirthDate { get; set; }
        public int Gender { get; set; }
        [Required]
        public double Lat { get; set; }
        [Required]
        public double Lng { get; set; }

        public virtual VolunteerInfo volenteerInfo { get; set; }
    }
}
