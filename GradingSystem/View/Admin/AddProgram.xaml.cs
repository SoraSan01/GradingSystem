using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for AddProgram.xaml
    /// </summary>
    public partial class AddProgram : Window
    {
        public ProgramViewModel ViewModel { get; set; }

        public event Action ProgramAdded;

        public AddProgram(ProgramViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        private void addProgramBtn(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                MessageBox.Show("ViewModel is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(ProgramIdTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(ProgramNameTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(DescriptionTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(MajorTxt.Text?.Trim()))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            try
            {
                using (var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()))
                {
                    var newProgram = new Program
                    {
                        ProgramId = ProgramIdTxt.Text.Trim(),
                        ProgramName = ProgramNameTxt.Text.Trim(),
                        Description = DescriptionTxt.Text.Trim(),
                        Major = MajorTxt.Text.Trim(),
                    };

                    ViewModel.AddProgramAsync(newProgram);

                    ProgramAdded?.Invoke();
                    clear();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }

        private void clear()
        {
            ProgramNameTxt.Clear();
            ProgramIdTxt.Clear();
            DescriptionTxt.Clear();
            MajorTxt.Clear();
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

        private void ProgramIdTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                // Ensure the length does not exceed the max limit (20 characters in this case)
                if (textBox.Text.Length >= 20 || !char.IsLetter(e.Text, 0))
                {
                    e.Handled = true; // Disallow the input
                }
            }
        }

        private void ProgramNameTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                // Ensure the length does not exceed the max limit (20 characters in this case)
                if (textBox.Text.Length >= 20 ||
                    (!char.IsLetter(e.Text, 0) && !char.IsWhiteSpace(e.Text, 0))) // Allow spaces for Program Name
                {
                    e.Handled = true; // Disallow the input
                }
            }
        }

        private void MajorTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                // Ensure the length does not exceed the max limit (20 characters in this case)
                if (textBox.Text.Length >= 20 ||
                    (!char.IsLetter(e.Text, 0) && e.Text != "-")) // Allow hyphen for Major (e.g., Computer-Science)
                {
                    e.Handled = true; // Disallow the input
                }
            }
        }


        private void DescriptionTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
