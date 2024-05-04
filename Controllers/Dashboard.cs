using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;


namespace GameZoneManagment.Controllers
{
    [Route("Dash")]
    public class Dashboard : Controller
    {
        SqlConnection con1 = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlConnection con2 = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        
        string todayDate = DateTime.Today.ToString("yyyy-MM-dd");
        string newDate = DateTime.Today.AddDays(6).ToString("yyyy-MM-dd");

        [Route("Adminshow")]
        public IActionResult Index(string date, int club)
        {
            string lbl = "[";
            string data = "[";
            con2.Open();
            SqlCommand cmd2 = new SqlCommand("select g.Game_name, COUNT(b.Booking_id) as booking from tblGame g inner join tblSlot s on s.Game = g.Game_id inner join tblBookings b on b.Slot = s.Slot_id inner join tblClub c ON c.Club_id = g.Club where c.Club_id="+club+" AND s.Date='"+date+"' Group by g.Game_name;", con2);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            int j = 1;
            while (reader2.Read()) {
                if (j == 1)
                {
                    lbl += "'" + reader2["Game_name"].ToString() + "'";
                    data += "'" + reader2["booking"].ToString() + "'";
                }
                else
                {
                    lbl += ",'" + reader2["Game_name"].ToString() + "'";
                    data += ",'" + reader2["booking"].ToString() + "'";
                }
                j++;
            }
            con2.Close();

            lbl += "]";
            data += "]";

            userData.Add("imgUrl", "https://quickchart.io/chart?c={type:'pie',data:{labels:" + lbl + ",datasets:[{data:" + data + "}]}}");

            con2.Open();
            cmd2 = new SqlCommand("select COUNT(b.Booking_id) as booking,SUM(b.Total_amount) AS booking_amount FROM tblClub c INNER JOIN tblGame g ON c.Club_id = g.Club INNER JOIN tblSlot s ON g.Game_id = s.Game LEFT JOIN tblBookings b ON s.Slot_id = b.Slot WHERE c.Club_id="+club+" AND s.Date='"+date+"';", con2);
            reader2 = cmd2.ExecuteReader();
            if (reader2.Read())
            {
                userData.Add("booking_amount", reader2["booking_amount"]);
                userData.Add("total_booking", reader2["booking"]);
            }
            con2.Close();


            lbl = "[";
            data = "[";
            con2.Open();
            cmd2 = new SqlCommand("select g.Game_name, COUNT(b.Booking_id) as booking from tblGame g inner join tblSlot s on s.Game = g.Game_id inner join tblBookings b on b.Slot = s.Slot_id inner join tblClub c ON c.Club_id = g.Club where c.Club_id="+club+" AND b.Date='"+date+"' Group by g.Game_name;", con2);
            reader2 = cmd2.ExecuteReader();
            int k = 1;
            while (reader2.Read())
            {
                if (k == 1)
                {
                    lbl += "'" + reader2["Game_name"].ToString() + "'";
                    data += "'" + reader2["booking"].ToString() + "'";
                }
                else
                {
                    lbl += ",'" + reader2["Game_name"].ToString() + "'";
                    data += ",'" + reader2["booking"].ToString() + "'";
                }
                k++;
            }
            con2.Close();

            lbl += "]";
            data += "]";

            userData.Add("imgUrl_today", "https://quickchart.io/chart?c={type:'pie',data:{labels:" + lbl + ",datasets:[{data:" + data + "}]}}");

            con2.Open();
            cmd2 = new SqlCommand("select COUNT(b.Booking_id) as booking,SUM(b.Total_amount) AS booking_amount FROM tblClub c INNER JOIN tblGame g ON c.Club_id = g.Club INNER JOIN tblSlot s ON g.Game_id = s.Game LEFT JOIN tblBookings b ON s.Slot_id = b.Slot WHERE c.Club_id="+club+" AND b.Date='"+date+"';", con2);
            reader2 = cmd2.ExecuteReader();
            if (reader2.Read())
            {
                userData.Add("booking_amount_today", reader2["booking_amount"]);
                userData.Add("total_booking_today", reader2["booking"]);
            }
            con2.Close();



            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");
        }

        [Route("allTimeDetailsShow")]
        public IActionResult alltimedetails()
        {
            con2.Open();
            SqlCommand cmd2 = new SqlCommand("select Sum(Wallet) as earn from tblManager;", con2);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            Dictionary<string, object> userData = new Dictionary<string, object>();
            if (reader2.Read())
            {
                userData.Add("Earning", reader2["earn"]);
            }
            con2.Close();

            con2.Open();
            cmd2 = new SqlCommand("select Count(Member_id)as Members from tblMember;", con2);
            reader2 = cmd2.ExecuteReader();
            if (reader2.Read())
            {
                userData.Add("Members", reader2["Members"]);
            }
            con2.Close();

            con2.Open();
            cmd2 = new SqlCommand("select Count(Club_id)as Members from tblClub;", con2);
            reader2 = cmd2.ExecuteReader();
            if (reader2.Read())
            {
                userData.Add("Clubs", reader2["Members"]);
            }
            con2.Close();

            con2.Open();
            cmd2 = new SqlCommand("select Count(Game_id)as Members from tblGame;", con2);
            reader2 = cmd2.ExecuteReader();
            if (reader2.Read())
            {
                userData.Add("Games", reader2["Members"]);
            }
            con2.Close();

            string jsonData = JsonConvert.SerializeObject(userData);
            return Content(jsonData, "application/json");
        }
    }
}
