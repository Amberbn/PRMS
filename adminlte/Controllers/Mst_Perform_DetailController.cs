using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using adminlte.Context;
using adminlte.Models;
using System.Data.Entity;
using System.Net;
using System.Data.SqlClient;
using adminlte.Authorization;


namespace adminlte.Controllers
{
    [UserAuthorize]
    [AdminAuthorize]
    public class Mst_Perform_DetailController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: Mst_Perform_Detail
        public ActionResult Index()
        {
            var mst_dtl = db.Mst_Perform_Details.Include(mper => mper.Mst_Perform.Mst_RoleMap);
            //return Json(mst_dtl, JsonRequestBehavior.AllowGet);
            return View(mst_dtl);
        }

        // GET: Mst_Perform_Detail/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var data = db.Mst_Perform_Details.Where(pdt => pdt.PerformDetailId == id).Include(pr => pr.Mst_Perform.Mst_RoleMap).Single();
            return View(data);
        }

        // GET: Mst_Perform_Detail/Create
        public ActionResult Create()
        {

            //var l = db.Mst_RoleMaps.ToList();
            //SelectList list = new SelectList(l, "RoleMapId", "RoleMapName");
            //ViewBag.listall = list;
            //SelectList list = new SelectList(l, "RoleMapId", "RoleMapName");
            ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName");

            if (Request.QueryString["RoleMapId"] != null)
            {
                if (Request.QueryString["RoleMapId"] != "")
                {
                    var rm = Request.QueryString["RoleMapId"];
                    int rmn = Convert.ToInt32(rm);
                    var prm = db.Mst_Performs.Where(p => p.RoleMapId == rmn);
                    return Json(prm, JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }

        // POST: Mst_Perform_Detail/Create
        [HttpPost]
        public ActionResult Create(Mst_Perform_Detail simpan)
        {
            try
            {
                // TODO: Add insert logic here
                ViewBag.PerformId = new SelectList(db.Mst_Performs, "PerformId", "PerformDesc");
                db.Mst_Perform_Details.Add(simpan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public List<SelectListItem> GetPerform(string RoleMapName)
        {
            List<SelectListItem> Performs = new List<SelectListItem>();
            var hasil = db.Mst_Performs.Include(m => m.Mst_RoleMap).Where(m => m.Mst_RoleMap.RoleMapName == RoleMapName);
            foreach (var item in hasil)
            {
                Performs.Add(new SelectListItem { Text = item.PerformDesc, Value = item.PerformId.ToString()});
            }
            return Performs;
        }

        // GET: Mst_Perform_Detail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var data = db.Mst_Perform_Details.Include(r => r.Mst_Perform.Mst_RoleMap).Where(r => r.PerformDetailId == id).Single();
            ViewBag.PerformId = new SelectList(db.Mst_Performs.Where(r => r.RoleMapId == data.Mst_Perform.Mst_RoleMap.RoleMapId), "PerformId", "PerformDesc", data.PerformId);
            return View(data);
        }
        // POST: Mst_Perform_Detail/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Mst_Perform_Detail saveedit)
        {
            try
            {
                // TODO: Add update logic here
                Mst_Perform_Detail sedit = db.Mst_Perform_Details.Find(id);
                sedit.PerformId = saveedit.PerformId;
                sedit.Detail_Description = saveedit.Detail_Description;
                sedit.Weight = saveedit.Weight;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Mst_Perform_Detail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var data = db.Mst_Perform_Details.Where(r => r.PerformDetailId == id).Include(s => s.Mst_Perform.Mst_RoleMap).Single();
            return View(data);
            //return View();
        }

        // POST: Mst_Perform_Detail/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Mst_Perform_Detail collection)
        {
            try
            {
                // TODO: Add delete logic here
                Mst_Perform_Detail dtl = db.Mst_Perform_Details.Find(id);
                //return Json(dtl, JsonRequestBehavior.AllowGet);
                db.Mst_Perform_Details.Remove(dtl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
