using SchoolProj.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models
{
    public class Teacher
    {
        [Key]
        public long Id { get; set; }

        [StringLength(50), Required]
        public string Name { get; set; }

        public Gender Gender { get; set; }

        public Department  Department { get; set; }

        [StringLength(15)]
        public string PhoneNo { get; set; }

        [StringLength(50), Required]
        public string Email { get; set; }

        [StringLength(500), DataType(DataType.MultilineText)]
        public string Address { get; set; }
    }
}