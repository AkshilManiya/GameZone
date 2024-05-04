using Microsoft.AspNetCore.Components;
using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GameZoneManagment.Controllers
{
    public class EmailService
    {
        public static bool SendOTP(string email, string otp)
        {
            try
            {
                // Configure the SMTP client
                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587; // Update the port if required
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("21bmiit005@gmail.com", "kvacvkpprdliydvl");
                    client.EnableSsl = true;

                    // Create the email message
                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("21bmiit005@gmail.com");
                        message.To.Add(email);
                        message.Subject = "One-Time Password (OTP)";

                        // HTML body for the email
                        string htmlBody = $@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <style>
                                    .container {{
                                        max-width: 600px;
                                        margin: 20px auto;
                                        padding: 20px;
                                        border: 1px solid #ddd;
                                        border-radius: 10px 50px;
                                        background-color: #fff;
                                        background-image: linear-gradient(to bottom right, #7a8ee4c9, #d3208bc5, #fda000c0);
                                    }}
                                    .header {{
                                        text-align: center;
                                        margin-bottom: 20px;
                                    }}
                                    .header h1 {{
                                        color: #1900ff;
                                        margin: 10px 0;
                                    }}
                                    .content {{
                                        margin-bottom: 20px;
                                    }}
                                    .content p {{
                                        font-size: 16px;
                                        line-height: 1.5;
                                        color: #333;
                                    }}
                                    .otp {{
                                        font-size: 35px;
                                        font-weight: bold;
                                        color: #0400ff;
                                        text-align: center;
                                        margin-bottom: 20px;
                                    }}
                                    .footer {{
                                        text-align: center;
                                    }}
                                    .footer p {{
                                        font-size: 14px;
                                        color: #666;
                                        margin-top: 20px;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class=""container"">
                                    <div class=""header"">
                                        <h1>Email Verification</h1>
                                    </div>
                                    <div class=""content"">
                                        <p>Dear User,</p>
                                        <p>Your One-Time Password (OTP) is:</p>
                                        <p class=""otp"">{otp}</p>
                                        <p>Please use this OTP to proceed with your action.</p>
                                    </div>
                                    <div class=""footer"">
                                        <p>Regards,<br>GameZone Team</p>
                                    </div>
                                </div>
                            </body>
                            </html>

                        ";
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        // Send the email
                        client.Send(message);
                    }
                }

                // Email sent successfully
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error sending email: " + ex.Message);
                // Email sending failed
                return false;
            }
        }

        public static string GenerateRandomOTP(int iOTPLength)
        {
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sOTP = String.Empty;
            Random rand = new Random();
            for (int i = 0; i < iOTPLength; i++)
            {
                sOTP += saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
            }
            return sOTP;
        }

        public static bool ValidateEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(email, pattern);   
        }
    }

}