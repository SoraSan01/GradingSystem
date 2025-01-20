using GradingSystem.Command;
using GradingSystem.Data;
using GradingSystem.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Microsoft.EntityFrameworkCore; // Add this line

public class StudentSubjectViewModel : INotifyPropertyChanged
{
    private readonly ApplicationDbContext _context;
    public ObservableCollection<StudentSubject> StudentSubjects { get; set; }
    public string StudentId { get; set; }

    public ICommand SaveCommand { get; }
    private bool _isSaving;

    public StudentSubjectViewModel(ApplicationDbContext context)
    {
        _context = context;
        StudentSubjects = new ObservableCollection<StudentSubject>();
        SaveCommand = new RelayCommand(async () => await SaveGrades(), CanSaveGrades);
    }

    // Method to load subjects asynchronously
    public async Task LoadSubjects(string studentId)
    {
        if (string.IsNullOrEmpty(studentId))
            throw new ArgumentException("Student ID cannot be null or empty.");

        if (_context == null)
            throw new InvalidOperationException("Database context is not initialized.");

        try
        {
            // Show loading indicator
            IsLoading = true;

            // Fetch subjects and associated grades from the database
            var subjects = await _context.Set<StudentSubject>()
                .Include(ss => ss.Subject)  // This is where the 'Include' method is used
                .Where(ss => ss.StudentId == studentId)
                .ToListAsync();

            // Clear existing subjects
            StudentSubjects.Clear();

            // Populate the collection with new data
            foreach (var subject in subjects)
            {
                if (subject.Subject != null)
                {
                    StudentSubjects.Add(subject);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle error while loading subjects
            MessageBox.Show($"An error occurred while loading subjects: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            // Hide loading indicator
            IsLoading = false;
        }
    }

    // Save grades to the database
    private async Task SaveGrades()
    {
        if (_isSaving)
            return;

        try
        {
            _isSaving = true;

            // Save changes made to the subjects and grades
            await _context.SaveChangesAsync();
            MessageBox.Show("Grades saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            // Handle error while saving grades
            MessageBox.Show($"An error occurred while saving grades: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isSaving = false;
        }
    }

    // Check if grades can be saved
    private bool CanSaveGrades()
    {
        return !IsLoading && StudentSubjects.Any(); // Only enable if subjects are loaded
    }

    // Loading status for UI
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
        _context.StudentSubjects.Update(studentSubject);
        await _context.SaveChangesAsync();
    }

    // Property changed event for data binding
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}