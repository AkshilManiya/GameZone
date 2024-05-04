using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace GameZoneManagment.Controllers
{
    [Route("RowSlot")]
    public class RowSlot : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlConnection con2 = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("ManageSlotView")]
        public IActionResult ManageGameView()
        {
            return View("../Manager/ManageSlot");
        }

        [Route("AddGameView")]
        public IActionResult AddGameView()
        {
            return View("../Manager/AddGame");
        }

        [Route("updateRowSlotView")]
        public IActionResult updateRowSlotView(int id) {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from tblRowslot where Rowslot_id = '" + id + "';", con);
            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
                userData.Add("Rowslot_id", result["Rowslot_id"]);
                userData.Add("Starting_time", result["Starting_time"]);
                userData.Add("Duration", result["Duration"]);
                userData.Add("Rant", result["Rant"]);
                
            }
            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");

        }

        [Route("fetchRowSlot")]
        public IActionResult fetchRowSlot(string game , string day) 
        {
            string html = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("select tblRowslot.* from tblRowslot join tblDay on tblDay.Day_id=tblRowslot.Day where tblRowslot.Game='"+game+"' and tblRowslot.Day=(select tblDay.Day_id from tblDay where tblDay.Date='"+day+"');", con);
            SqlDataReader reader = cmd.ExecuteReader();
            int i = 1;
            while (reader.Read())
            {
                string inputTime = reader["Starting_time"].ToString();
                DateTime time = DateTime.ParseExact(inputTime, "HH:mm:ss", null);
                string outputTime = time.ToString("h:mm tt");


                html += "<tr class='Admin-t-tr'>" +
                    "<td class='Admin-t-td'>" + i + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Rowslot_id"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Day"] + "</td> " +
                    "<td class='Admin-t-td'>" + outputTime + "</td> " +
                    "<td class='Admin-t-td'>" + ConvertTime(Convert.ToInt32(reader["Duration"])) + "</td> " +
                    "<td class='Admin-t-td'>" + GetTimeFrame(reader["Starting_time"].ToString()) + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Rant"] + "</td> " +
                    "<td class='Admin-t-td'><button class='manageGameMain-table-tr-update-btn' onclick='changeslotview(" + reader["Rowslot_id"] + ")'>Change</button></td>" +
                    "</tr>";

                i += 1;
            }
            con.Close();
            return Content(html, "text/html");
        }

        public string GetTimeFrame(string timeInput)
        {
            // Parse the input time string to extract hours
            if (TimeSpan.TryParse(timeInput, out TimeSpan time))
            {
                // Define hour ranges for different time frames
                TimeSpan morningStart = new TimeSpan(6, 0, 0);
                TimeSpan afternoonStart = new TimeSpan(12, 0, 0);
                TimeSpan eveningStart = new TimeSpan(18, 0, 0);
                TimeSpan nightStart = new TimeSpan(0, 0, 0);

                // Check which time frame the input time falls into
                if (time >= morningStart && time < afternoonStart)
                {
                    return "Morning";
                }
                else if (time >= afternoonStart && time < eveningStart)
                {
                    return "Afternoon";
                }
                else if (time >= eveningStart && time < nightStart)
                {
                    return "Evening";
                }
                else
                {
                    return "Night";
                }
            }
            else
            {
                // Return an appropriate default value if the input time is not valid
                return "Night";
            }
        }

        public string ConvertTime(int minutes)
        {
            int hours = minutes / 60;
            int remainingMinutes = minutes % 60;

            if (hours > 0 && remainingMinutes > 0)
            {
                return $"{hours}h {remainingMinutes}m";
            }
            else if (hours > 0)
            {
                return $"{hours}h";
            }
            else if (remainingMinutes > 0)
            {
                return $"{remainingMinutes}m";
            }
            else
            {
                return "0m";
            }
        }

        [Route("AddRowSlot")]
        public string AddRowSlot(int game, string day , string time , int duration, int rant, int timeframe)
        {

            TimeSpan startingTime = TimeSpan.Parse(time);
            TimeSpan endTime = startingTime.Add(new TimeSpan(0, duration, 0));

            if (endTime.Days > 0)
            {
                return "duration";
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM tblRowslot WHERE Game = "+game+" AND Day = (SELECT Day_id FROM tblDay WHERE Date = '"+day+"') AND ((Starting_time <= '"+startingTime+"' AND DATEADD(MINUTE, Duration, Starting_time) > '"+startingTime+"') OR (Starting_time < '"+endTime+"' AND DATEADD(MINUTE, Duration, Starting_time) >= '"+endTime+"') OR ('"+startingTime+"' <= Starting_time AND DATEADD(MINUTE, Duration, Starting_time) <= '"+endTime+"'))", con2);
                con2.Open();
                if ((int)cmd2.ExecuteScalar() == 0)
                {
                    con2.Close();
                    con.Open();
                    cmd = new SqlCommand("INSERT INTO tblRowslot(Game, Time_frame, Day, Starting_time, Duration, Rant) VALUES ('" + game + "' ,'" + timeframe + "' , (select Day_id from tblDay where Date='" + day + "'), '" + time + ":00', '" + duration + "', '" + rant + "');", con);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        con.Close();
                        return "success";
                    }
                    else
                    {
                        con.Close();
                        return "error";
                    }
                }
                else {
                    con2.Close();
                    return "slot";
                }
            }


        }

        [Route("DeleteRowSlot")]
        public string DeleteRowSlot(int id)
        {
            con.Open();
            cmd = new SqlCommand("delete from tblRowslot where Rowslot_id='"+id+"'", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                return "success";
            }
            else
            {
                con.Close();
                return "error";
            }
        }

        [Route("UpdateRowSlot")]
        public string UpdateRowSlot(int id, string time, int duration, int rant, int timeframe, int game, string day)
        {
            TimeSpan startingTime = TimeSpan.Parse(time);
            TimeSpan endTime = startingTime.Add(new TimeSpan(0, duration, 0));

            if (endTime.Days > 0)
            {
                return "duration";
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM tblRowslot WHERE Rowslot_id != "+id+" AND Game = " + game + " AND Day = (SELECT Day_id FROM tblDay WHERE Date = '" + day + "') AND ((Starting_time <= '" + startingTime + "' AND DATEADD(MINUTE, Duration, Starting_time) > '" + startingTime + "') OR (Starting_time < '" + endTime + "' AND DATEADD(MINUTE, Duration, Starting_time) >= '" + endTime + "') OR ('" + startingTime + "' <= Starting_time AND DATEADD(MINUTE, Duration, Starting_time) <= '" + endTime + "'))", con2);
                con2.Open();
                if ((int)cmd2.ExecuteScalar() == 0)
                {
                    con2.Close();
                    con.Open();
                    cmd = new SqlCommand("update tblRowslot set Starting_time='"+time+"', Duration='"+duration+"', Time_frame='"+timeframe+"', Rant='"+rant+"' where Rowslot_id='" + id + "'", con);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        con.Close();
                        return "success";
                    }
                    else
                    {
                        con.Close();
                        return "error";
                    }
                }
                else
                {
                    con2.Close();
                    return "slot";
                }
            }


        }
    }
}
