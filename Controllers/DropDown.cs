using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using System.IO.Pipelines;

namespace GameZoneManagment.Controllers
{
    [Route("DropDown")]
    public class DropDown : Controller
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-SBQBM8M\\SQLEXPRESS;Database=GameZoneManagement;Integrated Security=True;Encrypt=False;");
        SqlCommand cmd = new SqlCommand();

        [Route("State")]
        public String ReturnState() 
        {
            con.Open();
            cmd = new SqlCommand("select * from tblState;", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                int areaId = reader.GetInt32(0);
                string areaName = reader.GetString(1);
                result += $"<option value={areaId}>{areaName}</option>";
            }
            con.Close();
            return result;
        }

        [Route("City")]
        public String ReturnCity(int state)
        {
            con.Open();
            cmd = new SqlCommand("select * from tblCity where State='"+state+"';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                int areaId = reader.GetInt32(0);
                string areaName = reader.GetString(2);
                result += $"<option class='filtering-option' value={areaId}>{areaName}</option>";
            }
            con.Close();
            return result;
        }

        [Route("Area")]
        public String ReturnArea(int city)
        {
            con.Open();
            cmd = new SqlCommand("select * from tblArea where City='"+city+"';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                int areaId = reader.GetInt32(0);
                string areaName = reader.GetString(2);
                result += $"<option  class='filtering-option' value={areaId}>{areaName}</option>";
            }
            con.Close();
            return result;
        }

        [Route("Manager")]
        public String ReturnManager()
        {
            con.Open();
            cmd = new SqlCommand("select * from tblManager;", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                int areaId = reader.GetInt32(0);
                string areaName = reader.GetString(2);
                result += $"<option class='filtering-option' value={areaId}>{areaName}</option>";
            }
            con.Close();
            return result;
        }

        [Route("SelectManager")]
        public String ReturnToSelectManager()
        {
            con.Open();
            cmd = new SqlCommand("SELECT * FROM tblManager WHERE Manager_id NOT IN (SELECT Manager FROM tblClub);", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                int areaId = reader.GetInt32(0);
                string areaName = reader.GetString(2);
                result += $"<option class='filtering-option' value={areaId}>{areaName}</option>";
            }
            con.Close();
            return result;
        }

        public string ClubID()
        {
            string username = HttpContext.Session.GetString("Username").ToString();
            con.Close();
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

        [Route("Game")]
        public String ReturnGame()
        {
            string id = ClubID();
            con.Open();
            cmd = new SqlCommand("select * from tblGame where Club='"+id+"';", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                int areaId = reader.GetInt32(0);
                string areaName = reader.GetString(3);
                result += $"<option class='filtering-option' value={areaId}>{areaName}</option>";
            }
            con.Close();
            return result;
        }


        [Route("AllGames")]
        public String ReturnAllGames()
        {
            con.Open();
            cmd = new SqlCommand("SELECT DISTINCT Game_name FROM tblGame; ", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                //int areaId = reader.GetInt32(0);
                string areaName = reader.GetString(0);
                result += $"<option class='filtering-option' value='{areaName}'>{areaName}</option>";
            }
            con.Close();
            return result;
        }

        [Route("Club")]
        public String ReturnClub()
        {
            con.Open();
            cmd = new SqlCommand("SELECT Club_id,Club_name FROM tblClub;", con);
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                int areaId = reader.GetInt32(0);
                string areaName = reader.GetString(1);
                result += $"<option class='filtering-option' value={areaId}>{areaName}</option>";
            }
            con.Close();
            return result;
        }

    }
}
