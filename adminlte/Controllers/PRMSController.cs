using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using adminlte.Models;
using adminlte.Context;

namespace adminlte.Controllers
{
    public class PRMSController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: PRMS
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Mst_LoginView isi)
        {
            //return Json(isi,JsonRequestBehavior.AllowGet);
            if (ModelState.IsValid)
            {
                var data = db.Mst_Users.Include(r => r.Mst_Role).Include(rm => rm.Mst_RoleMap);
                var res = data.Where(a => a.UserName.Equals(isi.UserName) && a.Password.Equals(isi.Password)).FirstOrDefault();
                //return Json(isi, JsonRequestBehavior.AllowGet);
                if (res != null)
                {
                    Session["loginuserid"] = res.UserId.ToString();
                    Session["loginusername"] = res.UserName.ToString();
                    Session["level"] = res.Mst_Role.RoleName.ToString();
                    Session["performance"] = res.PerfReviewOpen;
                    Session["commplan"] = res.CommPlanOpen;
                    if (res.Mst_Role.RoleName == "Admin")
                    {
                        return RedirectToAction("Index", "Task");
                    }
                    else if (res.Mst_Role.RoleName == "PMO")
                    {
                        return RedirectToAction("Index", "Task");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Task");
                    }
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.RemoveAll();
            return View();
        }
    }
}