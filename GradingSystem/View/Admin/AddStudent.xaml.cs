using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;

namespace GradingSystem.View.Admin
{
    public partial class AddStudent : Window
    {
        public StudentsViewModel ViewModel { get; set; }
        public ProgramViewModel ProgramViewModel { get; set; }

        public event Action StudentAdded;

        // Inject ApplicationDbContext via dependency injection
        private readonly ApplicationDbContext _context;

        public AddStudent(StudentsViewModel viewModel, ProgramViewModel programViewModel, ApplicationDbContext context)
        {
            InitializeComponent();
            ViewModel = viewModel;
            ProgramViewModel = programViewModel;
            _context = context ?? throw new ArgumentNullException(nameof(context));

            programCmb.ItemsSource = ProgramViewModel.Programs;
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

        private async void AddStudentBtn(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            try
            {
                var newStudent = CreateStudent();
                string year = yearCmb.SelectedValue?.ToString();
                string semester = semesterCmb.SelectedValue?.ToString();
                string program = programCmb.SelectedValue?.ToString();

                // Pass year and semester to AddStudentAsync
                await ViewModel.AddStudentAsync(newStudent, year, semester, program);

                StudentAdded?.Invoke();
                clear();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Simplified validation method
        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(FnameTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(LnameTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(idTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(emailTxt.Text?.Trim()) ||
                programCmb.SelectedItem == null || semesterCmb.SelectedItem == null ||
                yearCmb.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        // Create a new student object
        private Student CreateStudent()
        {
            return new Student
            {
                StudentId = idTxt.Text.Trim(),
                Email = emailTxt.Text.Trim(),
                FirstName = FnameTxt.Text.Trim(),
                LastName = LnameTxt.Text.Trim(),
                Program = programCmb.SelectedValue?.ToString(),
                YearLevel = yearCmb.SelectedValue?.ToString(),
                Semester = semesterCmb.SelectedValue?.ToString(),
            };
        }

        // Clear the form inputs
        private void clear()
        {
            FnameTxt.Clear();
            LnameTxt.Clear();
            idTxt.Clear();
            emailTxt.Clear();
            programCmb.SelectedIndex = -1;
            yearCmb.SelectedIndex = -1;
            semesterCmb.SelectedIndex = -1;
            FnameTxt.Focus();
        }

        // Allow window dragging by clicking anywhere outside textboxes and buttons
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }
    }
}
