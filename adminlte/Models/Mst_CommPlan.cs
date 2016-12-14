using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace adminlte.Models
{
    public class Mst_CommPlan
    {
        [Key]
        public int CommPlanid { get; set; }

        [Required]
        public string CommPlanDesc { get; set; }

        [Required]
        public bool isActive { get; set; }
    }
}