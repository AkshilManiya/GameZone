using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Data.SqlClient;

namespace GameZoneManagment.Controllers
{
    [Route("Booking")]
    public class Booking : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();
        string todayDate = DateTime.Today.ToString("yyyy-MM-dd");


        [Route("Book/{id}")]
        public IActionResult Index(int id)
        {
            string username = "";
            if (HttpContext.Session.TryGetValue("Username", out byte[] value))
            {
                username = HttpContext.Session.GetString("Username");
            }
            else
            {
                return View("../Home/Login");
            }

            con.Open();
            SqlCommand cmd = new SqlCommand("select tblSlot.*, tblGame.*, tblClub.Club_name from tblSlot join tblGame on tblGame.Game_id=tblSlot.Game join tblClub on tblClub.Club_id = tblGame.Club where tblSlot.Slot_id='"+id+"';", con);
            SqlDataReader result = cmd.ExecuteReader();
            int members;
            if (result.Read())
            {
                ViewBag.Image = result["Image"];
                ViewBag.Game_name = result["Game_name"];
                ViewBag.Club_name = result["Club_name"];
                members = Convert.ToInt32(result["Members"]);
                ViewBag.Offer = result["Offer"];
                ViewBag.Charge_method = result["Charge_method"];
                ViewBag.Slot_date = result["Date"];
                ViewBag.Slot_time = result["Starting_time"];
                ViewBag.Duration = ConvertTime(Convert.ToInt32(result["Duration"]));
                ViewBag.Rant = result["Rant"];

                ViewBag.Members = "";
                for (int i = 1; i <= members; i++)
                {
                    ViewBag.Members += $"<option value={i}>{i}</option>";
                }
            }
            con.Close();
            con.Open();
            SqlCommand cmd2 = new SqlCommand("select * from tblMember where Email='" + username + "';", con);
            SqlDataReader result2 = cmd2.ExecuteReader();
            if (result2.Read())
            {
                ViewBag.Member_id = result2["Member_id"];
                ViewBag.Member_name = result2["Name"];
                ViewBag.Contact = result2["Mobile_no"];
                ViewBag.Email = result2["Email"];
            }
            con.Close();
            ViewBag.Id = id;
            return View("../Member/BookingPage");
        }

        public static string ConvertTime(int minutes)
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

        [Route("checkWallet")]
        public Boolean checkWallet(int amount, int memberId)
        {
            con.Open();
            cmd = new SqlCommand("select * from tblMember where Member_id="+memberId+" And Wallet > "+amount+";", con);
            SqlDataReader result = cmd.ExecuteReader();
            if (result.Read())
            {
                con.Close();
                return true;
            }
            else {
                return false;
            }
        }

