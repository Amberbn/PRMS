using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;
using adminlte.Models;
using adminlte.Context;
using adminlte.Authorization;
using System.Net;

namespace adminlte.Controllers
{
    [UserAuthorize]
    [AdminAuthorize]
    public class Mst_UserController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: User
        public ActionResult Index()
        {
            var mst_userJoin = db.Mst_Users.Include(m => m.Mst_Role).Include(m => m.Mst_RoleMap);
            return View(mst_userJoin.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var mst_userJoin = db.Mst_Users.Include(m => m.Mst_Role).Include(m => m.Mst_RoleMap).Where(m => m.UserId == id).Single();
            if (mst_userJoin == null)
                return HttpNotFound();
            return View(mst_userJoin);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.Mst_Roles, "RoleId", "RoleName");
            ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName");
            ViewBag.DirectReportId = new SelectList(db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName == "Team Leader"), "UserId", "FullName");
            

            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(Mst_User mst_User)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.Mst_Users.Add(mst_User);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.RoleId = new SelectList(db.Mst_Roles, "RoleId", "RoleName", mst_User.RoleId);
                ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName",mst_User.RoleMapId);
                ViewBag.DirectReportId = new SelectList(db.Mst_Users, "UserId", "FullName", mst_User.DirectReportId);

                return View(mst_User);
            }
            catch
            {
                return View();
            }
        }

        //public JsonResult GetRoleMap(int id)
        //{
        //    List<SelectListItem> RoleMapList = new List<SelectListItem>();
        //    Mst_RoleMap roles = db.Mst_RoleMaps;
        //    RoleMapList.Add(new SelectListItem { Text = roles. });
        //}

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var mst_userJoin = db.Mst_Users.Include(m => m.Mst_Role).Include(m => m.Mst_RoleMap).Where(m => m.UserId == id).Single();

            if (mst_userJoin == null)
                return HttpNotFound();

            ViewBag.RoleId = new SelectList(db.Mst_Roles, "RoleId", "RoleName", mst_userJoin.RoleId);
            ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName", mst_userJoin.RoleMapId);
            ViewBag.DirectReportId = new SelectList(db.Mst_Users, "UserId", "FullName", mst_userJoin.DirectReportId);

            return View(mst_userJoin);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Mst_User mst_User)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    if (mst_User.DirectReportId != mst_User.UserId)
                    {
                        db.Entry(mst_User).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else {
                        ViewBag.message = "Wrong Input. Direct Report is reporting to itself.";
                    }
                }

                ViewBag.RoleId = new SelectList(db.Mst_Roles, "RoleId", "RoleName", mst_User.RoleId);
                ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName", mst_User.RoleMapId);
                ViewBag.DirectReportId = new SelectList(db.Mst_Users, "UserId", "FullName", mst_User.DirectReportId);

                return View(mst_User);
            }
            catch
            {
                return View(mst_User);
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var mst_userJoin = db.Mst_Users.Include(m => m.Mst_Role).Include(m => m.Mst_RoleMap).Where(m => m.UserId == id).Single();

            if (mst_userJoin == null)
                return HttpNotFound();

            ViewBag.RoleId = new SelectList(db.Mst_Roles, "RoleId", "RoleName", mst_userJoin.RoleId);
            ViewBag.RoleMapId = new SelectList(db.Mst_RoleMaps, "RoleMapId", "RoleMapName", mst_userJoin.RoleMapId);
            ViewBag.DirectReportId = new SelectList(db.Mst_Users, "UserId", "FullName", mst_userJoin.DirectReportId);

            return View(mst_userJoin);
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Mst_User mst_User)
        {
            try
            {
                // TODO: Add delete logic here
                Mst_User mst_User2 = db.Mst_Users.Find(id);
                db.Mst_Users.Remove(mst_User2);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(mst_User);
            }
        }
    }
}
