using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System;
using System.Net;
using System.Net.Mail;


namespace WebApplication1.Pages
{


    [ValidateAntiForgeryToken]
    public class InfoModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public InfoModel(IConfiguration configuration)
        {
            _configuration = configuration;

            UserID = string.Empty;
            UserName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Password = string.Empty;
        }



        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public string UserID { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string PhoneNumber { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Message { get; set; } // New property for user message




        public void OnGet(bool? refresh)
        {
            if (refresh.HasValue && refresh.Value)
            {
                // Perform any actions you want when the refresh parameter is true
                // For example, re-fetch user information from the database
                FetchUserInfo();
            }
            else
            {
                // Your regular OnGet logic
                FetchUserInfo();
            }
        }



        public IActionResult OnPost()
        {

            // Fetch user information from the form
            Email = Request.Form["userEmail"];
            PhoneNumber = Request.Form["userPhone"];
            Password = Request.Form["userPassword"];
            Message = Request.Form["userMessage"];

            // TODO: Implement additional validation if needed


            // Check if the parameters are not empty before updating
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(Password))
            {
                // Update user information in the database
                UpdateUserInfo();
                
                // Fetch user information again after the update
                FetchUserInfo();
                // Set a success message to be displayed on the page
                TempData["Message"] = "Changes saved successfully.";
            }
            else
            {
                // Set an error message to be displayed on the page
                TempData["ErrorMessage"] = "Invalid input. Please provide values for Email, Phone Number, and Password.";
                
            }

            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            // Redirect back to the same page
            return RedirectToPage("/Info");
        }




        private void FetchUserInfo()
        {
            // Fetch user information using your database connection
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Assuming there's a Users table with a column named UserName
                using (SqlCommand command = new SqlCommand("SELECT TOP 1 UserName, Email, PhoneNumber, Password FROM Users", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Assign the user information to the properties
                            UserName = reader["UserName"].ToString();
                            Email = reader["Email"].ToString();
                            PhoneNumber = reader["PhoneNumber"].ToString();
                            Password = reader["Password"].ToString();
                        }
                    }
                }
            }
        }





        private void UpdateUserInfo()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand command = new SqlCommand("UPDATE Users SET Email = @Email, PhoneNumber = @PhoneNumber, Password = @Password WHERE UserID = @UserID", connection, transaction))
                    {
                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = PhoneNumber;
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = Password;
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = 15693;

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine("Update successful");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error updating user information: {ex.Message}");
                    throw;
                }
            }
        }




        private void SendUserMessage()
        {
            var userInfo = GetUserInformation();

            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "qutaibaashqar@gmail.com"; // Your Gmail address
            string smtpPassword = "qutaiba toto"; // Your Gmail password

            string fromEmail = "qutaibaashqar@gmail.com";
            string toEmail = "qbworkk@gmail.com"; // Replace with the company's email address

            string subject = "User Message";
            string body = $"From: {userInfo.Email}\nPhone: {userInfo.PhoneNumber}\n\nMessage:\n{Message}";

            using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                using (MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body))
                {
                    smtpClient.Send(mailMessage);
                }
            }
        }


        private UserInfo GetUserInformation()
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Email = Email;
            userInfo.PhoneNumber = PhoneNumber;
            userInfo.UserName = UserName;
            // ... (fetch other user information)

            return userInfo;
        }

        private class UserInfo
        {
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string UserName { get; set; }
            // ... (other user information properties)
        }


    }
}
