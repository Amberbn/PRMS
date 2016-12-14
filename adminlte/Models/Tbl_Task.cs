using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Tbl_Task
    {
        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int TaskFor { get; set; }
        public int TaskMaker { get; set; }
        public string Description { get; set; }
        public bool IsAction { get; set; }
        public string ActionDesc { get; set; }
        public string Periode { get; set; }
        public DateTime SubmitDate { get; set; }
    }
}