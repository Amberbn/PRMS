using adminlte.Models;
using adminlte.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Net.NetworkInformation;
using Quartz;
using Quartz.Impl;
using System.Diagnostics;

namespace adminlte.Controllers
{
    public class SendEmail
    {
        Mst_UserDataContext db = new Mst_UserDataContext();
        public void Send(string emailTujuan, string subjek, string isi)
        {
            string emailPengirim = "prms.ebiz.test@gmail.com";

            using (var message = new MailMessage(emailPengirim, emailTujuan))
            {
                message.Subject = subjek;
                message.IsBodyHtml = true;
                message.Body = isi;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("prms.ebiz.test@gmail.com", "passwordprms")
                })
                {
                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception)
                    {
                        AddQueue(emailTujuan, message.Subject, message.Body);
                    }
                }
            }
        }

        public void SendQueue(int id, string subjek, string isi, string tujuan)
        {
            string emailPengirim = "prms.ebiz.test@gmail.com";

            using (var message = new MailMessage(emailPengirim, tujuan))
            {
                message.Subject = subjek;
                message.IsBodyHtml = true;
                message.Body = isi;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("prms.ebiz.test@gmail.com", "passwordprms")
                })
                {
                    try
                    {
                        client.Send(message);
                        DeleteQueue(id);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }
                }
            }
        }

        public class EmailJob : IJob
        {
            Mst_UserDataContext db = new Mst_UserDataContext();
            public void Execute(IJobExecutionContext context)
            {
                int count = db.Queue_Emails.Count();
                bool connected = CheckInternet();

                if (connected && count > 0)
                {
                    var first = db.Queue_Emails.FirstOrDefault();
                    SendEmail mail = new SendEmail();
                    mail.SendQueue(first.Id, first.Subject, first.Message, first.EmailTo);
                }

            }
            public class JobScheduler
            {
                public static void Start()
                {
                    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                    scheduler.Start();

                    IJobDetail job = JobBuilder.Create<EmailJob>().Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithDailyTimeIntervalSchedule
                          (s =>
                             s.WithIntervalInMinutes(1)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(9, 0))
                          )
                        .Build();

                    scheduler.ScheduleJob(job, trigger);
                }
            }
        }

        public class ReminderJob : IJob
        {
            Mst_UserDataContext db = new Mst_UserDataContext();
            public void Execute(IJobExecutionContext context)
            {
                
                if (CekTanggalReminder())
                {
                    SendEmail send = new SendEmail();
                    send.CreateReminder();
                }
            }
            public class JobScheduler
            {
                public static void Start()
                {
                    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                    scheduler.Start();

                    IJobDetail job = JobBuilder.Create<ReminderJob>().Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithDailyTimeIntervalSchedule
                          (s =>
                             s.OnEveryDay()
                             .WithRepeatCount(0)
                             .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(9, 0))
                          )
                        .Build();

                    scheduler.ScheduleJob(job, trigger);
                }
            }
        }

        public class OpenCloseJob : IJob
        {
            Mst_UserDataContext db = new Mst_UserDataContext();
            public void Execute(IJobExecutionContext context)
            {
                BukaTutup bukaTutup = new BukaTutup();
                string cek = CekTanggalBukaTutup();

                if (cek == "Buka")
                {
                    bukaTutup.OpenAll();
                }

                if (cek == "Tutup")
                {
                    bukaTutup.CloseAll();
                }
            }
            public class JobScheduler
            {
                public static void Start()
                {
                    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                    scheduler.Start();

                    IJobDetail job = JobBuilder.Create<OpenCloseJob>().Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithDailyTimeIntervalSchedule
                          (s =>
                             s.OnEveryDay()
                             .WithRepeatCount(0)
                             .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(9, 0))
                          )
                        .Build();

                    scheduler.ScheduleJob(job, trigger);
                }
            }
        }

        private void CreateReminder() {
            List<Mst_User> list = db.Mst_Users.ToList();
            string subjek = "Performance Review and Commitment Plan Reminder";
            for (int i = 0; i < list.Count; i++)
            {
                string tujuan = list[i].Email;
                string deskripsi = "Hi, " + list[i].FullName + "<br><p>We remind you to make commitment plan or performance review you haven't made in this period, because this period is soon to be closed.</p>"
                    + "<p>Regards,</p><p>e-PRMS Admin</p>";
                AddQueue(tujuan, subjek, deskripsi);
            }
        }

        private void AddQueue(string emailto, string subject, string message)
        {
            Queue_Email queue = new Queue_Email();
            queue.Subject = subject;
            queue.Message = message;
            queue.EmailTo = emailto;
            db.Queue_Emails.Add(queue);
            db.SaveChanges();
        }

        private void DeleteQueue(int id)
        {
            Queue_Email queue = db.Queue_Emails.Find(id);
            db.Queue_Emails.Remove(queue);
            db.SaveChanges();
        }

        public static bool CheckInternet()
        {
            Ping googlePing = new Ping();
            string host = "www.google.com";
            byte[] buffer = new byte[32];
            int timeout = 2000;
            PingOptions options = new PingOptions();
            try
            {
                PingReply reply = googlePing.Send(host, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool CekTanggalReminder() {
            DateTime now = DateTime.Now.Date;
            DateTime batasFebruari = new DateTime(DateTime.Now.Year, 2, 15);
            DateTime batasAgustus = new DateTime(DateTime.Now.Year, 8, 15);
            if (now == batasFebruari || now == batasAgustus)
            {
                return true;
            }
            else
                return false;
        }

        private static string CekTanggalBukaTutup() {

            DateTime now = DateTime.Now.Date;
            DateTime batasMaret = new DateTime(DateTime.Now.Year, 3, 15);
            DateTime batasMei = new DateTime(DateTime.Now.Year, 5, 1);
            DateTime batasSeptember = new DateTime(DateTime.Now.Year, 9, 15);
            DateTime batasNovember = new DateTime(DateTime.Now.Year, 11, 1);

            if (now == batasMaret || now == batasSeptember)
            {
                return "Tutup";
            }

            if (now == batasMei || now == batasNovember)
            {
                return "Buka";
            }

            return "";
        }
    }
}