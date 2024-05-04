using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace GameZoneManagment.Controllers
{
    [Route("MemberRegister")]
    public class Register : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("registerView")]
        public IActionResult RegisterView()
        {
            return View("../Member/RegisterMember");
        }

        [Route("Registeration")]
        public string RegisterMember(string username, string password, string email, int contact, int area)
        {
            password = PasswordHasher.HashedPassword(password);
            con.Open();
            cmd = new SqlCommand("insert into tblMember (Area, Name, Email, Password, Mobile_no) values(" + area + ", '" + username + "', '" + email + "', '" + password + "', '" + contact.ToString() + "')", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                cmd = new SqlCommand("insert into tblLogin (Username, Password, Role) values ('"+email+"', '"+password+"', 'Member');", con);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    con.Close();
                    return "success";
                }
                else {
                    return "error";
                }
            }
            else
            {
                con.Close();
                return "error";
            }
        }

        [Route("sendOtp")]
        public String SendOtp(String email) {
            String otp = EmailService.GenerateRandomOTP(6);
            if (EmailService.SendOTP(email, otp))
            {
                return otp;
            }
            else {
                return "error";
            }
        }

        [Route("checkEmail")]
        public string checkemail(String email) {
            if (EmailService.ValidateEmail(email))
            {
                con.Open();
                cmd = new SqlCommand("SELECT * FROM tblLogin WHERE Username='" + email + "';", con);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    con.Close();
                    return "exist";
                }
                else
                {
                    con.Close();
                    return "send";
                }
            }
            else {
                con.Close();
                return "notValid";
            }
        }

        [Route("forgotPassword")]
        public string reset( string email, string password)
        {
            string usertbl = usertable(email);
            password = PasswordHasher.HashedPassword(password);
            con.Open();
            cmd = new SqlCommand("update "+usertbl+" set Password='"+password+"' where Email='"+email+"';", con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                cmd = new SqlCommand("update tblLogin set Password='"+password+"' where Username='"+email+"';", con);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    con.Close();
                    return "success";
                }
                else
                {
                    con.Close();
                    return "error login table" + usertbl;
                }
            }
            else
            {
                con.Close();
                return "error user table"+usertbl;
            }
        }

        [Route("hasing/{str}")]
        public string Hashing(string str) {
            return PasswordHasher.HashedPassword(str);
        }


        public string usertable(string email) {
            con.Open();
            cmd = new SqlCommand("SELECT * FROM tblLogin WHERE Username='" + email + "';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string role;
            if (reader.Read())
            {
                if ((string)reader["Role"] == "Admin") {
                    role = "tblAdmin";
                }
                else if ((string)reader["Role"] == "Member")
                {
                    role = "tblMember";
                }
                else
                {
                    role = "tblManager";
                }
            }
            else
            {
                role = "error fetch user role";
            }
            con.Close();
            return role;
        }
        
    }
}
