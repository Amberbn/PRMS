using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using adminlte.Models;
using adminlte.Context;
using adminlte.Authorization;
using System.Net;

namespace adminlte.Controllers
{
    [UserAuthorize]
    [AdminAuthorize]
    public class Mst_RoleMapController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: Mst_RoleMap
        public ActionResult Index()
        {
            
            return View(db.Mst_RoleMaps.ToList());
        }

        // GET: Mst_RoleMap/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Mst_RoleMap mst_RoleMap = new Mst_RoleMap();
            mst_RoleMap = db.Mst_RoleMaps.Find(id);
            if (mst_RoleMap == null)
                return HttpNotFound();  
            return View(mst_RoleMap);
        }

        // GET: Mst_RoleMap/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mst_RoleMap/Create
        [HttpPost]
        public ActionResult Create(Mst_RoleMap mst_RoleMap)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.Mst_RoleMaps.Add(mst_RoleMap);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Mst_RoleMap/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Mst_RoleMap mst_RoleMap = db.Mst_RoleMaps.Find(id);
            if (mst_RoleMap == null)
                return HttpNotFound();
            return View(mst_RoleMap);
        }

        // POST: Mst_RoleMap/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Mst_RoleMap mst_RoleMap)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    db.Entry(mst_RoleMap).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Mst_RoleMap/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Mst_RoleMap mst_RoleMap = new Mst_RoleMap();
            mst_RoleMap = db.Mst_RoleMaps.Find(id);
            if (mst_RoleMap == null)
                return HttpNotFound();
            return View(mst_RoleMap);
        }

        // POST: Mst_RoleMap/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Mst_RoleMap mst_RoleMap = db.Mst_RoleMaps.Find(id);
                db.Mst_RoleMaps.Remove(mst_RoleMap);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.message = "Delete failed. Please check if record is used by another table.";
                Mst_RoleMap mst_RoleMap2 = db.Mst_RoleMaps.Find(id);
                return View(mst_RoleMap2);
            }
        }
    }
}
