using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace GameZoneManagment.Controllers
{

    [Route("Admin")]
    public class Admin : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("Home")]
        public IActionResult HomeView()
        {
            return View("../Admin/AdminHome");
        }

        [Route("Profile")]
        public IActionResult ProfileView()
        {
            return View("AdminProfile");
        }

        [Route("AddClub")]
        public IActionResult AddClubView()
        {
            return View("../Admin/AddClub");
        }

        [Route("AddManagerView")]
        public IActionResult AddManagerView()
        {
            return View("../Admin/RegisterManager");
        }

        [Route("ManageClubView")]
        public IActionResult ManageClubView()
        {
            return View("../Admin/ManageClub");
        }

        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            return View("../Admin/Dashboard");
        }

        [Route("RegisterationManager")]
        public string RegisterMember(string username, string email, int contact, int area)
        {
            string password = PasswordHasher.HashedPassword(username);
            con.Open();
            cmd = new SqlCommand("insert into tblManager (Area, Name, Email, Password, Mobile_no) values(" + area + ", '" + username + "', '" + email + "', '" + password + "', '" + contact.ToString() + "')", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                cmd = new SqlCommand("insert into tblLogin (Username, Password, Role) values ('" + email + "', '" + password + "', 'Manager');", con);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    con.Close();
                    return "success";
                }
                else
                {
                    return "error";
                }
            }
            else
            {
                con.Close();
                return "error";
            }
        }
    }
}
