using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using adminlte.Context;
using System.Data.Entity;
using System.Data.SqlClient;
using adminlte.Models;
using System.Data.Sql;
using System.Data.Entity.Infrastructure;
using adminlte.Authorization;
using System.IO;

namespace adminlte.Controllers
{

    public class Mst_CommPlanController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: CommPlan
        [AdminAuthorize]
        public ActionResult Index()
        {
            return View(db.Mst_CommPlans.ToList());
        }

        [CommPlanAuthorize]
        public ActionResult commplan()
        {
            int id = Convert.ToInt32(Session["loginuserid"]);
            var user = db.Mst_Users.Include(rl => rl.Mst_Role).Where(us => us.UserId == id).Single();
            var report = db.Mst_Users.Where(us => us.UserId == user.DirectReportId).Single();
            ViewBag.role = user.Mst_Role.RoleName;
            ViewBag.roleid = user.RoleId;
            ViewBag.report = report.FullName;
            var commplan = db.Mst_CommPlan_Details.Include(cm => cm.Mst_CommPlan).Where(cd => cd.RoleId == user.RoleId);
            int count = commplan.Count();
            ViewBag.hitung = count;
            string cek = CekPeriode.CheckPeriode();
            int ceksubmit = db.Trx_Comm_Plans.Include(ts => ts.Tbl_Task).Where(ts => ts.SubmitBy == id && ts.Periode == cek).Count();
            ViewBag.ceksubmit = ceksubmit;
            ViewBag.cek = cek;
            ViewBag.isopen = user.CommPlanOpen;
            return View(commplan);
        }
        public string nama;
        public string hasil;
        public int tskawal;
        [HttpPost]
        public ActionResult commplan(FormCollection form)
        {
            //var task = db.Tbl_Tasks.Max(ax => ax.TaskId)+1;
            int usrid = Convert.ToInt32(Session["loginuserid"]);
            var data = db.Mst_Users.Include(rl => rl.Mst_Role).Where(ui => ui.UserId == usrid).Single();
            var tsfo = db.Mst_Users.Where(us => us.UserId == data.DirectReportId).Single();
            string penerima = tsfo.Email;
            Tbl_Task tsk = new Tbl_Task();

            string ActionDesc;

            int kondisi = db.Tbl_Tasks.Count();
            string periode = form["periode"];
            tsk.Periode = periode;

            tsk.TaskName = "Commitment Plan";
            int reportid = Convert.ToInt32(data.DirectReportId);
            tsk.TaskFor = reportid;
            tsk.TaskMaker = data.UserId;


            int cek = Convert.ToInt32(form["check"]);
            if (cek == 1)
            {
                ActionDesc = "Save";
                tsk.Description = data.FullName + " has saved Commitment Plan for Periode " + periode;
                tsk.IsAction = false;
            }
            else
            {
                ActionDesc = "Submit";
                tsk.Description = data.FullName + " has submitted Commitment Plan for Periode " + periode;
                tsk.IsAction = false;
            }
            tsk.ActionDesc = ActionDesc;
            if (kondisi == null || kondisi == 0)
            {
                tskawal = 1;
            }
            else
            {
                tskawal = db.Tbl_Tasks.Max(ax => ax.TaskId) + 1;
            }
            tsk.TaskId = tskawal;
            tsk.SubmitDate = DateTime.Now.Date;
            string linknya = "http://localhost:56913/Task/TrxCommPlan/" + tsk.TaskId;
            string deskripsi = "hi, " + tsfo.FullName + "<br>" + tsk.Description + "<a href=" + linknya + "><br>Click Here to Review</a><br><br><p>Regards,</p><p>e-PRMS Admin</p>";
            //return Json(tsk, JsonRequestBehavior.AllowGet);
            SendEmail email = new SendEmail();
            email.Send(penerima, tsk.Description, deskripsi);
            db.Tbl_Tasks.Add(tsk);
            db.SaveChanges();
            //DateTime submitDate = DateTime.Now.Date;
            int i = 0;
            for (i = 0; i <= form.Count; i++)
            {
                string hitung = i.ToString();
                if (form["detail_" + hitung] != null)
                {
                    string[] detail = form["detail_" + hitung].Split(char.Parse(","));
                    string[] results = form["resultan_" + hitung].Split(char.Parse(","));
                    string[] complaid = form["complainid_" + hitung].Split(char.Parse(","));
                    HttpPostedFileBase file = Request.Files["file_" + hitung];
                    string a = UploadFiles(file);
                    string dtku = detail[0];
                    string hasil = results[0];
                    string com = complaid[0];
                    Trx_Comm_Plan trx = new Trx_Comm_Plan();
                    trx.TaskId = tskawal;
                    trx.CommPlanid = Convert.ToInt16(com);
                    trx.DescriptionPlan = dtku;
                    trx.IsAchievable = hasil;
                    trx.SubmitBy = usrid;
                    trx.Periode = periode;
                    trx.File = a;
                    //trx.SubmitDate = submitDate;
                    db.Trx_Comm_Plans.Add(trx);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Task");
            //return Json(form, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public string UploadFiles(HttpPostedFileBase file)
        {
            string i = "";
            string path = "";
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    i = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/Files/") + i);
                    file.SaveAs(path);
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                ViewBag.Message = "coba lagi";
            }
            return i;
        }

        [AdminAuthorize]
        // GET: CommPlan/Details/5
        public ActionResult Details(int? id)
        {
            Mst_CommPlan mstcommplan = db.Mst_CommPlans.Find(id);
            if (mstcommplan == null)
            {
                return HttpNotFound();
            }
            return View(mstcommplan);
        }

        [AdminAuthorize]
        // GET: CommPlan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CommPlan/Create
        [HttpPost]
        public ActionResult Create(Mst_CommPlan add)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.Mst_CommPlans.Add(add);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [AdminAuthorize]
        // GET: CommPlan/Edit/5
        public ActionResult Edit(int? id)
        {
            Mst_CommPlan mstcomplan = db.Mst_CommPlans.Find(id);
            if (id == null)
            {
                return HttpNotFound();
            }
            //string data = mstperform.ToString();
            //return View(mstperform);
            string data = mstcomplan.ToString();
            return View(mstcomplan);
        }

        // POST: CommPlan/Edit/5
        [HttpPost]
        public ActionResult Edit(int? id, Mst_CommPlan cEdit)
        {
            try
            {
                // TODO: Add update logic here
                if (id == null)
                {
                    return HttpNotFound();
                }
                Mst_CommPlan mstcomplan = db.Mst_CommPlans.Find(id);
                mstcomplan.CommPlanid = cEdit.CommPlanid;
                mstcomplan.CommPlanDesc = cEdit.CommPlanDesc;
                mstcomplan.isActive = cEdit.isActive;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [AdminAuthorize]
        // GET: CommPlan/Delete/5
        public ActionResult Delete(int? id)
        {
            Mst_CommPlan user = db.Mst_CommPlans.Find(id);
            if (user == null)
                return HttpNotFound();
            return View(user);
        }

        // POST: CommPlan/Delete/5
        [HttpPost]
        public ActionResult Delete(int? id, Mst_CommPlan test)
        {
            try
            {
                Mst_CommPlan mstp = db.Mst_CommPlans.Find(id);
                //return Json(mstp,JsonRequestBehavior.AllowGet);
                db.Mst_CommPlans.Remove(mstp);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            catch (DbUpdateException)
            {
                ViewBag.message = "Delete failed. Please check if record is used by another table.";
                Mst_CommPlan mstp2 = db.Mst_CommPlans.Find(id);
                return View(mstp2);
            }
        }
    }
}
