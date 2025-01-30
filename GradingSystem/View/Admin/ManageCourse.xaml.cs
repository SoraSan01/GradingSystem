using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for ManageCourse.xaml
    /// </summary>
    public partial class ManageCourse : UserControl
    {
        public readonly ApplicationDbContext _context;

        public ProgramViewModel Programs { get; set; }

        public ManageCourse(ApplicationDbContext context)
        {
            InitializeComponent();

            _context = context;

            // Initialize the ViewModel
            Programs = new ProgramViewModel(_context);

            // Set the DataContext for binding, if required
            DataContext = Programs;

            // Load initial data
            _ = Programs.LoadProgramsAsync();
        }

        private void AddBtn(object sender, RoutedEventArgs e)
        {
            // Open the AddProgram window and pass the ViewModel
            var addCourseWindow = new AddProgram(Programs);
            addCourseWindow.ProgramAdded += async () =>
            {
                // Refresh the list of programs when a new program is added
                await Programs.LoadProgramsAsync();
            };
            addCourseWindow.ShowDialog();
        }

        private void DeleteBtn(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var ProgramToDelete = button?.DataContext as Program;

            if (ProgramToDelete != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {ProgramToDelete.ProgramName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _ = Programs.DeleteProgramAsync(ProgramToDelete);
                    // Refresh the list after deletion
                    _ = Programs.LoadProgramsAsync();
                }
            }
        }

        private void EditBtn(object sender, RoutedEventArgs e)
        {
            var SelectedProgram = (Program)programDataGrid.SelectedItem; // Get selected program

            if (SelectedProgram != null)
            {
                // Pass the selected program to the EditProgram window
                var editWindow = new EditProgram(SelectedProgram); // Pass the selected program to the constructor

                // You can also set the DataContext if needed
                var viewModel = new ProgramViewModel(_context);
                viewModel.SelectedProgram = SelectedProgram;
                editWindow.DataContext = viewModel;

                // Subscribe to the event to refresh the list after editing
                editWindow.ProgramUpdated += async () =>
                {
                    // Refresh the list of programs after editing
                    await Programs.LoadProgramsAsync();
                };

                // Show the window
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a Program to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchTextBox(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox searchBox)
            {
                string filter = searchBox.Text.ToLower();

                // Apply filter to the list
                programDataGrid.ItemsSource = string.IsNullOrWhiteSpace(filter)
                    ? Programs.Programs // Reset to full list when search is empty
                    : Programs.Programs.Where(p =>
                        p.ProgramName.ToLower().Contains(filter) ||
                        p.Description.ToLower().Contains(filter) ||
                        p.Major.ToLower().Contains(filter)).ToList();
            }
        }
    }
}