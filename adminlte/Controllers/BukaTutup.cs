using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using adminlte.Context;
using adminlte.Models;

namespace adminlte.Controllers
{
    public class BukaTutup
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        public void CloseAll() {
            List<Mst_User> listUser = db.Mst_Users.ToList();
            for (int i = 0; i < listUser.Count; i++)
            {
                listUser[i].CommPlanOpen = false;
                listUser[i].PerfReviewOpen = false;
                db.SaveChanges();
            }
        }

        public void OpenAll() {
            List<Mst_User> listUser = db.Mst_Users.ToList();
            for (int i = 0; i < listUser.Count; i++)
            {
                listUser[i].CommPlanOpen = true;
                listUser[i].PerfReviewOpen = true;
                db.SaveChanges();
            }
        }
    }
}