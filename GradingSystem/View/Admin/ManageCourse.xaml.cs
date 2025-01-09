using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for ManageCourse.xaml
    /// </summary>
    public partial class ManageCourse : UserControl
    {
        public readonly ApplicationDbContext _context;

        public CourseViewModel course { get; set; }


        public ManageCourse(ApplicationDbContext context)
        {
            InitializeComponent();

            _context = context;

            // Initialize the ViewModel
            course = new CourseViewModel();

            // Set the DataContext for binding, if required
            DataContext = course;
        }

        private void AddBtn(object sender, RoutedEventArgs e)
        {
            // Open the AddStudent window and pass the ViewModel
            var addCourseWindow = new AddProgram(course);
            addCourseWindow.ProgramAdded += () =>
            {
                // Refresh the list of students when a new student is added
                course.LoadCourse();
            };
            addCourseWindow.ShowDialog();
        }

        private void DeleteBtn(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            var CourseToDelete = button?.DataContext as Course;

            if (CourseToDelete != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {CourseToDelete.CourseName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    course.DeleteCourse(CourseToDelete);
                }
            }
        }
    }
}
