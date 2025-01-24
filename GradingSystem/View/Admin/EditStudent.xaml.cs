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
using System.Windows.Shapes;

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for EditStudent.xaml
    /// </summary>
    public partial class EditStudent : Window
    {
        public Student SelectedStudent { get; set; }
        public ProgramViewModel ProgramViewModel { get; set; }

        public EditStudent(Student student, ProgramViewModel Programs)
        {
            InitializeComponent();

            ProgramViewModel = Programs;
            this.SelectedStudent = student;
            this.DataContext = this;

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
                    // Call the ViewModel's EditStudent method and await its completion
                    var viewModel = (StudentsViewModel)this.DataContext;
                    await viewModel.EditStudentAsync(SelectedStudent); // Ensure the update is completed

                    // Close the window after editing
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
            TextBox textBox = sender as TextBox;

            // Check if text length exceeds the limit or if the character is not a letter
            if (textBox != null && (textBox.Text.Length >= 20 || !char.IsDigit(e.Text, 0)))
            {
                e.Handled = true; // Prevent further input if either condition is met
            }
        }

        private void FnameTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if text length exceeds the limit or if the character is not a letter
            if (textBox != null && (textBox.Text.Length >= 20 || !char.IsLetter(e.Text, 0)))
            {
                e.Handled = true; // Prevent further input if either condition is met
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

        private void emailTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if text length exceeds the limit or if the character is not a letter
            if (textBox != null && (textBox.Text.Length >= 50))
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
