using GradingSystem.Data;
using GradingSystem.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

public class EnrollmentViewModel : INotifyPropertyChanged
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceScopeFactory _scopeFactory;


    private ObservableCollection<StudentSubject> _studentSubjects = new();
    private ObservableCollection<StudentSubject> _filteredStudentSubjects = new();
    private ObservableCollection<Subject> _allSubjects = new();

    public ObservableCollection<Enrollment> Enrollments { get; set; }

    private string _selectedYear;
    private string _selectedSemester;
    private string _programId;
    private string _status;
    private string _fullName;
    private string _programName;
    private string _semester;
    private string _yearLevel;
    private DateTime _currentDate = DateTime.Now;

    public EnrollmentViewModel(IServiceScopeFactory scopeFactory, ApplicationDbContext context)
    {
        _scopeFactory = scopeFactory;
        _context = context;
        Enrollments = new ObservableCollection<Enrollment>();
    }

    public async Task InitializeAsync() => await LoadAllSubjects();

    public string SelectedYear { get => _selectedYear; set => SetProperty(ref _selectedYear, value); }
    public string SelectedSemester { get => _selectedSemester; set => SetProperty(ref _selectedSemester, value); }
    public ObservableCollection<Subject> AllSubjects { get => _allSubjects; set => SetProperty(ref _allSubjects, value); }
    public ObservableCollection<StudentSubject> StudentSubjects { get => _studentSubjects; set => SetProperty(ref _studentSubjects, value); }
    public ObservableCollection<StudentSubject> FilteredStudentSubjects { get => _filteredStudentSubjects; set => SetProperty(ref _filteredStudentSubjects, value); }
    public string ProgramId { get => _programId; set => SetProperty(ref _programId, value); }
    public string Status { get => _status; set => SetProperty(ref _status, value); }
    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
    public string ProgramName { get => _programName; set => SetProperty(ref _programName, value); }
    public string Semester { get => _semester; set => SetProperty(ref _semester, value); }
    public string YearLevel { get => _yearLevel; set => SetProperty(ref _yearLevel, value); }
    public DateTime CurrentDate { get => _currentDate; set => SetProperty(ref _currentDate, value); }
    public Student? SelectedStudent { get; set; }

    public async Task LoadAllSubjects()
    {
        try
        {
            var subjects = await _context.Subjects.ToListAsync();
            AllSubjects = new ObservableCollection<Subject>(subjects);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync("loading subjects", ex);
        }
    }

    public async Task LoadEnrollmentsAsync()
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Student)  // Include related data
            .Include(e => e.Program)
            .ToListAsync();

        Enrollments.Clear();
        foreach (var enrollment in enrollments)
        {
            Enrollments.Add(enrollment);
        }
    }


    public async Task AddEnrollmentAsync(string studentId, string name, string year, string semester, string programId, string status)
    {
        try
        {
            // Check if the student is already enrolled in the same year and semester
            if (await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.YearLevel == year && e.Semester == semester))
            {
                await ShowMessageAsync("Enrollment already exists for this semester", "Error", MessageBoxImage.Error);
                return;
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                FullName = name,
                YearLevel = year,
                Semester = semester,
                ProgramId = programId,
                Status = status
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Assign the subjects after enrollment
            await AssignSubjectsToStudentAsync(studentId, year, semester, programId);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync("adding enrollment", ex);
        }
    }
    public async Task<Student> GetStudentByIdAsync(string studentId)
    {
        return await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task AssignSubjectsToStudentAsync(string studentId, string year, string semester, string programId)
    {
        if (_context == null)
        {
            await ShowMessageAsync("Database context is not initialized", "Error", MessageBoxImage.Error);
            return;
        }

        try
        {
            // Fetch subjects based on program, year, and semester
            var subjects = await GetSubjectsByProgramYearAndSemesterAsync(programId, year, semester);

            if (subjects == null || !subjects.Any())
            {
                await ShowMessageAsync("No subjects found for the selected program, year, and semester.", "Error", MessageBoxImage.Error);
                return;
            }

            foreach (var subject in subjects)
            {
                var studentSubject = new StudentSubject
                {
                    Id = await _context.GenerateUniqueStudentSubjectId(_context, studentId, subject.SubjectId),
                    StudentId = studentId,
                    SubjectId = subject.SubjectId,
                    YearLevel = year,
                    Semester = semester
                };

                // Ensure that the generated ID is unique and the student-subject pairing is valid
                if (await _context.StudentSubjects.AnyAsync(ss => ss.Id == studentSubject.Id))
                {
                    // If the subject is already assigned to the student, skip it
                    continue;
                }

                _context.StudentSubjects.Add(studentSubject);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Notify user that subjects were assigned successfully
            await ShowMessageAsync("Subjects assigned successfully.", "Success", MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            await ShowMessageAsync($"An error occurred: {ex.Message}", "Error", MessageBoxImage.Error);
        }
    }

    private async Task<List<Subject>> GetSubjectsByProgramYearAndSemesterAsync(string programId, string year, string semester)
    {
        // Null or empty checks
        if (string.IsNullOrWhiteSpace(programId))
        {
            await ShowMessageAsync("Program ID is null or empty", "Error", MessageBoxImage.Error);
            return new List<Subject>();
        }

        if (string.IsNullOrWhiteSpace(year))
        {
            await ShowMessageAsync("Year is null or empty", "Error", MessageBoxImage.Error);
            return new List<Subject>();
        }

        if (string.IsNullOrWhiteSpace(semester))
        {
            await ShowMessageAsync("Semester is null or empty", "Error", MessageBoxImage.Error);
            return new List<Subject>();
        }

        try
        {
            // Debugging to check if context is null
            if (_context == null)
            {
                await ShowMessageAsync("Database context is not initialized", "Error", MessageBoxImage.Error);
                return new List<Subject>();
            }

            var subjects = await _context.Subjects
                .Where(s => s.ProgramId == programId && s.YearLevel == year && s.Semester == semester)
                .ToListAsync();

            return subjects;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync("retrieving subjects by program, year, and semester", ex);
            return new List<Subject>();
        }
    }


    public async Task LoadStudentDataAsync(string studentId, string yearLevel, string semester)
    {
        try
        {
            var student = await _context.Enrollments
                .Include(e => e.Program)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.YearLevel == yearLevel && e.Semester == semester);

            if (student == null)
            {
                await ShowMessageAsync("Student not found", "Error", MessageBoxImage.Warning);
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                FullName = student.FullName;
                ProgramName = student.Program?.ProgramName ?? "No Program Assigned";
                Semester = student.Semester;
                YearLevel = student.YearLevel;
            });

            await LoadStudentSubjectsAsync(studentId, yearLevel, semester);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync("loading student data", ex);
        }
    }

    private async Task LoadStudentSubjectsAsync(string studentId, string year, string semester)
    {
        if (string.IsNullOrWhiteSpace(studentId)) return;

        try
        {
            var subjects = await _context.StudentSubjects
                .Include(ss => ss.Subject)
                .Where(ss => ss.StudentId == studentId && ss.Subject.YearLevel == year && ss.Subject.Semester == semester)
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
        catch (Exception ex)
        {
            await HandleErrorAsync("loading subjects", ex);
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
        Console.Error.WriteLine($"{action}: {ex}");
        await ShowMessageAsync($"{action}: {ex.Message}", "Error", MessageBoxImage.Error);
    }

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (!Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
