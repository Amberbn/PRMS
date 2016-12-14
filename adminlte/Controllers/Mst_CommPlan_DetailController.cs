using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using adminlte.Context;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Sql;
using adminlte.Models;
using adminlte.Authorization;

namespace adminlte.Controllers
{
    [UserAuthorize]
    [AdminAuthorize]
    public class Mst_CommPlan_DetailController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: CommPlanDetail
        public ActionResult Index()
        {
            var data = db.Mst_CommPlan_Details.Include(ax => ax.Mst_CommPlan).Include(zx => zx.Mst_Role).OrderBy(ax=>ax.CommPlanid);
            //return Json(data, JsonRequestBehavior.AllowGet);
            return View(data);
            
        }

        // GET: CommPlanDetail/Details/5
        public ActionResult Details(int id)
        {
            var mstcommplandtl = db.Mst_CommPlan_Details.Include(pdt => pdt.Mst_CommPlan).Include(pr => pr.Mst_Role).Where(cd => cd.CommPlanDetailId == id).Single();
          //  Mst_CommPlan_Detail mstcommplandtl = db.Mst_CommPlan_Details.Find(id);
            if (mstcommplandtl == null)
            {
                return HttpNotFound();
            }
            return View(mstcommplandtl);
        }

        // GET: CommPlanDetail/Create
        public ActionResult Create()
        {
            var l = db.Mst_Roles.ToList();
            SelectList list = new SelectList(l, "RoleId", "RoleName");
            ViewBag.listall = list;

            ViewBag.CommPlanid = new SelectList(db.Mst_CommPlans, "CommPlanid", "CommPlanDesc");
            ViewBag.RoleId = new SelectList(db.Mst_Roles, "RoleId", "RoleName");
            //return Json(db.Mst_Role,JsonRequestBehavior.AllowGet);
            return View();
        }

        // POST: CommPlanDetail/Create
        [HttpPost]
        public ActionResult Create(Mst_CommPlan_Detail tambah)
        {
            try
            {
                //return Json(tambah,JsonRequestBehavior.AllowGet);
                // TODO: Add insert logic here
                //ViewBag.CommPlanid = new SelectList(db.Mst_CommPlans, "CommPlanid", "CommPlanDesc");
                //ViewBag.RoleId = new SelectList(db.Mst_Role, "RoleId", "RoleName");
                db.Mst_CommPlan_Details.Add(tambah);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CommPlanDetail/Edit/5
        public ActionResult Edit(int? id)
        {
            Mst_CommPlan_Detail mstcomplandtl = db.Mst_CommPlan_Details.Find(id);
            if (id == null)
            {
                return HttpNotFound();
            }
            //string data = mstperform.ToString();
            //return View(mstperform);
            ViewBag.CommPlanid = new SelectList(db.Mst_CommPlans, "CommPlanid", "CommPlanDesc");
            ViewBag.RoleId = new SelectList(db.Mst_Roles, "RoleId", "RoleName");
            string data = mstcomplandtl.ToString();
            return View(mstcomplandtl);
        }

        // POST: CommPlanDetail/Edit/5
        [HttpPost]
        public ActionResult Edit(int? id, Mst_CommPlan_Detail dEdit)
        {
            try
            {
                // TODO: Add update logic here
                if (id == null)
                {
                    return HttpNotFound();
                }
                Mst_CommPlan_Detail mstcomplandtl = db.Mst_CommPlan_Details.Find(id);
                mstcomplandtl.CommPlanDetailId = dEdit.CommPlanDetailId;
                mstcomplandtl.CommPlanid = dEdit.CommPlanid;
                mstcomplandtl.Detail_Description = dEdit.Detail_Description;
                mstcomplandtl.RoleId = dEdit.RoleId;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CommPlanDetail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();
            Mst_CommPlan_Detail user = db.Mst_CommPlan_Details.Include(m => m.Mst_CommPlan).Include(m => m.Mst_Role).Where(m => m.CommPlanDetailId == id).Single();
            if (user == null)
                return HttpNotFound();
            return View(user);
        }

        // POST: CommPlanDetail/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Mst_CommPlan_Detail mstp = db.Mst_CommPlan_Details.Find(id);
            //return Json(mstp,JsonRequestBehavior.AllowGet);
            db.Mst_CommPlan_Details.Remove(mstp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
