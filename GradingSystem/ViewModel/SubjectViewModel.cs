using GradingSystem.Command;
using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GradingSystem.ViewModel
{
    public class SubjectViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Subject> _subjects = new();
        private ObservableCollection<Subject> _filteredSubjects = new();

        private readonly ApplicationDbContext _context;

        public SubjectViewModel(ApplicationDbContext context)
        {
            _context = context;
            // Load subjects asynchronously when the ViewModel is initialized
            LoadSubjectsAsync();
        }

        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            private set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
                ApplySearch();
            }
        }

        public ObservableCollection<Subject> FilteredSubjects
        {
            get => _filteredSubjects;
            private set
            {
                _filteredSubjects = value;
                OnPropertyChanged(nameof(FilteredSubjects));
            }
        }

        public string? SearchText { get; set; }

        private Subject? _SelectedSubject;
        public Subject? SelectedSubject
        {
            get => _SelectedSubject;
            set
            {
                if (_SelectedSubject != value)
                {
                    _SelectedSubject = value;
                    OnPropertyChanged(nameof(SelectedSubject));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public async Task LoadSubjectsAsync()
        {
            try
            {
                // Fetch subjects from the database asynchronously
                var subjects = await _context.Subjects.ToListAsync();
                Subjects = new ObservableCollection<Subject>(subjects);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading subjects: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        public void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredSubjects = new ObservableCollection<Subject>(Subjects);
            }
            else
            {
                var lowerSearch = SearchText.ToLower();
                var filtered = _context.Subjects
                    .Where(s => (s.SubjectName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.SubjectId ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.CourseCode ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.ProfessorName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.Schedule ?? string.Empty).ToLower().Contains(lowerSearch))
                    .ToList();
                FilteredSubjects = new ObservableCollection<Subject>(filtered);
            }
        }

        public async Task AddSubjectAsync(Subject newSubject)
        {
            try
            {
                _context.Subjects.Add(newSubject);
                await _context.SaveChangesAsync();

                Subjects.Add(newSubject);
                ApplySearch();

                ShowMessage("Subject added successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding subject: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        public async Task DeleteSubjectAsync(Subject subject)
        {
            if (subject == null)
            {
                ShowMessage("Subject cannot be null.", "Error", MessageBoxImage.Error);
                return;
            }

            try
            {
                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete the subject '{subject.SubjectName}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    var subjectToDelete = await _context.Subjects.FindAsync(subject.SubjectId);

                    if (subjectToDelete == null)
                    {
                        ShowMessage("The subject no longer exists in the database.", "Error", MessageBoxImage.Warning);
                        return;
                    }

                    _context.Subjects.Remove(subjectToDelete);
                    await _context.SaveChangesAsync();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Subjects.Remove(subject);
                        ApplySearch();
                    });

                    ShowMessage("Subject deleted successfully.", "Success", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting subject: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        private void ShowMessage(string message, string caption, MessageBoxImage icon) =>
            Application.Current.Dispatcher.Invoke(() =>
                MessageBox.Show(message, caption, MessageBoxButton.OK, icon));
    }
}
