using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Mst_Perform_Detail
    {
        [Key]
        public int PerformDetailId { get; set; }

        [Required]
        [ForeignKey("Mst_Perform")]
        public int PerformId { get; set; }

        [Required]
        public string Detail_Description { get; set; }

        [Required]
        public int Weight { get; set; }

        public Mst_Perform Mst_Perform { get; set; }

    }
}