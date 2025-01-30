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
        public ProgramViewModel Program { get; set; }

        public event Action StudentAdded;

        // Inject ApplicationDbContext via dependency injection
        private readonly ApplicationDbContext _context;

        public AddStudent(StudentsViewModel viewModel, ProgramViewModel programViewModel, ApplicationDbContext context)
        {
            InitializeComponent();
            ViewModel = viewModel;
            Program = programViewModel;
            _context = context ?? throw new ArgumentNullException(nameof(context));

            DataContext = ViewModel;

            // Load programs asynchronously
            LoadProgramsAsync();
        }

        private async Task LoadProgramsAsync()
        {
            await Program.LoadProgramsAsync();
            programCmb.ItemsSource = Program.Programs;
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
                string programId = programCmb.SelectedValue?.ToString();
                string status = scholarCmb.SelectedValue?.ToString();

                if (string.IsNullOrWhiteSpace(programId))
                {
                    MessageBox.Show("Please select a valid program.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await ViewModel.AddStudentAsync(newStudent, year, semester, programId, status);

                StudentAdded?.Invoke(); // Notify parent component
                clear();
                MessageBox.Show("Student added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                ProgramId = programCmb.SelectedValue?.ToString(),
                YearLevel = yearCmb.SelectedValue?.ToString(),
                Semester = semesterCmb.SelectedValue?.ToString(),
                Status = scholarCmb.SelectedValue?.ToString()
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

        private void FnameTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if text length exceeds the limit or if the character is not a letter
            if (textBox != null && (textBox.Text.Length >= 20 || !char.IsLetter(e.Text, 0)))
            {
                e.Handled = true;
            }
        }

        private void LnameTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if text length exceeds the limit or if the character is not a letter
            if (textBox != null && (textBox.Text.Length >= 20 || !char.IsLetter(e.Text, 0)))
            {
                e.Handled = true; // Prevent further input if either condition is met
            }
        }

        private void IdTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if text length exceeds the limit or if the character is not a letter
            if (textBox != null && (textBox.Text.Length >= 15 || !char.IsDigit(e.Text, 0)))
            {
                e.Handled = true; // Prevent further input if either condition is met
            }
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void emailTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && (textBox.Text.Length >= 50))
            {
                e.Handled = true;
            }
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
