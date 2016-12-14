using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Trx_Perform_Review
    {
        [Key]
        public int TrxId { get; set; }

        [ForeignKey("Mst_Perform_Detail")]
        public int PerformDetailId { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public int ReviewBy { get; set; }

        [Required]
        public int ReviewFor { get; set; }

        [Required]
        public string Periode { get; set; }

        public string Notes { get; set; }

        public string FileName { get; set; }

        public Mst_Perform_Detail Mst_Perform_Detail { get; set; }
    }
}