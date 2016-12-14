using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Tbl_Comment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int CommentMaker { get; set; }
        public string CommentText { get; set; }
        public string Action { get; set; }
    }
}