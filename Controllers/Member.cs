using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameZoneManagment.Controllers
{
    [Route("/Member")]
    public class Member : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();
        string todayDate = DateTime.Today.ToString("yyyy-MM-dd");

        [Route("~/")]
        [Route("Home")]
        public IActionResult HomeView()
        {
            return View("MemberHome");
        }

        [Route("Profile")]
        public IActionResult ProfileView() { 
            return View("MemberProfile");
        }

        [Route("ClubView")]
        public IActionResult ClubView()
        {
            return View("../Member/TopClub");
        }

        [Route("GameView")]
        public IActionResult GameView()
        {
            return View("../Member/OfferGame");
        }

        [Route("AboutUsView")]
        public IActionResult AboutUsView()
        {
            return View("../Member/AboutUs");
        }

        [Route("ContactUsView")]
        public IActionResult ContactUsView()
        {
            return View("../Member/ContactUs");
        }

        [Route("MyBooingsView")]
        public IActionResult MyBooingsView()
        {
            return View("../Member/MyBookings");
        }

        [Route("MyWalletView")]
        public IActionResult MyWalletView()
        {
            return View("../Member/MyWallet");
        }

        [Route("fetch_topClub")]
        public IActionResult fetch_topClub()
        {
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT Top 3 tblclub.*, tblArea.Area_name, tblState.State_name, tblmanager.Mobile_no, (select count(*) from tblgame where Club=Club_id) as game_count from tblclub join tblarea on tblclub.Area = tblarea.Area_id join tblCity on tblCity.City_id = tblArea.City join tblState on tblState.State_id = tblCity.State join tblManager on tblclub.Manager = tblmanager.Manager_id ORDER BY Rating DESC;", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                html +=
                "<div class='col-lg-4 col-md-6 col-12 mb-4 custom-block-over'>" +
                "<div class='custom-block-wrap'>" +
                "<img src = '/Assets/Club/" + reader["Image"] + "' class='custom-block-image img-fluid' >" +
                "<div class='custom-block'>" +
                "<div class='custom-block-body'>" +
                "<h5 class='mb-3 gamesTitle'>" + reader["Club_name"] + "</h5>" +
                "<p class='gamesDetails'><strong> Games : " + reader["game_count"] + "</strong></p>" +
                "<div class='rating'>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div></strong>" +
                "</div>" +
                "<a href='/Club/UserClubView/" + reader["Club_id"] + "'><button class='custom-btn-book'>Book now</button></a>" +
                "</div>" +
                "</div>" +
                "</div>";
                /*
                html += "<div>" +
                    "<div><a href='/Club/UserClubView/" + reader["Club_id"] + "'><img src='/Assets/Club/" + reader["Image"] +"' style='width: 200px; height: 100px;'> </img></a></div>" +
                    "<div>" + reader["Club_name"] + "</div>" +
                    "<div> Games : " + reader["game_count"] + "</div>" +
                    "<div>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div>" +
                    "</div></a>";
                */

                //<button onclick="window.location.href='/Club/UserClubView/" + reader["Club_id"] + "'">
            }
            con.Close();
            return Content(html, "text/html");
        }

        [Route("fetch_topGame")]
        public IActionResult fetch_topGame()
        {
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT Top 3 tblgame.*, tblclub.Club_name FROM tblgame JOIN tblclub ON tblgame.Club = tblclub.Club_id ORDER BY Rating DESC;", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                html +=
                "<div class='col-lg-4 col-md-6 col-12 mb-4 custom-block-over'>" +
                "<div class='custom-block-wrap'>" +
                "<img src = '/Assets/Game/" + reader["Image"] + "' class='custom-block-image img-fluid' >" +
                "<div class='custom-block'>" +
                "<div class='custom-block-body'>" +
                "<h5 class='mb-3 gamesTitle'>" + reader["Game_name"] + "</h5>" +
                "<p class='gamesDetails'><strong> Club : " + reader["Club_name"] + "</strong></p>" +
                "<div class='rating'>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div></strong>" +
                "</div>" +
                "<a href='/Slot/UserSlotView/" + reader["Game_id"] + "'><button class='custom-btn-book'>Book now</button></a>" +
                "</div>" +
                "</div>" +
                "</div>";
                /*
                html += "<div>" +
                    "<div><a href='/Slot/UserSlotView/" + reader["Game_id"] + "'><img src='/Assets/Game/" + reader["Image"] + "' style='width: 200px; height: 100px;'> </img></a></div>" +
                    "<div>" + reader["Game_name"] + "</div>" +
                    "<div> Club : " + reader["Club_name"] + "</div>" +
                    "<div>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div>" +
                    "</div></a>";*/
            }
            con.Close();
            return Content(html, "text/html");
        }

        [Route("fetch_offerGame")]
        public IActionResult fetch_offerGame()
        {
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT Top 3 tblgame.*, tblclub.Club_name FROM tblgame JOIN tblclub ON tblgame.Club = tblclub.Club_id ORDER BY Offer DESC", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                /*html += "<div>" +
                    "<div><a href='/Slot/UserSlotView/" + reader["Game_id"] + "'><img src='/Assets/Game/" + reader["Image"] + "' style='width: 200px; height: 100px;'> </img></a></div>" +
                    "<div>" + reader["Game_name"] + "</div>" +
                    "<div> Club : " + reader["Club_name"] + "</div>" +
                    "<div>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div>" +
                    "</div></a>";
                */
                html +=
                "<div class='col-lg-4 col-md-6 col-12 mb-4 custom-block-over'>" +
                "<div class='custom-block-wrap'>" +
                "<div class='custom-block-wrap-offer'>" + reader["Offer"] +"%</div>" +
                "<img src = '/Assets/Game/" + reader["Image"] + "' class='custom-block-image img-fluid' >" +
                "<div class='custom-block'>" +
                "<div class='custom-block-body'>" +
                "<h5 class='mb-3 gamesTitle'>" + reader["Game_name"] + "</h5>" +
                "<p class='gamesDetails'><strong> Club : " + reader["Club_name"] + "</strong></p>" +
                "<div class='rating'>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div></strong>" +
                "</div>" +
                "<a href='/Slot/UserSlotView/" + reader["Game_id"] + "'><button class='custom-btn-book'>Book now</button></a>" +
                "</div>" +
                "</div>" +
                "</div>";
            }
            con.Close();
            return Content(html, "text/html");
        }

        [Route("fetch_todayBooking")]
        public IActionResult fetch_todayBooking()
        {
            string username = HttpContext.Session.GetString("Username").ToString();
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT tblBookings.*, tblGame.*, tblSlot.*, tblSlot.Date as slot_date FROM tblMember JOIN tblBookings ON tblBookings.Member = tblMember.Member_id join tblSlot on tblSlot.Slot_id=tblBookings.Slot JOIN tblGame ON tblGame.Game_id = tblSlot.Game WHERE tblMember.Email = '"+username+"' And tblSlot.Date='"+todayDate+"';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string inputTime = reader["Starting_time"].ToString();
                DateTime time = DateTime.ParseExact(inputTime, "HH:mm:ss", null);
                string outputTime = time.ToString("h:mm tt");
                DateTime bookingDate = (DateTime)reader["slot_date"];
                string date = bookingDate.ToString("yyyy-MM-dd");
                html += "<td>" +
                    "<div><a href='/Booking/UserBooking/" + reader["Booking_id"] +"'><img src='/Assets/Game/" + reader["Image"] + "' style='width: 200px; height: 100px;'> </img></a></div>" +
                    "<div>" + reader["Game_name"] + "</div>" +
                    "<div>" + date + "</div>" +
                    "<div>" + outputTime + "</div>" +
                    "<div>" + Booking.ConvertTime(Convert.ToInt32(reader["Duration"])) + "</div>" +
                    "<div>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div>" +
                    "</td></a>";
            }
            con.Close();
            return Content(html, "text/html");
        }

    }
}
