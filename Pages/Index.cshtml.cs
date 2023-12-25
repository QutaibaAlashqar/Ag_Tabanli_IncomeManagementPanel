using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

// NEW
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;



namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public void OnGet()
        {
            ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            ViewData["Message"] = TempData["Message"];
        }

        public IActionResult OnPost()
        {
            // Fetch user information from the database
            string email = Request.Form["email"];
            string password = Request.Form["password"];

            if (IsValidUser(email, password))
            {
                // Redirect to the welcome page
                return RedirectToPage("/Home");
            }
            else
            {
                // Set an error message to be displayed on the login page
                TempData["ErrorMessage"] = "Invalid email or password.";
                return RedirectToPage("/Index");
            }
        }

        private bool IsValidUser(string email, string password)
        {
            // Fetch user information using your database connection
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Assuming there's a Users table with columns Email and Password
                using SqlCommand command = new SqlCommand("SELECT TOP 1 Email, Password FROM Users WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", email);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Retrieve password from the database
                        string passwordFromDatabase = reader["Password"]?.ToString() ?? string.Empty;


                        // Compare the entered password with the one from the database
                        if (passwordFromDatabase == password)
                        {
                            // Passwords match, user is valid
                            return true;
                        }
                    }
                }
            }

            // If any errors occurred or passwords don't match, return false
            TempData["ErrorMessage"] = "Invalid email or password.";
            return false;
        }
    }
}
