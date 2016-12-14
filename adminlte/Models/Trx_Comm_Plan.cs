using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Trx_Comm_Plan
    {
        [Key]
        public int TrxId { get; set; }
        [ForeignKey("Tbl_Task")]
        public int TaskId { get; set; }
        public int CommPlanid { get; set; }
        public string DescriptionPlan { get; set; }
        public string IsAchievable { get; set; }
        [ForeignKey("Mst_User")]
        public int SubmitBy { get; set; }
        public string Periode { get; set; }
        public string File { get; set; }
        public Tbl_Task Tbl_Task { get; set; }
        public Mst_User Mst_User { get; set; }
        public Mst_CommPlan Mst_CommPlan { get; set; }
    }
}