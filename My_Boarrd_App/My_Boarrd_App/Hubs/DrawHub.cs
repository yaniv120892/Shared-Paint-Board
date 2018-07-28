using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using My_Boarrd_App.Models;

namespace My_Boarrd_App.Hubs
{
    [HubName("drawHub")]
    public class DrawHub : Hub
    {
        private boardDBEntities boardDB = new boardDBEntities();




                        



        public void drawOrEraseElement
            (string action, int startPointX, int startPointY,
            int endPointX, int endPointY,
            string color, string curElementType,
            int groupID, int user_id)
        {
            if (action == "Draw")
            {
                boardDB.tbl_elements_in_group_draw.Add(new tbl_elements_in_group_draw(groupID, startPointX, startPointY, endPointX, endPointY, color, curElementType, DateTime.Now, user_id));
                boardDB.SaveChanges();
                Clients.All.UpdateCanvas("Draw",startPointX, startPointY, endPointX, endPointY, color, curElementType, groupID);
            }
            if (action == "Erase")
            {
                var toDelete = getElemntToDelete(startPointX, startPointY);

                if (toDelete != null)
                {
                    boardDB.tbl_elements_in_group_draw.Remove(toDelete);
                    boardDB.SaveChanges();
                    Clients.All.UpdateCanvas("Erase", toDelete.startPointX, toDelete.startPointY, toDelete.endPointX, toDelete.endPointY, toDelete.color, toDelete.element_type, toDelete.group_id);
                }
            }
        }

        private tbl_elements_in_group_draw getElemntToDelete(int toDeletePointX, int toDeletePointY)
        {
            var listElementsToDelete = 
                boardDB.tbl_elements_in_group_draw.Where(m =>
            (m.element_type == "RECTANGULAR" 
                && m.startPointX <= toDeletePointX && toDeletePointX <= m.endPointX
                && m.startPointY <= toDeletePointY && toDeletePointY <= m.endPointY)
            ||
             (m.element_type == "LINE" 
                &&
                    (((toDeletePointX - m.startPointX) * 1.0 / (m.endPointX - m.startPointX))
                        /
                    ((toDeletePointY - m.startPointY) * 1.0 / (m.endPointY - m.startPointY)) >= 0.9)
                &&
                    (((toDeletePointX - m.startPointX) * 1.0 / (m.endPointX - m.startPointX))
                        /
                    ((toDeletePointY - m.startPointY) * 1.0 / (m.endPointY - m.startPointY)) <= 1.1)
                    )

                    ||
           (m.element_type == "CIRCLE"
                &&
                (Math.Pow(

                    Math.Pow((m.endPointX + m.startPointX) / 2.0 - toDeletePointX, 2)
                    +
                    Math.Pow((m.endPointY + m.startPointY) / 2.0 - toDeletePointY, 2)
                 , 0.5)
                 <=
                 (Math.Pow(
                     Math.Pow((m.endPointX - m.startPointX), 2)
                       +
                       Math.Pow(m.endPointY - m.startPointY, 2)
                  , 0.5)
                  +1))
                          ))
                   .Select(m => m).ToList();

            if (listElementsToDelete.Count > 0)
                return listElementsToDelete[0];
            return null;
        }
    }
}