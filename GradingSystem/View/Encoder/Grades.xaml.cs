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

            //if (string.IsNullOrWhiteSpace(searchText))
            //{
            //    students.FilteredStudents = new ObservableCollection<Student>(students.Students);
            //}
            //else
            //{
            //    students.FilteredStudents = new ObservableCollection<Student>(
            //        students.Students.Where(s => s.StudentName.ToLower().Contains(searchText) ||
            //                                     s.StudentId.ToString().Contains(searchText) ||
            //                                     s.Program.ProgramName.ToLower().Contains(searchText) ||
            //                                     s.YearLevel.ToString().Contains(searchText) ||
            //                                     s.Semester.ToString().Contains(searchText))
            //    );
            //}

            studentsDataGrid.ItemsSource = students.FilteredStudents;
        }

        private void AddGradeBtn(object sender, RoutedEventArgs e)
        {

        }

        private void ShowGradeBtn(object sender, RoutedEventArgs e)
        {

        }
    }
}
