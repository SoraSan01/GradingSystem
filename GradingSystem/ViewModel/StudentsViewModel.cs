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

        private DateTime _currentDate = DateTime.Now;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate)); // Ensure PropertyChanged is triggered
            }
        }

        private string? _program;
        public string? Program
        {
            get => _program;
            set
            {
                if (_program != value)
                {
                    _program = value;
                    OnPropertyChanged(nameof(Program));
                }
            }
        }

        private string? _semester;
        public string? Semester
        {
            get => _semester;
            set
            {
                if (_semester != value)
                {
                    _semester = value;
                    OnPropertyChanged(nameof(Semester));
                }
            }
        }

        private string? _status;
        public string? Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
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

            // Ensure Program, Semester, and Status are set correctly
            if (SelectedStudent.Program != null)
            {
                Program = SelectedStudent.Program?.ProgramId;
            }

            Semester = SelectedStudent.Semester; // Assuming you have a property Semester
            Status = SelectedStudent.Status; // Assuming you have a property Status

            await LoadStudentSubjectsAsync(student.StudentId);
        }


        // Load all students asynchronously and update the UI
        public async Task LoadStudentsAsync()
        {
            using var context = new ApplicationDbContext();
            try
            {
                // Include the Program navigation property
                var studentsList = await context.Students
                                 .Include(s => s.Program)
                                 .ToListAsync();


                // Update the UI thread to update the observable collection
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
                // Check if student ID already exists
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

                // Add the new student to the database
                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();

                // Now assign subjects to the student
                await AssignSubjectsToStudentAsync(newStudent.StudentId, year, semester, programId);

                // Update the UI thread (main thread) with the new student entry
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Students.Add(newStudent); // Add the new student to the ObservableCollection
                });

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
                // Fetch subjects that match the year, semester, and program
                var subjects = await _context.Subjects
                    .Where(s => s.YearLevel == year && s.Semester == semester && s.ProgramId == programId)
                    .ToListAsync();

                if (!subjects.Any())
                {
                    await ShowMessageAsync("No matching subjects found for the selected year, semester, and program.", "Warning", MessageBoxImage.Warning);
                    return;
                }

                // Create a list of StudentSubject entries to assign to the student
                var studentSubjects = subjects.Select(s => new StudentSubject
                {
                    StudentId = studentId,
                    SubjectId = s.SubjectId,
                    Id = $"{studentId}_{s.SubjectId}" // Ensure unique ID
                }).ToList();

                // Add new student-subject records to the database
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
