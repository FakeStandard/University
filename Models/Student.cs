using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    public class Student
    {
        public int ID { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [Column("LastName")]
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Column("FirstName")]
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [DisplayName("Full Name")]
        public string FullName
        {
            get => $"{LastName}, {FirstName}";
        }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
