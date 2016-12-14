using adminlte.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using adminlte.Models;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.Entity;
using adminlte.Authorization;

namespace adminlte.Controllers
{
    public class RequestController : Controller
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        SendEmail send = new SendEmail();
        // GET: Request/Create
        [UserAuthorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Request/Create
        [HttpPost]
        public ActionResult Create(Request_Ticket req)
        {
            try
            {
               
                Request_Ticket tiket = new Request_Ticket();
                int hitung = db.Tbl_Tasks.Count();
                int userid = Convert.ToInt32(Session["loginuserid"]);
                tiket.UserId = userid;
                tiket.Reason = req.Reason;
                tiket.Date = DateTime.Now.Date;
                tiket.RequestType = req.RequestType;
                if(hitung!=0)
                {
                    tiket.TaskId = db.Tbl_Tasks.Max(ax => ax.TaskId) + 1;
                }else
                {
                    tiket.TaskId = 1;
                }
                
                //return Json(tiket, JsonRequestBehavior.AllowGet);
                db.Request_Tickets.Add(tiket);
                db.SaveChanges();

                var user = db.Mst_Users.Include(rl => rl.Mst_Role).Where(rl => rl.Mst_Role.RoleName == "Admin").FirstOrDefault();
                var loginuser = db.Mst_Users.Where(us => us.UserId == userid).FirstOrDefault();
                string cekperiode = CekPeriode.CheckPeriode();
                Tbl_Task tsk = new Tbl_Task();
                tsk.TaskId = tiket.TaskId;
                tsk.TaskMaker = userid;
                tsk.TaskFor = user.UserId;
                tsk.ActionDesc = "Request";
                tsk.IsAction = false;
                if(req.RequestType=="Commitment Plan")
                {
                    tsk.Description = loginuser.FullName + " has request Commitment Plan ticket for Periode " + cekperiode;
                    tsk.TaskName = "Commitment Plan";
                }
                else
                {
                    tsk.Description = loginuser.FullName + " has request Performance Review ticket for Periode " + cekperiode;
                    tsk.TaskName = "Performance Review";
                }
                tsk.SubmitDate = DateTime.Now;
                tsk.Periode = cekperiode;
                db.Tbl_Tasks.Add(tsk);
                db.SaveChanges();

                string tujuan = user.Email;
                string deskripsi = "Hi, " + user.FullName + "." + "<br>" + tsk.Description
                                + "<br><br>" + "Reason :" + tiket.Reason + "<br><br>" + "<a href='http://localhost:53400/Request/Detail/" + tiket.TaskId + "'>Click here to take action</a>"
                                + "<br><br>" + "Regards,<br>" + loginuser.FullName;
                string subjek = tsk.Description;
                send.Send(tujuan, subjek, deskripsi);

                return RedirectToAction("Index", "Task");
            }
            catch
            {
                return View();
            }
        }

        // GET: Request/Edit/5
        [AdminAuthorize]
        public ActionResult Detail(int id)
        {
            bool any = db.Request_Tickets.Where(r => r.TaskId == id).Any();
            if (any == false)
            {
                return RedirectToAction("Error", "Error");
            }
            var edit = db.Request_Tickets.Include(us=>us.Mst_User).Include(rl=>rl.Mst_User.Mst_Role).Where(rq=>rq.TaskId==id).FirstOrDefault();
            return View(edit);
        }

        // POST: Request/Edit/5
        [HttpPost]
        public ActionResult Detail(int id, FormCollection collection)
        {
            try
            {
                string aksi = collection["aksi"];
                //return Json(collection, JsonRequestBehavior.AllowGet);
                string cekperiode = CekPeriode.CheckPeriode();
                var req = db.Request_Tickets.Where(rq => rq.TaskId == id).FirstOrDefault();
                var user = db.Mst_Users.Include(rl => rl.Mst_Role).Where(rl => rl.Mst_Role.RoleName == "Admin").FirstOrDefault();
                int userid = Convert.ToInt32(Session["loginuserid"]);
                var userreq = db.Mst_Users.Find(req.UserId);
                string request = req.RequestType;
                string komentar = collection["komentar"];
                var edit = db.Mst_Users.Where(us => us.UserId == req.UserId).FirstOrDefault();
                if (request== "Commitment Plan"&&aksi=="Approve")
                {
                    edit.CommPlanOpen = true;
                    string tujuan = userreq.Email;
                    string deskripsi = "Hi, " + userreq.FullName + "." + "<br>Your request to open commitment plan has been approved by admin."
                                    + "<br><br>" + "Comments :" + komentar
                                    + "<br><br>" + "Regards,<br>e-PRMS Admin";
                    string subjek = "Your Request to Open Commitment Plan Has Been Approved";
                    send.Send(tujuan, subjek, deskripsi);
                }
                else if(request == "Performance Review" && aksi == "Approve")
                {
                    edit.PerfReviewOpen = true;
                    string tujuan = userreq.Email;
                    string deskripsi = "Hi, " + userreq.FullName + "." + "<br>Your request to open performance review has been approved by admin."
                                    + "<br><br>" + "Comments :" + komentar
                                    + "<br><br>" + "Regards,<br>e-PRMS Admin";
                    string subjek = "Your Request to Open Performance Review Has Been Approved";
                    send.Send(tujuan, subjek, deskripsi);
                }
                db.SaveChanges();
                var task = db.Tbl_Tasks.Where(ts => ts.TaskMaker == edit.UserId && ts.Periode == cekperiode && ts.ActionDesc == "Request" && ts.IsAction == false && ts.TaskName == request).FirstOrDefault();
                //return Json(user, JsonRequestBehavior.AllowGet);
                task.IsAction = true;
                db.SaveChanges();
                Tbl_Comment komen = new Tbl_Comment();
                komen.TaskId = req.TaskId;
                komen.CommentMaker = userid;
                komen.CommentText = komentar;

                komen.Action = aksi;
                db.Tbl_Comments.Add(komen);
                if (request == "Commitment Plan" && aksi == "Reject")
                {
                    var editreject = db.Tbl_Tasks.Where(ts => ts.TaskId == req.TaskId).FirstOrDefault();
                    editreject.IsAction = true;
                    string tujuan = userreq.Email;
                    string deskripsi = "Hi, " + userreq.FullName + "." + "<br>Your request to open commitment plan has been rejected by admin."
                                    + "<br><br>" + "Comments :" + komentar
                                    + "<br><br>" + "Regards,<br>e-PRMS Admin";
                    string subjek = "Your Request to Open Commitment Plan Has Been Rejected";
                    send.Send(tujuan, subjek, deskripsi);
                }
                else if (request == "Performance Review" && aksi == "Reject")
                {
                    var editreject = db.Tbl_Tasks.Where(ts => ts.TaskId == req.TaskId).FirstOrDefault();
                    editreject.IsAction = true;
                    string tujuan = userreq.Email;
                    string deskripsi = "Hi, " + userreq.FullName + "." + "<br>Your request to open performance review has been rejected by admin."
                                    + "<br><br>" + "Comments :" + komentar
                                    + "<br><br>" + "Regards,<br>e-PRMS Admin";
                    string subjek = "Your Request to Open Performance Review Has Been Rejected";
                    send.Send(tujuan, subjek, deskripsi);
                }
                db.SaveChanges();
                return RedirectToAction("Index", "Task");
            }
            catch
            {
                return View();
            }
        }
    }
}
