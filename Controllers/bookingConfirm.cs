using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace GameZoneManagment.Controllers
{
    [Route("CB")]
    public class bookingConfirm : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlConnection con2 = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        public string ClubID()
        {
            string username = HttpContext.Session.GetString("Username").ToString();
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT tblClub.Club_id from tblManager join tblClub on tblClub.Manager = tblManager.Manager_id WHERE tblManager.Email = '" + username + "';", con);
            SqlDataReader result = cmd.ExecuteReader();
            string id;
            if (result.Read())
            {
                id = result["Club_id"].ToString();
            }
            else
            {
                id = "error";
            }
            con.Close();
            return id;
        }

        [Route("CBShow")]
        public IActionResult Index()
        {
            return View("../Manager/ConformBookingPage");
        }

        [Route("conform")]
        public IActionResult conform(int id)
        {

            string username = HttpContext.Session.GetString("Username").ToString();

            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT tblBookings.Booking_id, tblManager.Email, tblGame.Game_name, tblSlot.Starting_time, tblSlot.Date, tblMember.Name, tblbookings.Total_amount, tblbookings.Players, tblbookings.Check_in FROM tblBookings JOIN tblslot ON tblbookings.Slot = tblslot.Slot_id JOIN tblgame ON tblslot.Game = tblgame.Game_id JOIN tblclub ON tblgame.Club = tblclub.Club_id JOIN tblmanager ON tblclub.Manager = tblmanager.Manager_id JOIN tblmember ON tblbookings.Member = tblmember.Member_id WHERE tblBookings.Booking_id = '"+id+"';", con);
            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
                DateTime bookingDate = (DateTime)result["Date"];
                string date = bookingDate.ToString("yyyy-MM-dd");

                if (result["Email"].Equals(username))
                {
                    con2.Open();
                    SqlCommand cmd2 = new SqlCommand("UPDATE tblBookings SET Check_in = 1 WHERE Booking_id = '"+id+"';", con2);
                    if (cmd2.ExecuteNonQuery() > 0)
                    {
                        con2.Close();
                        userData.Add("Member_name", result["Name"]);
                        userData.Add("Game_name", result["Game_name"]);
                        userData.Add("Date", date);
                        userData.Add("Starting_time", result["Starting_time"]);
                        userData.Add("total_amount", result["Total_amount"]);
                        userData.Add("error", "Success");
                        userData.Add("username", username);
                        userData.Add("Players", result["Players"]);
                    }
                    else
                    {
                        con2.Close();
                        userData.Add("error", "Update");
                        userData.Add("username", username);
                    }
                }
                else {
                    userData.Add("error", "Manager");
                    userData.Add("username", username);
                }
            }
            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");
        }
    }
}
