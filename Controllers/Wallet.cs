using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace GameZoneManagment.Controllers
{
    [Route("Wallet")]
    public class Wallet : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();
        string todayDate = DateTime.Today.ToString("yyyy-MM-dd");

        [Route("addMemberAmount")]
        public string Index(int amount)
        {
            string username = HttpContext.Session.GetString("Username");
            con.Open();
            cmd = new SqlCommand("update tblMember set Wallet = (Wallet + @amount) where Email = @username;", con);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@username", username);

            if (cmd.ExecuteNonQuery() > 0)
            {
                con.Close();
                con.Open();
                cmd = new SqlCommand("INSERT INTO tblpayment(Login, Amount, Date, Opration, Game) VALUES ((SELECT Login_id FROM tblLogin WHERE Username = '"+username+"'), "+amount+", '"+todayDate+"', '2', 'Recharge')", con);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    con.Close();
                    return "success";
                }
                else {
                    con.Close();
                    return "error insert";
                }
            }
            else
            {
                con.Close();
                return "error update";
            }
        }

        [Route("fetch_wallet_amount")]
        public int fetch_amount() {
            string username = HttpContext.Session.GetString("Username");
            con.Open();
            cmd = new SqlCommand("select Wallet from tblMember where Email = @username;", con);
            cmd.Parameters.AddWithValue("@username", username);
            SqlDataReader result = cmd.ExecuteReader();
            int amount = 0;
            if (result.Read())
            {
                amount = Convert.ToInt32(result["Wallet"]);
            }
            con.Close();
            return amount;
        }

        [Route("fetch_wallet_history")]
        public IActionResult fetch_history(int method)
        {
            string username = HttpContext.Session.GetString("Username");

            con.Open();
            if (method == 0)
            {
                cmd = new SqlCommand("select * from tblPayment where Login=(select Login_id from tblLogin where Username='"+username+"');", con);
            }
            else {
                cmd = new SqlCommand("select * from tblPayment where Login=(select Login_id from tblLogin where Username='"+username+"') And Opration='"+method+"';", con);
            }
            SqlDataReader result = cmd.ExecuteReader();
            int amount = 0;
            string html = "";
            int i = 1;
            while (result.Read())
            {
                DateTime bookingDate = (DateTime)result["Date"];
                string date = bookingDate.ToString("yyyy-MM-dd");

                string operationText = Convert.ToInt32(result["Opration"]) == 2 ? "Credit" : "Debit";
                html +=
"<tr class='wallet-Div-down-data-tr'>" +
"<td class='wallet-Div-down-data-td'>" + i + "</td>" +
"<td class='wallet-Div-down-data-td'>" + date + "</td>" +
"<td class='wallet-Div-down-data-td'>" + result["Amount"] + "</td>" +
"<td class='wallet-Div-down-data-td'>" + operationText + "</td>" +
"<td class='wallet-Div-down-data-td'>" + result["Game"] + "</td>" +
"</tr>";
                /*
                html += "<tr>" +
                    "<td>"+i+"</td>" +
                    "<td>" + result["Amount"] + "</td>" +
                    "<td>" + date + "</td>" +
                    "<td>" + operationText + "</td>" +
                    "<td>" + result["Game"] + "</td>" +
                    "</tr>";*/
                i += 1;
            }
            con.Close();
            return Content(html, "text/html");
        }
    }
}
