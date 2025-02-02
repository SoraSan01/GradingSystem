using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GradingSystem.View.Admin
{
    public partial class EditStudent : Window
    {
        public event Action? StudentEdited; // Declare the event as nullable
        private readonly Student _student;
        public StudentsViewModel StudentsViewModel { get; set; }

        public EditStudent(Student student, StudentsViewModel viewModel)
        {
            InitializeComponent();

            _student = student ?? throw new ArgumentNullException(nameof(student));
            StudentsViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = StudentsViewModel;

            // Initialize the fields with the student data
            FnameTxt.Text = _student.FirstName;
            LnameTxt.Text = _student.LastName;
            emailTxt.Text = _student.Email;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private async void saveBtn(object sender, RoutedEventArgs e)
        {
            if (_student != null)
            {
                try
                {
                    // Update the student details
                    _student.FirstName = FnameTxt.Text.Trim();
                    _student.LastName = LnameTxt.Text.Trim();
                    _student.Email = emailTxt.Text.Trim();

                    // Detach the existing tracked entity
                    var existingEntity = StudentsViewModel.Context.ChangeTracker.Entries<Student>()
                        .FirstOrDefault(entry => entry.Entity.StudentId == _student.StudentId);
                    if (existingEntity != null)
                    {
                        StudentsViewModel.Context.Entry(existingEntity.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    }

                    // Attach the updated student entity and mark it as modified
                    StudentsViewModel.Context.Students.Attach(_student);
                    StudentsViewModel.Context.Entry(_student).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    await StudentsViewModel.Context.SaveChangesAsync(); // Ensure the update is completed

                    // Close the window after editing
                    StudentEdited?.Invoke();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No student selected to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }

        private void IdTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox && (textBox.Text.Length >= 20 || !char.IsDigit(e.Text, 0)))
            {
                e.Handled = true; // Prevent further input if either condition is met
            }
        }

        private void FnameTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox && (textBox.Text.Length >= 20 || !char.IsLetter(e.Text, 0)))
            {
                e.Handled = true; // Prevent further input if either condition is met
            }
        }

        private void LnameTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox && (textBox.Text.Length >= 20 || !char.IsLetter(e.Text, 0)))
            {
                e.Handled = true; // Prevent further input if either condition is met
            }
        }

        private void emailTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text.Length >= 50)
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
            this.Close();
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
