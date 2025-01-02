using GradingSystem.Model;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using GradingSystem.View;
using GradingSystem.Data;
using System.Windows;

namespace GradingSystem.View.Admin
{
    public partial class ManageStudents : UserControl
    {
        public ObservableCollection<Student> Students { get; set; }

        public ManageStudents()
        {
            InitializeComponent();

            // Bind to the DataGrid
            Students = new ObservableCollection<Student>();
            DataContext = this;
            LoadStudents();

        }

        private void addStudentBtn(object sender, System.Windows.RoutedEventArgs e)
        {
            AddStudent addstudent = new AddStudent();
            addstudent.ShowDialog();
        }

        private void LoadStudents()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Query the database to get all students
                    var studentList = context.Students.ToList();

                    // Clear the ObservableCollection and add the students
                    Students.Clear();
                    foreach (var student in studentList)
                    {
                        Students.Add(student);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading students: {ex.Message}");
            }
        }
    }
}