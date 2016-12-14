using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using adminlte.Models;

namespace adminlte.Context
{
    public class Mst_UserDataContext : DbContext
    {
        public DbSet<Mst_User> Mst_Users { get; set; }

        public DbSet<Mst_Role> Mst_Roles { get; set; }

        public DbSet<Mst_RoleMap> Mst_RoleMaps { get; set; }
        
        public DbSet<Mst_Perform> Mst_Performs { get; set; }

        public DbSet<Mst_Perform_Detail> Mst_Perform_Details { get; set; }

        public DbSet<Mst_CommPlan> Mst_CommPlans { get; set; }

        public DbSet<Mst_CommPlan_Detail> Mst_CommPlan_Details { get; set; }

        public DbSet<Tbl_Task> Tbl_Tasks { get; set; }

        public DbSet<Trx_Perform_Review> Trx_Perform_Reviews { get; set; }

        public DbSet<Trx_Comm_Plan> Trx_Comm_Plans { get; set; }

        public DbSet<Queue_Email> Queue_Emails { get; set; }

        public DbSet<Tbl_Comment> Tbl_Comments { get; set; }

        public DbSet<Request_Ticket> Request_Tickets { get; set; }
    }
}