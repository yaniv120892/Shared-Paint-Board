using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using My_Boarrd_App.Models;

namespace My_Boarrd_App.Controllers
{
    public class HomeController : Controller
    {
        private boardDBEntities boardDB = new boardDBEntities();

        public ActionResult Index(string id = null)
        {
            ViewBag.group_name = id == null? "null" : id;
            return View("Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Login(string user_name, string password)
        {
            string group_name = TempData["group_name"].ToString();
            int user_id = -1;
            string user_type = null;
            if (this.HttpContext.Request.Form["Login_Or_Register"] == "Register")
            {
                user_id = Register(user_name, password);
                if (user_id == -1)
                {
                    ViewBag.group_name = group_name;
                    ViewBag.Error = "Error, user name already exists";
                    return View("Login");
                }
                user_type = boardDB.tbl_users.Where(m => m.user_id == user_id).Select(m => m.type).Single();

            }
            else
            {
                string hashPass = password.GetHashCode() + "";
                var logged_in_user = boardDB.tbl_users.Where(m => m.username == user_name & m.password == hashPass).Select(m => m).SingleOrDefault();
                if (logged_in_user == null)
                {
                    ViewBag.group_name = group_name;
                    ViewBag.Error = "Error, password or user name are incorrect";
                    return View("Login");
                }
                //update useragent and ip
                user_id = logged_in_user.user_id;
                user_type = logged_in_user.type;
            }
            string ip = Request.UserHostAddress;
            string useragent = Request.UserAgent;
            return RedirectToAction("showBoard", "Board",new { user_id  = user_id, user_type = user_type, group_name = group_name, ip = ip, useragent = useragent});
        }
        public int Register(string user_name, string password)
        {
            int user_id = -1;
            string hashPass = password.GetHashCode() + "";
            var list_users_with_same_username = boardDB.tbl_users.Where(m => m.username == user_name).Select(m => m).ToList();
            if (list_users_with_same_username.Count > 0)
            {
                return user_id;
            }
            try
            {
                user_id = boardDB.tbl_users.Max(m => m.user_id) + 1;
            }
            catch(Exception e)
            {
                user_id = 1;
            }
            string user_type = "Registered";
            string ip = Request.UserHostAddress;
            string useragent = Request.UserAgent;

            boardDB.tbl_users.Add(new tbl_users(user_id, user_name, hashPass, ip, useragent, user_type));
            boardDB.SaveChanges();
            return user_id;
        }

    }
}