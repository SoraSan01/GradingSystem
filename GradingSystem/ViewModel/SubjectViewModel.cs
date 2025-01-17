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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SubjectViewModel(ApplicationDbContext context)
        {
            _context = context;
            Subjects = new ObservableCollection<Subject>();
            LoadSubjectsAsync();
        }

        public async Task LoadSubjectsAsync()
        {
            try
            {
                var subjectList = await Task.Run(() => _context.Subjects.ToListAsync());
                Subjects.Clear();

                // Ensure UI updates happen on the UI thread
                foreach (var subject in subjectList)
                {
                    Subjects.Add(subject);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
