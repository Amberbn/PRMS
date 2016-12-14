using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using adminlte.Models;
using adminlte.Context;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.IO;
using adminlte.Authorization;

namespace adminlte.Controllers
{
    [UserAuthorize]
    [PerfReviewAuthorize]
    public class Mst_Performance_ReviewController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        // GET: Mst_Performance_Review
        public ActionResult Index()
        {
            int userId = Convert.ToInt32(Session["loginuserid"]);
            bool isPerfReviewOpen = Convert.ToBoolean(Session["performance"]);
            ViewBag.periode = CekPeriode.CheckPeriode();
            ViewBag.isopen = isPerfReviewOpen;
            Mst_User mst_User = new Mst_User();
            string role = db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.UserId == userId).FirstOrDefault().Mst_Role.RoleName;
            if (role == "PMO")
            {
                ViewBag.UserId = new SelectList(db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName != "Management" && r.Mst_Role.RoleName != "PMO" && r.Mst_Role.RoleName != "Head of PMO" && r.Mst_Role.RoleName != "Head of Delivery"), "UserId", "FullName");
            }
            else if (role == "Team Leader")
            {
                ViewBag.UserId = new SelectList(db.Mst_Users.Where(r => r.DirectReportId == userId), "UserId", "FullName");
            }
            else if (role == "Head of PMO" || role == "Head of Delivery")
            {
                ViewBag.UserId = new SelectList(db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName != "Management" && r.Mst_Role.RoleName != "Head of PMO" && r.Mst_Role.RoleName != "Head of Delivery"), "UserId", "FullName");
            }
            else if (role == "Management")
            {
                ViewBag.UserId = new SelectList(db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName != "Management"), "UserId", "FullName");
            }

            if (Request.QueryString["UserId"] != null)
            {
                if (Request.QueryString["UserId"] != "0")
                {
                    string nama_r = "";
                    var req = Request.QueryString["UserId"];
                    int id = Convert.ToInt32(req);

                    var datasemua = db.Mst_Users.Include(r => r.Mst_Role).Include(r => r.Mst_RoleMap).Where(r => r.UserId == id).Single();
                    var data_r = db.Mst_Users.Where(r => r.UserId == datasemua.DirectReportId).SingleOrDefault();

                    if (data_r != null) nama_r = data_r.FullName.ToString();
                    else nama_r = "";

                    string periode = CekPeriode.CheckPeriode();
                    bool exist = ExistReview(userId, id, periode);
                    Debug.WriteLine("periode = " + periode + " , exist = " + exist);
                    //string periodePotong = periode.Remove(periode.Length - 5);

                    return Json(new { datasemua, nama_r, exist, periode }, JsonRequestBehavior.AllowGet);
                }
            }

            if (Request.QueryString["ReviewId"] != null)
            {
                if (Request.QueryString["ReviewId"] != "0")
                {
                    var req = Request.QueryString["ReviewId"];
                    int id = Convert.ToInt32(req);

                    var data2 = db.Mst_Perform_Details.Include(r => r.Mst_Perform).Include(r => r.Mst_Perform.Mst_RoleMap).Where(r => r.Mst_Perform.Mst_RoleMap.RoleMapId == id).OrderBy(r => r.Mst_Perform.PerformId);
                    return Json(data2, JsonRequestBehavior.AllowGet);
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int ReviewBy = Convert.ToInt32(Session["loginuserid"]);
            Mst_User mst_User = new Mst_User();
            string role = db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.UserId == ReviewBy).FirstOrDefault().Mst_Role.RoleName;
            if (role == "PMO")
            {
                ViewBag.UserId = new SelectList(db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName != "Management" && r.Mst_Role.RoleName != "PMO"), "UserId", "FullName");
            }
            else if (role == "Team Leader")
            {
                ViewBag.UserId = new SelectList(db.Mst_Users.Where(r => r.DirectReportId == ReviewBy), "UserId", "FullName");
            }
            else if (role == "Head of PMO" || role == "Head of Delivery")
            {
                ViewBag.UserId = new SelectList(db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName != "Management" && r.Mst_Role.RoleName != "Head of PMO" && r.Mst_Role.RoleName != "Head of Delivery"), "UserId", "FullName");
            }
            else if (role == "Management")
            {
                ViewBag.UserId = new SelectList(db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName != "Management"), "UserId", "FullName");
            }

            int ReviewFor = Convert.ToInt32(form["UserId"]);
            string Periode = form["Periode"];
            int TaskId = 0;
            if (db.Tbl_Tasks.Count() == 0)
            {
                TaskId = 1;
            }
            else
            {
                TaskId = db.Tbl_Tasks.Max(r => r.TaskId) + 1;
            }

            DateTime now = DateTime.Now.Date;
            int i = 0;
            while (true)
            {
                string hitung = i.ToString();
                if (form["PerformDetId" + hitung] == null) break;
                string[] PerformDetailIds = form["PerformDetId" + hitung].Split(char.Parse(","));
                string PerformDetailId = PerformDetailIds[0];
                int PerformDetailIdInt = Convert.ToInt32(PerformDetailId);
                string[] Scores = form["ScoreSelect" + hitung].Split(char.Parse(","));
                string Score = Scores[0];
                int ScoreInt = Convert.ToInt32(Score);
                string[] Notes = form["NotesInput" + hitung].Split(char.Parse(","));
                string Note = Notes[0];
                //string[] Directories = form["UploadFile" + hitung].Split(char.Parse(","));
                //string Directory = Directories[0];
                string Name = "";
                HttpPostedFileBase file = Request.Files["UploadFile" + i];
                if (file != null && file.ContentLength > 0)
                {
                    Name = saveFile(file, TaskId);
                }
                saveTrxPerformReview(PerformDetailIdInt, TaskId, ScoreInt, ReviewBy, ReviewFor, Periode, Note, Name);
                //Debug.WriteLine("file = "+file);
                i++;
            }

            saveTaskSubmit(TaskId, ReviewBy, ReviewFor, Periode, now.Date);

            string emailTujuan = db.Mst_Users.Where(r => r.Mst_Role.RoleName == "Management").FirstOrDefault().Email;
            string managementName = db.Mst_Users.Where(r => r.Mst_Role.RoleName == "Management").FirstOrDefault().FullName;
            string ReviewByName = db.Mst_Users.Find(ReviewBy).FullName;
            string ReviewForName = db.Mst_Users.Find(ReviewFor).FullName;
            string deskripsi = "Hi, " + managementName + "." + "<br><br>" + ReviewByName + " has submitted performance review for " + ReviewForName
                                + "<br><br>" + "<a href='http://localhost:53400/Mst_Performance_Review/Detail/" + TaskId + "'>Click here to take action</a>"
                                + "<br><br>" + "Regards,<br>e-PRMS Admin";
            string dateString = now.ToShortDateString();
            string subjek = ReviewByName + " Submit Performance Review for " + ReviewForName + " on " + dateString;
            string id = TaskId.ToString();

            SendEmail email = new SendEmail();

            email.Send(emailTujuan, subjek, deskripsi);

            return View();
        }

        private void saveTrxPerformReview(int pId, int tId, int nilai, int pereview, int direview, string periode, string catatan, string name)
        {
            Trx_Perform_Review Trx_Perform_Review = new Trx_Perform_Review();
            Trx_Perform_Review.PerformDetailId = pId;
            Trx_Perform_Review.TaskId = tId;
            Trx_Perform_Review.Score = nilai;
            Trx_Perform_Review.ReviewBy = pereview;
            Trx_Perform_Review.ReviewFor = direview;
            Trx_Perform_Review.Periode = periode;
            Trx_Perform_Review.Notes = catatan;
            Trx_Perform_Review.FileName = name;
            //Trx_Perform_Review.SubmitDate = date;
            db.Trx_Perform_Reviews.Add(Trx_Perform_Review);
            db.SaveChanges();
        }

        private void saveTaskSubmit(int tId, int tMaker, int reviewFor, string periode, DateTime date)
        {
            Tbl_Task Tbl_Task = new Tbl_Task();
            Tbl_Task.TaskId = tId;
            Tbl_Task.TaskName = "Review";
            int tFor = (db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName == "Management").SingleOrDefault()).UserId;
            Tbl_Task.TaskFor = tFor;
            string nameTaskFor = db.Mst_Users.Where(r => r.UserId == tFor).SingleOrDefault().FullName;
            Tbl_Task.TaskMaker = tMaker;
            string nameTaskMaker = db.Mst_Users.Where(r => r.UserId == tMaker).SingleOrDefault().FullName;
            string nameReviewFor = db.Mst_Users.Where(r => r.UserId == reviewFor).SingleOrDefault().FullName;
            string dateString = date.ToShortDateString();
            Tbl_Task.Description = nameTaskMaker + " has submitted a performance review for " + nameReviewFor + " on " + dateString;
            Tbl_Task.IsAction = false;
            Tbl_Task.ActionDesc = "Submit";
            Tbl_Task.Periode = periode;
            Tbl_Task.SubmitDate = date;

            Tbl_Task taskReject = new Tbl_Task();
            int countReject = db.Tbl_Tasks.Where(r => r.TaskId == tId && r.ActionDesc == "Revise" && r.IsAction == false).Count();
            if (countReject != 0)
            {
                taskReject = db.Tbl_Tasks.Where(r => r.TaskId == tId && r.ActionDesc == "Revise" && r.IsAction == false).FirstOrDefault();
                taskReject.IsAction = true;
            }

            db.Tbl_Tasks.Add(Tbl_Task);
            db.SaveChanges();
        }

        private string saveFile(HttpPostedFileBase file, int taskId)
        {

            //var file = Request.Files[directory];
            var InputFileName = Path.GetFileName(file.FileName);
            Directory.CreateDirectory(Server.MapPath("~/PerfReviewFiles/") + taskId.ToString());
            var ServerSavePath = Path.Combine(Server.MapPath("~/PerfReviewFiles/") + taskId.ToString() + "/" + InputFileName);
            file.SaveAs(ServerSavePath);

            return InputFileName;
        }

        // GET: Mst_Performance_Review/Edit/5
        public ActionResult Edit(int id)
        {
            int userId = Convert.ToInt32(Session["loginuserid"]);
            int countReview = db.Tbl_Tasks.Where(r => r.TaskId == id && r.TaskName == "Review").Count();
            if (countReview == 0)
            {
                return HttpNotFound();
            }
            int count = db.Tbl_Tasks.Where(r => r.TaskName == "Review" && r.TaskId == id && r.TaskFor == userId && r.ActionDesc == "Revise" && r.IsAction == false).Count();
            if (count == 0)
            {
                return HttpNotFound();
            }
            var list = db.Trx_Perform_Reviews.Include(r => r.Mst_Perform_Detail).Include(r => r.Mst_Perform_Detail.Mst_Perform).Where(r => r.TaskId == id).OrderBy(r => r.Mst_Perform_Detail.PerformId);

            string komentar = db.Tbl_Comments.Where(r => r.TaskId == id && r.Action == "Revise").OrderByDescending(r => r.Id).FirstOrDefault().CommentText;
            

            var mst_user = db.Mst_Users;
            var listFirst = list.FirstOrDefault();
            ViewBag.ReviewByName = mst_user.Where(r => r.UserId == listFirst.ReviewBy).FirstOrDefault().FullName;
            ViewBag.ReviewForName = mst_user.Where(r => r.UserId == listFirst.ReviewFor).FirstOrDefault().FullName;
            string periodePotong = listFirst.Periode.Remove(listFirst.Periode.Length - 5);
            ViewBag.Periode = periodePotong;
            ViewBag.Comment = komentar;

            return View(list.ToList());
        }

        // POST: Mst_Performance_Review/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            try
            {
                int TaskId = Convert.ToInt32(form["TaskId"]);
                int ReviewBy = Convert.ToInt32(form["ReviewBy"]);
                int ReviewFor = Convert.ToInt32(form["ReviewFor"]);
                string Periode = form["[0].Periode"];
                DateTime now = DateTime.Now.Date;
                int i = 0;
                while (true)
                {
                    if (form["[" + i.ToString() + "].TrxId"] == null) break;
                    string TrxIdString = form["[" + i.ToString() + "].TrxId"];
                    int TrxId = Convert.ToInt32(TrxIdString);
                    string PerformDetailIdString = form["[" + i.ToString() + "].PerformDetailId"];
                    int PerformDetailId = Convert.ToInt32(PerformDetailIdString);
                    string ScoreString = form["[" + i.ToString() + "].Score"];
                    int Score = Convert.ToInt32(ScoreString);
                    string Notes = form["[" + i.ToString() + "].Notes"];
                    string Name = "";
                    HttpPostedFileBase file = Request.Files["[" + i.ToString() + "].FileName"];
                    if (file != null && file.ContentLength > 0)
                    {
                        Name = saveFile(file, TaskId);
                    }
                    saveEditedTrx_Perform_Review(TrxId, PerformDetailId, TaskId, Score, ReviewBy, ReviewFor, Periode, Notes, Name);
                    i++;
                }

                saveTaskSubmit(TaskId, ReviewBy, ReviewFor, Periode, now.Date);

                string email = db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName == "Management").FirstOrDefault().Email;
                string managementName = db.Mst_Users.Include(r => r.Mst_Role).Where(r => r.Mst_Role.RoleName == "Management").FirstOrDefault().FullName;
                string ReviewByName = db.Mst_Users.Find(ReviewBy).FullName;
                string ReviewForName = db.Mst_Users.Find(ReviewFor).FullName;
                string nowString = now.ToShortDateString();
                string subjek = ReviewByName + " Submit Performance Review for " + ReviewForName + " on " + nowString;
                string deskripsi = "Hi, " + managementName + "." + "<br><br>" + ReviewByName + " has submitted performance review for " + ReviewForName
                                + "<br><br>" + "<a href='http://localhost:53400/Mst_Performance_Review/Detail/" + TaskId + "'>Click here to take action</a>"
                                + "<br><br>" + "Regards,<br>e-PRMS Admin";

                SendEmail send = new SendEmail();
                send.Send(email, subjek, deskripsi);

                return RedirectToAction("Index", "Task");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return RedirectToAction("Index", "Task");
            }
        }

        private void saveEditedTrx_Perform_Review(int TrxId, int PerformDetailId, int TaskId, int Score, int ReviewBy, int ReviewFor, string Periode, string Notes, string FileName)
        {
            Trx_Perform_Review Trx_Perform_Review = db.Trx_Perform_Reviews.Find(TrxId);
            Trx_Perform_Review.PerformDetailId = PerformDetailId;
            Trx_Perform_Review.TaskId = TaskId;
            Trx_Perform_Review.Score = Score;
            Trx_Perform_Review.ReviewBy = ReviewBy;
            Trx_Perform_Review.ReviewFor = ReviewFor;
            Trx_Perform_Review.Periode = Periode;
            Trx_Perform_Review.Notes = Notes;
            //Trx_Perform_Review.SubmitDate = date;
            if (Trx_Perform_Review.FileName == "")
            {
                Trx_Perform_Review.FileName = FileName;
            }
            db.Entry(Trx_Perform_Review).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult Detail(int id)
        {
            int countReview = db.Tbl_Tasks.Where(r => r.TaskId == id && r.TaskName == "Review").Count();
            if (countReview == 0)
            {
                return HttpNotFound();
            }
            int TaskFor = 0;
            int TaskMaker = 0;
            int userId = Convert.ToInt32(Session["loginuserid"]);
            Tbl_Task task = db.Tbl_Tasks.Where(r => r.TaskId == id && r.ActionDesc == "Submit").FirstOrDefault();
            TaskFor = task.TaskFor;
            TaskMaker = task.TaskMaker;
            if (!(userId == TaskFor || userId == TaskMaker))
            {
                return HttpNotFound();
            }
            var ReviewList = db.Trx_Perform_Reviews.Include(r => r.Mst_Perform_Detail).Include(r => r.Mst_Perform_Detail.Mst_Perform).Where(r => r.TaskId == id).OrderBy(r => r.Mst_Perform_Detail.PerformId);
            int ReviewFor = db.Trx_Perform_Reviews.Where(r => r.TaskId == id).FirstOrDefault().ReviewFor;
            int ReviewBy = db.Trx_Perform_Reviews.Where(r => r.TaskId == id).FirstOrDefault().ReviewBy;

            ViewBag.CekApprove = false;
            bool cekApprove = db.Tbl_Tasks.Where(r => r.TaskId == id && r.ActionDesc == "Approve" && r.IsAction == true).Any();
            if (cekApprove)
            {
                ViewBag.CekApprove = cekApprove;
            }

            ViewBag.nameReviewFor = db.Mst_Users.Where(r => r.UserId == ReviewFor).SingleOrDefault().FullName;
            ViewBag.nameReviewBy = db.Mst_Users.Where(r => r.UserId == ReviewBy).SingleOrDefault().FullName;
            return View(ReviewList);
        }

        [HttpPost]
        public ActionResult Detail(string submit, int id, string komentar)
        {

            //int id = Convert.ToInt32(stringId);
            Tbl_Task TaskSubmit = new Tbl_Task();
            TaskSubmit = db.Tbl_Tasks.Where(r => r.TaskId == id && r.ActionDesc == "Submit" && r.IsAction == false).FirstOrDefault();
            TaskSubmit.IsAction = true;
            db.Entry(TaskSubmit).State = EntityState.Modified;
            db.SaveChanges();

            string action = "";
            if (submit == "Approve")
            {
                action = "Approve";
            }
            else if (submit == "Revise")
            {
                action = "Revise";
            }

            SaveComments(id, TaskSubmit.TaskFor, komentar, action);
            makeTaskApproveRevise(submit, TaskSubmit, komentar);

            return RedirectToAction("Index", "Task");
        }

        public FileResult Download(string FileName, int TaskId)
        {
            return File(Server.MapPath("~/PerfReviewFiles/") + TaskId.ToString() + "/" + FileName, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        //public ActionResult Delete(string FileName, int TaskId, int TrxId)
        //{

        //    string path = Server.MapPath("~/PerfReviewFiles/") + TaskId.ToString() + "/" + FileName;
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //    }

        //    Trx_Perform_Review review = db.Trx_Perform_Reviews.Find(TrxId);
        //    review.FileName = "";
        //    db.Entry(review).State = EntityState.Modified;
        //    db.SaveChanges();

        //    return RedirectToAction("Edit", "Mst_Performance_Review", new { id = TaskId });
        //}

        public ActionResult Delete(int TrxId)
        {
            Debug.WriteLine("Insert Success");

            Trx_Perform_Review review = db.Trx_Perform_Reviews.Find(TrxId);
            int TaskId = review.TaskId;
            string FileName = review.FileName;

            string path = Server.MapPath("~/PerfReviewFiles/") + TaskId.ToString() + "/" + FileName;
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            //Trx_Perform_Review review = db.Trx_Perform_Reviews.Find(TrxId);
            review.FileName = "";
            db.Entry(review).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Edit", "Mst_Performance_Review", new { id = TaskId });
        }

        private void SaveComments(int id, int commentby, string comment, string action)
        {
            Tbl_Comment Tbl_Comment = new Tbl_Comment();
            Tbl_Comment.TaskId = id;
            Tbl_Comment.CommentMaker = commentby;
            Tbl_Comment.CommentText = comment;
            Tbl_Comment.Action = action;
            db.Tbl_Comments.Add(Tbl_Comment);
            db.SaveChanges();
        }

        private bool ExistReview(int ReviewBy, int ReviewFor, string periode)
        {
            int count = db.Trx_Perform_Reviews.Where(r => r.ReviewBy == ReviewBy && r.ReviewFor == ReviewFor && r.Periode == periode).Count();
            Debug.WriteLine("ReviewBy = " + ReviewBy + " , ReviewFor = " + ReviewFor + ", periode = " + periode);
            if (count > 0)
            {
                return true;
            }
            else return false;
        }

        private void makeTaskApproveRevise(string result, Tbl_Task task, string komentar)
        {

            Tbl_Task newTask = new Tbl_Task();
            newTask.TaskId = task.TaskId;
            newTask.TaskFor = task.TaskMaker;
            newTask.TaskMaker = task.TaskFor;
            newTask.TaskName = task.TaskName;
            newTask.Periode = task.Periode;
            newTask.SubmitDate = task.SubmitDate;
            //newTask.Description = task.Description;

            int ReviewFor = db.Trx_Perform_Reviews.Where(r => r.TaskId == task.TaskId).FirstOrDefault().ReviewFor;
            string nameReviewFor = db.Mst_Users.Find(ReviewFor).FullName;
            string emailFor = db.Mst_Users.Find(task.TaskMaker).Email;
            string managementName = db.Mst_Users.Find(newTask.TaskMaker).FullName;
            string PMOName = db.Mst_Users.Find(newTask.TaskFor).FullName;
            string subjek = "";
            string deskripsi = "";

            if (result == "Approve")
            {
                deskripsi = "Hi, " + PMOName + "." + "<br>" + managementName + " has approved your performance review for " + nameReviewFor
                                + "<br><br>" + "Comments :" + komentar + "<br><br>" + "Regards,<br>e-PRMS Admin";
                newTask.ActionDesc = "Approve";
                newTask.IsAction = true;
                subjek = managementName + " Has Approved Your Performance Review";
                newTask.Description = subjek;
            }
            else if (result == "Revise")
            {
                deskripsi = "Hi, " + PMOName + "." + "<br>" + managementName + " need revision from your performance review for " + nameReviewFor
                                + "<br><br>" + "Comments :" + komentar + "<br><br>" + "<a href='http://localhost:53400/Mst_Performance_Review/Edit/" + newTask.TaskId + "'>Click here to take action</a>"
                                + "<br><br>" + "Regards,<br>e-PRMS Admin";
                newTask.ActionDesc = "Revise";
                newTask.IsAction = false;
                subjek = managementName + " " + "Need Revision from Your Performance Review";
                newTask.Description = subjek;
            }

            db.Tbl_Tasks.Add(newTask);
            db.SaveChanges();

            SendEmail email = new SendEmail();
            email.Send(emailFor, subjek, deskripsi);
        }
    }
}
