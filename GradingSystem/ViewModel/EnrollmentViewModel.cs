using GradingSystem.Data;
using GradingSystem.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

public class EnrollmentViewModel : INotifyPropertyChanged
{
    private readonly ApplicationDbContext _context;
    public ObservableCollection<StudentSubject> StudentSubjects { get; set; } = new ObservableCollection<StudentSubject>();

    public EnrollmentViewModel(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Student> GetStudentByIdAsync(string studentId)
    {
        return await _context.Students.FindAsync(studentId);
    }

    public Student? SelectedStudent { get; set; }

    public async Task AddEnrollmentAsync(string studentId, string name, string studentName, string year, string semester, string programId, string status)
    {
        if (await EnrollmentExistsAsync(studentId, year, semester, status))
        {
            await ShowMessageAsync("Enrollment already exists", "Error", MessageBoxImage.Error);
            return;
        }

        if (!await ProgramExistsAsync(programId))
        {
            await ShowMessageAsync("Program does not exist", "Error", MessageBoxImage.Error);
            return;
        }

        await AddNewEnrollmentAsync(studentId, name, year, semester, programId, status);
    }

    private async Task<bool> EnrollmentExistsAsync(string studentId, string year, string semester, string status)
    {
        return await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.YearLevel == year && e.Semester == semester && e.Status == status);
    }

    private async Task<bool> ProgramExistsAsync(string programId)
    {
        return await _context.Programs.AnyAsync(p => p.ProgramId == programId);
    }

    private async Task AddNewEnrollmentAsync(string studentId, string name, string year, string semester, string programId, string status)
    {
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

        await AssignSubjectsToStudentAsync(studentId, year, semester);
    }

    private async Task AssignSubjectsToStudentAsync(string studentId, string year, string semester)
    {
        var subjects = await GetSubjectsByYearAndSemesterAsync(year, semester);
        foreach (var subject in subjects)
        {
            var studentSubject = new StudentSubject
            {
                Id = Guid.NewGuid().ToString(),
                StudentId = studentId,
                SubjectId = subject.SubjectId
            };

            _context.StudentSubjects.Add(studentSubject);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<List<Subject>> GetSubjectsByYearAndSemesterAsync(string year, string semester)
    {
        return await _context.Subjects
                             .Where(s => s.YearLevel == year && s.Semester == semester)
                             .ToListAsync();
    }

    public async Task LoadStudentDataAsync(string studentId, string yearLevel, string semester)
    {
        // Load student data from the database
        var student = await _context.Enrollments
            .Include(e => e.Program) // Include the Program details
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.YearLevel == yearLevel && e.Semester == semester);

        if (student != null)
        {
            FullName = student.FullName;
            ProgramName = student.Program?.ProgramName; // Safely access ProgramName
            Semester = student.Semester;
            YearLevel = student.YearLevel;

            // Load subjects and grades based on YearLevel and Semester
            var subjects = await _context.StudentSubjects
                .Include(ss => ss.Subject) // Include the Subject details
                .Where(ss => ss.StudentId == studentId &&
                             ss.Subject.YearLevel == yearLevel &&
                             ss.Subject.Semester == semester)
                .ToListAsync();

            StudentSubjects.Clear();
            foreach (var subject in subjects)
            {
                StudentSubjects.Add(subject);
            }
        }
    }
    public async Task LoadStudentSubjectsAsync(string studentId, string year, string semester)
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


    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task ShowMessageAsync(string message, string caption, MessageBoxImage icon)
    {
        // MessageBox doesn't need async, but the Task is here for future async changes
        await Task.Run(() => MessageBox.Show(message, caption, MessageBoxButton.OK, icon));
    }

    private async Task HandleErrorAsync(string action, Exception ex)
    {
        // Log the error (consider using a logging framework)
        Console.Error.WriteLine($"{action}: {ex}");

        await ShowMessageAsync($"{action}: {ex.Message}", "Error", MessageBoxImage.Error);
    }

    // Implementing the PropertyChanged pattern correctly for INotifyPropertyChanged
    public string ProgramId
    {
        get => _programId;
        set
        {
            if (_programId != value)
            {
                _programId = value;
                OnPropertyChanged(nameof(ProgramId));
            }
        }
    }
    private string _programId;

    public string Status
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
    private string _status;

    private string _fullName;
    public string FullName
    {
        get => _fullName;
        set
        {
            _fullName = value;
            OnPropertyChanged(nameof(FullName));
        }
    }

    private string _programName;
    public string ProgramName
    {
        get => _programName;
        set
        {
            _programName = value;
            OnPropertyChanged(nameof(ProgramName));
        }
    }

    private string _semester;
    public string Semester
    {
        get => _semester;
        set
        {
            _semester = value;
            OnPropertyChanged(nameof(Semester));
        }
    }

    private string _yearLevel;
    public string YearLevel
    {
        get => _yearLevel;
        set
        {
            _yearLevel = value;
            OnPropertyChanged(nameof(YearLevel));
        }
    }

    public DateTime CurrentDate
    {
        get => _currentDate;
        set
        {
            if (_currentDate != value)
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
            }
        }
    }
    private DateTime _currentDate = DateTime.Now;

    public event PropertyChangedEventHandler? PropertyChanged;
}
