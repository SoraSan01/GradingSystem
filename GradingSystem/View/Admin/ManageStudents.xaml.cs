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
            var programViewModel = new ProgramViewModel(_context); // Or fetch it from somewhere

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
            // Ensure the selected student is retrieved properly
            var selectedStudent = (Student)studentsDataGrid.SelectedItem;

            if (selectedStudent != null)
            {
                // Initialize the ProgramViewModel with the selected student's Program (if available)
                var programViewModel = new ProgramViewModel(_context)
                {
                    // Populate with the program associated with the selected student (assuming you have a Program property)
                    SelectedProgram = selectedStudent.Program
                };

                // Initialize the StudentsViewModel and set the SelectedStudent
                var viewModel = new StudentsViewModel(_context)
                {
                    SelectedStudent = selectedStudent
                };

                // Create the EditStudent window, passing the view models as needed
                var editWindow = new EditStudent(selectedStudent, programViewModel)
                {
                    DataContext = viewModel // Bind the DataContext to the StudentsViewModel
                };

                // Show the dialog
                editWindow.ShowDialog();
            }
            else
            {
                // Show a warning if no student is selected
                MessageBox.Show("Please select a student to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchTextBox(object sender, TextChangedEventArgs e)
        {
            if (students == null || students.Students == null) return;

            var searchText = (sender as TextBox)?.Text?.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                students.FilteredStudents = new ObservableCollection<Student>(students.Students);
            }
            else
            {
                students.FilteredStudents = new ObservableCollection<Student>(
                    students.Students.Where(s => s.StudentName.ToLower().Contains(searchText) ||
                                                 s.StudentId.ToString().Contains(searchText) ||
                                                 s.Program.ProgramName.ToLower().Contains(searchText) ||
                                                 s.YearLevel.ToString().Contains(searchText) ||
                                                 s.Semester.ToString().Contains(searchText))
                );
            }

            studentsDataGrid.ItemsSource = students.FilteredStudents;
        }
    }
}