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
        private ObservableCollection<StudentSubject> _studentSubjects = new ObservableCollection<StudentSubject>();

        public ObservableCollection<StudentSubject> StudentSubjects
        {
            get => _studentSubjects;
            set
            {
                _studentSubjects = value;
                OnPropertyChanged(nameof(StudentSubjects));
            }
        }

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
                    _ = LoadStudentSubjectsAsync(_selectedStudent?.StudentId);
                }
            }
        }

        public async Task LoadStudentSubjectsAsync(string? studentId)
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        StudentSubjects.Clear();
                        foreach (var subject in subjects)
                        {
                            StudentSubjects.Add(subject);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("loading subjects", ex);
            }
        }

        public async Task LoadStudentDataAsync(Student student)
        {
            if (student == null)
            {
                await ShowMessageAsync("No student selected.", "Error", MessageBoxImage.Error);
                return;
            }

            SelectedStudent = student;

            Program = SelectedStudent.Program?.ProgramId;
            Semester = SelectedStudent.Semester;
            Status = SelectedStudent.Status;

            await LoadStudentSubjectsAsync(student.StudentId);
        }

        public async Task LoadStudentsAsync()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var studentsList = await context.Students
                        .Include(s => s.Program)
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
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("loading students", ex);
            }
        }

        public async Task AddStudentAsync(Student newStudent, string year, string semester, string programId, string status)
        {
            if (newStudent == null) throw new ArgumentNullException(nameof(newStudent));

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var existingStudents = await context.Students
                        .Where(s => s.StudentId == newStudent.StudentId)
                        .ToListAsync();

                    if (existingStudents.Any())
                    {
                        // Ensure the name matches exactly
                        if (!existingStudents.Any(s => s.StudentName == newStudent.StudentName))
                        {
                            await ShowMessageAsync("A student with this ID but a different name already exists.", "Duplicate Entry", MessageBoxImage.Warning);
                            return;
                        }

                        // Ensure at least one of year, semester, or status is different
                        if (existingStudents.Any(s => s.YearLevel == year && s.Semester == semester && s.Status == status))
                        {
                            await ShowMessageAsync("A student with this ID, same name, and the same year, semester, and status already exists.", "Duplicate Entry", MessageBoxImage.Warning);
                            return;
                        }
                    }

                    var programExists = await context.Programs.AnyAsync(p => p.ProgramId == programId);
                    if (!programExists)
                    {
                        await ShowMessageAsync("Selected program does not exist.", "Error", MessageBoxImage.Error);
                        return;
                    }

                    // Detach all tracked entities to avoid conflicts
                    context.ChangeTracker.Clear();

                    context.Students.Add(newStudent);
                    await context.SaveChangesAsync();

                    await AssignSubjectsToStudentAsync(newStudent.StudentId, year, semester, programId);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Students.Add(newStudent);
                    });
                }
            }
            catch (Exception ex)
            {
                await ShowMessageAsync($"An error occurred while adding the student: {ex.Message}\n{ex.InnerException?.Message}", "Error", MessageBoxImage.Error);
            }
        }

        private async Task AssignSubjectsToStudentAsync(string studentId, string year, string semester, string programId)
        {
            try
            {
                var subjects = await _context.Subjects
                    .Where(s => s.ProgramId == programId && s.YearLevel == year && s.Semester == semester)
                    .ToListAsync();

                foreach (var subject in subjects)
                {
                    var studentSubject = new StudentSubject
                    {
                        Id = await _context.GenerateUniqueStudentSubjectId(_context, studentId, subject.SubjectId),
                        StudentId = studentId,
                        SubjectId = subject.SubjectId,
                        CreatedAt = DateTime.Now
                    };

                    _context.StudentSubjects.Add(studentSubject);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync("assigning subjects", ex);
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
                studentToUpdate.ProgramId = updatedStudent.ProgramId;
                studentToUpdate.YearLevel = updatedStudent.YearLevel;
                studentToUpdate.Semester = updatedStudent.Semester;
                studentToUpdate.Status = updatedStudent.Status;

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
                studentInCollection.ProgramId = updatedStudent.ProgramId;
                studentInCollection.YearLevel = updatedStudent.YearLevel;
                studentInCollection.Semester = updatedStudent.Semester;
                studentInCollection.Status = updatedStudent.Status;
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
