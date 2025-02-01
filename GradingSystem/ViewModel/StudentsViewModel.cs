using GradingSystem.Data;
using GradingSystem.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class StudentsViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        private ObservableCollection<Student> _filteredStudents = new ObservableCollection<Student>();

        private DateTime _currentDate = DateTime.Now;
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
            }
        }

        public ObservableCollection<Student> FilteredStudents
        {
            get => _filteredStudents;
            set
            {
                _filteredStudents = value;
                OnPropertyChanged(nameof(FilteredStudents));
            }
        }

        public StudentsViewModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _ = LoadStudentsAsync();
        }

        public ApplicationDbContext Context => _context;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (_selectedStudent != value)
                {
                    _selectedStudent = value;
                    OnPropertyChanged(nameof(SelectedStudent));
                }
            }
        }

        public async Task LoadStudentsAsync()
        {
            try
            {
                var studentsList = await _context.Students
                    .AsNoTracking()
                    .ToListAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Students.Clear();
                    foreach (var student in studentsList)
                    {
                        Students.Add(student);
                    }
                });
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("loading students", ex);
            }
        }

        public async Task AddStudentAsync(Student newStudent)
        {
            if (newStudent == null) throw new ArgumentNullException(nameof(newStudent));

            try
            {
                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Students.Add(newStudent);
                });

                await ShowMessageAsync("Student added successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("adding student", ex);
            }
        }


        public async Task EditStudentAsync(Student updatedStudent)
        {
            if (updatedStudent == null) throw new ArgumentNullException(nameof(updatedStudent));

            try
            {
                var studentToUpdate = await _context.Students.FindAsync(updatedStudent.StudentId);
                if (studentToUpdate == null)
                {
                    await ShowMessageAsync("Student not found.", "Error", MessageBoxImage.Error);
                    return;
                }

                studentToUpdate.FirstName = updatedStudent.FirstName;
                studentToUpdate.LastName = updatedStudent.LastName;
                studentToUpdate.Email = updatedStudent.Email;

                await _context.SaveChangesAsync();

                UpdateStudentInUI(updatedStudent);

                await ShowMessageAsync("Student updated successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("editing student", ex);
            }
        }

        private void UpdateStudentInUI(Student updatedStudent)
        {
            var studentInCollection = Students.FirstOrDefault(s => s.StudentId == updatedStudent.StudentId);
            if (studentInCollection != null)
            {
                studentInCollection.FirstName = updatedStudent.FirstName;
                studentInCollection.LastName = updatedStudent.LastName;
                studentInCollection.Email = updatedStudent.Email;
            }
        }

        public async Task DeleteStudentAsync(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            try
            {
                var confirmResult = MessageBox.Show($"Are you sure you want to delete the student '{student.FirstName} {student.LastName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    var existingEntity = _context.ChangeTracker.Entries<Student>()
                        .FirstOrDefault(e => e.Entity.StudentId == student.StudentId);
                    if (existingEntity != null)
                    {
                        _context.Entry(existingEntity.Entity).State = EntityState.Detached;
                    }

                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Students.Remove(student);
                    });

                    await ShowMessageAsync("Student deleted successfully.", "Success", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("deleting student", ex);
            }
        }

        private async Task ShowMessageAsync(string message, string caption, MessageBoxImage icon)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
            });
        }

        private async Task HandleErrorAsync(string action, Exception ex)
        {
            await ShowMessageAsync($"An error occurred while {action}: {ex.Message}", "Error", MessageBoxImage.Error);
        }
    }
}
