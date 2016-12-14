using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Queue_Email
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string EmailTo { get; set; }
    }
}