using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for AddStudent.xaml
    /// </summary>
    public partial class AddStudent : Window
    {
        public StudentsViewModel ViewModel { get; set; }

        public event Action StudentAdded;

        public AddStudent(StudentsViewModel viewModel)
        {
            InitializeComponent();

            // Set the ViewModel from the constructor parameter
            ViewModel = viewModel;

            // Optional: Set DataContext if you want to bind to the ViewModel in XAML
            DataContext = ViewModel;

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void addStudentBtn(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                MessageBox.Show("ViewModel is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(FnameTxt.Text) || string.IsNullOrWhiteSpace(LnameTxt.Text)
                || courseCmb.SelectedItem == null || yearCmb.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var newStudent = new Student
                    {
                        StudentId = context.GenerateStudentId(),
                        FirstName = FnameTxt.Text,
                        LastName = LnameTxt.Text,
                        Course = courseCmb.SelectedValue?.ToString(),
                        YearLevel = yearCmb.SelectedValue?.ToString(),
                    };

                    ViewModel.AddStudent(newStudent);

                    StudentAdded?.Invoke();
                    MessageBox.Show("Student added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    clear();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void clear()
        {
            FnameTxt.Clear();
            LnameTxt.Clear();
            courseCmb.SelectedIndex = -1;
            yearCmb.SelectedIndex = -1;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                // Call DragMove to allow the window to be dragged
                this.DragMove();
            }
        }
    }
}
