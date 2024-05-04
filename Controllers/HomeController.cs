using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace GameZoneManagment.Controllers
{
    [Route("app")]
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        
        [Route("loginView")]
        public IActionResult LoginView() {
            return View("../Home/Login");
        }

        [Route("forgotView")]
        public IActionResult ForgotView()
        {
            return View("../Home/ForgotPassword");
        }


        [Route("checkSession")]
        public string CheckSession() 
        {
            string user = "";
            string op = "";
            if (HttpContext.Session.GetString("Username") != null)
            {
                user = HttpContext.Session.GetString("Usertbl").ToString();
                op = "session";
            }
            else if (Request.Cookies.ContainsKey("Username"))
            {
                string username = Request.Cookies["Username"];
                user = Request.Cookies["Usertbl"];

                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("Usertbl", user);
                op = "cookie";
            }
            return user;
        }

        [Route("login")]
        public string Login(string username, string password, bool remember)
        {
            string userrole = "";
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT * FROM tblLogin WHERE Username='" + username + "';", con);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string user = reader.GetString(1);
                        string pass = reader.GetString(2);

                        if (username == user)
                        {
                            if (PasswordHasher.VerifyPassword(pass, password))
                            {
                                string tbl = "";
                                userrole = reader.GetString(3);
                                if (userrole == "Member")
                                {
                                    tbl = "tblMember";
                                }
                                else if (userrole == "Manager")
                                {
                                    tbl = "tblManager";
                                }
                                else
                                {
                                    tbl = "tblAdmin";
                                }

                                HttpContext.Session.SetString("Username", username);
                                HttpContext.Session.SetString("Usertbl", tbl);

                                if (remember) {
                                    var UsernameCookie = new CookieOptions
                                    {
                                        Expires = DateTime.UtcNow.AddHours(1),
                                        HttpOnly = true
                                    };
                                    Response.Cookies.Append("Username", username, UsernameCookie);
                                    var UsertblCookie = new CookieOptions
                                    {
                                        Expires = DateTime.UtcNow.AddHours(1),
                                        HttpOnly = true
                                    };
                                    Response.Cookies.Append("Usertbl", tbl, UsertblCookie);
                                }
                            }
                            else
                            {
                                userrole = "password";
                            }
                        }
                    }
                }
                else
                {
                    userrole = "username";
                }
                con.Close();
            }
            catch (Exception ex)
            {
                userrole = "error";
            }
            return userrole;
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                HttpContext.Session.Remove("Username");
            }

            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("Usertbl");

            return View("../Member/MemberHome");
        }

    }
}
