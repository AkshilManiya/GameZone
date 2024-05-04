using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace GameZoneManagment.Controllers
{
    [Route("Date")]
    public class DayTask : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlConnection con2 = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        string todayDate = DateTime.Today.ToString("yyyy-MM-dd");
        string oldDayDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
        string newDayDate = DateTime.Today.AddDays(6).ToString("yyyy-MM-dd");
        string newDayDateRowSlot = DateTime.Today.AddDays(13).ToString("yyyy-MM-dd");

        [Route("show")]
        public void DeleteSlot() {
            con.Open();
            cmd = new SqlCommand("delete from tblSlot where Date='"+oldDayDate+"' And State='0'", con);
            if (cmd.ExecuteNonQuery() > 0) {
                UpdateSlot();
            }
        }

        [Route("T")]
        public void UpdateSlot() {
            con.Close();
            con.Open();

            cmd = new SqlCommand("select tblRowSlot.*,tblDay.Date from tblRowslot join tblDay on tblDay.Day_id=tblRowslot.Day where tblDay.Date='"+newDayDate+"'",con);
            SqlDataReader result = cmd.ExecuteReader();
            while (result.Read())
            {
                int Game = Convert.ToInt32(result["Game"]);
                string TimeFrame = result["Time_frame"].ToString();
                DateTime dateTime = Convert.ToDateTime(result["Date"]);
                string Date = dateTime.Date.ToString("yyyy-MM-dd").ToString();
                string Starting_time = result["Starting_time"].ToString();
                int Duration = Convert.ToInt32(result["Duration"]);
                int Rant = Convert.ToInt32(result["Rant"]);

                con2.Open();
                using (SqlCommand insertCmd = new SqlCommand("INSERT INTO tblSlot VALUES (@Game, @TimeFrame, @Date, @Duration, @Starting_time, @Rant, '0')", con2))
                {
                    insertCmd.Parameters.AddWithValue("@Game", Game);
                    insertCmd.Parameters.AddWithValue("@TimeFrame", TimeFrame);
                    insertCmd.Parameters.AddWithValue("@Date", Date);
                    insertCmd.Parameters.AddWithValue("@Duration", Duration);
                    insertCmd.Parameters.AddWithValue("@Starting_time", Starting_time);
                    insertCmd.Parameters.AddWithValue("@Rant", Rant);

                    insertCmd.ExecuteNonQuery();
                }
                con2.Close();
                
            }
            con.Close();
            UpdateDay();
        }

        public string UpdateDay() {
            con.Open();
            cmd = new SqlCommand("update tblDay set Date='"+newDayDateRowSlot+"' where Date='"+newDayDate+"'", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                return "success";
            }
            else {
                return "fails";
            }
        }

        [Route("day")]
        public string ReturnDay() { 
            return todayDate.ToString()+" // "+oldDayDate+" // "+newDayDate+" // "+newDayDateRowSlot;
        }
    }
}
