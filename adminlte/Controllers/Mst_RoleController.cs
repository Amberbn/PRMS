using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using adminlte.Models;
using adminlte.Context;
using adminlte.Authorization;
using System.Data.Entity.Infrastructure;

namespace adminlte.Controllers
{
    [UserAuthorize]
    [AdminAuthorize]
    public class Mst_RoleController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: Mst_Role
        public ActionResult Index()
        {
            return View(db.Mst_Roles.ToList());
        }

        // GET: Mst_Role/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();
            Mst_Role mst_Role = new Mst_Role();
            mst_Role = db.Mst_Roles.Find(id);
            if (mst_Role == null)
                return HttpNotFound();
            return View(mst_Role);
        }

        // GET: Mst_Role/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mst_Role/Create
        [HttpPost]
        public ActionResult Create(Mst_Role mst_Role)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.Mst_Roles.Add(mst_Role);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Mst_Role/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();
            Mst_Role mst_Role = new Mst_Role();
            mst_Role = db.Mst_Roles.Find(id);
            if (mst_Role == null)
                return HttpNotFound();
            return View(mst_Role);
        }

        // POST: Mst_Role/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Mst_Role mst_Role)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    db.Entry(mst_Role).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Mst_Role/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();
            Mst_Role mst_Role = new Mst_Role();
            mst_Role = db.Mst_Roles.Find(id);
            if (mst_Role == null)
                return HttpNotFound();
            return View(mst_Role);
        }

        // POST: Mst_Role/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                Mst_Role mst_Role = db.Mst_Roles.Find(id);
                db.Mst_Roles.Remove(mst_Role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(DbUpdateException)
            {
                ViewBag.message = "Delete failed. Please check if record is used by another table";
                Mst_Role mst_Role2 = db.Mst_Roles.Find(id);
                return View(mst_Role2);
            }
        }
    }
}