        [Route("BookSlot")]
        public string insert_bookings(int amount, int slotId, int memberId, int players, int Payment_method, string game_name) {
            amount = Convert.ToInt32(amount);
            con.Open();
            cmd = new SqlCommand("INSERT INTO tblBookings(Slot, Member, Date, players, Total_amount, Rating, Check_in) VALUES ('" + slotId + "' ,'" + memberId + "' , '" + todayDate + "', '" + players + "', '" + amount + "', '0', '0');", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                con.Open();
                cmd = new SqlCommand("update tblSlot set State=1 where Slot_id='" + slotId + "';", con);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    con.Close();
                    return updateAmount(slotId, amount, Payment_method, game_name);
                }
                else
                {
                    con.Close();
                    return "error update";
                }
            }
            else
            {
                con.Close();
                return "error booking";
            }
        }

        [Route("updateAmount")]
        public string updateAmount(int SlotId, int amount,int Payment_method, string game_name) {

            string username = HttpContext.Session.GetString("Username");

            cmd = new SqlCommand("INSERT INTO tblpayment(Login, Amount, Date, Opration, Game) VALUES ((SELECT Login_id FROM tblLogin WHERE Username = @username), @amount, @today_date, '1', @game_name)", con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@today_date", todayDate);
            cmd.Parameters.AddWithValue("@game_name", game_name);

            con.Open();
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                con.Open();
                cmd = new SqlCommand("UPDATE tblManager SET Wallet = Wallet + @amount WHERE Manager_id = (SELECT Manager FROM tblClub WHERE Club_id = (SELECT Club FROM tblGame WHERE Game_id = (select Game from tblSlot where Slot_id=@SlotId)))", con);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@SlotId", SlotId);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    con.Close();
                    if (Payment_method == 0)
                    {
                        con.Open();
                        cmd = new SqlCommand("update tblMember set Wallet=(select Wallet from tblMember where Email=@username) - @amount where Email=@username;", con);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@amount", amount);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            con.Close();
                            return "success";
                        }
                        else
                        {
                            con.Close();
                            return "Error: Inserting into tblpayment";
                        }
                    }
                    else {
                        return "success";
                    }
                }
                else
                {
                    con.Close();
                    return "Error: Updating tblManager";
                }
            }
            else
            {
                con.Close();
                return "Error: Updating tblMember";
            }

        }

        // fetch Bookings
        [Route("fetchUserBookings")]
        public IActionResult fetchUserBookings()
        {
            string username = HttpContext.Session.GetString("Username").ToString();
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT tblBookings.*, tblClub.Club_name, tblGame.Game_name, tblSlot.*, tblSlot.Date as slot_date FROM tblMember JOIN tblBookings ON tblBookings.Member = tblMember.Member_id join tblSlot on tblSlot.Slot_id=tblBookings.Slot JOIN tblGame ON tblGame.Game_id = tblSlot.Game join tblClub on tblClub.Club_id=tblGame.Club WHERE tblMember.Email = '" + username + "';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string inputTime = reader["Starting_time"].ToString();
                DateTime time = DateTime.ParseExact(inputTime, "HH:mm:ss", null);
                string outputTime = time.ToString("h:mm tt");
                DateTime bookingDate = (DateTime)reader["slot_date"];
                string date = bookingDate.ToString("yyyy-MM-dd");

                html +=
                    "<a class='mybooking-set-items-btn' href='/Booking/UserBooking/" + reader["Booking_id"] + "' >" +
                    "<div class='mybooking-item-num'>" + reader["Booking_id"] + "</div>" +
                    "<div class='mybooking-item-division'></div> " +
                    "<div class='mybooking-item-info'>" +
                    "<div class='mybooking-item-info-up'>" +
                    "   <div class='mybooking-item-info-up-l'> " + reader["Game_name"] + "</div>" +
                    "   <div class='mybooking-item-info-up-r'> " +
                    "       <i class='ri-map-pin-fill' style='color: #45d100;'></i>" + reader["Club_name"] +
                    "</div>" +
                    "</div>" +
                    "<div class='mybooking-item-info-down'> " + date + " and " + outputTime + "</div>" +
                    "</div>" +
                    "<div class='mybooking-item-division'></div> " +
                    "<div class='mybooking-item-rating-btn'>" + generatRating(Convert.ToInt32(reader["Rating"])) + "</div>" +
                    "</a>";
                /*
                html += "<tr>" +
                    "<td> <a href='/Booking/UserBooking/" + reader["Booking_id"] +"'><img src='/Assets/Game/" + reader["Image"] + "' style='width: 200px; height: 100px;'></img></a></td>" +
                    "<td>" + reader["Game_name"] +"</td>" +
                    "<td>" + date + "</td>" +
                    "<td>" + club_name + "</td>" +
                    "<td>" + outputTime +"</td>" +
                    "<td>" + ConvertTime(Convert.ToInt32(reader["Duration"])) + "</td>" +
                    "<td>" + generatRating(Convert.ToInt32(reader["Rating"])) + "</td>" +
                    "</tr></a>";
            }*/
            }
                con.Close();
                return Content(html, "text/html");
        }

        public static string generatRating(int rating) {
            string color = "★";
            string non = "☆";
            string final = "";
            for (int i=0;i<5;i++) {
                if (i < rating)
                {
                    final += color;
                }
                else {
                    final += non;
                }
            }
            return final;
		}

        [Route("UserBooking/{id}")]
        public IActionResult fetchUserBooking(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT tblBookings.*, tblGame.Rating as rating, tblBookings.Date as Booking_date, tblGame.*, tblSlot.Date as Slot_date , tblSlot.*, tblClub.*, tblManager.Name as Manager_name,tblManager.Mobile_no as Manager_contact FROM tblMember JOIN tblBookings ON tblBookings.Member = tblMember.Member_id join tblSlot on tblSlot.Slot_id=tblBookings.Slot JOIN tblGame ON tblGame.Game_id = tblSlot.Game join tblClub on tblClub.Club_id=tblGame.Club join tblManager on tblManager.Manager_id=tblClub.Manager WHERE tblBookings.Booking_id="+id+";", con);
            SqlDataReader result = cmd.ExecuteReader();
            int members;
            if (result.Read())
            {
                ViewBag.Image = result["Image"];
                ViewBag.Game_name = result["Game_name"];
                ViewBag.Club_name = result["Club_name"];
                ViewBag.Rating = result["rating"];
                ViewBag.BookingRating = result["Rating"];

                ViewBag.Manager_name = result["Manager_name"];
                ViewBag.Contact = result["Manager_contact"];
                ViewBag.Address = result["Address"];


                ViewBag.Booking_id = result["Booking_id"];
                ViewBag.Check_in = result["Check_in"];

                DateTime bookingDate = (DateTime)result["Booking_date"];
                ViewBag.Booking_date = bookingDate.ToString("yyyy-MM-dd");
                DateTime slotDate = (DateTime)result["Slot_date"];
                ViewBag.Slot_date = slotDate.ToString("yyyy-MM-dd");

                ViewBag.DateDiff = DateDiff(slotDate.ToString("yyyy-MM-dd"));

                ViewBag.Slot_time = result["Starting_time"];
                ViewBag.Duration = ConvertTime(Convert.ToInt32(result["Duration"]));
                ViewBag.Rant = result["Rant"];
                int rant = Convert.ToInt32(result["Rant"]);
                ViewBag.Players = result["Players"];
                ViewBag.Offer = rant * Convert.ToInt32(result["Offer"]) / 100;
                ViewBag.Tax = rant * 0.05;
                ViewBag.amount = result["Total_amount"];
            }
            con.Close();
            ViewBag.Id = id;
            return View("../Member/BookingConfirm");
        }


        public int DateDiff(string dateString)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            DateTime dayAfterTomorrow = today.AddDays(2);

            if (DateTime.Parse(dateString).Date == tomorrow.Date)
            {
                return 1;
            }
            else if (DateTime.Parse(dateString).Date == dayAfterTomorrow.Date)
            {
                return 1;
            }
            else if (DateTime.Parse(dateString).Date == today.Date)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        [Route("cancelBooking")]
        public string cancelbooking(int id) {

            string username = HttpContext.Session.GetString("Username").ToString();


            con.Close();
            con.Open();
            cmd = new SqlCommand("INSERT INTO tblpayment(Login, Amount, Date, Opration, Game) VALUES ((SELECT Login_id FROM tblLogin WHERE Username = '" + username+"'), (select Total_amount from tblbookings where Booking_id="+id+"), '"+todayDate+"', '2', (select Game_name from tblgame where Game_id=(select Game from tblslot where Slot_id=(select Slot from tblbookings where Booking_id="+id+"))));", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                con.Open();
                cmd = new SqlCommand("update tblslot set State=0 where Slot_id=(select Slot from tblbookings where Booking_id='"+id+"')", con);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    con.Close();
                    con.Open();
                    cmd = new SqlCommand("update tblMember set Wallet=Wallet + (select Total_amount from tblbookings where Booking_id="+id+") where Email='"+username+"';", con);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        con.Close();
                        con.Open();
                        cmd = new SqlCommand("UPDATE tblManager SET Wallet = Wallet - (select Total_amount from tblbookings where Booking_id='"+id+"') WHERE Manager_id = (SELECT Manager FROM tblClub WHERE Club_id = (SELECT Club FROM tblGame WHERE Game_id=(select Game from tblslot where Slot_id=(select Slot from tblbookings where Booking_id='"+id+"'))))", con);
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            con.Close();
                            con.Open();
                            cmd = new SqlCommand("delete from tblbookings where Booking_id = "+id+";", con);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                con.Close();
                                return "success";
                            }
                            else
                            {
                                con.Close();
                                return "error update";
                            }
                        }
                        else
                        {
                            con.Close();
                            return "error update";
                        }
                    }
                    else
                    {
                        con.Close();
                        return "error update";
                    }
                }
                else
                {
                    con.Close();
                    return "error update";
                }
            }
            else
            {
                con.Close();
                return "error update";
            }
        }

        [Route("storeRating")]
        public string storeRating(int rating, int id) {
            con.Open();
            cmd = new SqlCommand("update tblbookings set Rating="+rating+" where Booking_id="+id+";", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                return "success";
            }
            else
            {
                con.Close();
                return "error update";
            }
        }
    }
}