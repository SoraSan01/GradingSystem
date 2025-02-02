using GradingSystem.Command;
using GradingSystem.Data;
using GradingSystem.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

public class StudentSubjectViewModel : INotifyPropertyChanged
{
    private readonly IServiceProvider _serviceProvider;
    public ObservableCollection<StudentSubject> StudentSubjects { get; private set; }
    public string StudentId { get; private set; }

    public ICommand SaveCommand { get; }
    private bool _isSaving;

    public StudentSubjectViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        StudentSubjects = new ObservableCollection<StudentSubject>();
        SaveCommand = new RelayCommand(async () => await SaveGrades(), CanSaveGrades);
    }

    public async Task AddStudentSubjectAsync(StudentSubject studentSubject)
    {
        try
        {
            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                context.StudentSubjects.Add(studentSubject);
                await context.SaveChangesAsync();
                StudentSubjects.Add(studentSubject); // Directly add to the collection
            }
        }
        catch (Exception ex)
        {
            HandleError($"Error adding student subject: {ex.Message}");
        }
    }

    public async Task LoadSubjects(string studentId)
    {
        try
        {
            IsLoading = true;
            StudentId = studentId;

            var subjects = await _serviceProvider.GetRequiredService<ApplicationDbContext>()
                .StudentSubjects
                .Where(ss => ss.StudentId == studentId)
                .ToListAsync();

            StudentSubjects.Clear(); // Clear existing items to prevent duplication
            foreach (var subject in subjects)
            {
                StudentSubjects.Add(subject);
            }
        }
        catch (Exception ex)
        {
            HandleError($"Error loading subjects: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadSubjectsBasedOnStudentId(string studentId)
    {
        try
        {
            IsLoading = true;

            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            var studentSubjects = await context.StudentSubjects
                .Where(ss => ss.StudentId == studentId)
                .ToListAsync();

            // Update ObservableCollection on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                StudentSubjects.Clear();
                foreach (var subject in studentSubjects)
                {
                    StudentSubjects.Add(subject);
                }
            });
        }
        catch (Exception ex)
        {
            HandleError($"Error loading subjects based on student ID: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }


    public async Task LoadSubjectsBasedOnYearAndSemester(string studentId, string yearLevel, string semester)
    {
        try
        {
            IsLoading = true;

            var subjects = await _serviceProvider.GetRequiredService<ApplicationDbContext>()
                .StudentSubjects
                .Where(s => s.StudentId == studentId && s.YearLevel == yearLevel && s.Semester == semester)
                .Include(s => s.Subject)
                .ToListAsync();

            if (!subjects.Any())
            {
                MessageBox.Show("No subjects found for the given year and semester.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            StudentSubjects.Clear();
            foreach (var subject in subjects)
            {
                StudentSubjects.Add(subject);
            }
        }
        catch (Exception ex)
        {
            HandleError($"Error loading subjects: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task RemoveStudentSubjectAsync(StudentSubject studentSubject)
    {
        try
        {
            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                context.StudentSubjects.Remove(studentSubject);
                await context.SaveChangesAsync();
                StudentSubjects.Remove(studentSubject);
            }
        }
        catch (Exception ex)
        {
            HandleError($"Error removing student subject: {ex.Message}");
        }
    }

    private async Task SaveGrades()
    {
        if (_isSaving) return;

        try
        {
            _isSaving = true;

            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                if (context.ChangeTracker.HasChanges())
                {
                    await context.SaveChangesAsync();
                    MessageBox.Show("Grades saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        catch (Exception ex)
        {
            HandleError($"Error saving grades: {ex.Message}");
        }
        finally
        {
            _isSaving = false;
        }
    }


    private bool CanSaveGrades()
    {
        return !IsLoading && StudentSubjects.Any();
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }

    public async Task UpdateGradeAsync(StudentSubject studentSubject)
    {
        try
        {
            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var existingSubject = await context.StudentSubjects
                    .FirstOrDefaultAsync(ss => ss.Id == studentSubject.Id);

                if (existingSubject != null)
                {
                    existingSubject.Grade = studentSubject.Grade;
                    await context.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            HandleError($"Error updating grade: {ex.Message}");
        }
    }

    private void HandleError(string message)
    {
        // Here you can log the error to a file or show it in the UI
        Console.Error.WriteLine(message);
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
