﻿using GradingSystem.Data;
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

        public SubjectViewModel()
        {
            Subjects = new ObservableCollection<Subject>();
            LoadSubjectsAsync();
        }

        public async Task LoadSubjectsAsync()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var subjectList = await Task.Run(() => context.Subjects.ToList());
                    Subjects.Clear();
                    foreach (var subject in subjectList)
                    {
                        Subjects.Add(subject);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load subjects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
