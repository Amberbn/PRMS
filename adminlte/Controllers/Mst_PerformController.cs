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
using System.Data.Entity.Infrastructure;
using adminlte.Authorization;


namespace adminlte.Controllers
{
    [UserAuthorize]
    [AdminAuthorize]
    public class Mst_PerformController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: Mst_Perform
        public ActionResult Index()
        {
            var mst_per = db.Mst_Performs.Include(mp => mp.Mst_RoleMap);

            //return Json(mst_performs, JsonRequestBehavior.AllowGet);
            return View(mst_per.ToList());
        }

        // GET: Mst_Perform/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var data = db.Mst_Performs.Where(rm => rm.PerformId == id).Include(rm => rm.Mst_RoleMap).Single();
            return View(data);
        }

        // GET: Mst_Perform/Create
        public ActionResult Create()
        {
            ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName");
            return View();
        }

        // POST: Mst_Perform/Create
        [HttpPost]
        public ActionResult Create(Mst_Perform simpan)
        {
            try
            {
                // TODO: Add insert logic here
                ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName");
                db.Mst_Performs.Add(simpan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Mst_Perform/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var data = db.Mst_Performs.Include(r => r.Mst_RoleMap).Where(r => r.PerformId == id).Single();
            ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName", data.RoleMapId);
            return View(data);
        }

        // POST: Mst_Perform/Edit/5
        [HttpPost]
        public ActionResult Edit(int? id, Mst_Perform simpanEdit)
        {
            try
            {
                if (id == null)
                {
                    return HttpNotFound();
                }
                // TODO: Add update logic here
                Mst_Perform sedit = db.Mst_Performs.Find(id);
                //return Json(simpanEdit, JsonRequestBehavior.AllowGet);
                sedit.IsActive = simpanEdit.IsActive;
                sedit.PerformDesc = simpanEdit.PerformDesc;
                sedit.RoleMapId = simpanEdit.RoleMapId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Mst_Perform/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var data = db.Mst_Performs.Where(r => r.PerformId == id).Include(s => s.Mst_RoleMap).Single();
            return View(data);
        }

        // POST: Mst_Perform/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Mst_Perform collection)
        {
            try
            {
                // TODO: Add delete logic here
                Mst_Perform mst = db.Mst_Performs.Find(id);
                db.Mst_Performs.Remove(mst);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(DbUpdateException)
            {
                ViewBag.message = "Delete failed. Plese check if record is used by another table.";
                Mst_Perform mst2 = db.Mst_Performs.Find(id);
                return View();
            }
        }
    }
}