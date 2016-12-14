using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Mst_CommPlan_Detail
    {
        [Key]
        public int CommPlanDetailId { get; set; }

        [Required]
        [ForeignKey("Mst_CommPlan")]
        public int CommPlanid { get; set; }

        [Required]
        public string Detail_Description { get; set; }

        [Required]
        [ForeignKey("Mst_Role")]
        public int RoleId { get; set; }
        public Mst_CommPlan Mst_CommPlan { get; set; }
        public Mst_Role Mst_Role { get; set; }
    }
}