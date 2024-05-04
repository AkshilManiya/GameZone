using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace GameZoneManagment.Controllers
{
    [Route("Slot")]
    public class Slot : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("UserSlotView/{id}")]
        public IActionResult UserSlotView(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT tblGame.*, tblClub.Club_name FROM tblGame inner join tblClub on tblClub.Club_id = tblGame.Club WHERE tblGame.Game_id = '" + id + "';", con);
            SqlDataReader result = cmd.ExecuteReader();
            if (result.Read())
            {
                ViewBag.image = result["Image"];
                ViewBag.game_name = result["Game_name"];
                ViewBag.club_name = result["Club_name"];
                ViewBag.offer = result["Offer"];
                ViewBag.members = result["Members"];
                ViewBag.rating = result["Rating"];
                ViewBag.caption = result["Caption"];
            }
            ViewBag.Id = id;
            return View("../Member/SlotView");
        }


        [Route("fetchSlotUser")]
        public IActionResult fetchSlotUser(int id, string day)
        {
            con.Open();
            cmd = new SqlCommand("SELECT * FROM tblSlot where Game='"+id+"' and Date='"+day+"';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            string morningslot = "";
            string afternoonslot = "";
            string nightslot = "";

            while (reader.Read())
            {
                string inputTime = reader["Starting_time"].ToString();
                DateTime time = DateTime.ParseExact(inputTime, "HH:mm:ss", null);
                string outputTime = time.ToString("h:mm tt");

                string btn = "";
                string dot = "";
                if (reader["State"].ToString() == "0")
                {
                    btn = "/Booking/Book/" + reader["Slot_id"];
                    btn = "<a class='slot-button-touch' href='" + btn + "' ></a>";
                    dot = "<div class='slot-status available'></div>";
                }
                else
                {
                    btn = "<div class='slot-button-touch'></div>";
                    dot = "<div class='slot-status booked'></div>";
                }
                string slotHtml =
"<div class='col-lg-3 col-12 mb-5 mb-lg-0'>" +
"<div class='slot-button'>" +
"" + btn  + "" +
"<div class='slot-time'>" +
"<div class='slot-time-text'>" + outputTime + " | " + ConvertTime(Convert.ToInt32(reader["Duration"])) + "</div>" +
"</div>" +
"<div class='slot-details'>" +
"<div class='slot-rent' > " + reader["Rant"] + "₹</div>" +
"</div>" + dot +
"</div>" +
"</div>";
                /*string slotHtml = "<td class='slot'>" +
                                    //"<div>" + ConvertTo12HourFormat(reader["Starting_time"].ToString()) + "</div>" +
                                    "<div>" + outputTime + "</div>" +
                                    "<div>" + ConvertTime(Convert.ToInt32(reader["Duration"])) + "</div>" +
                                    "<div>" + reader["Rant"] + "</div>" +
                                    "<div> " + btn + "<div>" +
                                 "</td>";*/

                /*
                string slotHtml = "<td><div class='col-lg-2 col-12 mb-5 mb-lg-0'>" +
                "<div class='slot-button'>" +
                "<div class='slot-time'>" +
                "<div class='slot-time-text'>" + reader["Starting_time"].ToString() + "</div>" +
                "</div>" +
                "<div class='slot-details'>" +
                "<div class='slot-rent'>" + ConvertTime(Convert.ToInt32(reader["Duration"])) + "</div>" +
                "<div class='slot-rent'>" + reader["Rant"] + "</div><br>" +
                "</div>" +
                "<div>" + btn + "<div>" +
                "</div>" +
                "</div></td>";*/
                int timeFrame = Convert.ToInt32(reader["Time_frame"]);

                if (timeFrame == 1)
                    morningslot += slotHtml;
                else if (timeFrame == 2)
                    afternoonslot += slotHtml;
                else if (timeFrame == 4)
                    nightslot += slotHtml;
            }

            userData.Add("morning", morningslot);
            userData.Add("afternoon", afternoonslot);
            userData.Add("night", nightslot);

            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");
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

        public static string ConvertTo12HourFormat(string time24Hour)
        {
            time24Hour = time24Hour.Trim();

            if (TimeSpan.TryParse(time24Hour, out TimeSpan time))
            {
                string time12Hour = time.ToString("hh:mm tt");
                return time12Hour;
            }
            else
            {
                return "error";
            }
        }
    }
}
