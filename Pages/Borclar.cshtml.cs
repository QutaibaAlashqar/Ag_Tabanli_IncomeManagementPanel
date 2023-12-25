
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace WebApplication1.Pages
{

    [ValidateAntiForgeryToken]
    public class BorclarModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public BorclarModel(IConfiguration configuration)
        {
            _configuration = configuration;

            Notes = new List<NoteModel>();
        }

        [BindProperty]
        public List<NoteModel> Notes { get; set; }

        public void OnGet()
        {
            // Fetch notes information from the database
            FetchNotes();
        }

        public IActionResult OnPostAddNote()
        {
            // Add a new note to the database
            AddNote();

            // Fetch notes information again after the update
            FetchNotes();

            // Set a success message to be displayed on the page
            TempData["Message"] = "Note added successfully.";

            // Redirect back to the same page
            return RedirectToPage("/Borclar");
        }

        public IActionResult OnPostEditNote(int noteID, string Content)
        {
            // Edit an existing note in the database
            EditNote(noteID, Content);

            // Fetch notes information again after the update
            FetchNotes();

            // Set a success message to be displayed on the page
            TempData["Message"] = "Note edited successfully.";

            // Redirect back to the same page
            return RedirectToPage("/Borclar");
        }

        public IActionResult OnPostDeleteNote(int noteID)
        {
            // Delete an existing note from the database
            DeleteNote(noteID);

            // Fetch notes information again after the update
            FetchNotes();

            // Set a success message to be displayed on the page
            TempData["Message"] = "Note deleted successfully.";

            // Redirect back to the same page
            return RedirectToPage("/Borclar");
        }

        private void FetchNotes()
        {
            // Fetch notes information using your database connection
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Assuming there's a Notes table with columns: NoteID, UserID, Content, NoteType
                using (SqlCommand command = new SqlCommand("SELECT NoteID, Content FROM Notes WHERE UserID = @UserID AND NoteType = 3", connection))
                {
                    command.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = "15693"; // Replace with the actual user ID

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create a NoteModel and add it to the list
                            Notes.Add(new NoteModel
                            {
                                NoteID = Convert.ToInt32(reader["NoteID"]),
                                Content = reader["Content"].ToString(),
                                NoteType = 3, // NoteType is hardcoded to 2 as per your requirement
                            });
                        }
                    }
                }
            }
        }

        private void AddNote()
        {
            // Add a new note to the database
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Fetch content from the form or wherever it's coming from
                    string newNoteContent = Request.Form["newNoteContent"]; // Adjust this based on your actual form

                    using (SqlCommand command = new SqlCommand("INSERT INTO Notes (UserID, Content, NoteType) VALUES (@UserID, @Content, 3)", connection, transaction))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = "15693"; // Replace with the actual user ID
                        command.Parameters.Add("@Content", SqlDbType.NVarChar).Value = newNoteContent;
                        command.Parameters.Add("@NoteType", SqlDbType.Int).Value = 3; // NoteType is hardcoded to 2 as per your requirement

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine("Note added successfully");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error adding note: {ex.Message}");
                    throw;
                }
            }
        }

        private void EditNote(int noteId, string content)
        {
            // Edit an existing note in the database
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand command = new SqlCommand("UPDATE Notes SET Content = @Content WHERE NoteID = @NoteID AND NoteType = 3", connection, transaction))
                    {
                        command.Parameters.Add("@Content", SqlDbType.NVarChar).Value = content;
                        command.Parameters.Add("@NoteID", SqlDbType.Int).Value = noteId;
                        command.Parameters.Add("@NoteType", SqlDbType.Int).Value = 3; // NoteType is hardcoded to 2 as per your requirement

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine("Note edited successfully");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error editing note: {ex.Message}");
                    throw;
                }
            }
        }

        private void DeleteNote(int noteId)
        {
            // Delete an existing note from the database
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand command = new SqlCommand("DELETE FROM Notes WHERE NoteID = {noteId} AND NoteType = 3", connection, transaction))
                    {
                        command.Parameters.Add("@NoteID", SqlDbType.Int).Value = noteId;
                        command.Parameters.Add("@NoteType", SqlDbType.Int).Value = 3; // NoteType is hardcoded to 2 as per your requirement
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine("Note deleted successfully");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error deleting note: {ex.Message}");
                    throw;
                }
            }
        }
    }

    public class NoteModel
    {
        public int NoteID { get; set; }
        public string Content { get; set; }
        public int NoteType { get; set; }
        public int UserID { get; set; }
    }
}
