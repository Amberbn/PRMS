using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Mst_User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Mst_Role")]
        public int RoleId { get; set; }
        [Required]
        [ForeignKey("Mst_RoleMap")]
        public int RoleMapId { get; set; }
        [Required]
        [MaxLength(50)]
   
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string StatusKaryawan { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartEmploye { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool CommPlanOpen { get; set; }

        [Required]
        public bool PerfReviewOpen { get; set; }
        public int? DirectReportId { get; set; }

        //[ForeignKey("DirectReportId")]
        //public virtual Mst_User DirectReport { get; set; }
        public Mst_Role Mst_Role { get; set; }

        public Mst_RoleMap Mst_RoleMap { get; set; }
    }
}