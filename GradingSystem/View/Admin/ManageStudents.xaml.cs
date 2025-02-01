using GradingSystem.Model;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using GradingSystem.View;
using GradingSystem.Data;
using System.Windows;
using GradingSystem.ViewModel;
using Notifications.Wpf;
using System;
using System.Linq;
using System.Threading.Tasks;
using GradingSystem.View.Admin.Dialogs;

namespace GradingSystem.View.Admin
{
    public partial class ManageStudents : UserControl
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();
        private readonly ApplicationDbContext _context;
        public StudentsViewModel students { get; set; }

        public ManageStudents(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            students = new StudentsViewModel(context);
            DataContext = students;
        }

        private void addStudentBtn(object sender, RoutedEventArgs e)
        {
            var programViewModel = new ProgramViewModel(_context);
            var addStudentWindow = new AddStudent(students, programViewModel, _context);

            addStudentWindow.StudentAdded += async () =>
            {
                await ReloadStudentsAsync();
                ShowNotification("Success", "Student added successfully!", NotificationType.Success);
            };

            addStudentWindow.Show();
        }

        private async void deleteStudentBtn(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var studentToDelete = button?.DataContext as Student;

            if (studentToDelete != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {studentToDelete.FirstName} {studentToDelete.LastName}?",
                    "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await students.DeleteStudentAsync(studentToDelete);
                        await ReloadStudentsAsync();
                        ShowNotification("Success", "Student deleted successfully!", NotificationType.Success);
                    }
                    catch (Exception ex)
                    {
                        ShowNotification("Error", $"Failed to delete student: {ex.Message}", NotificationType.Error);
                    }
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = (Student)studentsDataGrid.SelectedItem;

            if (selectedStudent != null)
            {
                var programViewModel = new ProgramViewModel(_context)
                {
                    SelectedProgram = selectedStudent.Program
                };

                var viewModel = new StudentsViewModel(_context)
                {
                    SelectedStudent = selectedStudent
                };

                var editWindow = new EditStudent(selectedStudent, programViewModel)
                {
                    DataContext = viewModel
                };

                editWindow.StudentEdited += async () =>
                {
                    await ReloadStudentsAsync();
                    ShowNotification("Success", "Student details updated!", NotificationType.Information);
                };

                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a student to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchTextBox(object sender, TextChangedEventArgs e)
        {
            if (students == null || students.Students == null) return;

            var searchText = (sender as TextBox)?.Text?.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                students.FilteredStudents = new ObservableCollection<Student>(students.Students);
            }
            else
            {
                students.FilteredStudents = new ObservableCollection<Student>(
                    students.Students.Where(s => s.StudentName.ToLower().Contains(searchText) ||
                                                 s.StudentId.ToString().Contains(searchText) ||
                                                 s.Program.ProgramName.ToLower().Contains(searchText) ||
                                                 s.YearLevel.ToString().Contains(searchText) ||
                                                 s.Semester.ToString().Contains(searchText))
                );
            }

            studentsDataGrid.ItemsSource = students.FilteredStudents;
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

        private async Task ReloadStudentsAsync()
        {
            try
            {
                await students.LoadStudentsAsync();
            }
            catch (Exception ex)
            {
                ShowNotification("Error", $"Failed to load students: {ex.Message}", NotificationType.Error);
            }
        }

        private void ShowGradeBtn(object sender, RoutedEventArgs e)
        {
            if (studentsDataGrid.SelectedItem is not Student selectedStudent)
            {
                ShowNotification("Warning", "Please select a student.", NotificationType.Warning);
                return;
            }

            try
            {
                var showGradeWindow = new ShowGrade(selectedStudent, _context)
                {
                    DataContext = students
                };
                showGradeWindow.Show();
            }
            catch (Exception ex)
            {
                HandleError("An error occurred", ex);
            }
        }

        private void HandleError(string action, Exception ex)
        {
            // Log the error (consider using a logging framework)
            Console.Error.WriteLine($"{action}: {ex}");

            ShowNotification("Error", $"{action}: {ex.Message}", NotificationType.Error);
        }

    }
}
