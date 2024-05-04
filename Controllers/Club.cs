using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace GameZoneManagment.Controllers
{
    [Route("Club")]
    public class Club : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("AddClub")]
        public string AddClub(int area, int manager, string name, string addr, string date, string image)
        {
            con.Open();
            cmd = new SqlCommand("INSERT INTO tblclub(Area, Manager, Club_name, Address, Joining_date, Image, Rating, Disable, Club_isDeleted) VALUES ('" + area + "' ,'" + manager + "' , '" + name + "', '" + addr + "', '" + date + "', '" + image + "', 0,0,0);", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                return "success";
            }
            else
            {
                con.Close();
                return "error" + "INSERT INTO tblclub VALUES (NUll, '" + area + "' ,'" + manager + "' , '" + name + "', '" + addr + "', '" + date + "', '" + image + "', 0,0,0);";
            }
        }

        [Route("UpdateClubView")]
        public IActionResult UpdateClubView(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT tblClub.*, tblState.State_id, tblCity.City_id, tblArea.Area_id, tblManager.Manager_id FROM tblClub inner join tblManager on tblManager.Manager_id = tblClub.Manager inner join tblArea on tblArea.Area_id = tblClub.Area INNER JOIN tblCity ON tblCity.City_id = tblArea.City INNER JOIN tblState ON tblState.State_id = tblCity.State WHERE tblClub.Club_id = '" + id + "';", con);
            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
                userData.Add("club_id", result["Club_id"]);
                userData.Add("image", result["Image"]);
                userData.Add("name", result["Club_name"]);
                userData.Add("state", result["State_id"]);
                userData.Add("city", result["City_id"]);
                userData.Add("area", result["Area_id"]);
                userData.Add("manager", result["Manager_id"]);
                userData.Add("addr", result["Address"]);
                userData.Add("joining_date", result["Joining_date"]);
            }
            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");
        }



        [Route("fetchClub")]
        public IActionResult fetchClub(int area, string text)
        {
            string html = "";
            con.Open();
            if (area != 0)
            {
                cmd = new SqlCommand("SELECT tblClub.*, tblState.State_name, tblCity.City_name, tblArea.Area_name, tblManager.Name FROM tblClub INNER JOIN tblManager ON tblManager.Manager_id = tblClub.Manager INNER JOIN tblArea ON tblArea.Area_id = tblClub.Area INNER JOIN tblCity ON tblCity.City_id = tblArea.City INNER JOIN tblState ON tblState.State_id = tblCity.State WHERE tblClub.Area = @area AND Club_name LIKE @text;", con);
                cmd.Parameters.AddWithValue("@area", area);
                cmd.Parameters.AddWithValue("@text", text + "%");
            }
            else
            {
                cmd = new SqlCommand("SELECT tblClub.*, tblState.State_name, tblCity.City_name, tblArea.Area_name, tblManager.Name FROM tblClub INNER JOIN tblManager ON tblManager.Manager_id = tblClub.Manager INNER JOIN tblArea ON tblArea.Area_id = tblClub.Area INNER JOIN tblCity ON tblCity.City_id = tblArea.City INNER JOIN tblState ON tblState.State_id = tblCity.State WHERE Club_name LIKE @text;", con);
                cmd.Parameters.AddWithValue("@text", text + "%");

            }
            SqlDataReader reader = cmd.ExecuteReader();
            int i = 1;
            while (reader.Read())
            {
                string btn = "";

                if ((bool)reader["Disable"])
                {
                    btn = "<button class='manageGameMain-table-tr-active-btn' onclick='enableclub(" + reader["Club_id"] + ")'>Active</button>";
                }
                else {
                    btn = "<button class='manageGameMain-table-tr-deactive-btn' onclick='disableclub(" + reader["Club_id"] + ")'>Disable</button>";
                }
                html += "<tr class='Admin-t-tr'>" +
                    "<td class='Admin-t-td'>" + i + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Club_name"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Name"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["State_name"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["City_name"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Area_name"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Address"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Joining_date"] + "</td> " +
                    "<td class='Admin-t-td'>" + reader["Rating"] + "</td> " +
                    "<td class='Admin-t-td'><button class='manageGameMain-table-tr-update-btn'  onclick='updateclubview(" + reader["Club_id"] + ")'>Update</button>" + btn + "</td>" +
                    "</tr>";
                i += 1;
            }
            con.Close();
            return Content(html, "text/html");
        }

        [Route("UpdateClub")]
        public string UpdateClub(int id, int area, int manager, string name, string addr)
        {
            con.Open();
            cmd = new SqlCommand("update tblClub set Area='" + area + "', Manager='" + manager + "', Club_name='" + name + "', Address='" + addr + "' where Club_id='" + id + "';", con);
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

        [Route("DisableClub")]
        public string DisableClub(int id)
        {
            con.Open();
            cmd = new SqlCommand("update tblClub set Disable='1' where Club_id=" + id + ";", con);
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

        [Route("EnableClub")]
        public string EnableClub(int id)
        {
            con.Open();
            cmd = new SqlCommand("update tblClub set Disable='0' where Club_id=" + id + ";", con);
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

        [Route("UpdateClubImage")]
        public string Image(int id, string img)
        {

            con.Open();
            cmd = new SqlCommand("update tblClub set Image = '" + img + "' where Club_id = '" + id + "';", con);
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

        // userside club oprations

        [Route("fetchAllClubDetails")]
        public IActionResult fetchallClubdetails(int s, int c, int a, string g, string search)
        {
            string wherecluse = "SELECT tblClub.*, tblState.State_name, tblCity.City_name, tblArea.Area_name, tblManager.Name FROM tblClub inner join tblManager on tblManager.Manager_id = tblClub.Manager inner join tblArea on tblArea.Area_id = tblClub.Area INNER JOIN tblCity ON tblCity.City_id = tblArea.City INNER JOIN tblState ON tblState.State_id = tblCity.State WHERE tblClub.Disable='0'";

            if (search != "0") {
                wherecluse += " And tblClub.Club_name like '"+search+"'";
            }
            if (s != 0) {
                wherecluse += " And tblState.State_id='"+s+"'";
            }
            if (c != 0) {
                wherecluse += " And tblCity.City_id='"+c+"'";
            }
            if (a != 0) {
                wherecluse += " And tblArea.Area_id='"+a+"'";
            }
            if (g != "0")
            {
                con.Open();
                cmd = new SqlCommand("select Club from tblGame WHERE Game_name like '"+g+"' ", con);
                SqlDataReader reader1 = cmd.ExecuteReader();
                int j = 1;
                while (reader1.Read())
                {
                    if (j == 1)
                    {
                        wherecluse += " And tblClub.Club_id='" + reader1["Club"] + "'";
                    }
                    else { 
                        wherecluse += " Or tblClub.Club_id='" + reader1["Club"] + "'";
                    }
                    j++;
                }
                con.Close();
            }

            //return wherecluse;

            string html = "";
            con.Open();
            cmd = new SqlCommand(wherecluse, con);
            SqlDataReader reader = cmd.ExecuteReader();
            int i = 1;
            while (reader.Read())
            {

                html +=
                "<div class='col-lg-4 col-md-6 col-12 mb-4 custom-block-over'>" +
                "<div class='custom-block-wrap'>" +
                "<img src = '/Assets/Club/" + reader["Image"] + "' class='custom-block-image img-fluid' >" +
                "<div class='custom-block'>" +
                "<div class='custom-block-body'>" +
                "<h5 class='mb-3 gamesTitle'>" + reader["Club_name"] + "</h5>" +
                "<p class='gamesDetails'><strong> Address : " + reader["Address"] + "</strong></p>" +
                "<div class='rating'>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div></strong>" +
                "</div>" +
                "<a href='/Club/UserClubView/" + reader["Club_id"] + "'><button class='custom-btn-book'>Book now</button></a>" +
                "</div>" +
                "</div>" +
                "</div>";
                /*
                html += "<tr class='t-c-tr'>" +
                            "<td class='t-c-td'> " +
                                "<a href='/Club/UserClubView/" + reader["Club_id"]+"'>" +
                                "<div class='t-c-img'><img src='/Assets/Club/" + reader["Image"] +"' style='width: 200px; height: 100px;'> </img></div>" +
                            "</td>" +
                            "<td class='t-c-td'>" +
                                "<div class='t-c-td-div'>" +
                                    "<div class='t-c-td-div2'><h3>" + reader["Club_name"] + "</h3></div>" +
                                    "<div class='t-c-td-div2'>" + reader["Address"] + "</div>" +
                                    "<div class='t-c-td-div2'>" + Booking.generatRating(Convert.ToInt32(reader["Rating"])) + "</div>" +
                                "</div>" +
                            "</a></td>" +
                        "" +
                    "</tr>";*/
            }
            con.Close();
            return Content(html, "text/html");
        }

        [Route("UserClubView/{id}")]
        public IActionResult UserClubView(int id) 
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT tblClub.*, tblArea.Area_name,tblManager.Name FROM tblClub inner join tblArea on tblArea.Area_id = tblClub.Area join tblManager on tblManager.Manager_id = tblClub.Manager WHERE tblClub.Club_id = '" + id + "';", con);
            SqlDataReader result = cmd.ExecuteReader();
            if (result.Read())
            {
                ViewBag.image = result["Image"];
                ViewBag.name = result["Club_name"];
                ViewBag.area = result["Area_name"];
                ViewBag.manager = result["Name"];
                ViewBag.rating = result["Rating"];
                ViewBag.addr = result["Address"];
            }
            ViewBag.Id = id;
            return View("../Member/ClubView");
        }

    }
}
