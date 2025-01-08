using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.ViewModel
{
    public class UserViewModel
    {
        public ObservableCollection<User> Users { get; set; }

        public UserViewModel(ApplicationDbContext context) {

            // Initialize the ObservableCollection
            Users = new ObservableCollection<User>();

            // Load data (this can be from your database or a static list for testing)
            LoadUsers();
        }

        public void LoadUsers()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Query the database to get all students
                    var userList = context.Users.ToList();

                    // Clear the ObservableCollection and add the students
                    Users.Clear();
                    foreach (var user in userList)
                    {
                        Users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public void AddUser(User newUser)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Users.Add(newUser);
                    context.SaveChanges();

                    // Refresh the ObservableCollection
                    Users.Add(newUser);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred while adding the student: {ex.Message}");
            }
        }
    }
}
