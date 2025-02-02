using GradingSystem.ViewModel;
using System.Windows;
using System.Windows.Input;
using GradingSystem.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GradingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace GradingSystem.View.Admin.Dialogs
{
    public partial class EnrollStudent : Window
    {
        private readonly EnrollmentViewModel _viewModel;
        private readonly ProgramViewModel _programs;
        private readonly ApplicationDbContext _context;

        public EnrollStudent(ApplicationDbContext context, EnrollmentViewModel viewModel, ProgramViewModel programs)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _programs = programs ?? throw new ArgumentNullException(nameof(programs));
            DataContext = _viewModel;

            LoadPrograms();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Minimize(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void CloseWindow(object sender, RoutedEventArgs e) => this.Close();

        private void CancelBtn(object sender, RoutedEventArgs e) => this.Close();

        private async void LoadPrograms()
        {
            try
            {
                await _programs.LoadProgramsAsync();
                programCmb.ItemsSource = new ObservableCollection<Program>(_programs.Programs);
                programCmb.DisplayMemberPath = "ProgramName";
                programCmb.SelectedValuePath = "ProgramId";
            }
            catch (Exception ex)
            {
                ShowError("Failed to load programs", ex);
            }
        }

        private async void EnrollStudentBtn(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string studentId = idTxt.Text.Trim();
            string studentName = NameTxt.Text.Trim();
            string year = yearCmb.SelectedValue?.ToString();
            string semester = semesterCmb.SelectedValue?.ToString();
            string programId = programCmb.SelectedValue?.ToString();
            string status = scholarCmb.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(semester) || string.IsNullOrEmpty(programId) || string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Please select all required dropdown options.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _viewModel.AddEnrollmentAsync(studentId, studentName, year, semester, programId, status);

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Student enrolled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"An error occurred while enrolling the student.\n\nDetails: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        public async Task<string> GetStudentFullNameByIdAsync(string studentId)
        {
            try
            {
                using var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
                {
                    var student = await context.Students
                        .Where(s => s.StudentId == studentId)
                        .FirstOrDefaultAsync();
                    return student?.StudentName ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching the student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(idTxt.Text) ||
                string.IsNullOrWhiteSpace(NameTxt.Text) ||
                string.IsNullOrWhiteSpace(yearCmb.SelectedValue as string) ||
                string.IsNullOrWhiteSpace(semesterCmb.SelectedValue as string) ||
                string.IsNullOrWhiteSpace(programCmb.SelectedValue as string) ||
                string.IsNullOrWhiteSpace(scholarCmb.SelectedValue as string))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private static void ShowError(string message, Exception ex)
        {
            MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void IdTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input for the Student ID
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text)
        {
            return text.All(char.IsDigit);
        }

        private async void IdTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string studentId = idTxt.Text.Trim();
                if (!string.IsNullOrWhiteSpace(studentId))
                {
                    await LoadStudentNameAsync(studentId);
                }
            }
        }

        private async Task LoadStudentNameAsync(string studentId)
        {
            try
            {
                string fullName = await GetStudentFullNameByIdAsync(studentId);
                if (string.IsNullOrEmpty(fullName))
                {
                    MessageBox.Show("Student not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    NameTxt.Clear();
                }
                else
                {
                    NameTxt.Text = fullName;
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load student details", ex);
            }
        }
    }
}
