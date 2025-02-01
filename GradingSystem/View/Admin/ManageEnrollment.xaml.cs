using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _context;
        private readonly EnrollmentViewModel _enrollment;
        private readonly ProgramViewModel _program;

        public ObservableCollection<Enrollment> Enrollments { get; set; }

        public ManageEnrollment(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Enrollments = new ObservableCollection<Enrollment>();
            _program = new ProgramViewModel(_context);
            LoadEnrollments();
            EnrollmentDataGrid.ItemsSource = Enrollments;
        }

        private async void LoadEnrollments()
        {
            var enrollments = await _context.Enrollments
                                            .Include(e => e.Program) // Eagerly load the Program entity
                                            .ToListAsync();
            foreach (var enrollment in enrollments)
            {
                Enrollments.Add(enrollment);
            }
        }

        private void EnrollStudentBtn(object sender, RoutedEventArgs e)
        {
            var viewModel = new EnrollmentViewModel(_context);
            var programViewModel = new ProgramViewModel(_context);
            var enrollStudentWindow = new EnrollStudent(viewModel, programViewModel);
            enrollStudentWindow.Show();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnrollmentDataGrid.SelectedItem is not Enrollment selectedEnrollment || selectedEnrollment.Student == null)
            {
                ShowNotification("Warning", "Please select a valid enrollment.", NotificationType.Warning);
                return;
            }

            try
            {
                var showGradeWindow = new ShowGrade(selectedEnrollment, _context);
                showGradeWindow.Show();
            }
            catch (Exception ex)
            {
                HandleError("An error occurred", ex);
            }
        }

        private void SearchTextBox(object sender, TextChangedEventArgs e)
        {

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
    }
}
