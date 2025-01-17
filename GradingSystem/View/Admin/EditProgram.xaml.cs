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
    /// Interaction logic for EditProgram.xaml
    /// </summary>
    public partial class EditProgram : Window
    {
        public ProgramViewModel ViewModel { get; set; }

        public EditProgram(Program selectedProgram)
        {
            InitializeComponent();
            ViewModel = new ProgramViewModel();
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
            if (string.IsNullOrWhiteSpace(ViewModel.SelectedProgram.ProgramName) || string.IsNullOrWhiteSpace(ViewModel.SelectedProgram.Description))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Call the EditProgram method from the ViewModel to update the program
            ViewModel.EditProgram(ViewModel.SelectedProgram);

            // Close the window after saving the changes
            this.Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
