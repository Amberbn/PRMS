using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adminlte.Controllers
{
    public class CekPeriode
    {
        public static string CheckPeriode()
        {
            DateTime now = DateTime.Today;

            int year = DateTime.Today.Year;
            int nextyear = DateTime.Today.Year + 1;
            string yearString = year.ToString();
            string nextyearString = nextyear.ToString();

            DateTime april = new DateTime(year, 4, 30, 23, 59, 59);
            DateTime oktober = new DateTime(year, 10, 31, 23, 59, 59);
            DateTime begin = new DateTime(year, 1, 1, 0, 0, 0);
            DateTime end = new DateTime(year, 12, 31, 23, 59, 59);

            string periode = "";
            if (now >= begin && now <= april)
            {
                periode = "April " + yearString;
            }
            else if (now > april && now <= oktober)
            {
                periode = "Oktober " + yearString;
            }
            else if (now > oktober && now <= end)
            {
                periode = "April " + nextyearString;
            }
            return periode;
        }
    }
}