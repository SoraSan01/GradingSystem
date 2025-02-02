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
    /// Interaction logic for EditProgram.xaml
    /// </summary>
    public partial class EditProgram : Window
    {
        public ProgramViewModel ViewModel { get; set; }
        public event Action ProgramUpdated;
        private readonly ApplicationDbContext _context;


        public EditProgram(ApplicationDbContext context, Program selectedProgram)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            ViewModel = new ProgramViewModel(_context);
            ViewModel.SelectedProgram = selectedProgram;
            DataContext = ViewModel;

            // If the program is null, close the window
            if (ViewModel.SelectedProgram == null)
            {
                MessageBox.Show("No program selected to edit.");
                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }

        private void SaveBtn(object sender, RoutedEventArgs e)
        {
            // Validate the input values
            if (string.IsNullOrWhiteSpace(ViewModel.SelectedProgram.ProgramName) ||
                string.IsNullOrWhiteSpace(ViewModel.SelectedProgram.Description) ||
                string.IsNullOrEmpty(ViewModel.SelectedProgram.ProgramId))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Save the changes to the program
            ViewModel.EditProgramAsync(ViewModel.SelectedProgram);

            // Raise the event to notify that the program has been updated
            ProgramUpdated?.Invoke();

            // Close the window after saving
            this.Close();
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
                ProgramUpdated?.Invoke();
            }
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            // Optionally, ask the user if they want to discard changes before closing the window
            var result = MessageBox.Show("Are you sure you want to discard your changes?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close(); // Simply close the window without saving changes
                ProgramUpdated?.Invoke();
            }
        }

    }
}
