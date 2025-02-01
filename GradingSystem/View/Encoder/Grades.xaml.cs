using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;

namespace GradingSystem.View.Encoder
{
    public partial class Grades : UserControl
    {
        private readonly ApplicationDbContext _context;
        public StudentsViewModel students { get; set; }
        public Grades(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            students = new StudentsViewModel(_context);
            DataContext = students;
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

        private void AddGradeBtn(object sender, RoutedEventArgs e)
        {

        }

        private void ShowGradeBtn(object sender, RoutedEventArgs e)
        {
            if (_context == null)
            {
                MessageBox.Show("Database context is not initialized.");
                return;
            }

            if (studentsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select a student.");
                return;
            }


            try
            {
                // Check if a student is selected
                var selectedStudent = (Student)studentsDataGrid.SelectedItem;

                // Debugging: Verify if the student is selected
                if (selectedStudent == null)
                {
                    MessageBox.Show("No student selected.");
                    return;
                }

                // Ensure 'students' is properly initialized before using it
                if (students == null)
                {
                    MessageBox.Show("Students data is not initialized.");
                    return;
                }

                var showGradeWindow = new ShowGrade(selectedStudent, _context);
                showGradeWindow.DataContext = students;
                showGradeWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
