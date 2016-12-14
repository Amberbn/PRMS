using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Request_Ticket
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Mst_User")]
        public int UserId { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public string RequestType { get; set; }
        [ForeignKey("Tbl_Task")]
        public int TaskId { get; set; }
        public Mst_User Mst_User { get; set; }
        public Tbl_Task Tbl_Task { get; set; }
    }
}