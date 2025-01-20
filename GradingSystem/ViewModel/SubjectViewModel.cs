using GradingSystem.Data;
using GradingSystem.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class SubjectViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;

        private ObservableCollection<Subject>? _subjects;
        public ObservableCollection<Subject>? Subjects
        {
            get => _subjects;
            set
            {
                if (_subjects != value)
                {
                    _subjects = value;
                    OnPropertyChanged(nameof(Subjects));
                    ApplySearch(); // Update filtered list when subjects change
                }
            }
        }

        private ObservableCollection<Subject>? _filteredSubjects;
        public ObservableCollection<Subject>? FilteredSubjects
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

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplySearch(); // Update filtered list when search text changes
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SubjectViewModel(ApplicationDbContext context)
        {
            _context = context;
            Subjects = new ObservableCollection<Subject>();
            FilteredSubjects = new ObservableCollection<Subject>();
            _ = LoadSubjectsAsync();
        }

        public async Task LoadSubjectsAsync()
        {
            try
            {
                var subjects = await Task.Run(() => _context.Subjects.ToList());
                Subjects = new ObservableCollection<Subject>(subjects);
                FilteredSubjects = new ObservableCollection<Subject>(Subjects);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredSubjects = new ObservableCollection<Subject>(Subjects);
            }
            else
            {
                var lowerSearch = SearchText.ToLower();
                FilteredSubjects = new ObservableCollection<Subject>(Subjects.Where(s =>
                    (s.SubjectName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                    (s.ProfessorName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                    (s.Schedule ?? string.Empty).ToLower().Contains(lowerSearch)));
            }
        }
    }
}
