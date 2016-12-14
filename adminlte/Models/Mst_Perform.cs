using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Mst_Perform
    {
        [Key]
        public int PerformId { get; set; }

        [Required]
        public string PerformDesc { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [ForeignKey("Mst_RoleMap")]
        public int RoleMapId { get; set; }
        public Mst_RoleMap Mst_RoleMap { get; set; }
    }
}