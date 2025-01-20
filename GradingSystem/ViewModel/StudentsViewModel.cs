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
            LoadStudentsAsync();
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
        public async Task AddStudentAsync(Student newStudent, string year, string semester, string programid)
        {
            if (newStudent == null) throw new ArgumentNullException(nameof(newStudent));

            try
            {
                if (await IsStudentDuplicateAsync(newStudent.StudentId))
                {
                    await ShowMessageAsync("Duplicate student found.", "Warning", MessageBoxImage.Warning);
                    return;
                }

                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();

                await AssignSubjectsToStudentAsync(newStudent.StudentId, year, semester, programid);

                Students.Add(newStudent); // ObservableCollection auto-updates the UI.

                await ShowMessageAsync("Student added successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("adding student", ex);
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
            // Ensure programId is not null or empty
            if (string.IsNullOrWhiteSpace(programId))
            {
                await ShowMessageAsync("Program ID is invalid.", "Error", MessageBoxImage.Error);
                return;
            }

            try
            {
                // Retrieve the program based on the programId
                var program = await _context.Programs
                    .FirstOrDefaultAsync(p => p.ProgramId == programId);

                // Check if the program was found
                if (program == null)
                {
                    await ShowMessageAsync("Program not found.", "Error", MessageBoxImage.Error);
                    return;
                }

                // Fetch subjects based on the given year, semester, and programId
                var subjects = await _context.Subjects
                    .Where(s => s.YearLevel == year && s.Semester == semester && s.ProgramId == programId) // Corrected filter to use ProgramId
                    .ToListAsync();

                // Check if any subjects were found
                if (!subjects.Any())
                {
                    await ShowMessageAsync("No subjects found for the given year, semester, and program.", "Warning", MessageBoxImage.Warning);
                    return;
                }

                // Create the StudentSubject entries for each subject found
                var studentSubjects = subjects.Select(subject => new StudentSubject
                {
                    StudentId = studentId,
                    SubjectId = subject.SubjectId,
                    Id = $"{studentId}_{subject.SubjectId}"
                }).ToList();

                // Add the new StudentSubjects to the context
                _context.StudentSubjects.AddRange(studentSubjects);

                // Save changes to the database
                var saveResult = await _context.SaveChangesAsync();

                // If no changes were made, notify the user
                if (saveResult == 0)
                {
                    await ShowMessageAsync("No subjects were assigned. Please check the input details and try again.", "Error", MessageBoxImage.Error);
                    return;
                }

                // Optionally show a success message with the program name
                await ShowMessageAsync($"Subjects assigned to the student for program '{program.ProgramName}' successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Log and display the error
                await HandleErrorAsync($"Error assigning subjects for program ID {programId}", ex);
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
