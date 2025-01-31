using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using Notifications.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace GradingSystem.View.Admin
{
    public partial class ManageSubjects : UserControl
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();
        private readonly SubjectViewModel _subjectViewModel;
        private readonly ApplicationDbContext _context;

        public ManageSubjects(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            _subjectViewModel = new SubjectViewModel(context);
            DataContext = _subjectViewModel;
        }

        private async void AddSubject(object sender, RoutedEventArgs e)
        {
            var programViewModel = new ProgramViewModel(_context);
            var addSubjectWindow = new AddSubject(_context, programViewModel);
            addSubjectWindow.SubjectAdded += async () => await _subjectViewModel.LoadSubjectsAsync();
            bool? result = addSubjectWindow.ShowDialog();

            if (result == true)
            {
                await _subjectViewModel.LoadSubjectsAsync();

                // Show success notification
                _notificationManager.Show(new NotificationContent
                {
                    Title = "Success",
                    Message = "Subject added successfully.",
                    Type = NotificationType.Success
                });
            }
        }

        // Delete subject with confirmation dialog
        private async void DeleteSubjectBtn(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Subject subject)
            {
                var result = MessageBox.Show("Are you sure you want to delete this subject?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _subjectViewModel.DeleteSubjectAsync(subject);
                        _notificationManager.Show(new NotificationContent
                        {
                            Title = "Deleted",
                            Message = "Subject deleted successfully.",
                            Type = NotificationType.Warning
                        });
                    }
                    catch (Exception ex)
                    {
                        _notificationManager.Show(new NotificationContent
                        {
                            Title = "Error",
                            Message = $"Failed to delete subject: {ex.Message}",
                            Type = NotificationType.Error
                        });
                    }
                }
            }
        }

        // Edit subject logic and show notification after edit
        private async void EditSubjectBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedSubject = (Subject)subjectsDataGrid.SelectedItem;

            if (selectedSubject != null)
            {
                var viewModel = new SubjectViewModel(_context)
                {
                    SelectedSubject = selectedSubject
                };

                var editWindow = new EditSubject(selectedSubject, new ProgramViewModel(_context))
                {
                    DataContext = viewModel
                };

                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    await _subjectViewModel.LoadSubjectsAsync();

                    // Show notification after editing
                    _notificationManager.Show(new NotificationContent
                    {
                        Title = "Updated",
                        Message = "Subject updated successfully.",
                        Type = NotificationType.Information
                    });
                }
            }
            else
            {
                MessageBox.Show("Please select a subject to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Search logic for subject search box
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _subjectViewModel.SearchText = textBox.Text;
                _subjectViewModel.ApplySearch();
            }
        }
    }
}
