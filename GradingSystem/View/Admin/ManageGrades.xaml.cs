using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GradingSystem.View.Admin
{
    public partial class ManageGrades : UserControl
    {
        public event Action<UIElement> UpdateMainContent;

        private readonly ApplicationDbContext _context;
        public GradeViewModel Grades { get; set; }

        public ManageGrades(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            Grades = new GradeViewModel(_context);
            DataContext = Grades;

            // Load grades on initialization
            LoadGrades();
        }

        private async void LoadGrades()
        {
            try
            {
                await Grades.LoadGradeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading grades: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintGrades(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    var printVisual = studentsDataGrid as Visual;
                    if (printVisual != null)
                    {
                        printDialog.PrintVisual(printVisual, "Grades Report");
                    }
                    else
                    {
                        MessageBox.Show("No data available for printing.", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while printing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddGradeBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                var addGradeDialog = new AddGrade(_context);
                addGradeDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the Add Grade dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowGradeBtn(object sender, RoutedEventArgs e)
        {
            // Get the selected grade from the DataGrid (assuming the DataGrid contains Grade objects)
            var selectedGrade = studentsDataGrid.SelectedItem as Grade;

            // Check if the selected item is valid (not null)
            if (selectedGrade != null)
            {
                // Create a new instance of the ShowGrade dialog
                var showGradeWindow = new ShowGrade
                {
                    // Set the DataContext to the selected grade so that it can be bound in the ShowGrade view
                    DataContext = selectedGrade
                };

                // Show the dialog as a modal window (it will block the user from interacting with other windows until closed)
                showGradeWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a grade from the list.");
            }
        }

    }
}
