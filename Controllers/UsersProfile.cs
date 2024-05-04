using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace GameZoneManagment.Controllers
{
    [Route("Profile")]
    public class UsersProfile : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("fetchUser")]
        public IActionResult Index()
        {
            string usertbl = HttpContext.Session.GetString("Usertbl").ToString();
            string username = HttpContext.Session.GetString("Username").ToString();

            con.Open();
            cmd = new SqlCommand("SELECT "+usertbl+".*, tblState.State_id, tblCity.City_id, tblArea.Area_name FROM "+usertbl+" inner join tblArea on tblArea.Area_id = "+usertbl+".Area INNER JOIN tblCity ON tblCity.City_id = tblArea.City INNER JOIN tblState ON tblState.State_id = tblCity.State WHERE Email = '"+username+"';", con);
            SqlDataReader result = cmd.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            while (result.Read())
            {
                userData.Add("image", result["Image"]);
                userData.Add("name", result["Name"]);
                userData.Add("contact", result["Mobile_no"]);
                userData.Add("email", result["Email"]);
                userData.Add("area_name", result["Area_name"]);
                userData.Add("state", result["State_id"]);
                userData.Add("city", result["City_id"]);
                userData.Add("area", result["Area"]);
            }
            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");
        }

        [Route("updateImage")]
        public string Image(string img)
        {
            string usertbl = HttpContext.Session.GetString("Usertbl").ToString();
            string username = HttpContext.Session.GetString("Username").ToString();

            con.Open();
            cmd = new SqlCommand("update " + usertbl + " set Image = '" + img + "' where Email = '" + username + "';", con);
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

        [Route("updateUser")]
        public String updateuser(string name, string contact, int area)
        {
            string usertbl = HttpContext.Session.GetString("Usertbl").ToString();
            string username = HttpContext.Session.GetString("Username").ToString();

            con.Open();
            cmd = new SqlCommand("update " + usertbl + " set Name='" + name + "', Mobile_no='" + contact + "', Area='" + area + "' where Email = '" + username + "';", con);
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

        [Route("resetPassword")]
        public string reset(string oldpass, string email, string password)
        {
            string response = "";
            string oldp = "";
            string usertbl = HttpContext.Session.GetString("Usertbl").ToString();

            con.Open();
            cmd = new SqlCommand("SELECT * FROM tblLogin WHERE Username='" + email + "';", con);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    oldp = reader.GetString(2);
                }
                con.Close();
            }
            else
            {
                con.Close();
                return "username";
            }

            if (oldp != "")
            {
                if (PasswordHasher.VerifyPassword(oldp, oldpass))
                {
                    password = PasswordHasher.HashedPassword(password);
                    con.Open();
                    cmd = new SqlCommand("update " + usertbl + " set Password='" + password + "' where Email='" + email + "';", con);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        cmd = new SqlCommand("update tblLogin set Password='" + password + "' where Username='" + email + "';", con);
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            con.Close();
                            response = "success";
                        }
                        else
                        {
                            con.Close();
                            response = "error";
                        }
                    }
                    else
                    {
                        con.Close();
                        response = "error";
                    }
                }
                else
                {
                    response = "old";
                }
            }

            return response;
        }
    }
}
