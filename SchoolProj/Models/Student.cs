using SchoolProj.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProj.Models
{
    public class Student
    {
        [Key]
        public long Id { get; set; }

        [StringLength(50),Required]
        public String Name { get; set; }

        public Gender Gender { get; set; }

        public String RollNo { get; set; }

        public long Age { get; set; }

        [StringLength(15)]
        public String PhoneNo { get; set; }

        [StringLength(50), Required]
        public String Email { get; set; }

        [StringLength(500), DataType(DataType.MultilineText)]
        public String Address { get; set; }


    }
}