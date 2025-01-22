using GradingSystem.Data;
using GradingSystem.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class SubjectViewModel : INotifyPropertyChanged
    {
        private string? _searchText;
        private ObservableCollection<Subject> _subjects = new();
        private ObservableCollection<Subject> _filteredSubjects = new();

        private readonly ApplicationDbContext _context;

        public SubjectViewModel(ApplicationDbContext context)
        {
            _context = context;
            LoadSubjectsAsync().ConfigureAwait(false);
        }

        // Binding to the list of all subjects.
        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set
            {
                if (_subjects != value)
                {
                    _subjects = value;
                    OnPropertyChanged(nameof(Subjects));
                    ApplySearch(); // Apply search whenever the subjects collection is updated.
                }
            }
        }

        // Filtered subjects based on the search.
        public ObservableCollection<Subject> FilteredSubjects
        {
            get => _filteredSubjects;
            set
            {
                if (_filteredSubjects != value)
                {
                    _filteredSubjects = value;
                    OnPropertyChanged(nameof(FilteredSubjects));
                }
            }
        }

        // Search text property for binding to the UI.
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplySearch(); // Apply search whenever the text changes.
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Load subjects from the database.
        public async Task LoadSubjectsAsync()
        {
            try
            {
                var subjects = await Task.Run(() => _context.Subjects.ToList());
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Subjects = new ObservableCollection<Subject>(subjects);
                    FilteredSubjects = new ObservableCollection<Subject>(Subjects); // Initialize FilteredSubjects with all subjects
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading subjects: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        // Apply the search filter to the subjects.
        public void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // If no search text, show all subjects.
                FilteredSubjects = new ObservableCollection<Subject>(Subjects);
            }
            else
            {
                var lowerSearch = SearchText.ToLower();
                var filtered = Subjects.Where(s =>
                    (s.SubjectName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                    (s.ProfessorName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                    (s.Schedule ?? string.Empty).ToLower().Contains(lowerSearch)
                ).ToList();

                FilteredSubjects = new ObservableCollection<Subject>(filtered); // Assign the filtered list.
            }
        }

        // Delete a subject from the database.
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
                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Subjects.Remove(subject); // Remove from Subjects collection
                        ApplySearch(); // Reapply search after deletion to update FilteredSubjects
                    });

                    ShowMessage("Subject deleted successfully.", "Success", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"An error occurred while deleting the subject: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        // Helper method to show message boxes.
        private void ShowMessage(string message, string caption, MessageBoxImage icon) =>
            Application.Current.Dispatcher.Invoke(() =>
                MessageBox.Show(message, caption, MessageBoxButton.OK, icon));
    }
}
