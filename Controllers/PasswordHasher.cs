using System.Security.Cryptography;
using System.Text;

namespace GameZoneManagment.Controllers
{
    public class PasswordHasher
    {
        public PasswordHasher() { 
        
        }

        public static String HashedPassword(String Password) {
            using (SHA256 sha256 = SHA256.Create()) {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool VerifyPassword( String hashedPassword, String Password)
        {
            string hashedInput = HashedPassword(Password);
            return hashedInput == hashedPassword;
        }
    }
}
