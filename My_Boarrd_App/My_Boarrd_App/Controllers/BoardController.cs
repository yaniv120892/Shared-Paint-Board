using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using My_Boarrd_App.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace My_Boarrd_App.Controllers
{
    public class BoardController : Controller
    {
        private boardDBEntities boardDB = new boardDBEntities();
        // GET: Board
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult showBoard(int user_id, string user_type, string group_name,string ip, string useragent)
        {
            int groupID = 1;
            if (group_name != "null")
            {
                groupID = boardDB.tbl_groups.Where(m => m.group_name.Equals(group_name)).Select(m => m.group_id).SingleOrDefault();
                if(groupID == 0)
                {
                    groupID = boardDB.tbl_groups.Select(m => m.group_id).Max() + 1;
                    boardDB.tbl_groups.Add(new tbl_groups(groupID, group_name));
                }
            }
                
            tbl_elements_in_group_draw[] elementsInDraw =  boardDB.tbl_elements_in_group_draw.Select(m => m).Where(m=>m.group_id == groupID).ToArray();
            var jsoElementsInDraw = JsonConvert.SerializeObject(elementsInDraw);
            ViewBag.jsoElementsInDraw = jsoElementsInDraw;
            ViewBag.groupID = groupID;
            ViewBag.groupName = group_name == "null"? "public" : group_name;
            ViewBag.userID = user_id;
            ViewBag.userType = user_type;
            ViewBag.useragent = useragent;
            ViewBag.ip = ip;
            ViewBag.username = boardDB.tbl_users.Where(m => m.user_id == user_id).Select(m => m.username).SingleOrDefault();
            return View();
        }
    }
}