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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GradingSystem.View.Admin.Dialogs
{
    public partial class ShowGrade : Window
    {
        public string CurrentDate => DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

        private readonly EnrollmentViewModel _students;

        public ShowGrade(Enrollment enrollment)
        {
            InitializeComponent();
            DateText.Text = DateTime.Now.ToString("MMMM dd, yyyy");

            _students = App.ServiceProvider.GetRequiredService<EnrollmentViewModel>();

            // Load student data based on the enrollment info
            _ = LoadStudentDataAsync(enrollment.StudentId, enrollment.YearLevel, enrollment.Semester);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        // Modified LoadStudentDataAsync method that loads based on studentId, year, and semester
        private async Task LoadStudentDataAsync(string studentId, string yearLevel, string semester)
        {
            try
            {
                // Use a separate instance of ApplicationDbContext
                var viewModel = App.ServiceProvider.GetRequiredService<EnrollmentViewModel>();
                    await viewModel.LoadStudentDataAsync(studentId, yearLevel, semester);

                    // Ensure UI updates happen on the main thread
                    Dispatcher.Invoke(() =>
                    {
                        DataContext = viewModel;
                    });
            }
            catch (Exception ex)
            {
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

            // Find the buttons you want to exclude from printing
            var closeButton = CloseButton; // Assuming CloseButton is the x:Name of the close button
            var printButton = PrintButton; // Assuming PrintButton is the x:Name of the print button

            // Temporarily hide the buttons
            closeButton.Visibility = Visibility.Collapsed;
            printButton.Visibility = Visibility.Collapsed;

            if (printDialog.ShowDialog() == true)
            {
                // Ensure the window is fully loaded and rendered
                UpdateLayout();

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

            // Restore the buttons' visibility
            closeButton.Visibility = Visibility.Visible;
            printButton.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void PrintBtn(object sender, RoutedEventArgs e)
        {
            PrintDialog();
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
