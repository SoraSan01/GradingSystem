using GradingSystem.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            Programs = new ProgramViewModel();

            // Set the DataContext for binding, if required
            DataContext = Programs;
        }

        private void AddBtn(object sender, RoutedEventArgs e)
        {
            // Open the AddStudent window and pass the ViewModel
            var addCourseWindow = new AddProgram(Programs);
            addCourseWindow.ProgramAdded += () =>
            {
                // Refresh the list of students when a new student is added
                Programs.LoadProgramsAsync();
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
                    Programs.DeleteProgram(ProgramToDelete);
                }
            }
        }

        private void EditBtn(object sender, RoutedEventArgs e)
        {
            var SelectedProgram = (Program)programDataGrid.SelectedItem; // Get selected student

            if (SelectedProgram != null)
            {
                // Pass the selected student to the EditStudent window
                var editWindow = new EditProgram(SelectedProgram); // Pass the selected student to the constructor

                // You can also set the DataContext if needed
                var viewModel = new ProgramViewModel();
                viewModel.SelectedProgram = SelectedProgram;
                editWindow.DataContext = viewModel;

                // Show the window
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a Program to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
