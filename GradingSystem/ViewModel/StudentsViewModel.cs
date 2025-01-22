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
        public ObservableCollection<Student> Students { get; set; }

        private ObservableCollection<StudentSubject> _studentSubjects = new ObservableCollection<StudentSubject>();
        public ObservableCollection<StudentSubject> StudentSubjects
        {
            get { return _studentSubjects; }
            set
            {
                _studentSubjects = value;
                OnPropertyChanged(nameof(StudentSubjects));
            }
        }

        // Constructor with dependency injection for ApplicationDbContext
        public StudentsViewModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Students = new ObservableCollection<Student>();
            _ = LoadStudentsAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Select a student and load their subjects
        public async Task SelectStudentAsync(Student student)
        {
            if (student == null)
            {
                await ShowMessageAsync("No student selected.", "Error", MessageBoxImage.Error);
                return;
            }

            SelectedStudent = student;
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
                    LoadStudentSubjectsAsync(_selectedStudent.StudentId);
                }
            }
        }

        public async Task LoadStudentSubjectsAsync(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) return;

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var subjects = await context.StudentSubjects
                        .Include(ss => ss.Subject)
                        .Where(ss => ss.StudentId == studentId)
                        .ToListAsync();

                    StudentSubjects.Clear();
                    foreach (var subject in subjects)
                    {
                        StudentSubjects.Add(subject);
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("loading subjects", ex);
            }
        }


        // Load student data (general method for both editing and selecting)
        public async Task LoadStudentDataAsync(Student student)
        {
            if (student == null)
            {
                await ShowMessageAsync("No student selected.", "Error", MessageBoxImage.Error);
                return;
            }

            SelectedStudent = student;
            await LoadStudentSubjectsAsync(student.StudentId);
        }

        // Load all students asynchronously and update the UI
        public async Task LoadStudentsAsync()
        {
            using var context = new ApplicationDbContext();
            try
            {
                var studentsList = await context.Students.ToListAsync().ConfigureAwait(false); // Avoid UI thread dependency
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

        // Add a new student to the database
        public async Task AddStudentAsync(Student newStudent, string year, string semester, string programId, string status)
        {
            if (newStudent == null) throw new ArgumentNullException(nameof(newStudent));

            try
            {
                if (await IsStudentDuplicateAsync(newStudent.StudentId))
                {
                    await ShowMessageAsync("A student with this ID already exists.", "Duplicate Entry", MessageBoxImage.Warning);
                    return;
                }

                // Validate the program ID
                var program = await _context.Programs.FirstOrDefaultAsync(p => p.ProgramId == programId);
                if (program == null)
                {
                    await ShowMessageAsync("Selected program does not exist.", "Error", MessageBoxImage.Error);
                    return;
                }

                // Add the student
                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();

                // Assign subjects to the student
                await AssignSubjectsToStudentAsync(newStudent.StudentId, year, semester, programId);

                // Update the ObservableCollection on the UI thread
                Application.Current.Dispatcher.Invoke(() => Students.Add(newStudent));

                await ShowMessageAsync("Student added and subjects assigned successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("adding the student", ex);
            }
        }

        // Check if a student is a duplicate based on StudentId
        private async Task<bool> IsStudentDuplicateAsync(string studentId)
        {
            return await _context.Students.AnyAsync(s => s.StudentId == studentId);
        }

        // Assign subjects to a student after adding them
        private async Task AssignSubjectsToStudentAsync(string studentId, string year, string semester, string programId)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(year) || string.IsNullOrWhiteSpace(semester) || string.IsNullOrWhiteSpace(programId))
            {
                await ShowMessageAsync("Invalid input for assigning subjects.", "Error", MessageBoxImage.Error);
                return;
            }

            try
            {
                var subjects = await _context.Subjects
                    .Where(s => s.YearLevel == year && s.Semester == semester && s.ProgramId == programId)
                    .ToListAsync();

                if (!subjects.Any())
                {
                    await ShowMessageAsync("No matching subjects found for the selected year, semester, and program.", "Warning", MessageBoxImage.Warning);
                    return;
                }

                var studentSubjects = subjects.Select(s => new StudentSubject
                {
                    StudentId = studentId,
                    SubjectId = s.SubjectId,
                    Id = $"{studentId}_{s.SubjectId}"
                }).ToList();

                _context.StudentSubjects.AddRange(studentSubjects);
                await _context.SaveChangesAsync();

                await ShowMessageAsync($"Successfully assigned {subjects.Count} subjects to the student.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("assigning subjects", ex);
            }
        }

        // Edit an existing student
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
                studentToUpdate.Program = updatedStudent.Program;
                studentToUpdate.YearLevel = updatedStudent.YearLevel;

                await _context.SaveChangesAsync();

                UpdateStudentInUI(updatedStudent);

                await ShowMessageAsync("Student updated successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("editing student", ex);
            }
        }

        // Update the student details in the UI
        private void UpdateStudentInUI(Student updatedStudent)
        {
            var studentInCollection = Students.FirstOrDefault(s => s.StudentId == updatedStudent.StudentId);
            if (studentInCollection != null)
            {
                studentInCollection.FirstName = updatedStudent.FirstName;
                studentInCollection.LastName = updatedStudent.LastName;
                studentInCollection.Email = updatedStudent.Email;
                studentInCollection.Program = updatedStudent.Program;
                studentInCollection.YearLevel = updatedStudent.YearLevel;
            }
        }

        // Delete a student from the database
        public async Task DeleteStudentAsync(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            try
            {
                var confirmResult = MessageBox.Show($"Are you sure you want to delete the student '{student.FirstName} {student.LastName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();

                    Students.Remove(student);

                    await ShowMessageAsync("Student deleted successfully.", "Success", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("deleting student", ex);
            }
        }

        // Centralized method for displaying messages asynchronously
        private async Task ShowMessageAsync(string message, string caption, MessageBoxImage icon)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
            });
        }

        // Centralized error handling method
        private async Task HandleErrorAsync(string action, Exception ex)
        {
            await ShowMessageAsync($"An error occurred while {action}: {ex.Message}", "Error", MessageBoxImage.Error);
        }
    }
}
