using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace adminlte.Models
{
    public class Mst_RoleMap
    {
        [Key]
        public int RoleMapId { get; set; }

        [Required]
        public string RoleMapName { get; set; }

        [Required]
        public bool IsActive { get; set; }     
    }
}