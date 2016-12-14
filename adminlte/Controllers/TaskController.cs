using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Diagnostics;
using adminlte.Models;
using adminlte.Context;
using System.IO;
using adminlte.Authorization;

namespace adminlte.Controllers
{
    public class TaskController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        SendEmail kirim = new SendEmail();
        // GET: Task
        [UserAuthorize]
        public ActionResult Index()
        {
            int id = Convert.ToInt32(Session["loginuserid"]);
            var list = db.Tbl_Tasks.Where(r => r.TaskFor == id && r.IsAction == false || r.TaskMaker == id && r.IsAction == false && r.ActionDesc == "Save").ToList();
            string level = Session["level"].ToString();
            ViewBag.level = level;
            return View(list);
        }
        [CommPlanAuthorize]
        public ActionResult TrxCommPlan(int id)
        {
            var trx = db.Trx_Comm_Plans.Include(t => t.Mst_CommPlan).Include(s => s.Mst_User).Where(ts => ts.TaskId == id);
            int usrid = Convert.ToInt32(Session["loginuserid"]);
            var user = db.Trx_Comm_Plans.Include(us => us.Mst_User).Include(rl => rl.Mst_User.Mst_Role).Where(tx => tx.TaskId == id).FirstOrDefault();
            int htt = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.TaskName == "Commitment Plan" && ts.ActionDesc != "Request").Count();
            if (htt == 0)
            {
                return RedirectToAction("Error", "Error");
            }
            ViewBag.user = user.Mst_User.FullName;
            ViewBag.role = user.Mst_User.Mst_Role.RoleName;
            var act = db.Tbl_Tasks.Where(ts => ts.TaskId == user.TaskId).OrderByDescending(asx => asx.Id).First();
            ViewBag.ActionDesc = act.ActionDesc;
            var report = db.Mst_Users.Where(us => us.UserId == user.Mst_User.DirectReportId).Single();
            string cekperiode = CekPeriode.CheckPeriode();
            int cekhitung = db.Tbl_Tasks.Where(ts => ts.TaskFor == user.Mst_User.UserId || ts.TaskMaker == user.Mst_User.UserId && ts.Periode == cekperiode && ts.TaskName == "Commitment Plan" && ts.ActionDesc == "Save").Count();
            ViewBag.cek = cekhitung;
            ViewBag.report = report.FullName;
            ViewBag.periode = cekperiode;
            string level = Session["level"].ToString();
            int pmoteamlead = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.ActionDesc == "Submit" && ts.IsAction == false && ts.Periode == cekperiode && ts.TaskName == "Commitment Plan").Count();
            int management = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.ActionDesc == "Approve" && ts.IsAction == false && ts.Periode == cekperiode && ts.TaskName == "Commitment Plan").Count();
            if (level=="Management" && report.UserId== usrid)
            {
                management = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.ActionDesc == "Submit"|| ts.ActionDesc == "Approve" && ts.IsAction == false && ts.Periode == cekperiode && ts.TaskName == "Commitment Plan").Count();
            }
            ViewBag.pmoteamlead = pmoteamlead;
            ViewBag.management = management;
            int hitungisi = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.ActionDesc == "Submit" || ts.ActionDesc == "Save" && ts.Periode == cekperiode).Count();
            if (hitungisi != 0)
            {
                var taskmaker = db.Tbl_Tasks.OrderByDescending(ts => ts.Id).Where(ts => ts.TaskId == id && ts.ActionDesc == "Submit" || ts.ActionDesc == "Save" && ts.Periode == cekperiode).FirstOrDefault();
                if (taskmaker.TaskMaker == usrid || taskmaker.TaskFor == usrid || level == "Management")
                {
                    return View(trx.ToList());
                }
            }

            return RedirectToAction("Index", "Task");

        }

        [CommPlanAuthorize]
        public ActionResult TrxCommPlanEdit(int id)
        {
            int htt = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.TaskName == "Commitment Plan" && ts.ActionDesc != "Request").Count();
            if (htt == 0)
            {
                return RedirectToAction("Error", "Error");
            }
            var trx = db.Trx_Comm_Plans.Include(t => t.Mst_CommPlan).Include(s => s.Mst_User).Where(ts => ts.TaskId == id);
            int usrid = Convert.ToInt32(Session["loginuserid"]);
            var user = db.Trx_Comm_Plans.Include(us => us.Mst_User).Include(rl => rl.Mst_User.Mst_Role).Where(tx => tx.TaskId == id).FirstOrDefault();
            ViewBag.user = user.Mst_User.FullName;
            ViewBag.role = user.Mst_User.Mst_Role.RoleName;
            string cekperiode = CekPeriode.CheckPeriode();
            ViewBag.periode = cekperiode;
            var report = db.Mst_Users.Where(us => us.UserId == user.Mst_User.DirectReportId).Single();
            ViewBag.report = report.FullName;
            ViewBag.level = Session["level"].ToString();
            int cekhitung = db.Tbl_Tasks.Where(ts => ts.TaskFor == usrid && ts.Periode == cekperiode && ts.TaskName == "Commitment Plan" && ts.ActionDesc == "Revise" && ts.IsAction == false).Count();
            ViewBag.cek = cekhitung;
            int ceksave = db.Tbl_Tasks.Where(ts => ts.TaskId == user.TaskId && ts.Periode == cekperiode && ts.TaskName == "Commitment Plan" && ts.ActionDesc == "Save" && ts.IsAction == false).Count();
            ViewBag.ceksave = ceksave;
            int hitungkomentar = db.Tbl_Comments.Where(r => r.TaskId == id && r.Action == "Revise").Count();
            if (hitungkomentar != 0)
            {
                string komentar = db.Tbl_Comments.Where(r => r.TaskId == id && r.Action == "Revise").OrderByDescending(r => r.Id).FirstOrDefault().CommentText;
                ViewBag.komentar = komentar;
            }

            if (user.Mst_User.UserId != usrid)
            {
                return RedirectToAction("index");
            }
            return View(trx.ToList());
        }
        public int atasan;
        public int tskawal;
        public string penerima_atasan;
        [HttpPost]
        public ActionResult TrxCommPlan(FormCollection form, int id)
        {

            int usrid = Convert.ToInt32(Session["loginuserid"]);
            var data = db.Mst_Users.Where(ui => ui.UserId == usrid).Single();
            var task = db.Trx_Comm_Plans.Include(us => us.Mst_User).Include(rl=>rl.Mst_User.Mst_Role).Where(tt => tt.TaskId == id).FirstOrDefault();
            string periodecek = CekPeriode.CheckPeriode();
            var submit = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.ActionDesc == "Submit" && ts.Periode == periodecek).FirstOrDefault();
            DateTime submitDate = submit.SubmitDate.Date;
            string penerima = task.Mst_User.Email;
            int taskid = task.TaskId;
            string nama = task.Mst_User.FullName;
            int userid = task.Mst_User.UserId;
            var tsfo = db.Mst_Users.Where(us => us.UserId == task.Mst_User.DirectReportId).Single();
            string atasan = tsfo.Email;
            var act = db.Tbl_Tasks.Where(ts => ts.TaskId == task.TaskId).OrderByDescending(asx => asx.Id).First();
            ViewBag.ActionDesc = act.ActionDesc;
            var management = db.Mst_Users.Include(rl => rl.Mst_Role).Where(ll => ll.Mst_Role.RoleName == "Management").First();
            int managementid = management.UserId;
            Tbl_Task tsk = new Tbl_Task();
            tsk.SubmitDate = submitDate;
            int kondisi = db.Tbl_Tasks.Count();
            string level = Session["level"].ToString();
            //string periode = form["periode"] + " " + DateTime.Now.Year;
            string periode = CekPeriode.CheckPeriode();
            int aksi = Convert.ToInt32(form["check"]);
            string komentar = form["komentar"];
            Tbl_Comment comment = new Tbl_Comment();
            var date = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Submit").FirstOrDefault();
            tsk.SubmitDate = date.SubmitDate;
            int apprev = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Submit" && ts.Periode == periode && ts.IsAction == false || ts.TaskId == taskid && ts.ActionDesc == "Revise" && ts.Periode == periode && ts.IsAction == false || ts.TaskId == taskid && ts.ActionDesc == "Approve" && ts.Periode == periode && ts.IsAction == false).Count();

            /*--------------------------------------------------------FOR USER---------------------------------------------------------------*/

            if (level == "Senior Developer" || level == "Middle Developer" || level == "Junior Developer")
            {

                tsk.Periode = periode;
                tsk.TaskId = taskid;
                tsk.TaskName = "Commitment Plan";
                int reportid = Convert.ToInt32(data.DirectReportId);
                tsk.TaskFor = reportid;
                tsk.TaskMaker = data.UserId;
                tsk.ActionDesc = "Submit";
                tsk.Description = data.FullName + " has submitted Commitment Plan for Periode " + periode;
                string linknya = "http://localhost:53400/Task/TrxCommPlan/" + tsk.TaskId;
                string deskripsi = "Hi, " + tsfo.FullName + "<br><br>" + tsk.Description + "<a href=" + linknya + "><br><br>Click Here to Review</a><br><p>Regards</p><p>e-PRMS Admin</p>";
                var app = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Save").FirstOrDefault();
                kirim.Send(atasan, tsk.Description, deskripsi);
                app.IsAction = true;
                db.Tbl_Tasks.Add(tsk);
                db.SaveChanges();
                if (apprev != 0)
                {
                    var edit = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Submit" && ts.Periode == periode && ts.IsAction == false || ts.TaskId == taskid && ts.ActionDesc == "Revise" && ts.Periode == periode && ts.IsAction == false || ts.TaskId == taskid && ts.ActionDesc == "Save" && ts.Periode == periode && ts.IsAction == false).FirstOrDefault();
                    edit.IsAction = true;
                    db.SaveChanges();
                }
            }
            /*--------------------------------------------------------FOR MANAGEMENT----------------------------------------------------------*/

            if (level == "Management")
            {
                tsk.TaskId = taskid;
                tsk.TaskName = "Commitment Plan";
                tsk.TaskFor = task.Mst_User.UserId;
                tsk.TaskMaker = usrid;
                tsk.Periode = periode;
                if (aksi == 1)
                {

                    tsk.ActionDesc = "Complete";
                    tsk.Description = "Your commitment plan has been approved by " + data.FullName;
                    string linknya = "http://localhost:53400/Task/TrxCommPlan/" + tsk.TaskId;
                    string deskripsi = "Hi, " + task.Mst_User.FullName + "<br><br>" + tsk.Description + "<br><br>Komentar : " + komentar + "<br>" + "<a href=" + linknya + "><br>Click Here to Review</a><br><p>Regards</p><p>e-PRMS Admin</p>";
                    tsk.IsAction = true;
                    string role = task.Mst_User.Mst_Role.RoleName;
                    if (role!="Head of PMO"&&role!="Head of Delivery")
                    {
                        string subject_atasan = data.FullName + " has approved commitment plan from " + task.Mst_User.FullName + " for Periode " + periode;
                        string body_atasan = "Hi, " + tsfo.FullName + "<br><br>" + subject_atasan + "<a href=" + linknya + "><br><br>Click Here to Review</a><br><p>Regards</p><p>e-PRMS Admin</p>";
                        kirim.Send(tsfo.Email, subject_atasan, body_atasan);
                    }
                    
                    kirim.Send(task.Mst_User.Email, tsk.Description, deskripsi);

                }
                else
                {
                    tsk.ActionDesc = "Revise";
                    tsk.Description = data.FullName + " ask for revision from your commitment plan for " + periodecek;
                    string linknya = "http://localhost:53400/Task/TrxCommPlanEdit/" + tsk.TaskId;
                    string deskripsi = "Hi, " + task.Mst_User.FullName + "<br><br>" + tsk.Description + "<br><br>Komentar : " + komentar + "<br>" + "<a href=" + linknya + "><br>Click Here to Review</a><br><p>Regards</p><p>e-PRMS Admin</p>";
                    tsk.IsAction = false;
                    string role = task.Mst_User.Mst_Role.RoleName;

                    if (role != "Head of PMO" && role != "Head of Delivery")
                    {
                        string subject_atasan = data.FullName + " ask for Revision from Commitment Plan of " + task.Mst_User.FullName + " for Periode " + periode;
                        string body_atasan = "Hi, " + tsfo.FullName + "<br><br>" + subject_atasan + "<a href=" + linknya + "><br><br>Click Here to Review</a><br><p>Regards</p><p>e-PRMS Admin</p>";
                        kirim.Send(tsfo.Email, subject_atasan, body_atasan);
                    }
                    kirim.Send(task.Mst_User.Email, tsk.Description, deskripsi);
                }

                if (apprev != 0)
                {
                    var edit = db.Tbl_Tasks.OrderByDescending(ts=>ts.Id).Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Approve" || ts.ActionDesc == "Submit" && ts.Periode == periode && ts.IsAction == false).FirstOrDefault();
                    edit.IsAction = true;
                    db.SaveChanges();
                }

                db.Tbl_Tasks.Add(tsk);
                db.SaveChanges();
            }
            if (level == "PMO" || level == "Team Leader")
            {
                /*--------------------------------------------------------FOR PMO/TEAM LEAD--------------------------------------------------------*/

                tsk.TaskId = taskid;
                tsk.TaskName = "Commitment Plan";
                tsk.TaskMaker = usrid;
                tsk.Periode = periode;

                if (aksi == 1)
                {
                    tsk.TaskFor = managementid;
                    tsk.ActionDesc = "Approve";
                    string subject_user = "Your commitment plan has been approved by " + data.FullName;
                    tsk.Description = data.FullName + " forwarded commitment plan from " + task.Mst_User.FullName + " for " + periode;
                    string linknya = "http://localhost:53400/Task/TrxCommPlan/" + tsk.TaskId;
                    string deskripsi_user = "Hi, " + task.Mst_User.FullName + "<br><br>" + subject_user + "<br><br>Komentar : " + komentar + "<br>" + "<a href=" + linknya + "><br>Click Here to Review</a><br><p>Regards</p><p>e-PRMS Admin</p>";
                    tsk.IsAction = false;
                    string subject_management = tsk.Description;
                    string isi_email_manager = "Hi, " + management.FullName + "<br><br>" + subject_management + "<a href=" + linknya + "><br><br>Click Here to Review</a><br><p>Regards</p><p>e-PRMS Admin</p>";
                    kirim.Send(management.Email, subject_management, isi_email_manager);
                    kirim.Send(task.Mst_User.Email, subject_user, deskripsi_user);
                }
                else
                {
                    tsk.TaskFor = task.Mst_User.UserId;
                    tsk.ActionDesc = "Revise";
                    tsk.Description = data.FullName + " ask for revision from your commitment plan for " + periodecek;
                    tsk.IsAction = false;
                    string linknya = "http://localhost:53400/Task/TrxCommPlanEdit/" + tsk.TaskId;
                    string deskripsi = "Hi, " + task.Mst_User.FullName + "<br><br>" + tsk.Description + "<br><br>Komentar : " + komentar + "<br>" + "<a href=" + linknya + "><br>Click Here to Review</a><br><p>Regards</p><p>e-PRMS Admin</p>";
                    kirim.Send(task.Mst_User.Email, tsk.Description, deskripsi);
                }
                db.Tbl_Tasks.Add(tsk);
                db.SaveChanges();
                if (apprev != 0)
                {
                    var edit = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Submit" && ts.Periode == periode && ts.IsAction == false || ts.TaskId == taskid && ts.ActionDesc == "Revise" && ts.Periode == periode && ts.IsAction == false || ts.TaskId == taskid && ts.ActionDesc == "Save" && ts.Periode == periode && ts.IsAction == false).FirstOrDefault();
                    edit.IsAction = true;
                    db.SaveChanges();
                }

            }
            comment.TaskId = taskid;
            comment.CommentMaker = userid;
            comment.CommentText = komentar;
            comment.Action = tsk.ActionDesc;
            db.Tbl_Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Index", "Task");
        }
        public string files;
        public string[] filed;
        [HttpPost]
        public ActionResult TrxCommPlanEdit(FormCollection form, int id)
        {
            int usrid = Convert.ToInt32(Session["loginuserid"]);
            var data = db.Mst_Users.Where(ui => ui.UserId == usrid).Single();
            var task = db.Trx_Comm_Plans.Where(ts => ts.TaskId == id).First();
            int taskid = task.TaskId;
            var atasan = db.Mst_Users.Where(us => us.UserId == data.DirectReportId).FirstOrDefault();
            string penerima = atasan.Email;
            string cekperiode = CekPeriode.CheckPeriode();
            //string periode = form["periode"] + " " + DateTime.Now.Year;

            Tbl_Task tsk = new Tbl_Task();

            int reportid = Convert.ToInt32(data.DirectReportId);
            tsk.TaskFor = reportid;
            tsk.TaskMaker = data.UserId;
            tsk.TaskName = "Commitment Plan";
            tsk.Description = data.FullName + " has submitted Commitment Plan for Periode " + cekperiode;
            tsk.Periode = cekperiode;
            int kondisi = db.Tbl_Tasks.Count();
            tsk.ActionDesc = "Submit";
            tsk.TaskId = taskid;
            int cektgl = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.Periode == cekperiode && ts.ActionDesc == "Submit" && ts.TaskName == "Commitment Plan").Count();
            if (cektgl != 0)
            {
                var cektanggalsubmit = db.Tbl_Tasks.Where(ts => ts.TaskId == id && ts.Periode == cekperiode && ts.ActionDesc == "Submit" && ts.TaskName == "Commitment Plan").FirstOrDefault();
                tsk.SubmitDate = cektanggalsubmit.SubmitDate;
            }
            else
            {
                tsk.SubmitDate = DateTime.Now;
            }


            db.Tbl_Tasks.Add(tsk);
            db.SaveChanges();
            int hit = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Revise" && ts.IsAction == false).Count();
            if (hit != 0)
            {
                var rm = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Revise" && ts.IsAction == false).First();
                rm.IsAction = true;
                db.SaveChanges();
            }
            int ceksave = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Save" && ts.IsAction == false).Count();
            if (ceksave != 0)
            {
                var rm = db.Tbl_Tasks.Where(ts => ts.TaskId == taskid && ts.ActionDesc == "Save" && ts.IsAction == false).First();
                rm.IsAction = true;
                db.SaveChanges();
            }

            string linknya = "http://localhost:53400/Task/TrxCommPlan/" + tsk.TaskId;
            string deskripsi = "Hi, " + atasan.FullName + "<br><br>" + tsk.Description + "<a href=" + linknya + "><br><br>Click Here to Review</a><br><br><p>Regards,</p><p>e-PRMS Admin</p>";
            //return Json(tsk, JsonRequestBehavior.AllowGet);
            SendEmail email = new SendEmail();
            email.Send(penerima, tsk.Description, deskripsi);


            var trxc = db.Trx_Comm_Plans.Where(tr => tr.TaskId == taskid);
            db.Trx_Comm_Plans.RemoveRange(trxc);
            int i = 0;
            while (true)
            {
                string hitung = i.ToString();
                if (form["detail_" + hitung] == null) break;
                string[] detail = form["detail_" + hitung].Split(char.Parse(","));
                string[] results = form["resultan_" + hitung].Split(char.Parse(","));
                string[] complaid = form["complainid_" + hitung].Split(char.Parse(","));
                HttpPostedFileBase file = Request.Files["file_" + hitung];
                if (file == null)
                {
                    filed = form["files_" + hitung].Split(char.Parse(","));
                    files = filed[0];
                }
                string a = UploadFiles(file);
                string dtku = detail[0];
                string hasil = results[0];
                string com = complaid[0];
                Trx_Comm_Plan trx = new Trx_Comm_Plan();
                trx.TaskId = taskid;
                trx.CommPlanid = Convert.ToInt16(com);
                trx.DescriptionPlan = dtku;
                trx.IsAchievable = hasil;
                trx.SubmitBy = usrid;
                trx.Periode = cekperiode;
                if (file != null)
                {
                    trx.File = a;
                }
                else
                {
                    trx.File = files;
                }
                db.Trx_Comm_Plans.Add(trx);
                db.SaveChanges();

                i++;
            }

            return RedirectToAction("Index", "Task");
            //return Json(tsk, JsonRequestBehavior.AllowGet);
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
                    //ViewBag.Message = "Upload sukses";
                }
                catch (Exception e)
                {
                    // ViewBag.Message = "Error" + e.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "coba lagi";
            }
            return i;
        }

        public ActionResult Download(string jeneng)
        {
            string actualPath = Server.MapPath("~/Files/") + jeneng;
            return File(actualPath, System.Net.Mime.MediaTypeNames.Application.Octet, jeneng);
        }

        [HttpPost]
        public ActionResult DownloadDelete(int trxid)
        {
            Trx_Comm_Plan trz = db.Trx_Comm_Plans.Find(trxid);
            string jeneng = trz.File;

            string actualPath = Server.MapPath("~/Files/") + jeneng;
            if (System.IO.File.Exists(actualPath))
            {
                System.IO.File.Delete(actualPath);
            }
            trz.File = "";
            db.Entry(trz).State = EntityState.Modified;
            db.SaveChanges();

            return Content("Success");
        }
    }
}
