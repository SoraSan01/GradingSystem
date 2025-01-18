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
    public class SubjectViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;

        private ObservableCollection<Subject> _subjects;
        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set
            {
                if (_subjects != value)
                {
                    _subjects = value;
                    OnPropertyChanged(nameof(Subjects));
                }
            }
        }

        // Property for loading status
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SubjectViewModel(ApplicationDbContext context)
        {
            _context = context;
            Subjects = new ObservableCollection<Subject>();
            IsLoading = false; // Set to false initially
            LoadSubjectsAsync();
        }

        // This method loads subjects asynchronously with better error handling
        public async Task LoadSubjectsAsync()
        {
            try
            {
                // Set loading indicator
                IsLoading = true;

                // Use Task.Run to run the database query on a separate thread to prevent UI freezing
                var subjects = await Task.Run(() => _context.Subjects.ToList());

                // After data is loaded, update the ObservableCollection
                Subjects = new ObservableCollection<Subject>(subjects);

                // Reset loading indicator
                IsLoading = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Reset loading indicator in case of error
                IsLoading = false;
            }
        }
    }
}
