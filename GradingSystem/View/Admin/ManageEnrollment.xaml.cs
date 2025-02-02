using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ManageEnrollment.xaml
    /// </summary>
    public partial class ManageEnrollment : UserControl
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();
        private readonly ProgramViewModel _program;
        private readonly EnrollmentViewModel _enrollmentViewModel;

        public ObservableCollection<Enrollment> Enrollments { get; set; }

        public ManageEnrollment(ApplicationDbContext context)
        {
            InitializeComponent();

            Enrollments = new ObservableCollection<Enrollment>();
            _program = new ProgramViewModel(context);
            _enrollmentViewModel = App.ServiceProvider.GetRequiredService<EnrollmentViewModel>();
            DataContext = _enrollmentViewModel;
            EnrollmentDataGrid.DataContext = _enrollmentViewModel;

            InitializeAsync();
        }
        private async Task InitializeAsync()
        {
            try
            {
                await _enrollmentViewModel.LoadEnrollmentsAsync(); // Assuming this loads the data asynchronously
                EnrollmentDataGrid.ItemsSource = _enrollmentViewModel.Enrollments; // Set the data source
            }
            catch (Exception ex)
            {
                HandleError("Failed to load enrollments", ex);
            }
        }


        private void EnrollStudentBtn(object sender, RoutedEventArgs e)
        {
            using (var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()))
            {
                var viewModel = App.ServiceProvider.GetRequiredService<EnrollmentViewModel>();
                var programViewModel = new ProgramViewModel(context);
                var enrollStudentWindow = new EnrollStudent(context ,viewModel, programViewModel);
                enrollStudentWindow.Show();
            }
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnrollmentDataGrid.SelectedItem is not Enrollment selectedEnrollment)
            {
                ShowNotification("Warning", "Please select a valid enrollment.", NotificationType.Warning);
                return;
            }

            try
            {
                // Load Student data separately
                using (var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()))
                {
                    // Modify the query to ensure it checks both StudentId, YearLevel, and Semester
                    var enrollment = context.Enrollments
                                              .Include(e => e.Student) // Load Student data
                                              .Include(e => e.Program)
                                              .FirstOrDefault(e => e.StudentId == selectedEnrollment.StudentId &&
                                                                   e.YearLevel == selectedEnrollment.YearLevel &&
                                                                   e.Semester == selectedEnrollment.Semester);

                    if (enrollment?.Student == null)
                    {
                        ShowNotification("Warning", "The selected enrollment has no associated student.", NotificationType.Warning);
                        return;
                    }

                    // Show the grade window with the correct enrollment information
                    var showGradeWindow = new ShowGrade(enrollment);
                    showGradeWindow.Show();
                }
            }
            catch (Exception ex)
            {
                HandleError("An error occurred", ex);
            }
        }
        private void SearchTextBox(object sender, TextChangedEventArgs e)
        {
            // Implement search functionality here
        }

        private void HandleError(string action, Exception ex)
        {
            // Log the error (consider using a logging framework)
            Console.Error.WriteLine($"{action}: {ex}");

            ShowNotification("Error", $"{action}: {ex.Message}", NotificationType.Error);
        }

        private void ShowNotification(string title, string message, NotificationType type)
        {
            _notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = type
            });
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = App.ServiceProvider.GetRequiredService<EnrollmentViewModel>();
                await viewModel.LoadEnrollmentsAsync();
                EnrollmentDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                HandleError("Failed to refresh enrollments", ex);
            }
        }

    }
}
