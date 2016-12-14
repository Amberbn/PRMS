using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using adminlte.Context;
using adminlte.Models;

namespace adminlte.Controllers
{
    public class CommitmentController : Controller
    {
        private Mst_UserDataContext db = new Mst_UserDataContext();

        // GET: Commitment
        public ActionResult Index()
        {
            return View(db.Mst_CommPlans.ToList());
        }

        // GET: Commitment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mst_CommPlan mst_CommPlan = db.Mst_CommPlans.Find(id);
            if (mst_CommPlan == null)
            {
                return HttpNotFound();
            }
            return View(mst_CommPlan);
        }

        // GET: Commitment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Commitment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommPlanid,CommPlanDesc,isActive")] Mst_CommPlan mst_CommPlan)
        {
            if (ModelState.IsValid)
            {
                db.Mst_CommPlans.Add(mst_CommPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mst_CommPlan);
        }

        // GET: Commitment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mst_CommPlan mst_CommPlan = db.Mst_CommPlans.Find(id);
            if (mst_CommPlan == null)
            {
                return HttpNotFound();
            }
            return View(mst_CommPlan);
        }

        // POST: Commitment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommPlanid,CommPlanDesc,isActive")] Mst_CommPlan mst_CommPlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mst_CommPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mst_CommPlan);
        }

        // GET: Commitment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mst_CommPlan mst_CommPlan = db.Mst_CommPlans.Find(id);
            if (mst_CommPlan == null)
            {
                return HttpNotFound();
            }
            return View(mst_CommPlan);
        }

        // POST: Commitment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mst_CommPlan mst_CommPlan = db.Mst_CommPlans.Find(id);
            db.Mst_CommPlans.Remove(mst_CommPlan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
