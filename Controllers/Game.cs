using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace GameZoneManagment.Controllers
{
    [Route("Game")]
    public class Game : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("ManageGameView")]
        public IActionResult ManageGameView()
        {
            return View("../Manager/ManageGame");
        }

        [Route("AddGameView")]
        public IActionResult AddGameView()
        {
            return View("../Manager/AddGame");
        }

        public string ClubID() {
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

        [Route("AddGame")]
        public string AddGame(string name, int member, string cap, string method, string image)
        {
            string club = ClubID();
            con.Open();
            cmd = new SqlCommand("INSERT INTO tblGame(Club, Offer, Rating, State, Game_name, Members, Image, Caption, Charge_method, Disable, Game_isDeleted) VALUES ('" + club + "' ,0,0,0,'" + name + "' , '" + member + "', '" + image + "', '" + cap + "', '" + method + "', '0','0');", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                return "success";
            }
            else
            {
                con.Close();
                return "INSERT INTO tblGame(Club, Game_name, Members, Image, Caption, Charge_method) VALUES('" + club + "', '" + name + "', '" + member + "', '" + image + "', '" + cap + "', '" + method + "');";
            }
        }

        [Route("UpdateGameView")]
        public IActionResult UpdateGameView(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from tblGame where Game_id = '" + id + "';", con);
            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
                userData.Add("game_id", result["Game_id"]);
                userData.Add("image", result["Image"]);
                userData.Add("offer", result["Offer"]);
                userData.Add("name", result["Game_name"]);
                userData.Add("method", result["Charge_method"]);
                userData.Add("member", result["Members"]);
                userData.Add("cap", result["Caption"]);
            }
            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");
        }



        [Route("fetchGame")]
        public IActionResult fetchGame(int area)
        {
            string club = ClubID();
            string html = "";
            con.Open();
            SqlCommand cmd = new SqlCommand("select tblGame.*, tblClub.Club_name from tblGame join tblClub on tblClub.Club_id = tblGame.Club where tblClub.Club_id='" + club + "';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            int i = 1;
            while (reader.Read())
            {
                string charge = "";
                if (reader["Charge_method"].ToString() == "1")
                {
                    charge = "per persone";
                }
                else {
                    charge = "per hours";
                }

                string btn = "";

                if ((bool)reader["Disable"])
                {
                    btn = "<button class='manageGameMain-table-tr-active-btn' onclick='enablegame(" + reader["Game_id"] + ")'>Active</button>";
                }
                else
                {
                    btn = "<button class='manageGameMain-table-tr-deactive-btn' onclick='disablegame(" + reader["Game_id"] + ")'>Disable</button>";
                }

                html += "<tr class='Admin-t-tr'>" +
                    "<td class='Admin-t-td'>" + i + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Game_name"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Offer"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Members"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Rating"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Caption"] + "</td> " +
                    "<td class='Admin-t-td'>" + charge + "</td> " +
                    "<td class='Admin-t-td'><button class='manageGameMain-table-tr-update-btn' onclick='updategameview(" + reader["Game_id"] + ")'>Update</button>"+btn+"</td>" +
                    "</tr>";

                i += 1;
            }
            con.Close();
            return Content(html, "text/html");
        }

        [Route("UpdateGame")]
        public string UpdateGame(int id, string name, int member, string cap, int method, int offer)
        {
            con.Open();
            cmd = new SqlCommand("update tblGame set Game_name='" + name + "', Offer='" + offer + "', Members='" + member + "', Caption='" + cap + "', Charge_method='" + method + "' where Game_id='" + id + "';", con);
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

        [Route("DisableGame")]
        public string DisableGame(int id)
        {
            con.Open();
            cmd = new SqlCommand("update tblGame set Disable='1' where Game_id=" + id + ";", con);
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

        [Route("EnableGame")]
        public string EnableGame(int id)
        {
            con.Open();
            cmd = new SqlCommand("update tblGame set Disable='0' where Game_id=" + id + ";", con);
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

        [Route("UpdateGameImage")]
        public string Image(int id, string img)
        {

            con.Open();
            cmd = new SqlCommand("update tblGame set Image = '" + img + "' where Game_id = '" + id + "';", con);
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

        // userside oprations

        // [Route("fetchAllClubDetails")]
        // public IActionResult fetchallClubdetails()
        // {
        //     string html = "";
        //     con.Open();
        //     cmd = new SqlCommand("SELECT tblClub.*, tblState.State_name, tblCity.City_name, tblArea.Area_name, tblManager.Name FROM tblClub inner join tblManager on tblManager.Manager_id = tblClub.Manager inner join tblArea on tblArea.Area_id = tblClub.Area INNER JOIN tblCity ON tblCity.City_id = tblArea.City INNER JOIN tblState ON tblState.State_id = tblCity.State WHERE Disable='0';", con);
        //     SqlDataReader reader = cmd.ExecuteReader();
        //     int i = 1;
        //     while (reader.Read())
        //     {
        //         html += "<tr>" +
        //             "<td> <a href='/Club/UserClubView/" + reader["Club_id"] + "'>" +
        //                 "<div><img src='/Assets/Club/" + reader["Image"] + "' style='width: 200px; height: 100px;'> </img></div>" +
        //             "</td>" +
        //             "<td>" +
        //                 "<div>" +
        //                     "<div><h3>" + reader["Club_name"] + "</h3></div>" +
        //                     "<div>" + reader["Address"] + "</div>" +
        //                     "<div>" + reader["Rating"] + "</div>" +
        //                 "</div>" +
        //             "</a></td>" +
        //             "</div>" +
        //             "</tr>";
        //     }
        //     con.Close();
        //     return Content(html, "text/html");
        // }

        [Route("fetchGamesUser/{id}")]
        public IActionResult fetchGamesUser(int id) {
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT * from tblGame where Club='"+id+"';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            int i = 1;
            while (reader.Read())
            {
                /*
                html += "<tr class='t-c-tr'>" +
                    "<td class='t-c-td'> <a href='/Slot/UserSlotView/" + reader["Game_id"] + "'>" +
                        "<div class='t-c-img'><img src='/Assets/Game/" + reader["Image"] + "' style='width: 200px; height: 100px;'> </img></div>" +
                    "</td>" +
                    "<td class='t-c-td'>" +
                        "<div class='t-c-td-div'>" +
                            "<div class='t-c-td-div2'><h3>" + reader["Game_name"] + "</h3></div>" +
                            "<div class='t-c-td-div2'>" + reader["Offer"] + "</div>" +
                            "<div class='t-c-td-div2'>" + reader["Caption"] + "</div>" +
                        "</div>" +
                    "</a></td>" +
                    "" +
                    "</tr>";*/

                html +=
                "<div class='col-lg-4 col-md-6 col-12 mb-4 custom-block-over'>" +
                "<div class='custom-block-wrap'>" +
                "<div class='custom-block-wrap-offer'>" + reader["Offer"] + "%</div>" +
                "<img src = '/Assets/Game/" + reader["Image"] + "' class='custom-block-image img-fluid' >" +
                "<div class='custom-block'>" +
                "<div class='custom-block-body'>" +
                "<h5 class='mb-3 gamesTitle'>" + reader["Game_name"] + "</h5>" +
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

        [Route("OfferGames")]
        public IActionResult offerGame() {
            string html = "";
            con.Open();
            cmd = new SqlCommand("SELECT tblgame.*, tblclub.Club_name FROM tblgame JOIN tblclub ON tblgame.Club = tblclub.Club_id WHERE Offer <> 0 ORDER BY Offer DESC", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                /* 
                html += "<div>" +
                    "<div><a href='/Slot/UserSlotView/" + reader["Game_id"] + "'><img src='/Assets/Game/" + reader["Image"] + "' style='width: 200px; height: 100px;'> </img></a></div>" +
                    "<div>" + reader["Game_name"] + "</div>" +
                    "<div> Club : " + reader["Club_name"] + "</div>" +
                    "<div>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div>" +
                    "</div></a>";*/

                html +=
                "<div class='col-lg-4 col-md-6 col-12 mb-4 custom-block-over'>" +
                "<div class='custom-block-wrap'>" +
                "<div class='custom-block-wrap-offer'>" + reader["Offer"] + "%</div>" +
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
    }
}
