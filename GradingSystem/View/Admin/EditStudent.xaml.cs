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
        public Student SelectedStudent { get; set; }
        public ProgramViewModel ProgramViewModel { get; set; }

        public EditStudent(Student student, ProgramViewModel Programs)
        {
            InitializeComponent();

            ProgramViewModel = Programs;
            this.SelectedStudent = student;
            this.DataContext = this;

            _ = LoadProgramsAsync();
        }

        private async Task LoadProgramsAsync()
        {
            await ProgramViewModel.LoadProgramsAsync();
            programCmb.ItemsSource = ProgramViewModel.Programs;
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
            if (SelectedStudent != null)
            {
                try
                {
                    var viewModel = (StudentsViewModel)this.DataContext;

                    // Detach the existing tracked entity
                    var existingEntity = viewModel.Context.ChangeTracker.Entries<Student>()
                        .FirstOrDefault(e => e.Entity.StudentId == SelectedStudent.StudentId);
                    if (existingEntity != null)
                    {
                        viewModel.Context.Entry(existingEntity.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    }

                    // Detach the existing tracked Program entity
                    var existingProgramEntity = viewModel.Context.ChangeTracker.Entries<Program>()
                        .FirstOrDefault(e => e.Entity.ProgramId == SelectedStudent.ProgramId);
                    if (existingProgramEntity != null)
                    {
                        viewModel.Context.Entry(existingProgramEntity.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    }

                    // Update the student details
                    SelectedStudent.FirstName = FnameTxt.Text.Trim();
                    SelectedStudent.LastName = LnameTxt.Text.Trim();
                    SelectedStudent.Email = emailTxt.Text.Trim();
                    SelectedStudent.ProgramId = ((Program)programCmb.SelectedItem).ProgramId;
                    SelectedStudent.YearLevel = yearCmb.SelectedValue?.ToString() ?? string.Empty;
                    SelectedStudent.Semester = semesterCmb.SelectedValue?.ToString() ?? string.Empty;
                    SelectedStudent.Status = scholarCmb.SelectedValue?.ToString() ?? string.Empty;

                    viewModel.Context.Students.Update(SelectedStudent);
                    await viewModel.Context.SaveChangesAsync(); // Ensure the update is completed

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

