using GradingSystem.Model;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using GradingSystem.View;
using GradingSystem.Data;
using System.Windows;
using GradingSystem.ViewModel;

namespace GradingSystem.View.Admin
{
    public partial class ManageStudents : UserControl
    {
        public StudentsViewModel ViewModel { get; set; }

        public ManageStudents()
        {
            InitializeComponent();
            // Initialize the ViewModel
            ViewModel = new StudentsViewModel();

            // Set the DataContext for binding, if required
            DataContext = ViewModel;
        }

        private void addStudentBtn(object sender, System.Windows.RoutedEventArgs e)
        {
            // Open the AddStudent window and pass the ViewModel
            var addStudentWindow = new AddStudent(ViewModel);
            addStudentWindow.StudentAdded += () =>
            {
                // Refresh the list of students when a new student is added
                ViewModel.LoadStudents();
            };
            addStudentWindow.ShowDialog();
        }

        private void deleteStudentBtn(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var studentToDelete = button?.DataContext as Student;

            if (studentToDelete != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {studentToDelete.FirstName} {studentToDelete.LastName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    ViewModel.DeleteStudent(studentToDelete);
                }
            }
        }
    }
}