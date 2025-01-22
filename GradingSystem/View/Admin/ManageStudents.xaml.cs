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
        private readonly ApplicationDbContext _context;

        public StudentsViewModel students { get; set; }

        public ManageStudents(ApplicationDbContext context)
        {
            InitializeComponent();

            _context = context;
            students = new StudentsViewModel(context);
            DataContext = students;
        }

        private void addStudentBtn(object sender, RoutedEventArgs e)
        {
            var programViewModel = new ProgramViewModel(); // Or fetch it from somewhere

            // Pass the context when creating the AddStudent window
            var addStudentWindow = new AddStudent(students, programViewModel, _context);

            // Handle the event when a new student is added
            addStudentWindow.StudentAdded += () =>
            {
                // Refresh the list of students when a new student is added
                students.LoadStudentsAsync();
            };

            // Show the dialog
            addStudentWindow.Show();
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
                    students.DeleteStudentAsync(studentToDelete);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var programViewModel = new ProgramViewModel(); // Or fetch it from somewhere
            var selectedStudent = (Student)studentsDataGrid.SelectedItem; // Get selected student

            if (selectedStudent != null)
            {
                // Pass the selected student to the EditStudent window
                var editWindow = new EditStudent(selectedStudent, programViewModel); // Pass the selected student to the constructor

                // You can also set the DataContext if needed
                var viewModel = new StudentsViewModel(_context);
                viewModel.SelectedStudent = selectedStudent;
                editWindow.DataContext = viewModel;

                // Show the window
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a student to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}