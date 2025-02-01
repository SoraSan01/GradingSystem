using GradingSystem.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using GradingSystem.Model;
using System.Collections.ObjectModel;

namespace GradingSystem.View.Admin.Dialogs
{
    public partial class EnrollStudent : Window
    {
        private readonly EnrollmentViewModel _viewModel;
        private readonly ProgramViewModel _programs;

        public EnrollStudent(EnrollmentViewModel viewModel, ProgramViewModel programs)
        {
            InitializeComponent();
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

        private void Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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
                MessageBox.Show($"Failed to load programs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EnrollStudentBtn(object sender, RoutedEventArgs e)
        {
            string studentId = idTxt.Text;
            string studentName = NameTxt.Text;
            string year = yearCmb.SelectedValue as string;
            string semester = semesterCmb.SelectedValue as string;
            string programId = programCmb.SelectedValue as string;
            string status = scholarCmb.SelectedValue as string;

            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(studentName) || string.IsNullOrWhiteSpace(year) || string.IsNullOrWhiteSpace(semester) || string.IsNullOrWhiteSpace(programId) || string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                await _viewModel.AddEnrollmentAsync(studentId, studentName, studentName, year, semester, programId, status);
                MessageBox.Show("Student enrolled successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IdTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Ensure that only numeric input is allowed for the Student ID field
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private async void idTxt_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string studentId = idTxt.Text;

            if (!string.IsNullOrWhiteSpace(studentId))
            {
                var student = await _viewModel.GetStudentByIdAsync(studentId);
                if (student != null)
                {
                    NameTxt.Text = student.StudentName;  // Set the student name in the text box
                }
                else
                {
                    NameTxt.Clear();  // Clear the name if student not found
                }
            }
        }
    }
}
