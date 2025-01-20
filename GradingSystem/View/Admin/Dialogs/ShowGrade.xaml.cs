using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using GradingSystem.ViewModel;
using GradingSystem.Data;
using GradingSystem.Model;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media;
using static MaterialDesignThemes.Wpf.Theme;

namespace GradingSystem.View.Admin.Dialogs
{
    public partial class ShowGrade : Window
    {
        private readonly ApplicationDbContext _context;
        public StudentsViewModel students { get; set; }

        public ShowGrade(Student student, ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;

            // Initialize the ViewModel
            students = new StudentsViewModel(_context);

            // Asynchronously load the student data
            _ = LoadStudentDataAsync(student);
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private async Task LoadStudentDataAsync(Student student)
        {
            try
            {
                // Load the student data asynchronously
                await students.LoadStudentDataAsync(student);

                // Once data is loaded, update the UI on the main thread
                Dispatcher.Invoke(() =>
                {
                    DataContext = students;
                });
            }
            catch (Exception ex)
            {
                // Handle any potential errors
                MessageBox.Show($"An error occurred while loading student data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Call the method to print the content
            PrintDialog();
        }

        private void PrintDialog()
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                // Ensure the window is fully loaded and rendered
                this.UpdateLayout();

                // Hide the scrollbars of the DataGrid temporarily
                var dataGrid = GradeDataGrid;
                if (dataGrid != null)
                {
                    // Save the current scrollbar visibility settings
                    var horizontalScrollbarVisibility = dataGrid.HorizontalScrollBarVisibility;
                    var verticalScrollbarVisibility = dataGrid.VerticalScrollBarVisibility;

                    // Temporarily hide the scrollbars
                    dataGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    dataGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    // Get the actual size of the window's layout
                    var bounds = VisualTreeHelper.GetDescendantBounds(this);

                    // Create a visual representation of the entire window
                    var visual = new DrawingVisual();
                    using (var context = visual.RenderOpen())
                    {
                        // Create a rectangle of the same size as the window's layout and render it
                        context.DrawRectangle(new VisualBrush(this), null, new Rect(new Point(), bounds.Size));
                    }

                    // Set the print document size to match the window layout's size
                    printDialog.PrintTicket.PageMediaSize = new System.Printing.PageMediaSize(bounds.Width, bounds.Height);

                    // Print the visual representation of the entire window
                    printDialog.PrintVisual(visual, "Printing Window");

                    // Restore the original scrollbar visibility settings
                    dataGrid.HorizontalScrollBarVisibility = horizontalScrollbarVisibility;
                    dataGrid.VerticalScrollBarVisibility = verticalScrollbarVisibility;
                }
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initially print the content when the window is loaded
            PrintDialog();
        }
    }
}
